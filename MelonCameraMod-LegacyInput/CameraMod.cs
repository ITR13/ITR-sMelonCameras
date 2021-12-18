using System;
using MelonCameraMod_LegacyInput;
using MelonLoader;

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
            CameraHandler.UpdateFovs();
        }

        public override void OnGUI() => CameraHandler.BlitRenderTextures();
    }
}
