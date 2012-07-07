using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vixen.IO;
using Vixen.IO.Result;
using Vixen.Module;
using Vixen.Module.SequenceType;
using Vixen.Sys;
using Vixen.Sys.Attribute;

namespace Vixen.Services {
	public class SequenceService {
		// There is no longer a Sequence class in the core assembly.  Better place for this?
		[DataPath]
		static public readonly string SequenceDirectory = Path.Combine(Paths.DataRootPath, "Sequence");

		static private SequenceService _instance;

		private SequenceService() { }

		public static SequenceService Instance {
			get { return _instance ?? (_instance = new SequenceService()); }
		}

		public string[] GetAllSequenceFileNames() {
			// We can't assume where all of the sequence file types will exist, so to provide
			// this functionality we will have to do the following:

			// Iterate all of the sequence type descriptors and build a set of file types.
			HashSet<string> fileTypes = new HashSet<string>();
			IEnumerable<ISequenceTypeModuleDescriptor> sequenceDescriptors = Modules.GetDescriptors<ISequenceTypeModuleInstance, ISequenceTypeModuleDescriptor>();
			foreach(ISequenceTypeModuleDescriptor descriptor in sequenceDescriptors) {
				fileTypes.Add(descriptor.FileExtension);
			}
			// Find all files of those types in the data branch.
			return fileTypes.SelectMany(x => Directory.GetFiles(Paths.DataRootPath, "*" + x, SearchOption.AllDirectories)).ToArray();
		}

		//*** if these are in the service, then should the like methods for other object types also be in their services?
		// Because there is no system-defined Sequence object any longer in which to have these persistence entry points.
		// Any sequence instance is going to be created by a module-defined sequence type factory.
		public void Save(ISequence sequence, string filePath) {
			if(sequence == null) throw new ArgumentNullException("sequence");
			if(string.IsNullOrWhiteSpace(filePath)) throw new InvalidOperationException("filePath must be a file name or path.");

			filePath = _QualifyPath(filePath);
			VersionedFileSerializer serializer = FileService.Instance.CreateSequenceSerializer(filePath);
			serializer.Write(sequence, filePath);
			sequence.FilePath = filePath;
		}

		public ISequence Load(string filePath) {
			if(string.IsNullOrWhiteSpace(filePath)) throw new InvalidOperationException("filePath must be a file name or path.");

			filePath = _QualifyPath(filePath);
			VersionedFileSerializer serializer = FileService.Instance.CreateSequenceSerializer(filePath);
			ISerializationResult result = serializer.Read(filePath);
			if(result.Success) {
				((ISequence)result.Object).FilePath = filePath;
			} else {
				VixenSystem.Logging.Error(result.Message);
			}
			return (ISequence)result.Object;
			//ISequence sequence = _CreateSequenceInstance(filePath);
			//if(sequence != null) {
			//    sequence.SequenceData = _LoadSequenceDataInstance(filePath);
			//}
			//return sequence;
		}

		public ISequence CreateNew(string fileType) {
			ISequence sequence = _CreateSequenceInstance(fileType);
			if(sequence != null) {
				sequence.SequenceData = _CreateSequenceDataInstance(fileType);
			}
			return sequence;
		}

		private string _QualifyPath(string filePath) {
			if(!Path.IsPathRooted(filePath)) {
				return Path.Combine(SequenceDirectory, Path.GetFileName(filePath));
			}
			return filePath;
		}

		private ISequence _CreateSequenceInstance(string fileType) {
			ISequenceTypeModuleInstance sequenceFactory = SequenceTypeService.Instance.CreateSequenceFactory(fileType);
			if(sequenceFactory != null) {
				return sequenceFactory.CreateSequence();
			}
			return null;
		}

		private ISequenceTypeDataModel _CreateSequenceDataInstance(string fileType) {
			ISequenceTypeModuleInstance sequenceFactory = SequenceTypeService.Instance.CreateSequenceFactory(fileType);
			if(sequenceFactory != null) {
				var sequenceTypeData = ModuleLocalDataSet.CreateModuleDataInstance(sequenceFactory);
				if(sequenceTypeData != null && !(sequenceTypeData is ISequenceTypeDataModel)) {
					VixenSystem.Logging.Warning("Could not create appropriate sequence data for new sequence due to the object type.  File type: " + fileType);
				} else {
					return (ISequenceTypeDataModel)sequenceTypeData;
				}
			}
			return null;
		}

		//private object _LoadSequenceDataInstance(string filePath) {
		//    VersionedFileSerializer serializer = FileService.Instance.CreateSequenceSerializer(filePath);
		//    ISerializationResult result = serializer.Read(filePath);
		//    if(!result.Success) {
		//        VixenSystem.Logging.Warning("Could not create the data object for sequence " + filePath);
		//    }
		//    return result.Object;
		//}
	}
}
