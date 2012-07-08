using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VixenModules.App.Scheduler {
	class EnumDescriptionAttribute : Attribute {
		public EnumDescriptionAttribute(string value) {
			Value = value;
		}

		public string Value { get; private set; }
	}
}
