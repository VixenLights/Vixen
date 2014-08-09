using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Vixen.IO.Factory;
using Vixen.Cache.Sequence;
using Vixen.Services;

namespace Vixen.IO.Loader
{
	internal class SequenceCacheLoader : IObjectLoader
	{
		public object LoadFromFile(string filePath)
		{
			if (filePath == null) throw new InvalidOperationException("Cannot load from a null file path.");
			if (!File.Exists(filePath)) throw new InvalidOperationException("File does not exist.");

			IFileReader fileReader = FileReaderFactory.Instance.CreateBinaryFileReader();
			if (fileReader == null) return null;

			IObjectContentWriter contentWriter = ObjectContentWriterFactory.Instance.CreateSequenceCacheContentWriter(filePath);
			if (contentWriter == null) return null;

			ISequenceCache sequenceCache = SequenceService.Instance.CreateNewCache(filePath);
			if (sequenceCache == null) return null;

			object content = fileReader.ReadFile(sequenceCache.CacheFilePath);
			if (content == null) return null;

			var cacheContainer = new CacheContainer();
			cacheContainer.SequenceCache = sequenceCache;
			contentWriter.WriteContentToObject(content, cacheContainer);

			return cacheContainer.SequenceCache;
		}
	}

	//Internal container to use with the not great contentwriter interface.
	//This is so I don't have to change the whole interface for this one item
	//This whole thing may get redone sometime in the future.
	internal class CacheContainer
	{
		public ISequenceCache SequenceCache { get; set; }
	}
}
