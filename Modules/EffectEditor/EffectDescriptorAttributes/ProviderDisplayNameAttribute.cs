using System.ComponentModel;

namespace VixenModules.EffectEditor.EffectDescriptorAttributes
{
	public class ProviderDisplayNameAttribute:DisplayNameAttribute
	{
		public ProviderDisplayNameAttribute(string keyName):base(EffectResourceManager.GetDisplayNameString(keyName))
		{
			
		}
	}
}
