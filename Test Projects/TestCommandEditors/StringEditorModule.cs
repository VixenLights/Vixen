using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Interface;
using CommandStandard;

namespace TestCommandEditors {
    public class StringEditorModule : ICommandEditorModuleDescriptor {
        static internal Guid _typeId = new Guid("{25052C9D-3190-424b-81DD-6138797A9987}");
        static internal CommandIdentifier _targetCommand = CommandStandard.Standard.Media.Audio.Load.Id;

        public Guid TypeId {
            get { return _typeId; }
        }

        public Type ModuleType {
            get { return typeof(StringEditor); }
        }

        public Type ModuleDataType {
            get { return null; }
        }

        public string Author {
            get { return ""; }
        }

        public string Name {
            get { return "String value editor"; }
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
