using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Interface;
using CommandStandard;

namespace TestCommandEditors {
    public class IntEditorModule : ICommandEditorModuleDescriptor {
        static internal Guid _typeId = new Guid("{E2C280A1-1193-49f5-A4D6-EDB98DC0B2A0}");
        static internal CommandIdentifier _targetCommand = CommandStandard.Standard.Media.Audio.Play.Id;

        public Guid TypeId {
            get { return _typeId; }
        }

        public Type ModuleType {
            get { return typeof(IntEditor); }
        }

        public Type ModuleDataType {
            get { return null; }
        }

        public string Author {
            get { return ""; }
        }

        public string Name {
            get { return "Integer value editor"; }
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
    }
}
