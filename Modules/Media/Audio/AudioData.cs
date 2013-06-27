using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Vixen.Module;
using Vixen.Module.Media;

namespace VixenModules.Media.Audio
{
	[DataContract]
	public class AudioData : ModuleDataModelBase
	{
		[DataMember]
		public string FilePath { get; set; }

		public override IModuleDataModel Clone()
		{
			AudioData result = new AudioData();
			result.FilePath = FilePath;
			return result;
		}
	}
}