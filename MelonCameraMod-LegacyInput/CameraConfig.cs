using System;
using System.Collections.Generic;
using UnityEngine;
// ReSharper disable InconsistentNaming

namespace MelonCameraMod
{
    [Serializable]
    public class FullConfig
    {
        public List<CameraConfig> CameraConfigs = new List<CameraConfig>();
    }

    [Serializable]
    public class CameraConfig
    {
        public bool Enabled;
        public bool Debug;

        public bool ForceUpdatePosition;
        public bool PositionIgnoresScale;

        public SerializedRect Rect;
        public SerializedVector3 LocalPosition;

        public bool StartUpright;
        public bool UseRotation;
        public SerializedQuaternion LocalRotation;
        public bool UseEuler;
        public SerializedVector3 EulerAngles;

        public bool UseAspect;
        public float Aspect;

        public float Depth;
        public SerializedColor BackgroundColor;
        public CameraClearFlags ClearFlags;

        public bool ForceUpdateFov;
        public bool Orthographic;
        public float FieldOfView;
        public float OrthographicSize;

        public float FarClipPlane;
        public float NearClipPlane;

        public uint CullingMask;

        public string ParentGameObject;
        public int ParentAscension;

        // Not properly implemented!
        public bool UseAvatarAsBasePosition;

        public bool UseRenderTexture;
        public int RenderTextureWidth, RenderTextureHeight, RenderTextureDepth;
        public ScaleMode RenderTextureScaleMode;

        public KeyCode HoldToToggle;
        public KeyCode PressToToggle;

        public int CameraIndex;

        public CameraConfig()
        {
            Aspect = 1;
            BackgroundColor = new SerializedColor(0, 0, 0, 0);
            ClearFlags = CameraClearFlags.SolidColor;
            Debug = false;
            Depth = 1;
            Enabled = false;
            FarClipPlane = 1000;
            FieldOfView = 60;
            ForceUpdatePosition = false;
            LocalPosition = new SerializedVector3(0, 0, 0);
            LocalRotation = new SerializedQuaternion(0, 0, 0, 1);
            NearClipPlane = 0.3f;
            Orthographic = false;
            OrthographicSize = 5;
            PositionIgnoresScale = false;
            Rect = new SerializedRect(0, 0, 1, 1);
            UseAspect = false;
            UseRotation = false;

            UseAvatarAsBasePosition = false;
            CullingMask = unchecked((uint)~0);
            ParentGameObject = "";
            ParentAscension = 0;

            UseRenderTexture = false;
            RenderTextureWidth = 512;
            RenderTextureHeight = 256;
            RenderTextureDepth = 16;
            RenderTextureScaleMode = ScaleMode.ScaleAndCrop;

            HoldToToggle = KeyCode.LeftControl;
            PressToToggle = KeyCode.Alpha0;

            CameraIndex = -1;
        }
    }

    [Serializable]
    public class SerializedRect
    {
        public float X, Y, Width, Height;

        public SerializedRect() { }

        public SerializedRect(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public static implicit operator Rect(SerializedRect rect)
        {
            return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
        }
    }

    [Serializable]
    public class SerializedVector3
    {
        public float X, Y, Z;

        public SerializedVector3() { }

        public SerializedVector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static implicit operator Vector3(SerializedVector3 Vector3)
        {
            return new Vector3(Vector3.X, Vector3.Y, Vector3.Z);
        }
    }

    [Serializable]
    public class SerializedQuaternion
    {
        public float X, Y, Z, W;

        public SerializedQuaternion() { }

        public SerializedQuaternion(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public static implicit operator Quaternion(SerializedQuaternion Quaternion)
        {
            return new Quaternion(Quaternion.X, Quaternion.Y, Quaternion.Z, Quaternion.W);
        }
    }


    [Serializable]
    public class SerializedColor
    {
        public float Red, Green, Blue, Alpha;

        public SerializedColor() { }

        public SerializedColor(float red, float green, float blue, float alpha)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        public static implicit operator Color(SerializedColor Color)
        {
            return new Color(Color.Red, Color.Green, Color.Blue, Color.Alpha);
        }
    }
}
