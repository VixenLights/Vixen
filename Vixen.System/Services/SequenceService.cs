using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vixen.Cache.Sequence;
using Vixen.Module;
using Vixen.Module.Media;
using Vixen.Module.SequenceType;
using Vixen.Sys;
using Vixen.Sys.Attribute;

namespace Vixen.Services
{
	public class SequenceService
	{
		// There is no longer a Sequence class in the core assembly.  Better place for this?
		[DataPath] public static readonly string SequenceDirectory = Path.Combine(Paths.DataRootPath, "Sequence");
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		private static SequenceService _instance;

		private SequenceService()
		{
		}

		public static SequenceService Instance
		{
			get { return _instance ?? (_instance = new SequenceService()); }
		}

		public string[] GetAllSequenceFileNames()
		{
			// We can't assume where all of the sequence file types will exist, so to provide
			// this functionality we will have to do the following:

			// Iterate all of the sequence type descriptors and build a set of file types.
			HashSet<string> fileTypes = new HashSet<string>();
			IEnumerable<ISequenceTypeModuleDescriptor> sequenceDescriptors =
				Modules.GetDescriptors<ISequenceTypeModuleInstance, ISequenceTypeModuleDescriptor>();
			foreach (ISequenceTypeModuleDescriptor descriptor in sequenceDescriptors) {
				fileTypes.Add(descriptor.FileExtension);
			}
			// Find all files of those types in the data branch.
			return
				fileTypes.SelectMany(x => Directory.GetFiles(Paths.DataRootPath, "*" + x, SearchOption.AllDirectories)).Where(s => fileTypes.Contains(Path.GetExtension(s))).ToArray();
		}

		public void Save(ISequence sequence, string filePath)
		{
			if (sequence == null) throw new ArgumentNullException("sequence");
			if (string.IsNullOrWhiteSpace(filePath))
				throw new InvalidOperationException("File path must be a file name or path.");

			FileService.Instance.SaveSequenceFile(sequence, filePath);
		}

		public ISequence Load(string filePath)
		{
			if (string.IsNullOrWhiteSpace(filePath))
				throw new InvalidOperationException("filePath must be a file name or path.");

			ISequence sequence = FileService.Instance.LoadSequenceFile(filePath);

			Parallel.ForEach(sequence.SequenceData.EffectData.Cast<IEffectNode>(), effectNode => AddMedia(effectNode, sequence));
			
			return sequence;
		}

		private void AddMedia(IEffectNode effectNode, ISequence sequence)
		{
			if (effectNode.Effect.SupportsMedia)
			{
				effectNode.Effect.Media = sequence.SequenceData.Media;

			}
		}

		public void SaveCache(ISequenceCache sequenceCache, string filePath)
		{
			if (sequenceCache == null) throw new ArgumentNullException("sequenceCache");
			if (string.IsNullOrWhiteSpace(filePath))
				throw new InvalidOperationException("File path must be a file name or path.");

			FileService.Instance.SaveSequenceCacheFile(sequenceCache, filePath);
		}

		public ISequenceCache LoadCache(string filePath)
		{
			if (string.IsNullOrWhiteSpace(filePath))
				throw new InvalidOperationException("filePath must be a file name or path.");

			return FileService.Instance.LoadSequenceCacheFile(filePath);
		}

		/// <summary>
		/// Create a cache object for the given sequence path.
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public ISequenceCache CreateNewCache(string filePath)
		{
			ISequenceCache sequenceCache = _CreateSequenceCacheInstance(filePath);

			if (sequenceCache != null)
			{
				sequenceCache.SequenceFilePath = filePath;
			}
			return sequenceCache;
		}

		public ISequence CreateNew(string fileType)
		{
			ISequence sequence = _CreateSequenceInstance(fileType);
			if (sequence != null) {
				sequence.SequenceData = _CreateSequenceDataInstance(fileType);
			}
			return sequence;
		}

		private ISequence _CreateSequenceInstance(string fileType)
		{
			ISequenceTypeModuleInstance sequenceFactory = SequenceTypeService.Instance.CreateSequenceFactory(fileType);
			if (sequenceFactory != null) {
				return sequenceFactory.CreateSequence();
			}
			return null;
		}

		private ISequenceCache _CreateSequenceCacheInstance(string fileType)
		{
			ISequenceTypeModuleInstance sequenceFactory = SequenceTypeService.Instance.CreateSequenceCacheFactory(fileType);
			if (sequenceFactory != null)
			{
				return sequenceFactory.CreateSequenceCache();
			}
			return null;
		}

		private ISequenceTypeDataModel _CreateSequenceDataInstance(string fileType)
		{
			ISequenceTypeModuleInstance sequenceFactory = SequenceTypeService.Instance.CreateSequenceFactory(fileType);
			if (sequenceFactory != null) {
				IModuleDataModel sequenceTypeData = ModuleLocalDataSet.CreateModuleDataInstance(sequenceFactory);
				if (sequenceTypeData != null && !(sequenceTypeData is ISequenceTypeDataModel)) {
					Logging.Warn(
						"Could not create appropriate sequence data for new sequence due to the object type.  File type: " + fileType);
				}
				else {
					if (sequenceTypeData != null) {
						sequenceTypeData.ModuleTypeId = sequenceFactory.TypeId;
						sequenceTypeData.ModuleInstanceId = sequenceFactory.InstanceId;
					}
					return (ISequenceTypeDataModel) sequenceTypeData;
				}
			}
			return null;
		}
	}
}