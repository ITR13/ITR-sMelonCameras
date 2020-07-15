***NOTE: DOESN'T WORK IN VR***  
Feel free to ask me any questions on discord: ITR#2941  

### What it does
Allows you to spawn cameras that render to your screen similarly to stream cameras.  
It's very flexible, it reads \[Game Root Folder\]\UserData\CameraConfig.json and sets some of the properties you can find [in the unity docs](https://docs.unity3d.com/ScriptReference/Camera.html). 

To quick-toggle cameras, press left control and 0. This can be changed in the config file

### How to use

![Cameras rendereing to 3 courners of the screen](https://raw.githubusercontent.com/ITR13/ITR-sMelonCameras/master/SAMPLE1.jpg)
[Sample 1](https://github.com/ITR13/ITR-sMelonCameras/blob/master/SAMPLE1.json) shows how you can set up multiple cameras in specific regions (check Rect) at different locations (check LocalPosition)

![Two overlapping cameras rendering over the main view](https://raw.githubusercontent.com/ITR13/ITR-sMelonCameras/master/SAMPLE2.jpg)
[Sample 2](https://github.com/ITR13/ITR-sMelonCameras/blob/master/SAMPLE2.json) shows how you can set up an aerial view camera (check LocalPosition and LocalRotation) that renders the player on top of everything else (check NearClipPlane, FarClipPlane, and ClearFlags)

![An orthographic view from above](https://raw.githubusercontent.com/ITR13/ITR-sMelonCameras/master/SAMPLE3.jpg)
[Sample 3](https://github.com/ITR13/ITR-sMelonCameras/blob/master/SAMPLE3.json) shows what an orthographic view looks like.

### Contact
To request a feature, either open an issue on github, or send me a DM on discord @ ITR#2941  
Pull-requests are also greatly appreciated

Note that this mod also works in other games than vrchat.

### Json Details

#### Toggling Camera
**KeyCode HoldToToggle**: This key will have to be held while pressing the key beneath. If set to "None", it will only use PressToToggle  
**KeyCode PressToToggle**: When you press this key the camera will toggle its enabled state.  
Both are strings with the names of [KeyCode](https://docs.unity3d.com/ScriptReference/KeyCode.html)s (scroll down to Properties for a list of all)

####
**string ParentGameObject**: Use this if you want to attach the camera to a specific gameobject in the scene.  
Uses [GameObject.Find](https://docs.unity3d.com/ScriptReference/GameObject.Find.html) to find the object, if none is found, it uses the main camera instead.  
**int ParentAscension**: Replaces the parent GameObject with [its parent](https://docs.unity3d.com/ScriptReference/Transform-parent.html) n amount of times, or until the top parent is found.  
In VRChat you can set this to 3​ if you want the camera to only rotate on the y axis.  

#### Special bools
**bool UseRotation**: Whether or not to override default direction with LocalRotation  
**bool UseAspect**: Whether or not to override default aspect ratio with Aspect  
These settings have special behaviour when not set, and therefore need an extra bool to know if you want the zero-value or to not use the value

#### Culling Mask
**LayerMask [CullingMask](https://docs.unity3d.com/ScriptReference/Camera-cullingMask.html)**  
Or the bits for each [layer](http://vrchat.wikidot.com/worlds:layers) you want.  
Example:  
To only see yourself you want 18: MirrorReflection, so you do 2^18 = 262144 and put 262144 as your LayerMask  
To also see other people you want 9: Player too, so you do 2^9 = 512 then 262144 + 512 = 262656 and put 262656 as your LayerMask  

#### Other
**bool [Enabled](https://docs.unity3d.com/ScriptReference/Behaviour-enabled.html)**  
**Rect [Rect](https://docs.unity3d.com/ScriptReference/Camera-rect.html)**  

**Vector3 [LocalPosition](https://docs.unity3d.com/ScriptReference/Transform-localPosition.html)**  
**Quaternion [LocalRotation](https://docs.unity3d.com/ScriptReference/Transform-localPosition.html)**  

**float [Aspect](https://docs.unity3d.com/ScriptReference/Camera-aspect.html)**  

**float [Depth](https://docs.unity3d.com/ScriptReference/Camera-depth.html)**  
**Color [BackgroundColor](https://docs.unity3d.com/ScriptReference/Camera-backgroundColor.html)**  
**CameraClearFlags [ClearFlags](https://docs.unity3d.com/ScriptReference/Camera-clearFlags.html)**  

**bool [Orthographic](https://docs.unity3d.com/ScriptReference/Camera-orthographic.html)**  
**float [FieldOfView](https://docs.unity3d.com/ScriptReference/Camera-fieldOfView.html)**  
**float [OrthographicSize](https://docs.unity3d.com/ScriptReference/Camera-orthographicSize.html)**  

**float [FarClipPlane](https://docs.unity3d.com/ScriptReference/Camera-farClipPlane.html)**  
**float [NearClipPlane](https://docs.unity3d.com/ScriptReference/Camera-nearClipPlane.html)**  
