using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.Property {
	public interface IProperty {
		ChannelNode Owner { get; set; }
		void Setup();
		//string[] GetPropertyValueNames();
		//string GetPropertyValue(string valueName);
	}
}
