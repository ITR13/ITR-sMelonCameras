using Il2CppSystem.Collections.Generic;
using MelonLoader;
using UnityEngine;

namespace MelonCameraMod
{
    public class CameraMod : MelonMod
    {
        private GameObject _cameraParent;
        private readonly List<Camera> _cameras = new List<Camera>();

        public override void OnApplicationQuit()
        {
            ConfigWatcher.Unload();
        }

        public override void OnUpdate()
        {
            CheckToggles();

            if (!ConfigWatcher.UpdateIfDirty() && _cameraParent != null) return;

            var mainCamera = Camera.main;
            if (mainCamera == null) return;
            _cameraParent = mainCamera.gameObject;
            
            UpdateCameras();
        }

        private void CheckToggles()
        {
            if (!Input.GetKey(KeyCode.LeftControl)) return;
            for (var i = 0; i < 10; i++)
            {
                if (!Input.GetKeyDown(KeyCode.Alpha0 + i)) continue;
                var index = (i + 9) % 10;
                if (index < _cameras.Count)
                {
                    _cameras[index].enabled = !_cameras[index].enabled;
                }
            }
        }

        private void UpdateCameras()
        {
            foreach (var camera in _cameras)
            {
                if (camera == null || camera.gameObject == null) continue;
                Object.Destroy(camera.gameObject);
            }
            _cameras.Clear();

            var cameraCount = ConfigWatcher.CameraConfigs?.Count ?? 0;
            MelonModLogger.Log($"Creating {cameraCount} cameras");

            for (var i = 0; i < cameraCount; i++)
            {
                // ReSharper disable once PossibleNullReferenceException
                var config = ConfigWatcher.CameraConfigs[i];
                if (config == null)
                {
                    MelonModLogger.LogWarning($"Camera {i} in config is null");
                    continue;
                }

                var child = new GameObject($"Modded Camera {i}");
                child.transform.parent = _cameraParent.transform;

                var camera = child.AddComponent<Camera>();
                _cameras.Add(camera);

                camera.enabled = config.Enabled;
                child.transform.localPosition = config.LocalPosition;

                if (config.UseRotation)
                {
                    child.transform.localRotation = config.LocalRotation;
                }
                else
                {
                    child.transform.LookAt(
                        _cameraParent.transform,
                        _cameraParent.transform.up
                    );
                }

                if (config.UseAspect)
                {
                    camera.aspect = config.Aspect;
                }

                camera.backgroundColor = config.BackgroundColor;
                camera.clearFlags = config.ClearFlags;
                camera.depth = config.Depth;
                camera.farClipPlane = config.FarClipPlane;
                camera.nearClipPlane = config.NearClipPlane;
                camera.orthographic = config.Orthographic;
                camera.orthographicSize = config.OrthographicSize;
                camera.rect = config.Rect;
                camera.cullingMask = config.CullingMask;

                camera.eventMask = 0;
                camera.stereoTargetEye = StereoTargetEyeMask.None;
            }
        }
    }
}
