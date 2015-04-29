using System.Resources;

namespace VixenModules.EffectEditor.EffectDescriptorAttributes
{
	public static class EffectResourceManager
	{
		private static readonly ResourceManager CategoryManager;
		private static readonly ResourceManager DisplayNameManager;
		private static readonly ResourceManager DescriptionManager;
		static EffectResourceManager()
		{
			var myAssembly = typeof(EffectResourceManager).Assembly;
			CategoryManager = new ResourceManager("VixenModules.EffectEditor.EffectDescriptorAttributes.EffectCategoryDescriptors", myAssembly);
			DisplayNameManager = new ResourceManager("VixenModules.EffectEditor.EffectDescriptorAttributes.EffectDisplayNameDescriptors", myAssembly);
			DescriptionManager = new ResourceManager("VixenModules.EffectEditor.EffectDescriptorAttributes.EffectDescriptionDescriptors", myAssembly);
		}

		public static string GetCategoryString(string key)
		{
			return CategoryManager.GetString(key);
		}

		public static string GetDisplayNameString(string key)
		{
			return DisplayNameManager.GetString(key);
		}

		public static string GetDescriptionString(string key)
		{
			return DescriptionManager.GetString(key);
		}
	}
}
