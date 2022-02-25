using System;
using UnityEngine;

namespace Toolnity
{
	[CreateAssetMenu(menuName = "Toolnity/Logger Log Levels", fileName = "Logger Log Levels")]
	public class LogLevels : ScriptableObject
	{
		public Logger.DefaultLogLevel defaultLogLevel;
		public LogLevel[] logLevels;
	}

	[Serializable]
	public class LogLevel
	{
		public string name;
		public Logger.LogLevel logLevel = Logger.LogLevel.Inherit;
	}
}