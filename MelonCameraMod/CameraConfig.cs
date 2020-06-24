using UnityEngine;
// ReSharper disable InconsistentNaming

namespace MelonCameraMod
{
    [System.Serializable]
    public class CameraConfig
    {
        public bool Enabled;

        public Rect Rect;
        public Vector3 LocalPosition;

        public bool UseRotation;
        public Quaternion LocalRotation;

        public bool UseAspect;
        public float Aspect;

        public float Depth;
        public Color BackgroundColor;
        public CameraClearFlags ClearFlags;

        public bool Orthographic;
        public float FieldOfView;
        public float OrthographicSize;

        public float FarClipPlane;
        public float NearClipPlane;

        public LayerMask CullingMask;

        public CameraConfig()
        {
            Aspect = 1;
            BackgroundColor = new Color(0, 0, 0, 0);
            ClearFlags = CameraClearFlags.SolidColor;
            Depth = 1;
            Enabled = false;
            FarClipPlane = 1000;
            FieldOfView = 60;
            LocalPosition = new Vector3(-1.5f, 0.5f, 1f);
            LocalRotation = Quaternion.identity;
            NearClipPlane = 0.3f;
            Orthographic = false;
            OrthographicSize = 5;
            Rect = new Rect(0, 0, 1, 1);
            UseAspect = false;
            UseRotation = false;

            CullingMask = ~0;
        }
    }
}
