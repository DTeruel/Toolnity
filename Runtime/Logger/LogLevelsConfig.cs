using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toolnity
{
	[CreateAssetMenu(menuName = "Toolnity/Logger Log Levels", fileName = "Logger Log Levels")]
	public class LogLevelsConfig : ScriptableObject
	{
		public Logger.DefaultLogLevel defaultLogLevel;
		public List<LogConfig> logsConfig;
	}

	[Serializable]
	public class LogConfig
	{
		public string name;
		public Logger.LogLevel logLevel;
		public LogConfig(string name, Logger.LogLevel logLevel)
		{
			this.name = name;
			this.logLevel = logLevel;
		}
	}
}