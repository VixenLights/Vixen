using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.EffectEditor;

namespace LauncherEditor
{
	public class Descriptor : EffectEditorModuleDescriptorBase
	{
		private static Guid _typeId = new Guid("{9A174BD1-F98D-47D6-B04B-A11697E5068D}");
		private static Guid _effectId = new Guid("{635364FD-B942-47E0-8B82-55D5E77A27C8}");

		public override string TypeName
		{
			get { return "Launcher Editor"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof(Module); }
		}

		public override string Author
		{
			get { return "Darren McDaniel"; }
		}

		public override string Description
		{
			get { return "Launcher Editor"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override Guid EffectTypeId
		{
			get { return _effectId; }
		}

		public override Type[] ParameterSignature
		{
			get { return new[] { typeof(String), typeof(String), typeof(String) }; }
		}
	}

}
