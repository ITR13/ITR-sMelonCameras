using System.Collections.Generic;
using System.Linq;
using System.Text;
using Il2CppSystem;
using MelonCameraMod;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MelonCameraMod_LegacyInput
{
    public static class CameraHandler
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
            public Transform[] PositionMarkers;
        }

        private class FovData
        {
            public Camera Camera;
            public float Fov;
        }

        private static GameObject _cameraParent;
        private static readonly List<CameraData> _cameras = new List<CameraData>();
        private static readonly List<RenderTextureData> _renderTextures = new List<RenderTextureData>();
        private static readonly List<PositionData> _positions = new List<PositionData>();
        private static readonly List<FovData> _fovs = new List<FovData>();

        public static void ForceReload() => _cameraParent = null;

        public static void CheckToggles()
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

        public static void MaybeUpdateConfigOrCameras()
        {
            if (!ConfigWatcher.UpdateIfDirty() && _cameraParent != null) return;

            var mainCamera = Camera.main;
            if (mainCamera == null) return;
            _cameraParent = mainCamera.gameObject;

            UpdateCameras();
        }

        public static void UpdatePositions()
        {
            foreach (var positionData in _positions)
            {
                if (!positionData.Enabled) continue;

                var parent = positionData.Transform.parent;

                var offset = Vector3.zero;
                if (positionData.PositionMarkers.Length > 0)
                {
                    var minY = positionData.PositionMarkers[0].position.y;
                    foreach (var marker in positionData.PositionMarkers)
                    {
                        var markerPos = marker.position;
                        offset += markerPos;
                        if (minY < markerPos.y) minY = markerPos.y;
                    }

                    offset /= positionData.PositionMarkers.Length;
                    offset.y = minY;
                    offset -= parent.position;
                }

                if (positionData.PositionIgnoresScale)
                {
                    positionData.Transform.position = 
                        parent.TransformDirection(positionData.Position) +
                        parent.position + offset;
                }
                else
                {
                    positionData.Transform.localPosition = positionData.Position + offset;
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

        public static void UpdateFovs()
        {
            foreach (var fovData in _fovs)
            {
                if (fovData.Camera == null) continue;
                fovData.Camera.fieldOfView = fovData.Fov;
            }
        }

        public static void BlitRenderTextures()
        {
            if (!Event.current.type.Equals(EventType.Repaint)) return;
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

        public static void UpdateCameras()
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

            var cameraCount = ConfigWatcher.Config?.CameraConfigs?.Count ?? 0;
            CameraMod.Msg($"Creating {cameraCount} cameras");

            void CreateCamera(int configIndex)
            {
                // ReSharper disable once PossibleNullReferenceException
                var config = ConfigWatcher.Config.CameraConfigs[configIndex];
                if (config == null)
                {
                    CameraMod.Warning($"Camera {configIndex} in config is null");
                    return;
                }

                var debug = config.Debug;

                var child = new GameObject($"Modded Camera {configIndex}");
                var childTransform = child.transform;
                var parent = _cameraParent.transform;

                if (debug) CameraMod.Msg("Creating " + child.name);

                if (!string.IsNullOrWhiteSpace(config.ParentGameObject))
                {
                    if (config.CameraIndex > -1)
                        CameraMod.Warning(
                            $"Both CameraIndex and ParentGameObject are set for Camera {configIndex}, using ParentGameObject"
                        );

                    var newParent = GameObject.Find(config.ParentGameObject);
                    if (newParent == null)
                    {
                        CameraMod.Msg($"Failed to find gameobject '{config.ParentGameObject}'");
                    }
                    else
                    {
                        if (debug) CameraMod.Msg($"Using obj '{newParent.name}' as parent");
                        parent = newParent.transform;
                    }
                }
                else if (config.CameraIndex > -1)
                {
                    var allCamerasCount = Camera.allCamerasCount;
                    if (config.CameraIndex >= allCamerasCount)
                    {
                        CameraMod.Msg(
                            $"Failed to find camera with index {config.CameraIndex} (only {allCamerasCount} exist)"
                        );
                    }
                    else
                    {
                        parent = Camera.allCameras[config.CameraIndex].gameObject.transform;
                        if (debug) CameraMod.Msg($"Using camera '{parent.name}' (index {config.CameraIndex}) as parent");
                    }
                }
                else if (config.Debug)
                {
                    CameraMod.Msg("Using main camera obj as parent");
                }

                for (var j = 0; j < config.ParentAscension; j++)
                {
                    if (parent.parent == null)
                    {
                        CameraMod.Msg($"Failed to ascend parent {j} times (goal was {config.ParentAscension})");
                        break;
                    }

                    parent = parent.parent;
                    if (debug) CameraMod.Msg($"Ascension {j} to {parent.name}");
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

                var positionMarkers = new Transform[0];
                if (config.UseAvatarAsBasePosition)
                {
                    // Not properly implemented!
                    positionMarkers = FindPositionMarkers(debug, configIndex, parent, parent.parent);
                }

                if (config.PositionIgnoresScale)
                    childTransform.position =
                        parent.TransformDirection(config.LocalPosition) +
                        parent.position;
                else
                    childTransform.localPosition = config.LocalPosition;

                if (debug)
                {
                    var position = childTransform.localPosition;
                    CameraMod.Msg($"Using local position {position.x},{position.x},{position.z}");
                }

                var eulerOrRotation = config.UseRotation || config.UseEuler;

                if (config.UseEuler)
                {
                    if (config.UseRotation) CameraMod.Warning("Both UseEuler and UseRotation are true, using Euler");

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
                    CameraMod.Msg(
                        $"Using {word} rotation {localRotation.x},{localRotation.y},{localRotation.z},{localRotation.w}"
                    );
                }

                if (config.UseAspect)
                {
                    camera.aspect = config.Aspect;
                    if (debug) CameraMod.Msg($"Using aspect {config.Aspect}");
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
                camera.cullingMask = (int)config.CullingMask;

                camera.eventMask = 0;
                camera.stereoTargetEye = StereoTargetEyeMask.None;

                if (config.UseRenderTexture)
                {
                    if (debug) CameraMod.Msg($"Creating render texture #{_renderTextures.Count}");
                    var renderTexture = new RenderTexture(
                        config.RenderTextureWidth,
                        config.RenderTextureHeight,
                        config.RenderTextureDepth,
                        RenderTextureFormat.ARGB32
                    );

                    camera.targetTexture = renderTexture;
                    camera.aspect = config.RenderTextureWidth / (float)config.RenderTextureHeight;
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
                    if (debug) CameraMod.Msg($"Creating position data #{_positions.Count}");
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
                            PositionMarkers = positionMarkers,
                        }
                    );
                }

                if (config.ForceUpdateFov)
                {
                    if (debug) CameraMod.Msg($"Creating fov data #{_fovs.Count}");
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
                        CameraMod.Msg(
                            $"Moved child to local rotation  {rotation.x},{rotation.y},{rotation.z},{rotation.w}"
                        );
                }

                if (!debug) return;
                var sb = ObjectPath(child, parent);

                sb.Insert(0, "Finished creating camera with path: ");
                CameraMod.Msg(sb.ToString());
            }

            for (var i = 0; i < cameraCount; i++)
            {
                CreateCamera(i);
            }
        }


        private static Transform[] FindPositionMarkers(bool debug, int index, Transform baseObject, Transform parent)
        {
            var animator = baseObject.GetComponentInParent<Animator>();
            if (animator == null)
            {
                animator = baseObject.GetComponentInChildren<Animator>();
            }

            if (animator == null)
            {
                CameraMod.Warning($"Failed to find animator for camera index {index}");
                return new Transform[0];
            }

            if (debug)
            {
                var sb = ObjectPath(baseObject.gameObject, parent);
                sb.Insert(0, "Using animator with path: ");
                CameraMod.Msg(sb.ToString());
            }

            var avatar = animator.avatar;
            if (avatar == null)
            {
                CameraMod.Warning($"Failed to find avatar for camera index {index} (found animator)");
                return new Transform[0];
            }

            if (!avatar.isHuman)
            {
                CameraMod.Warning($"Avatar for camera index {index} is not human, and will be ignored");
                return new Transform[0];
            }

            var possibleTransforms = new Transform[]
            {
                animator.GetBoneTransform(HumanBodyBones.Chest),
                animator.GetBoneTransform(HumanBodyBones.Hips),
                animator.GetBoneTransform(HumanBodyBones.Head),
                animator.GetBoneTransform(HumanBodyBones.RightUpperLeg),
                animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg),
                animator.GetBoneTransform(HumanBodyBones.LeftShoulder),
                animator.GetBoneTransform(HumanBodyBones.RightShoulder),
            };

            return possibleTransforms.Where(t => t != null).ToArray();
        }

        private static StringBuilder ObjectPath(GameObject baseObject, Transform parent)
        {
            var sb = new StringBuilder(baseObject.name, 100);
            var currentParent = parent;
            while (currentParent != null)
            {
                sb.Insert(0, "/");
                sb.Insert(0, currentParent.gameObject.name);
                currentParent = currentParent.parent;
            }

            return sb;
        }
    }
}
