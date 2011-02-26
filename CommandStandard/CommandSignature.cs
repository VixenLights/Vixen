using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandStandard {
    public class CommandSignature {
        public CommandSignature(string name, byte platform, byte category, byte index, params CommandParameterSpecification[] parameters) {
            Name = name;
            CommandPlatform = platform;
            CommandCategory = category;
            CommandIndex = index;
            Parameters = parameters;
            Identifier = new CommandIdentifier(platform, category, index);
        }

        public string Name { get; private set; }
        public byte CommandPlatform { get; private set; }
        public byte CommandCategory { get; private set; }
        public byte CommandIndex { get; private set; }
        public CommandIdentifier Identifier { get; private set; }
        public CommandParameterSpecification[] Parameters { get; private set; }

		/// <summary>
		/// Returns the full method signature of the command.
		/// </summary>
		/// <returns></returns>
		public override string ToString() {
			string parameters = string.Join(",",
				(from parameter in this.Parameters
				select parameter.ToString()).ToArray());
			return string.Format("{0}({1})", parameters);
		}
    }
}
