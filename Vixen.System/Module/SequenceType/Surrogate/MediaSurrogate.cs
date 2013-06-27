using System;
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
			FilePath = mediaModuleInstance.MediaFilePath;
		}

		[DataMember]
		public Guid TypeId { get; private set; }

		[DataMember]
		public Guid InstanceId { get; private set; }

		[DataMember]
		public string FilePath { get; private set; }

		public IMediaModuleInstance CreateMedia()
		{
			IMediaModuleInstance module = MediaService.Instance.GetMedia(FilePath);
			module.InstanceId = InstanceId;
			return module;
		}
	}
}