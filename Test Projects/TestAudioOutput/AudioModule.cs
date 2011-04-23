using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Common;
using Vixen.Module.Media;

namespace TestAudioOutput {
    public class AudioModule : IMediaModuleDescriptor {
        static internal Guid _typeId = new Guid("{BCD435FA-8B6E-4ad0-AD04-243AB7D84D88}");

        public Guid TypeId {
            get { return _typeId; }
        }

        public Type ModuleClass {
            get { return typeof(Audio); }
        }

        public Type ModuleDataClass {
            get { return typeof(AudioData); }
        }

        public string Author {
            get { throw new NotImplementedException(); }
        }

        public string TypeName {
            get { return "Single-card FMOD audio"; }
        }

        public string Description {
            get { throw new NotImplementedException(); }
        }

        public string Version {
            get { throw new NotImplementedException(); }
        }

		public string FileName { get; set; }
		public string ModuleTypeName { get; set; }

		public string[] FileExtensions {
			get { return new string [] { ".mp3", ".wma" }; }
		}

		public bool IsTimingSource {
			get { return true; }
		}
	}
}
