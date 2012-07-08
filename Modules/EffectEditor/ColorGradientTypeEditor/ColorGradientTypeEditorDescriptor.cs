using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.EffectEditor;
using VixenModules.App.ColorGradients;

namespace VixenModules.EffectEditor.ColorGradientTypeEditor
{
	class ColorGradientTypeEditorDescriptor : EffectEditorModuleDescriptorBase
	{
		private static Guid _typeId = new Guid("{eb7397db-bfac-4187-add4-f75b5e8fe773}");
		private static Guid _ColorGradientsId = new Guid("{64f4ab26-3ed4-49a3-a004-23656ed0424a}");

		public override string Author { get { return "Vixen Team"; } }

		public override string Description { get { return "A control which will edit a parameter of type ColorGradient."; } }

		public override Guid EffectTypeId { get { return Guid.Empty; } }

		public override Type ModuleClass { get { return typeof(ColorGradientTypeEditor); } }

		public override Guid TypeId { get { return _typeId; } }

		public override string TypeName { get { return "ColorGradient Type Editor"; } }

		public override string Version { get { return "1.0"; } }

		public override Guid[] Dependencies { get { return new Guid[] { _ColorGradientsId }; } }

		public override Type[] ParameterSignature
		{
			get { return new[] { typeof(ColorGradient) }; }
		}
	}
}
