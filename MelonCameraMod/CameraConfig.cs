﻿using MelonLoader.TinyJSON;
using UnityEngine;
// ReSharper disable InconsistentNaming

namespace MelonCameraMod
{
    [System.Serializable]
    public class CameraConfig
    {
        public bool Enabled;

        public SerializedRect Rect;
        public SerializedVector3 LocalPosition;

        public bool UseRotation;
        public SerializedQuaternion LocalRotation;

        public bool UseAspect;
        public float Aspect;

        public float Depth;
        public SerializedColor BackgroundColor;
        public CameraClearFlags ClearFlags;

        public bool Orthographic;
        public float FieldOfView;
        public float OrthographicSize;

        public float FarClipPlane;
        public float NearClipPlane;

        public uint CullingMask;

        public string ParentGameObject;
        public int ParentAscension;


        public KeyCode HoldToToggle;
        public KeyCode PressToToggle;

        public CameraConfig()
        {
            Aspect = 1;
            BackgroundColor = new SerializedColor(0, 0, 0, 0);
            ClearFlags = CameraClearFlags.SolidColor;
            Depth = 1;
            Enabled = false;
            FarClipPlane = 1000;
            FieldOfView = 60;
            LocalPosition = new SerializedVector3(-1.5f, 0.5f, 1f);
            LocalRotation = new SerializedQuaternion(0, 0, 0, 1);
            NearClipPlane = 0.3f;
            Orthographic = false;
            OrthographicSize = 5;
            Rect = new SerializedRect(0, 0, 1, 1);
            UseAspect = false;
            UseRotation = false;

            CullingMask = unchecked((uint)~0);
            ParentGameObject = "";
            ParentAscension = 0;

            HoldToToggle = KeyCode.LeftControl;
            PressToToggle = KeyCode.Alpha0;
        }
    }

    [System.Serializable]
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

    [System.Serializable]
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

    [System.Serializable]
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


    [System.Serializable]
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
