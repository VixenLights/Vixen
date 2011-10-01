using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Vixen.Commands {
	public class CommandParameterSpecification {
		private const char TYPE_NAME_DELIMITER = ' ';

		private string _name;

		public CommandParameterSpecification(string name, Type type) {
			Name = name;
			this.Type = type;
		}

		internal CommandParameterSpecification(string parameterString) {
			string[] parts = parameterString.Split(TYPE_NAME_DELIMITER);
			if(parts.Length != 2) {
				throw new Exception(string.Format("Invalid parameter specification: \"{0}\"", parameterString));
			}
			this.Type = System.Type.GetType(parts[0].Trim(), true, true);
			Name = parts[1].Trim();
		}

		public string Name {
			get { return _name; }
			internal set {
				if(value == null) throw new ArgumentNullException("Name");
				if(value.Length == 0) throw new Exception("Parameters cannot have empty names.");
				_name = value;
			}
		}

		public Type Type { get; internal set; }

		public override string ToString() {
			return this.Type.ToString() + " " + Name;
		}
	}

}
