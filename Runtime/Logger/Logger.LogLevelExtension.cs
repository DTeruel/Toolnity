using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Toolnity
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
            foreach(var loggerToAdd in loggersCreatedInInitializers)
            {
                var found = false;
                for (var i = 0; i < logLevelsAsset.logsConfig.Count; i++)
                {
                    if (logLevelsAsset.logsConfig[i].name == loggerToAdd.Key)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    logLevelsAsset.logsConfig.Add(new LogConfig(loggerToAdd.Key, loggerToAdd.Value));
                }
            }
            loggersCreatedInInitializers.Clear();
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
/*
        private static LogLevelsConfig SearchLoggerAsset(string path)
        {
            var directories = Directory.GetDirectories(path);
            foreach(var directory in directories)
            {
                var result = SearchLoggerAsset(directory);
                if (result)
                {
                    return result;
                }
            }

            var files = Directory.GetFiles(path);
            foreach(var file in files)
            {
                var fullPath = Path.GetFullPath(file);
                if (!fullPath.Contains("Resources"))
                {
                    continue;
                }

                var splitPath = fullPath.Split("Resources\\");
                var pathRelativeToResources = splitPath[splitPath.Length - 1];
                var splitPathWithoutExtension = pathRelativeToResources.Split(".asset");
                if (splitPathWithoutExtension.Length < 1)
                {
                    continue;
                }
                
                var loadedFile = Resources.Load<LogLevelsConfig>(splitPathWithoutExtension[0]);
                if (loadedFile)
                {
                    return loadedFile;
                }
            }

            return null;
        }*/

        public static void SetDefaultLogLevel(DefaultLogLevel value)
        {
            logLevelsAsset.defaultLogLevel = value;
            UnityEngine.Debug.Log("[Logger] Default Log Level set as: " + logLevelsAsset.defaultLogLevel);
        }

        public void SetLogLevel(LogLevel value)
        {
            logLevel = value;
            UnityEngine.Debug.Log("["+ logName+ "] Log Level set as: " + logLevel);
        }

        private bool IsLogLevelAllowed(DefaultLogLevel logLevelToCheck)
        {
            bool allowed;
            
            switch (logLevel)
            {
                case LogLevel.Inherit:
                    allowed = logLevelsAsset.defaultLogLevel <= logLevelToCheck;
                    break;
                case LogLevel.All:
                    allowed = true;
                    break;
                case LogLevel.Debug:
                    allowed = logLevelToCheck >= DefaultLogLevel.Debug;
                    break;
                case LogLevel.Info:
                    allowed = logLevelToCheck >= DefaultLogLevel.Info;
                    break;
                case LogLevel.Warning:
                    allowed = logLevelToCheck >= DefaultLogLevel.Warning;
                    break;
                case LogLevel.Error:
                    allowed = logLevelToCheck >= DefaultLogLevel.Error;
                    break;
                case LogLevel.Off:
                default:
                    allowed = false;
                    break;
            }

            return allowed;
        }
    }
}