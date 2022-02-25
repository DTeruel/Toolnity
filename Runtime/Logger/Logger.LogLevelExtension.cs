using System;
using System.Collections.Generic;
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

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoad()
        {
            defaultDefaultLogLevel = DefaultLogLevel.All;
            LogsFiltered.Clear();

            var anyFileFound = false;
            var loggerLogLevels = Resources.FindObjectsOfTypeAll<LogLevels>();
            if (loggerLogLevels.Length > 0)
            {
                anyFileFound = true;
                for (var i = 0; i < loggerLogLevels.Length; i++)
                {
                    UnityEngine.Debug.Log("[Logger] File found: " + loggerLogLevels[i].name, loggerLogLevels[i]);
                    LoadLogLevelAsset(loggerLogLevels[i]);
                    defaultDefaultLogLevel = loggerLogLevels[i].defaultLogLevel;
                }
            }

            var file = Resources.Load<LogLevels>("Logger Log Levels");
            if (file)
            {
                anyFileFound = true;
                UnityEngine.Debug.Log("[Logger] File found: " + file.name, file);
                LoadLogLevelAsset(file);
                defaultDefaultLogLevel = file.defaultLogLevel;
            }

            if (!anyFileFound)
            {
                UnityEngine.Debug.Log("[Logger] No 'Logger Log Levels' file found in the Resources folder.");
            }

            SetDefaultLogLevel(defaultDefaultLogLevel);
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