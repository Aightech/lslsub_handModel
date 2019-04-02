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
    private Transform tf;
    public Component[] tfs;
    public List<Transform> fingersTf;
    public liblsl.StreamInfo[] results;
    public string lastSample = String.Empty;
    private float incMax = 1.5f;

    private float[] fingersJoints = new float[3 * 5];
    private float[] fingersJoints_target = new float[3 * 5];
    private float[] fingersJoints_Max = new float[3 * 5];
    private string[] fingersTags = { "thu", "ind", "mid", "rin", "pin"};


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

        for (int i = 0; i < 3 * 5; i++)
        {
            fingersJoints[i] = 0.0f;
            fingersJoints_target[i] = 0.0f;
            fingersJoints_Max[i] = 60.0f + ((i % 3 == 0) ? 20.0f : 0.0f);
        }
        fingersJoints_Max[0] = 20;
        fingersJoints_Max[1] = 40;
    }

    // Update is called once per frame
    void Update()
    {
        /*t++;
        float mh = Input.GetAxis("Horizontal");
        float x = tf.position.x;
        int f = t / 30;
        float ang = 2.0f;
        if ((f / 5) % 2 == 0)
            ang = -ang;
        f = f % 5;*/
        float dTheta;
        for (int i = 0; i < 3 * 5; i++)
        {
            if( Math.Abs(fingersJoints[i] - fingersJoints_target[i]) > 1 )
            {
                dTheta = (fingersJoints[i] < fingersJoints_target[i]) ? incMax : -incMax;
                fingersTf[i].Rotate(0.0f, 0.0f, -dTheta);
                fingersJoints[i] += dTheta;
            }
            //Debug.Log("Test:" + fingersJoints_target[i].ToString());

        }
        // Debug.Log("Test:" + indexTf.Count.ToString());

    }

    protected override void Process(float[] newSample, double timeStamp)
    {
        // just as an example, make a string out of all channel values of this sample
        lastSample = string.Join(" ", newSample.Select(c => c.ToString()).ToArray());
        Debug.Log(string.Format("Got {0} samples at {1}", newSample.Length, timeStamp));


        for (int i = 0; i < 3 * 5; i++)
        {
            fingersJoints_target[i] = ((i < newSample.Length) ? ((newSample[i] > fingersJoints_Max[i]) ? fingersJoints_Max[i] : newSample[i]) : 0);
        }
    }
    

}