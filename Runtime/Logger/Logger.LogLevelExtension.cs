using System;
using System.Collections.Generic;
using System.IO;
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

        private static DefaultLogLevel defaultDefaultLogLevel = DefaultLogLevel.All;
        private static readonly Dictionary<string, LogLevel> LogsFiltered = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnAfterSceneLoad()
        {
            defaultDefaultLogLevel = DefaultLogLevel.All;
            LogsFiltered.Clear();

            var allLogLevelsAssets = Resources.LoadAll<LogLevels>("");
            if (allLogLevelsAssets.Length > 0)
            {
                var logLevelsAsset = allLogLevelsAssets[0];
                UnityEngine.Debug.Log("[Logger] File found: " + logLevelsAsset.name, logLevelsAsset);
                LoadLogLevelAsset(logLevelsAsset);
                defaultDefaultLogLevel = logLevelsAsset.defaultLogLevel;
            }
            else
            {
                UnityEngine.Debug.Log("[Logger] No 'Logger Log Levels' file found in the Resources folders.");
            }
            
            SetDefaultLogLevel(defaultDefaultLogLevel);
        }
        private static LogLevels SearchLoggerAsset(string path)
        {
            var directories = System.IO.Directory.GetDirectories(path);
            foreach(var directory in directories)
            {
                var result = SearchLoggerAsset(directory);
                if (result)
                {
                    return result;
                }
            }

            var files = System.IO.Directory.GetFiles(path);
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
                
                var loadedFile = Resources.Load<LogLevels>(splitPathWithoutExtension[0]);
                if (loadedFile)
                {
                    return loadedFile;
                }
            }

            return null;
        }

        public static void SetDefaultLogLevel(DefaultLogLevel value)
        {
            defaultDefaultLogLevel = value;
            UnityEngine.Debug.Log("[Logger] Default Log Level set as: " + defaultDefaultLogLevel);
        }

        private static void LoadLogLevelAsset(LogLevels logLevels)
        {
            for (var i = 0; i < logLevels.logLevels.Length; i++)
            {
                var logLevel = logLevels.logLevels[i];
                if (!LogsFiltered.ContainsKey(logLevel.name))
                {
                    LogsFiltered.Add(logLevel.name, logLevel.logLevel);
                }
            }
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
                    allowed = defaultDefaultLogLevel <= logLevelToCheck;
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