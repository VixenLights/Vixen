using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using CommandStandard.Types;

namespace CommandStandard {
	//If this were generic, you couldn't have a collection of them without
	//the generic subclassing a non-generic class.  Any benefits?
	public class CommandParameterSpecification {
		private const char TYPE_NAME_DELIMITER = ' ';

		private const string ATTR_PARAM_NAME = "name";
		private const string ATTR_PARAM_TYPE = "type";

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
				//Still true?
				//-> Assumed to be language-dependent
				//if(value.Contains(" ")) throw new Exception(string.Format("Parameter name ({0}) cannot contain spaces.", value));
				_name = value;
			}
		}

		public Type Type { get; internal set; }

		// Use for self-documentation
		internal void SaveToXml(XmlNode node) {
			// "node" will be the node to write to as previously created by the owner
			XmlAttribute attr;

			attr = node.OwnerDocument.CreateAttribute(ATTR_PARAM_TYPE);
			attr.Value = this.Type.Name;
			node.Attributes.Append(attr);

			attr = node.OwnerDocument.CreateAttribute(ATTR_PARAM_NAME);
			attr.Value = this.Name;
			node.Attributes.Append(attr);
		}

		public override string ToString() {
			return this.Type.ToString() + " " + Name;
		}
	}

}
