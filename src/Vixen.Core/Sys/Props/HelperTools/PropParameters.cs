using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VixenApplication.SetupDisplay.Wizards.HelperTools
{
	public class PropParameters : Dictionary<string, object>
	{
		public PropParameters()
		{
		}

		public PropParameters(string key, object value)
		{
			Update(key, value);
		}

		public PropParameters Update(string key, object value)
		{
			if (ContainsKey(key))
			{
				this[key] = value;
			}
			else
			{
				Add(key, value);
			}
			return this;
		}

		public object Get(string key)
		{
			if (ContainsKey(key))
			{
				return this[key];
			}
			else
			{
				return null;
			}
		}
	}
}
