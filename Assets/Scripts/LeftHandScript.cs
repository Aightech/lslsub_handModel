/**
 * \file LeftHandScript.cs
 * \brief TODO.
 * \author Alexis Devillard
 * \version 1.0
 * \date 03 march 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Threading;
using LSL;
using Assets.LSL4Unity.Scripts.AbstractInlets;

public class LeftHandScript : AFloatInlet
{
    //LSL outlet attributes
    private liblsl.StreamOutlet outlet;
    private liblsl.StreamInfo streamInfo;

    public string StreamOutName = "Left_Hand";
    public string StreamOutType = "Hand_Position";
    public int ChannelCount = 15;
    //**************//

    //model attribute
    private Transform tf;
    public Component[] tfs;
    public List<Transform> fingersTf;
    public string lastSample = String.Empty;
    private float incMax = 1.5f;

    private float[] fingersJoints = new float[3 * 5];
    private float[] fingersJoints_target = new float[3 * 5];
    private float[] fingersJoints_Max = new float[3 * 5];
    private string[] fingersTags = { "thu", "ind", "mid", "rin", "pin"};
    //**************//

    /**
    * @brief AdditionalStart The Start function is use by the AFloatInlet 
    * class so you should not overwrite it. So we initiate the different variable here
    */
    protected override void AdditionalStart()
    {
        //get all transform of the hand
        tfs = GetComponentsInChildren<Transform>();
        //store in a proper order the finger joints transform.
        //[j1_thumb, j2_thumb, j3_thumb, 
        // j1_index, j2_index, j3_index, 
        // j1_middle,j2_middle,j3_middle 
        // j1_ring,  j2_ring,  j3_ring, 
        // j1_pinky, j2_pinky, j3_pinky]
        foreach (string tag in fingersTags)
            foreach (Transform ch in tfs)
                if (ch.tag == tag)
                    fingersTf.Add(ch);

        //put some value and limits to the joint
        for (int i = 0; i < 3 * 5; i++)
        {
            fingersJoints[i] = 0.0f;
            fingersJoints_target[i] = 0.0f;
            fingersJoints_Max[i] = 60.0f + ((i % 3 == 0) ? 20.0f : 0.0f);
        }
        fingersJoints_Max[0] = 20;
        fingersJoints_Max[1] = 40;

        // Start LSL stream to publish the current position of the hand
        streamInfo = new liblsl.StreamInfo(StreamOutName, StreamOutType, ChannelCount, Time.fixedDeltaTime * 1000);
        outlet = new liblsl.StreamOutlet(streamInfo);
    }

    // Update is called once per frame
    void Update()
    {
        float dTheta;
        for (int i = 0; i < 3 * 5; i++)
        {//for each joints make it closer to the targeting pos
            if( Math.Abs(fingersJoints[i] - fingersJoints_target[i]) > 1 )
            {
                dTheta = (fingersJoints[i] < fingersJoints_target[i]) ? incMax : -incMax;
                fingersTf[i].Rotate(0.0f, 0.0f, -dTheta);
                fingersJoints[i] += dTheta;
            }

        }
        outlet.push_sample(fingersJoints);//publish the position


    }

    protected override void Process(float[] newSample, double timeStamp)
    {
        // just as an example, make a string out of all channel values of this sample
        lastSample = string.Join(" ", newSample.Select(c => c.ToString()).ToArray());
        Debug.Log(string.Format("Got {0} samples at {1}", newSample.Length, timeStamp));

        for (int i = 0; i < 3 * 5; i++)
            fingersJoints_target[i] = ((i < newSample.Length) ? ((newSample[i] > fingersJoints_Max[i]) ? fingersJoints_Max[i] : newSample[i]) : 0);
        
    }
    

}