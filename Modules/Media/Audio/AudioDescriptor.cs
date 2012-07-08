using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Media;

namespace VixenModules.Media.Audio
{
    public class AudioDescriptor : MediaModuleDescriptorBase
	{
		private Guid _typeId = new Guid("{fe460392-3fab-4c63-99dd-d76c48354150}");

        override public Guid TypeId
		{
            get { return _typeId; }
        }

		override public Type ModuleClass
		{
            get { return typeof(Audio); }
        }

		override public Type ModuleDataClass
		{
            get { return typeof(AudioData); }
        }

		override public string Author
		{
			get { return "Vixen Team"; }
		}

		override public string Description
		{
			get { return "Allows an audio file to be used as a media item in a sequence."; }
		}

		override public string Version
		{
			get { return "1.0"; }
		}

		override public string TypeName
		{
            get { return "Audio"; }
        }

		override public string[] FileExtensions
		{
			get { return new string[] { ".mp3", ".wma", ".aiff", ".flac", ".ogg", ".wav", }; }
		}

		override public bool IsTimingSource
		{
			get { return true; }
		}
	}
}
