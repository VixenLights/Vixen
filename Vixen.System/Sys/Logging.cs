using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using System.Dynamic;
using System.IO;
using Vixen.Sys.Attribute;

namespace Vixen.Sys {
	// Cannot be static because of DynamicObject inheritance.
	public class Logging : DynamicObject {
		private Dictionary<string, Log> _logs = new Dictionary<string, Log>();
		private object _locker = new object();

		static public event EventHandler<LogEventArgs> ItemLogged;

		private const string LOG_FILE_EXT = ".log";

		[DataPath]
		static private string _logDirectory = Path.Combine(Paths.DataRootPath, "Logs");

		public void AddLog(Log log) {
			//Allow them to override the standard logs.
			_logs[log.Name] = log;
			log.ItemLogged += _ItemLogged;
		}

		public void RemoveLog(string logName) {
			Log log;
			if(_logs.TryGetValue(logName, out log)) {
				_logs.Remove(logName);
				log.ItemLogged -= _ItemLogged;
			}
		}

		public IEnumerable<string> LogNames {
			get { return _logs.Keys.ToArray(); }
		}

		public IEnumerable<string> RetrieveLogContents(string logName) {
			Log log;
			if(_logs.TryGetValue(logName, out log)) {
				string logFilePath = _GetLogFileName(log);
				if(File.Exists(logFilePath)) {
					return File.ReadAllLines(logFilePath);
				}
				return new string[0];
			}
			// Log does not exist.
			return null;
		}

		private void _ItemLogged(object sender, LogItemEventArgs e) {
			Log log = sender as Log;
			string logFileName = _GetLogFileName(log);
			lock(_locker) {
				File.AppendAllText(logFileName, e.Text);
			}

			if(ItemLogged != null) {
				ItemLogged(this, new LogEventArgs(log.Name, e.Text));
			}
		}

		private string _GetLogFileName(Log log) {
			return Path.Combine(_logDirectory, log.Name) + LOG_FILE_EXT;
		}

		// i.e. Error("I had an error.")
		// i.e. Error(exception)
		// i.e. Error("You can't do that.", exception)
		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result) {
			result = null;
			Log log;
			if(_logs.TryGetValue(binder.Name, out log)) {
				try {
					if(args.Length == 1) {
						if(args[0] is string) {
							log.Write(args[0] as string);
							return true;
						} else if(args[0] is Exception) {
							log.Write(args[0] as Exception);
							return true;
						}
					} else if(args.Length == 2) {
						if(args[0] is string && args[1] is Exception) {
							log.Write(args[0] as string, args[1] as Exception);
							return true;
						}
					}
					return false;
				} catch {
					return false;
				}
			}
			return base.TryInvokeMember(binder, args, out result);
		}
	}
}
