using System;

namespace Vixen.Sys
{
	public class ParameterSpecification
	{
		private const char TYPE_NAME_DELIMITER = ' ';

		private string _name;

		public ParameterSpecification(string name, Type type, bool showLabel = true)
		{
			Name = name;
			Type = type;
			ShowLabel = showLabel;
		}

		public bool ShowLabel { get; set; }

		internal ParameterSpecification(string parameterString)
		{
			string[] parts = parameterString.Split(TYPE_NAME_DELIMITER);
			if (parts.Length != 2) {
				throw new Exception(string.Format("Invalid parameter specification: \"{0}\"", parameterString));
			}
			Type = Type.GetType(parts[0].Trim(), true, true);
			Name = parts[1].Trim();
		}

		public string Name
		{
			get { return _name; }
			internal set
			{
				if (string.IsNullOrWhiteSpace(value)) throw new Exception("Parameters cannot have empty names.");
				_name = value;
			}
		}

		public Type Type { get; internal set; }

		public override string ToString()
		{
			return Type + " " + Name;
		}
	}
}