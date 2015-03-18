using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vixen.Module.Analysis;
using Vixen.Module.App;


namespace VixenModules.Analysis.BeatsAndBars
{
	public class BeatsAndBarsDescriptor : AnalysisModuleDescriptorBase
	{
		private Guid _typeId = new Guid("{CF6326F5-424E-4C04-BF7B-EF511D9CA4E5}");

		public override string TypeName
		{
			get { return "Beats and Bars"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override string Author
		{
			get { return "Ed Brady"; }
		}

		public override string Description
		{
			get { return "Beats and Bars Autodetection"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override Type ModuleClass
		{
			get { return typeof(BeatsAndBars); }
		}

	}
}