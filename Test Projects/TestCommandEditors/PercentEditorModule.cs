using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Interface;
using CommandStandard;

namespace TestCommandEditors {
    //Must be public to be reflected.
    public class PercentEditorModule : ICommandEditorModuleDescriptor {
        static internal Guid _typeId = new Guid("{7DA5EF22-5CA2-4a88-A89F-2B273ADC08FB}");
        static internal CommandIdentifier _targetCommand = CommandStandard.Standard.Lighting.Monochrome.SetLevel.Id;

        public Guid TypeId {
            get { return _typeId; }
        }

        public Type ModuleClass {
            get { return typeof(PercentEditor); }
        }

        public Type ModuleDataClass {
            get { return null; }
        }

        public string Author {
            get { return ""; }
        }

        public string Name {
            get { return "Percent value editor"; }
        }

        public string Description {
            get { return ""; }
        }

        public string Version {
            get { return ""; }
        }

        public CommandIdentifier TargetCommand {
            get { return _targetCommand; }
        }

		public string FileName { get; set; }
		public string ModuleTypeName { get; set; }
	}
}
