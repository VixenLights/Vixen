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

		private string m_parameterString;
		private string m_name;

		public CommandParameterSpecification(string name, Type type) {
			Name = name;
			this.Type = type;
			m_parameterString = _CreateParameterString();
		}

		internal CommandParameterSpecification(string parameterString) {
			string[] parts = parameterString.Split(TYPE_NAME_DELIMITER);
			if(parts.Length != 2) {
				throw new Exception(string.Format("Invalid parameter specification: \"{0}\"", parameterString));
			}
			this.Type = System.Type.GetType(parts[0].Trim(), true, true);
			Name = parts[1].Trim();
			m_parameterString = _CreateParameterString();
		}

		private string _CreateParameterString() {
			return string.Format("{0}{1}{2}", this.Type.ToString(), TYPE_NAME_DELIMITER, Name);
		}

		public string Name {
			get { return m_name; }
			internal set {
				if(value.Length == 0) throw new Exception("Parameters cannot have empty names.");
				if(value.Contains(" ")) throw new Exception(string.Format("Parameter name ({0}) cannot contain spaces.", value));
				m_name = value;
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
			return m_parameterString;
		}
	}

}
