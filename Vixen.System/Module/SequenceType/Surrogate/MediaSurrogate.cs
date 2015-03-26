using System;
using System.IO;
using System.Runtime.Serialization;
using Vixen.Module.Media;
using Vixen.Services;

namespace Vixen.Module.SequenceType.Surrogate
{
	[DataContract(Namespace = "")]
	internal class MediaSurrogate
	{
		public MediaSurrogate(IMediaModuleInstance mediaModuleInstance)
		{
			TypeId = mediaModuleInstance.Descriptor.TypeId;
			InstanceId = mediaModuleInstance.InstanceId;
			FileName = Path.GetFileName(mediaModuleInstance.MediaFilePath);
		}

		[DataMember]
		public Guid TypeId { get; private set; }

		[DataMember]
		public Guid InstanceId { get; private set; }

		[DataMember]
		public string FileName { get; private set; }

		public IMediaModuleInstance CreateMedia()
		{
			IMediaModuleInstance module = MediaService.Instance.GetMedia(FileName);
			module.InstanceId = InstanceId;
			return module;
		}
	}
}