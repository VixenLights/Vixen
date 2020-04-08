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

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof (Audio); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof (AudioData); }
		}

		public override string Author
		{
			get { return "Vixen Team"; }
		}

		public override string Description
		{
			get { return "Allows an audio file to be used as a media item in a sequence."; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override string TypeName
		{
			get { return "Audio"; }
		}

		public override string[] FileExtensions
		{
			get { return new string[] {".mp3", ".wma", ".aiff", ".flac", ".ogg", ".wav",".mp4"}; }
		}

		public override bool IsTimingSource
		{
			get { return true; }
		}
	}
}