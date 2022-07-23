using Object = UnityEngine.Object;

namespace Toolnity
{
    public partial class Logger
    {
        private readonly string logName;
        private LogLevel logLevel;

        public static Logger Create<T>()
        {
            var newLogger = new Logger(typeof(T).FullName);
            return newLogger;
        }

        public Logger(string fullName)
        {
            logName = fullName;
            logLevel = LogLevel.Inherit;
            CheckLoggerIfIsInLogLevelsAsset();
            
            if (logLevel != LogLevel.Off && 
                (logLevel != LogLevel.Inherit || (logLevel == LogLevel.Inherit && LogLevelsAsset.defaultLogLevel != DefaultLogLevel.Off)))
            {
                UnityEngine.Debug.Log("[Logger] New Logger: " + logName + " (Log Level: " + logLevel + ")");
            }
        }

        private void CheckLoggerIfIsInLogLevelsAsset()
        {
            var alreadyInAsset = false;
            for (var i = 0; i < LogLevelsAsset.logsConfig.Count; i++)
            {
                if (LogLevelsAsset.logsConfig[i].name == logName)
                {
                    alreadyInAsset = true;
                    logLevel = LogLevelsAsset.logsConfig[i].logLevel;
                    break;
                }
            }
            if (!alreadyInAsset)
            {
                LogLevelsAsset.logsConfig.Add(new LogConfig(logName, logLevel));
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