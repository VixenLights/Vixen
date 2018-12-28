using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Vixen.Module.Effect;
using Vixen.Sys;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;

namespace VixenModules.Effect.Strobe
{
	public class StrobeDescriptor : EffectModuleDescriptorBase
	{
		private static Guid _typeId = new Guid("{2Bff1008-56E2-4BFF-8CE3-A23189EA4684}");
		private static Guid _ColorGradientId = new Guid("{64f4ab26-3ed4-49a3-a004-23656ed0424a}");


		public override string EffectName
		{
			get { return "Strobe"; }
		}

		public override EffectGroups EffectGroup
		{
			get { return EffectGroups.Basic; }
		}

		#region Overrides of EffectModuleDescriptorBase

		/// <inheritdoc />
		public override bool SupportsMarks => true;

		#endregion

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof(Strobe); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(StrobeData); }
		}

		public override string Author
		{
			get { return "Geoff Armstrong"; }
		}

		public override string TypeName
		{
			get { return EffectName; }
		}

		public override string Description
		{
			get { return "Sets the target elements to an Strobe output level and/or color."; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override Guid[] Dependencies
		{
			get { return new Guid[] { _ColorGradientId }; }
		}

		public override ParameterSignature Parameters
		{
			get { return new ParameterSignature(); }
		}
	}
}