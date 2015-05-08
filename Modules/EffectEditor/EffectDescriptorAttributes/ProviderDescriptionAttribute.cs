using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VixenModules.EffectEditor.EffectDescriptorAttributes
{
	public class ProviderDescriptionAttribute:DescriptionAttribute
	{
		public ProviderDescriptionAttribute(string keyName):base(EffectResourceManager.GetDescriptionString(keyName))
		{
			
		}
	}
}
