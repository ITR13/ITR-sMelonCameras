using System.Collections.Generic;
using Il2CppSystem.Text;
using MelonLoader;
using UnityEngine;

namespace MelonCameraMod
{
    public class CameraMod : MelonMod
    {
        private class CameraData
        {
            public Camera Camera;
            public KeyCode Hold, Press;
        }

        private GameObject _cameraParent;
        private readonly List<CameraData> _cameras = new List<CameraData>();

        public override void OnApplicationQuit()
        {
            ConfigWatcher.Unload();
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            _cameraParent = null;
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
            foreach (var cameraData in _cameras)
            {
                if (cameraData.Press == KeyCode.None) continue;
                if (cameraData.Hold != KeyCode.None && !Input.GetKey(cameraData.Hold)) continue;
                if (!Input.GetKeyDown(cameraData.Press)) continue;
                if (cameraData.Camera == null) continue;
                cameraData.Camera.enabled ^= true;
            }
        }

        private void UpdateCameras()
        {
            foreach (var cameraData in _cameras)
            {
                if (cameraData.Camera == null) continue;
                Object.Destroy(cameraData.Camera.gameObject);
            }
            _cameras.Clear();

            var cameraCount = ConfigWatcher.CameraConfigs?.Count ?? 0;
            MelonLogger.Msg($"Creating {cameraCount} cameras");

            for (var i = 0; i < cameraCount; i++)
            {
                // ReSharper disable once PossibleNullReferenceException
                var config = ConfigWatcher.CameraConfigs[i];
                if (config == null)
                {
                    MelonLogger.Warning($"Camera {i} in config is null");
                    continue;
                }

                var debug = config.Debug;

                var child = new GameObject($"Modded Camera {i}");
                var parent = _cameraParent.transform;

                if (debug) MelonLogger.Msg("Creating " + child.name);

                if (config.CameraIndex > -1)
                {
                    var allCamerasCount = Camera.allCamerasCount;
                    if (config.CameraIndex >= allCamerasCount)
                    {
                        MelonLogger.Msg(
                            $"Failed to find camera with index {config.CameraIndex} (only {allCamerasCount} exist)"
                        );
                    }
                    else
                    {
                        parent = Camera.allCameras[config.CameraIndex].gameObject.transform;
                        if (debug)
                        {
                            MelonLogger.Msg($"Using camera '{parent.name}' (index {config.CameraIndex}) as parent");
                        }
                    }
                }
                else if (!string.IsNullOrWhiteSpace(config.ParentGameObject))
                {
                    var newParent = GameObject.Find(config.ParentGameObject);
                    if (newParent == null)
                    {
                        MelonLogger.Msg($"Failed to find gameobject '{config.ParentGameObject}'");
                    }
                    else
                    {
                        if (debug) MelonLogger.Msg($"Using obj '{newParent.name}' as parent");
                        parent = newParent.transform;
                    }
                }
                else if(config.Debug)
                {
                    MelonLogger.Msg("Using main camera obj as parent");
                }

                for (var j = 0; j < config.ParentAscension; j++)
                {
                    if (parent.parent == null)
                    {
                        MelonLogger.Msg($"Failed to ascend parent {j} times (goal was {config.ParentAscension})");
                        break;
                    }

                    parent = parent.parent;
                    if(debug) MelonLogger.Msg($"Ascension {j} to {parent.name}");
                }

                child.transform.parent = parent;


                var camera = child.AddComponent<Camera>();
                _cameras.Add(
                    new CameraData
                    {
                        Camera = camera,
                        Hold = config.HoldToToggle,
                        Press = config.PressToToggle,
                    }
                );

                camera.enabled = config.Enabled;
                child.transform.localPosition = config.LocalPosition;
                if (debug)
                {
                    var position = child.transform.localPosition;
                    MelonLogger.Msg($"Using local position {position.x},{position.x},{position.z}");
                }

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

                if (debug)
                {
                    var rotation = child.transform.localRotation;
                    var word = config.UseRotation ? "local" : "look";
                    MelonLogger.Msg($"Using {word} rotation {rotation.x},{rotation.y},{rotation.z},{rotation.w}");
                }

                if (config.UseAspect)
                {
                    camera.aspect = config.Aspect;
                    if (debug) MelonLogger.Msg($"Using aspect {config.Aspect}");
                }

                camera.backgroundColor = config.BackgroundColor;
                camera.clearFlags = config.ClearFlags;
                camera.depth = config.Depth;
                camera.farClipPlane = config.FarClipPlane;
                camera.nearClipPlane = config.NearClipPlane;
                camera.orthographic = config.Orthographic;
                camera.orthographicSize = config.OrthographicSize;
                camera.rect = config.Rect;
                camera.cullingMask = (int)config.CullingMask;

                camera.eventMask = 0;
                camera.stereoTargetEye = StereoTargetEyeMask.None;

                if (debug)
                {
                    var sb = new StringBuilder(child.name, 100);
                    var currentParent = parent;
                    while (currentParent != null)
                    {
                        sb.Insert(0, "/");
                        sb.Insert(0, currentParent.gameObject.name);
                        currentParent = currentParent.parent;
                    }

                    sb.Insert(0, "Finished creating camera with path: ");
                    MelonLogger.Msg(sb.ToString());
                }
            }
        }
    }
}
