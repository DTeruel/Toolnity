using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Toolnity
{
    public partial class Logger
    {
        private string logName;
        private LogLevel logLevel;

        public static Logger Create<T>(LogLevel newLogLevel = LogLevel.Inherit)
        {
            var newLogger = new Logger(typeof(T).FullName, newLogLevel);
            return newLogger;
        }

        public static Logger Create(string fullName, LogLevel newLogLevel = LogLevel.Inherit)
        {
            var newLogger = new Logger(fullName, newLogLevel);
            return newLogger;
        }

        public Logger(string fullName, LogLevel newLogLevel = LogLevel.Inherit)
        {
            CreateLogger(fullName, newLogLevel);
        }

        private void CreateLogger(string fullName, LogLevel newLogLevel)
        {
            logName = fullName;
            logLevel = newLogLevel;
            CheckLoggerIfIsInLogLevelsAsset();
        }

        private static Dictionary<string, LogLevel> loggersCreatedInInitializers = new (); 
        
        private void CheckLoggerIfIsInLogLevelsAsset()
        {
            if (logLevelsAsset == null)
            {
                if (!loggersCreatedInInitializers.ContainsKey(logName))
                {
                    loggersCreatedInInitializers.Add(logName, logLevel);
                }
                return;
            }
            
            var alreadyInAsset = false;
            for (var i = 0; i < logLevelsAsset.logsConfig.Count; i++)
            {
                if (logLevelsAsset.logsConfig[i].name == logName)
                {
                    alreadyInAsset = true;
                    logLevel = logLevelsAsset.logsConfig[i].logLevel;
                    break;
                }
            }
            if (!alreadyInAsset)
            {
                logLevelsAsset.logsConfig.Add(new LogConfig(logName, logLevel));
            }
        }

        public void Debug(string message, Object obj = null)
        {
            if (!IsLogLevelAllowed(DefaultLogLevel.Debug))
            {
                return;
            }
            UnityEngine.Debug.Log(GetFinalMessage(logName, message), obj);
        }

        public void Info(string message, Object obj = null)
        {
            if (!IsLogLevelAllowed(DefaultLogLevel.Info))
            {
                return;
            }
            UnityEngine.Debug.Log(GetFinalMessage(logName, message), obj);
        }

        public void Warning(string message, Object obj = null)
        {
            if (!IsLogLevelAllowed(DefaultLogLevel.Warning))
            {
                return;
            }
            UnityEngine.Debug.LogWarning(GetFinalMessage(logName, message), obj);
        }

        public void Error(string message, Object obj = null)
        {
            if (!IsLogLevelAllowed(DefaultLogLevel.Error))
            {
                return;
            }
            UnityEngine.Debug.LogError(GetFinalMessage(logName, message), obj);
        }

        private static string GetFinalMessage(string name, string message)
        {
            if (name != string.Empty)
            {
                return "[" + name + "] " + message;
            }

            return message;
        }
    }
}