using System;
using Catel.Logging;
using NLog;
using LogManager = NLog.LogManager;

namespace VixenApplication
{
	/// <summary>
	/// This class adds NLog as an appender to the Catel Framework logging
	/// </summary>
	public class NLogListener : LogListenerBase
	{
		private static Logger Logging = LogManager.GetCurrentClassLogger();

		#region Overrides of LogListenerBase

		/// <inheritdoc />
		protected override void Debug(ILog log, string message, object extraData, LogData logData, DateTime time)
		{
			Logging.Debug(message);
		}

		protected override void Info(ILog log, string message, object extraData, LogData logData, DateTime time)
		{
			Logging.Info(message);
		}

		protected override void Warning(ILog log, string message, object extraData, LogData logData, DateTime time)
		{
			Logging.Warn(message);
		}

		protected override void Error(ILog log, string message, object extraData, LogData logData, DateTime time)
		{
			Logging.Error(message);
		}

		#endregion

	}
}
