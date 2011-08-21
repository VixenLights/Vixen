using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module.ModuleTemplate;

namespace TestTemplate {
	public class ScriptSequenceTemplateModule : ModuleTemplateModuleDescriptorBase {
		private Guid _typeId = new Guid("{F46FB19C-28D6-458e-8646-96E23A59BD96}");

		override public Guid TypeId {
			get { return _typeId; }
		}

		override public Type ModuleClass {
			get { return typeof(ScriptSequenceTemplate); }
		}

		override public Type ModuleDataClass {
			get { return typeof(ScriptSequenceTemplateData); }
		}

		override public string Author {
			get { throw new NotImplementedException(); }
		}

		override public string TypeName {
			get { return "Scripted sequence template"; }
		}

		override public string Description {
			get { throw new NotImplementedException(); }
		}

		override public string Version {
			get { throw new NotImplementedException(); }
		}
	}
}
