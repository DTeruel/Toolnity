using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Toolnity.Logger
{
    public partial class Logger
    {
        [Serializable]
        public enum DefaultLogLevel
        {
            All = 0,
            Debug = 1,
            Info = 2,
            Warning = 3,
            Error = 4,
            Off = 5
        }
        
        [Serializable]
        public enum LogLevel
        {
            Inherit = 0,
            All = 1,
            Debug = 2,
            Info = 3,
            Warning = 4,
            Error = 5,
            Off = 6
        }

        private static LogLevelsConfig logLevelsAsset;
        private static readonly Dictionary<string, LogLevel> LoggersCreatedInInitializers = new(); 

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnAfterSceneLoad()
        {
            GetOrCreateLogLevelsAsset();
        }

        private static void GetOrCreateLogLevelsAsset()
        {
            if (!LoadLogLevelsAsset())
            {
                CreateAssetFile();
                LoadLogLevelsAsset();
            }
            CheckLoggersCreatedInInitializers();
        }

        private static bool LoadLogLevelsAsset()
        {
            var allLogLevelsAssets = Resources.LoadAll<LogLevelsConfig>("");
            if (allLogLevelsAssets.Length > 0)
            {
                logLevelsAsset = allLogLevelsAssets[0];
                UnityEngine.Debug.Log("[Logger] File found: " + logLevelsAsset.name, logLevelsAsset);

                return true;
            }

            return false;
        }

        private static void CheckLoggersCreatedInInitializers()
        {
            if (logLevelsAsset.logsConfig == null)
            {
                logLevelsAsset.logsConfig = new List<LogConfig>();
            }
            foreach(var loggerToAdd in LoggersCreatedInInitializers)
            {
                CheckOrAddLogger(loggerToAdd.Key, loggerToAdd.Value);
            }
            LoggersCreatedInInitializers.Clear();
        }

        private static void CheckOrAddLogger(string name, LogLevel loggerLogLevel)
        {
            var found = false;
            for (var i = 0; i < logLevelsAsset.logsConfig.Count; i++)
            {
                if (logLevelsAsset.logsConfig[i].name == name)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                logLevelsAsset.logsConfig.Add(new LogConfig(name, loggerLogLevel));
            }
        }

        private static void CreateAssetFile()
        { 
            #if UNITY_EDITOR
            UnityEngine.Debug.Log("[Logger] No 'Logger Log Levels' file found in the Resources folders. Creating a new one in \"\\Assets\\Resources\"");
            
            logLevelsAsset = ScriptableObject.CreateInstance<LogLevelsConfig>();
            const string pathFolder = "Assets/Resources/";
            const string assetName = "Logger Log Level.asset";
            if (!Directory.Exists("Assets/Resources"))
            {
                Directory.CreateDirectory("Assets/Resources");
            }
            AssetDatabase.CreateAsset(logLevelsAsset, pathFolder + assetName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            #else
            UnityEngine.Debug.LogError("[Logger] No 'Logger Log Levels' file found in the Resources folders. Create one in the editor. ");
            #endif
        }

        public static void SetDefaultLogLevel(DefaultLogLevel value)
        {
            logLevelsAsset.defaultLogLevel = value;
            UnityEngine.Debug.Log("[Logger] Default Log Level set as: " + logLevelsAsset.defaultLogLevel);
        }
    }
}