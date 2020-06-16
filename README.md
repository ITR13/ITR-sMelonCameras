***NOTE: NOT TESTED IN VR***

### What it does
Allows you to spawn cameras that render to your screen similarly to stream cameras.
It's very flexible, it reads VRChat\UserData\CameraConfig.json and sets some of the properties you can find [in the unity docs](https://docs.unity3d.com/ScriptReference/Camera.html). 

To quick-toggle cameras, press ctrl then a digit key. The first config in the list is toggled with ctrl+1, all the way to the 10th with ctrl+0.

### How to use

[Cameras rendereing to 3 courners of the screen](https://github.com/ITR13/ITR-sMelonCameras/SAMPLE1.jpg]
[Sample 1](https://github.com/ITR13/ITR-sMelonCameras/SAMPLE1.json) shows how you can set up multiple cameras in specific regions (check Rect) at different locations (check LocalPosition)

[Two overlapping cameras rendering over the main view](https://github.com/ITR13/ITR-sMelonCameras/SAMPLE2.jpg]
[Sample 2](https://github.com/ITR13/ITR-sMelonCameras/SAMPLE2.json) shows how you can set up an aerial view camera (check LocalPosition and LocalRotation) that renders the player on top of everything else (check NearClipPlane, FarClipPlane, and ClearFlags)

[An orthographic view from above](https://github.com/ITR13/ITR-sMelonCameras/SAMPLE3.jpg]
[Sample 3](https://github.com/ITR13/ITR-sMelonCameras/SAMPLE3.json) shows what an orthographic view looks like.

### Contact
To request a feature, either open an issue on github, or send me a DM on discord @ ITR#2941
Pull-requests are also greatly appreciated

Note that this mod also works in other games than vrchat.