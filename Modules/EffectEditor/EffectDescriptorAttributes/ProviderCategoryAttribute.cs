using System.ComponentModel;
using Vixen.Attributes;

namespace VixenModules.EffectEditor.EffectDescriptorAttributes
{
	public class ProviderCategoryAttribute: CategoryAttribute, IOrderableAttribute
	{
		public ProviderCategoryAttribute(string categoryKeyName)
			: this(categoryKeyName, -1)
		{
			
		}

		public ProviderCategoryAttribute(string categoryKeyName, int order)
			: base(EffectResourceManager.GetCategoryString(categoryKeyName))
		{
			Order = order;
		}

		public int Order { get; set; }
	}
}
