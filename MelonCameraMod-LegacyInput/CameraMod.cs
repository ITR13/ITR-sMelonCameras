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

        private class RenderTextureData
        {
            public Camera Camera { set; private get; }
            public bool Enabled => Camera != null && Camera.enabled;
            public RenderTexture RenderTexture;
            public Rect Rect;
            public ScaleMode ScaleMode;
        }

        private class PositionData
        {
            public Camera Camera { set; private get; }
            public bool Enabled => Camera != null && Camera.enabled;
            public bool PositionIgnoresScale, StartUpright, UseRotation;
            public Transform Transform;
            public Vector3 Position;
            public Quaternion Rotation;
        }

        private class FovData
        {
            public Camera Camera;
            public float Fov;
        }

        private GameObject _cameraParent;
        private readonly List<CameraData> _cameras = new List<CameraData>();
        private readonly List<RenderTextureData> _renderTextures = new List<RenderTextureData>();
        private readonly List<PositionData> _positions = new List<PositionData>();
        private readonly List<FovData> _fovs = new List<FovData>();

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

        public override void OnLateUpdate()
        {
            UpdatePositions();
            UpdateFovs();
        }

        private void UpdatePositions()
        {
            foreach (var positionData in _positions)
            {
                if(!positionData.Enabled) continue;
                var parent = positionData.Transform.parent;
                if (positionData.PositionIgnoresScale)
                {
                    positionData.Transform.position =
                        parent.TransformDirection(positionData.Position) +
                        parent.position;
                }
                else
                {
                    positionData.Transform.localPosition = positionData.Position;
                }

                if (positionData.UseRotation)
                {
                    positionData.Transform.localRotation = positionData.Rotation;

                    if (!positionData.StartUpright) continue;
                    var diff = Quaternion.FromToRotation(parent.up, Vector3.up);
                    positionData.Transform.rotation = diff * positionData.Transform.rotation;
                }
                else
                {
                    positionData.Transform.LookAt(parent, positionData.StartUpright ? Vector3.up : parent.up);
                }
            }
        }

        private void UpdateFovs()
        {
            foreach (var fovData in _fovs)
            {
                if (fovData.Camera == null) continue;
                fovData.Camera.fieldOfView = fovData.Fov;
            }
        }


        public override void OnGUI() => BlitRenderTextures();

        private void BlitRenderTextures()
        {
            if(!Event.current.type.Equals(EventType.Repaint)) return;
            var screenSize = new Vector2(Screen.width, Screen.height);
            foreach (var rtData in _renderTextures)
            {
                if (!rtData.Enabled) continue;
                var size = rtData.Rect.size * screenSize;
                var screenRect = new Rect(
                    rtData.Rect.position * screenSize,
                    size
                );
                GUI.DrawTexture(
                    screenRect,
                    rtData.RenderTexture,
                    rtData.ScaleMode
                );
            }
        }


        private void UpdateCameras()
        {
            // Clear old data
            {
                foreach (var cameraData in _cameras)
                {
                    if (cameraData.Camera == null) continue;
                    Object.Destroy(cameraData.Camera.gameObject);
                }

                _cameras.Clear();
                foreach (var renderData in _renderTextures)
                {
                    if (renderData.RenderTexture == null) continue;
                    Object.Destroy(renderData.RenderTexture);
                }

                _renderTextures.Clear();
                _positions.Clear();
                _fovs.Clear();
            }

            var cameraCount = ConfigWatcher.CameraConfigs?.Count ?? 0;
            MelonLogger.Msg($"Creating {cameraCount} cameras");

            void CreateCamera(int configIndex)
            {
                // ReSharper disable once PossibleNullReferenceException
                var config = ConfigWatcher.CameraConfigs[configIndex];
                if (config == null)
                {
                    MelonLogger.Warning($"Camera {configIndex} in config is null");
                    return;
                }

                var debug = config.Debug;

                var child = new GameObject($"Modded Camera {configIndex}");
                var childTransform = child.transform;
                var parent = _cameraParent.transform;

                if (debug) MelonLogger.Msg("Creating " + child.name);

                if (!string.IsNullOrWhiteSpace(config.ParentGameObject))
                {
                    if (config.CameraIndex > -1)
                        MelonLogger.Warning(
                            $"Both CameraIndex and ParentGameObject are set for Camera {configIndex}, using ParentGameObject"
                        );

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
                else if (config.CameraIndex > -1)
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
                        if (debug) MelonLogger.Msg($"Using camera '{parent.name}' (index {config.CameraIndex}) as parent");
                    }
                }
                else if (config.Debug)
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
                    if (debug) MelonLogger.Msg($"Ascension {j} to {parent.name}");
                }

                childTransform.parent = parent;


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
                if (config.PositionIgnoresScale)
                    childTransform.position =
                        parent.TransformDirection(config.LocalPosition) +
                        parent.position;
                else
                    childTransform.localPosition = config.LocalPosition;

                if (debug)
                {
                    var position = childTransform.localPosition;
                    MelonLogger.Msg($"Using local position {position.x},{position.x},{position.z}");
                }

                var eulerOrRotation = config.UseRotation || config.UseEuler;

                if (config.UseEuler)
                {
                    if (config.UseRotation) MelonLogger.Warning("Both UseEuler and UseRotation are true, using Euler");

                    childTransform.localEulerAngles = config.EulerAngles;
                }
                else if (config.UseRotation)
                {
                    childTransform.localRotation = config.LocalRotation;
                }
                else
                {
                    childTransform.LookAt(
                        parent.transform,
                        config.StartUpright ? Vector3.up : parent.transform.up
                    );
                }

                var localRotation = childTransform.localRotation;
                if (debug)
                {
                    var word = eulerOrRotation ? "local" : "look";
                    MelonLogger.Msg(
                        $"Using {word} rotation {localRotation.x},{localRotation.y},{localRotation.z},{localRotation.w}"
                    );
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
                camera.fieldOfView = config.FieldOfView;
                camera.rect = config.Rect;
                camera.cullingMask = (int) config.CullingMask;

                camera.eventMask = 0;
                camera.stereoTargetEye = StereoTargetEyeMask.None;

                if (config.UseRenderTexture)
                {
                    if (debug) MelonLogger.Msg($"Creating render texture #{_renderTextures.Count}");
                    var renderTexture = new RenderTexture(
                        config.RenderTextureWidth,
                        config.RenderTextureHeight,
                        config.RenderTextureDepth,
                        RenderTextureFormat.ARGB32
                    );

                    camera.targetTexture = renderTexture;
                    camera.aspect = config.RenderTextureWidth / (float) config.RenderTextureHeight;
                    camera.rect = new Rect(0, 0, 1, 1);

                    var renderRect = new Rect(
                        config.Rect.X,
                        1 - config.Rect.Y - config.Rect.Height,
                        config.Rect.Width,
                        config.Rect.Height
                    );

                    _renderTextures.Add(
                        new RenderTextureData
                        {
                            Camera = camera,
                            RenderTexture = renderTexture,
                            Rect = renderRect,
                            ScaleMode = config.RenderTextureScaleMode,
                        }
                    );
                }

                if (config.ForceUpdatePosition)
                {
                    if (debug) MelonLogger.Msg($"Creating position data #{_positions.Count}");
                    _positions.Add(
                        new PositionData
                        {
                            Camera = camera,
                            PositionIgnoresScale = config.PositionIgnoresScale,
                            StartUpright = config.StartUpright,
                            UseRotation = eulerOrRotation,
                            Position = config.LocalPosition,
                            Rotation = localRotation,
                            Transform = childTransform,
                        }
                    );
                }

                if (config.ForceUpdateFov)
                {
                    if(debug) MelonLogger.Msg($"Creating fov data #{_fovs.Count}");
                    _fovs.Add(
                        new FovData
                        {
                            Camera = camera,
                            Fov = config.FieldOfView,
                        }
                    );
                }

                if (eulerOrRotation && config.StartUpright)
                {
                    var diff = Quaternion.FromToRotation(parent.up, Vector3.up);
                    var rotation = diff * childTransform.rotation;
                    childTransform.rotation = rotation;

                    if (debug)
                        MelonLogger.Msg(
                            $"Moved child to local rotation  {rotation.x},{rotation.y},{rotation.z},{rotation.w}"
                        );
                }

                if (!debug) return;
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

            for (var i = 0; i < cameraCount; i++)
            {
                CreateCamera(i);
            }
        }
    }
}
