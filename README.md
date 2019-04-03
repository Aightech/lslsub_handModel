# lslsub_handModel

This project is a 3D hands model able to listen and movement according to two LSL streams commanding the fingers position. This project has been realised with Unity 3D on Windows.

# Usage
In the release directory execute the program lslsub_handModel.exe

# Lab Streaming Layer
This program look for LSL streams called "Left_Hand" or "Right_Hand" and listen to them. The samples sent have to be vectors of 15 floats representing the rotation of each interphalangeal joints ( thumb_1st_ph , thumb_2nd_ph, thumb_3rd_ph, index_1st_ph , ... , pinky_3rd_ph) 
