using System;
using Vixen.IO.Factory;

namespace Vixen.IO.Persistor {
	using Vixen.Sys;

	class ChannelNodeTemplatePersistor : IObjectPersistor<ChannelNodeTemplate> {
		public void SaveToFile(ChannelNodeTemplate obj, string filePath) {
			IFileWriter fileWriter = FileWriterFactory.CreateFileWriter();
			IObjectContentReader contentReader = ObjectContentReaderFactory.Instance.CreateChannelNodeTemplateContentReader();
			ObjectPersistorService.Instance.SaveToFile(obj, fileWriter, contentReader, filePath);
		}

		void IObjectPersistor.SaveToFile(object obj, string filePath) {
			if(!(obj is ChannelNodeTemplate)) throw new InvalidOperationException("Object must be a ChannelNodeTemplate.");
			SaveToFile((ChannelNodeTemplate)obj, filePath);
		}
	}
}
