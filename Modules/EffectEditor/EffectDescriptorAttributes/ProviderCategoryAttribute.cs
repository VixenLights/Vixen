using System.ComponentModel;

namespace VixenModules.EffectEditor.EffectDescriptorAttributes
{
	public class ProviderCategoryAttribute: CategoryAttribute
	{
		public ProviderCategoryAttribute(string categoryKeyName)
			: base(EffectResourceManager.GetCategoryString(categoryKeyName))
		{
			
		}
	}
}
