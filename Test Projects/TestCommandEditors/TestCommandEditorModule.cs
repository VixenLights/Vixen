using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Common;
using CommandStandard;
using Vixen.Module.EffectEditor;

namespace TestCommandEditors {
	public class TestCommandEditorModule : IEffectEditorModuleDescriptor {
		internal static Guid _typeId = new Guid("{BA73EAC1-66D8-488e-9889-4E979557D72D}");

		public Guid TypeId {
			get { return _typeId; }
		}

		public Type ModuleClass {
			get { return typeof(TestCommandEditor); }
		}

		public Type ModuleDataClass {
			get { return null; }
		}

		public string Author {
			get { throw new NotImplementedException(); }
		}

		public string TypeName {
			get { return "Test command editor"; }
		}

		public string Description {
			get { throw new NotImplementedException(); }
		}

		public string Version {
			get { throw new NotImplementedException(); }
		}

		public string FileName { get; set; }

		public string ModuleTypeName { get; set; }
	}
}
