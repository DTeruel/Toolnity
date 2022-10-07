using Object = UnityEngine.Object;

namespace Toolnity.Logger
{
    public partial class Logger
    {
        private string logName;

        public static Logger Create<T>(LogLevel logLevel = LogLevel.Inherit)
        {
            var newLogger = new Logger(typeof(T).FullName, logLevel);
            return newLogger;
        }

        public static Logger Create(string fullName, LogLevel logLevel = LogLevel.Inherit)
        {
            var newLogger = new Logger(fullName, logLevel);
            return newLogger;
        }

        public Logger(string fullName, LogLevel logLevel = LogLevel.Inherit)
        {
            CreateLogger(fullName, logLevel);
        }

        private void CreateLogger(string fullName, LogLevel logLevel)
        {
            logName = fullName;
            CheckLoggerIfIsInLogLevelsAsset(logLevel);
        }
        
        private void CheckLoggerIfIsInLogLevelsAsset(LogLevel logLevel)
        {
            if (logLevelsAsset == null)
            {
                if (!LoggersCreatedInInitializers.ContainsKey(logName))
                {
                    LoggersCreatedInInitializers.Add(logName, logLevel);
                }
                return;
            }

            CheckOrAddLogger(logName, logLevel);
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

        public void SetLogLevel(LogLevel newLogLevel)
        {
            var found = false;
            for (var i = 0; i < logLevelsAsset.logsConfig.Count; i++)
            {
                if (logLevelsAsset.logsConfig[i].name == logName)
                {
                    found = true;
                    logLevelsAsset.logsConfig[i].logLevel = newLogLevel;
                    break;
                }
            }
            if (!found)
            {
                logLevelsAsset.logsConfig.Add(new LogConfig(logName, newLogLevel));
            }
            UnityEngine.Debug.Log("["+ logName+ "] Log Level set as: " + newLogLevel);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public LogLevel GetLogLevel()
        {
            for (var i = 0; i < logLevelsAsset.logsConfig.Count; i++)
            {
                if (logLevelsAsset.logsConfig[i].name == logName)
                {
                    return logLevelsAsset.logsConfig[i].logLevel;
                }
            }

            logLevelsAsset.logsConfig.Add(new LogConfig(logName, LogLevel.Inherit));
            
            return LogLevel.Inherit;
        }

        private bool IsLogLevelAllowed(DefaultLogLevel logLevelToCheck)
        {
            bool allowed;
            var logLevel = GetLogLevel();
            
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