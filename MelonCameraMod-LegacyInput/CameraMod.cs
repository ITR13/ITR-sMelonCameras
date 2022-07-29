using System;
using MelonCameraMod;
using MelonCameraMod_LegacyInput;
using MelonLoader;

[assembly: MelonInfo(typeof(CameraMod), "ITR's Melon Cameras", "1.5.1", "ITR")]
namespace MelonCameraMod
{
    public class CameraMod : MelonMod
    {
        public static Action<string> Msg;
        public static Action<string> Warning;
        public static Action<string> Error;

        public override void OnApplicationStart()
        {
            Msg = LoggerInstance.Msg;
            Warning = LoggerInstance.Warning;
            Error = LoggerInstance.Error;
        }

        public override void OnApplicationQuit()
        {
            ConfigWatcher.Unload();
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName) => CameraHandler.ForceReload();

        public override void OnUpdate()
        {
            CameraHandler.CheckToggles();
            CameraHandler.MaybeUpdateConfigOrCameras();
        }

        public override void OnLateUpdate()
        {
            CameraHandler.UpdatePositions();
            CameraHandler.UpdateSmooths();
            CameraHandler.UpdateFovs();
        }

        public override void OnGUI() => CameraHandler.BlitRenderTextures();
    }
}
