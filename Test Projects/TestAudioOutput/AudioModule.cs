using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module.Media;

namespace TestAudioOutput {
    public class AudioModule : MediaModuleDescriptorBase {
        private Guid _typeId = new Guid("{BCD435FA-8B6E-4ad0-AD04-243AB7D84D88}");

        override public Guid TypeId {
            get { return _typeId; }
        }

		override public Type ModuleClass {
            get { return typeof(Audio); }
        }

		override public Type ModuleDataClass {
            get { return typeof(AudioData); }
        }

		public override Type ModuleStaticDataClass {
			get { return typeof(AudioStaticData); }
		}

		override public string Author {
            get { throw new NotImplementedException(); }
        }

		override public string TypeName {
            get { return "Single-card FMOD audio"; }
        }

		override public string Description {
            get { throw new NotImplementedException(); }
        }

		override public string Version {
            get { throw new NotImplementedException(); }
        }

		override public string[] FileExtensions {
			get { return new string [] { ".mp3", ".wma" }; }
		}

		override public bool IsTimingSource {
			get { return true; }
		}
	}
}
