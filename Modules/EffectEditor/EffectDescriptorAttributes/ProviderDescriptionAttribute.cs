using System.ComponentModel;

namespace VixenModules.EffectEditor.EffectDescriptorAttributes
{
	public class ProviderDescriptionAttribute:DescriptionAttribute
	{
		public ProviderDescriptionAttribute(string keyName):base(EffectResourceManager.GetDescriptionString(keyName))
		{
			
		}
	}
}
