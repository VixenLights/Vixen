using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.EffectEditor;

namespace TestCommandEditors {
	public class LevelEditorModule : IEffectEditorModuleDescriptor {
		internal static Guid _typeId = new Guid("{5A987CD0-C631-43ea-8269-FB730C9E0B6C}");

		public Guid TypeId {
			get { return _typeId; }
		}

		public Type ModuleClass {
			get { return typeof(LevelEditor); }
		}

		public Type ModuleDataClass {
			get { return null; }
		}

		public string Author {
			get { throw new NotImplementedException(); }
		}

		public string TypeName {
			get { return "Level editor"; }
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
