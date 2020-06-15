### How to use
When you first start the game after installing the mod, a file named "CameraConfig.json" will appear in the same folder as VRChat.exe. It will contain two sample cameras to show which settings you can put. It's possible to add or remove cameras from the list.

In game you can use left-ctrl with a digit key to toggle the defined cameras (top camera being toggled by ctrl+1, all the way to the 10th camera with ctrl+0)

### Properties
All the properties match ones described [in the unity docs](https://docs.unity3d.com/ScriptReference/Camera.html)

#### Enabled 
_true / false_

Should this field be on or off by default?
#### Rect
_dict with 4 decimal values_

Where on the screen should the camera display?
#### LocalPosition
_dict with 3 decimal values_

Where should the camera be compared to the main camera?
#### LocalRotation
_dict with 4 decimal values_

The local rotation of the camera as a quaternion. Only used if UseRotation = true.
If UseRotation is false, then the camera will look at yourself.
#### Aspect
_decimal value_

The aspect ratio of the camera. Only used if UseAspect = true.
If UseAspect = false, then the aspect ratio will be based of the screen size.
#### Depth
_decimal value_

Which order cameras draw in.
#### BackgroundColor
_dict with 4 decimal values_

What color to show if ClearFlags is set to SolidColor.
#### ClearFlags
_Skybox / SolidColor / Depth / Nothing_

What to show when stuff is cut off by the ClipPlanes.
 - Skybox: Same as normal camera
 - SolidColor: A flat color defined with BackgroundColor
 - Depth: Don't overwrite colors, but depth
 - None: Don't overwrite colors, and don't overwrite depth
#### Orthographic
_true / false_

Orthographic = no perspective.
#### FieldOfView
_decimal value_

How wide the camera can see when using perspective.
#### OrthographicSize
_decimal value_

[Used when Orthographic](https://docs.unity3d.com/ScriptReference/Camera-orthographicSize.html)
#### NearClipPlane
_decimal value_

Everything closer than this value won't be drawn
#### FarClipPlane
_decimal value_

Everything further away than this value won't be drawn