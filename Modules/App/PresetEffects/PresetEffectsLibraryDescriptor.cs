using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.App;


namespace VixenModules.App.PresetEffects
{

	public class PresetEffectsLibraryDescriptor : AppModuleDescriptorBase
	{
		private static Guid _id = new Guid("{8B60F719-B4D5-4537-9A2B-8D75E3B47452}");

		public override string TypeName
		{
			get { return "Preset Effects"; }
		}

		public override Guid TypeId
		{
			get { return _id; }
		}

		public override Type ModuleClass
		{
			get { return typeof(PresetEffectsLibrary); }
		}

		public override Type ModuleStaticDataClass
		{
			get { return typeof(PresetEffectsLibraryStaticData); }
		}

		public override string Author
		{
			get { return "Vixen Team - James Bolding 2014"; }
		}

		public override string Description
		{
			get
			{
				return
					"Provides a data type which represents a preset effect.";
			}
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public static Guid ModuleID
		{
			get { return _id; }
		}
	}
}
