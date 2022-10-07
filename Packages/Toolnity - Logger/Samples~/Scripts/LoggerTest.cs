using UnityEngine;

namespace Toolnity.Test
{
    using Logger;

    public class LoggerTest : MonoBehaviour
    {
        private static readonly Logger LOGGER = Logger.Create<LoggerTest>();

        private void Start()
        {
            LOGGER.Debug("Debug log."); // This will be skipped by asset file
            LOGGER.Info("Info log.");
            LOGGER.Warning("Warning log.", gameObject);
            LOGGER.Error("Error log.", gameObject);

            var logLevel = LOGGER.GetLogLevel();
            LOGGER.Info("This Log Level: " + logLevel);
        }
    }
}