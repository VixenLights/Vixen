using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Interface;
using CommandStandard;

namespace TestCommandEditors {
    public class LoadPositionAndPlayModule : ICommandEditorModuleDescriptor {
        static internal Guid _typeId = new Guid("{4F19F44E-9980-4c63-B56A-AA42B7E8E4FC}");
        static internal CommandIdentifier _targetCommand = CommandStandard.Standard.Media.Audio.LoadPositionAndPlay.Id;

        public Guid TypeId {
            get { return _typeId; }
        }

        public Type ModuleType {
            get { return typeof(LoadPositionAndPlayEditor); }
        }

        public Type ModuleDataType {
            get { return null; }
        }

        public string Author {
            get { return ""; }
        }

        public string Name {
            get { return "Editor for the Load, Position, and Play audio command"; }
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
