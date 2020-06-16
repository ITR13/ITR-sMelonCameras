using System;
using System.Collections.Generic;
using System.IO;
using MelonLoader;
using TinyJSON;
using UnityEngine;

namespace MelonCameraMod
{
    static class ConfigWatcher
    {
        private const string FileName = "CameraConfig.json";

        private static readonly string FileDirectory =
            Application.dataPath + "/../";

        private static readonly string FullPath = FileDirectory + FileName;

        public static List<CameraConfig> CameraConfigs =
            new List<CameraConfig>();

        private static DateTime _lastUpdate = DateTime.MinValue;

        public static bool UpdateIfDirty()
        {
            if (!File.Exists(FullPath))
            {
                MelonModLogger.Log(
                    $"Creating default config file at \"{FullPath}\""
                );
                var sampleConfig = new List<CameraConfig>
                {
                    new CameraConfig(),
                    new CameraConfig
                    {
                        Aspect = 1,
                        LocalRotation = Quaternion.identity,
                        Rect = new Rect(0, 0, 0.25f, 0.25f),
                        UseAspect = true,
                        UseRotation = true,
                    }
                };

                var json = JSON.Dump(
                    sampleConfig,
                    EncodeOptions.PrettyPrint | EncodeOptions.NoTypeHints
                );
                File.WriteAllText(FullPath, json);
            }

            var lastWriteTime = File.GetLastWriteTime(FullPath);
            if (lastWriteTime <= _lastUpdate)
            {
                return false;
            }

            _lastUpdate = lastWriteTime;

            MelonModLogger.Log("Updating camera configs");

            CameraConfigs.Clear();

            try
            {
                var json = File.ReadAllText($"./{FileName}");
                JSON.MakeInto(JSON.Load(json), out CameraConfigs);
            }
            catch (System.Exception e)
            {
                MelonModLogger.LogError(e.ToString());
            }

            return true;
        }
    }
}
