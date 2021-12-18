﻿using System;
using System.Collections.Generic;
using System.IO;
using MelonLoader;
using MelonLoader.TinyJSON;
using Tomlet;
using UnityEngine;

namespace MelonCameraMod
{
    static class ConfigWatcher
    {
        private const string FileName = "CameraConfig.toml";
        private const string OldFileName = "CameraConfig.json";

        private static readonly string FileDirectory = Path.Combine(
            Environment.CurrentDirectory,
            "UserData"
        );

        private static readonly string FullPath = Path.Combine(
            FileDirectory,
            FileName
        );
        private static readonly string OldFullPath = Path.Combine(
            FileDirectory,
            OldFileName
        );

        public static FullConfig Config;

        private static readonly FileSystemWatcher FileSystemWatcher;
        private static bool _dirty = false;

        static ConfigWatcher()
        {
            TransferOldConfig();

            FileSystemWatcher = new FileSystemWatcher(FileDirectory, FileName)
            {
                NotifyFilter = (NotifyFilters)((1 << 9) - 1),
                EnableRaisingEvents = true
            };
            FileSystemWatcher.Changed += (_, __) => _dirty = true;
            FileSystemWatcher.Created += (_, __) => _dirty = true;
            FileSystemWatcher.Renamed += (_, __) => _dirty = true;
            FileSystemWatcher.Deleted += (_, __) => _dirty = true;
            _dirty = true;
        }


        private static void TransferOldConfig()
        {
            if (!File.Exists(OldFullPath)) return;

            var movedOldFullPath = OldFullPath + ".old";

            if (File.Exists(movedOldFullPath))
            {
                File.Delete(movedOldFullPath);
            }


            File.Move(OldFullPath, movedOldFullPath);

            MelonLogger.Msg($"Found json config at \"{OldFullPath}\", converting to toml config");

            List<CameraConfig> oldConfigs;
            try
            {
                var json = File.ReadAllText(movedOldFullPath);
                JSON.MakeInto(JSON.Load(json), out oldConfigs);
            }
            catch (Exception e)
            {
                MelonLogger.Error(e.ToString());
                MelonLogger.Msg(
                    "Something went wrong when deserializing json. Check the ReadMe in case something has changed"
                );
                return;
            }

            Config = new FullConfig
            {
                CameraConfigs = oldConfigs,
            };

            try
            {
                MelonLogger.Msg(
                    $"Creating toml file based on old json file at \"{FullPath}\""
                );

                var toml = TomletMain.TomlStringFrom(Config);
                File.WriteAllText(FullPath, toml);
            }
            catch (Exception e)
            {
                MelonLogger.Error(e.ToString());
                MelonLogger.Msg(
                    "Something went wrong when serializing toml"
                );
            }
        }
        public static void Unload()
        {
            FileSystemWatcher.EnableRaisingEvents = false;
            _dirty = false;
        }

        public static bool UpdateIfDirty()
        {
            if (!_dirty) return false;
            _dirty = false;

            if (!File.Exists(FullPath))
            {
                MelonLogger.Msg(
                    $"Creating default config file at \"{FullPath}\""
                );
                var sampleConfig = new FullConfig
                {
                    CameraConfigs = new List<CameraConfig>
                    {
                        new CameraConfig(),
                        new CameraConfig
                        {
                            Aspect = 1,
                            Rect = new SerializedRect(0, 0, 0.25f, 0.25f),
                            UseAspect = true,
                            UseRotation = true,
                        }
                    }
                };

                var toml = TomletMain.TomlStringFrom(sampleConfig);
                File.WriteAllText(FullPath, toml);
            }

            MelonLogger.Msg("Updating camera configs");
            
            try
            {
                var toml = File.ReadAllText(FullPath);
                Config = TomletMain.To<FullConfig>(toml);
            }
            catch (Exception e)
            {
                MelonLogger.Error(e.ToString());
                MelonLogger.Msg(
                    "Something went wrong when deserializing toml. Check the ReadMe in case something has changed"
                );
            }

            return true;
        }
    }
}
