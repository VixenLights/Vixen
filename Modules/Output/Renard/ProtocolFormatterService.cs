using System;
using System.Linq;

namespace VixenModules.Output.Renard {
	class ProtocolFormatterService {
		static private ProtocolFormatterService _instance;

		private ProtocolFormatterService() {
		}

		public static ProtocolFormatterService Instance {
			get {
				if(_instance == null) {
					_instance = new ProtocolFormatterService();
				}
				return _instance;
			}
		}

		public IRenardProtocolFormatter FindFormatter(int protocolVersion) {
			System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
			var formatterTypes = assembly.GetTypes().Where(x => x.GetCustomAttributes(typeof(ProtocolVersionAttribute), false).Any());
			Type protocolFormatterImplementation = formatterTypes.FirstOrDefault(x => (x.GetCustomAttributes(typeof(ProtocolVersionAttribute), false).First() as ProtocolVersionAttribute).Version == protocolVersion);
			if(protocolFormatterImplementation != null) {
				return (IRenardProtocolFormatter)Activator.CreateInstance(protocolFormatterImplementation);
			} else {
				throw new Exception("Renard module using an unsupported protocol version (" + protocolVersion + ")");
			}
		}
	}
}
