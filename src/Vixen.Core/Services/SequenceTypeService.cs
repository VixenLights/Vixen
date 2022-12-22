﻿using System.Runtime.Serialization;
using Vixen.Execution;
using Vixen.Module.SequenceType;
using Vixen.Sys;

namespace Vixen.Services
{
	public class SequenceTypeService
	{
		private static SequenceTypeService _instance;

		private SequenceTypeService()
		{
		}

		public static SequenceTypeService Instance
		{
			get { return _instance ?? (_instance = new SequenceTypeService()); }
		}

		public ISequenceTypeModuleInstance CreateSequenceFactory(string fileType)
		{
			SequenceTypeModuleManagement manager =
				Modules.GetManager<ISequenceTypeModuleInstance, SequenceTypeModuleManagement>();
			return manager.Get(fileType);
		}

		public ISequenceTypeModuleInstance CreateSequenceCacheFactory(string fileType)
		{
			SequenceTypeModuleManagement manager =
				Modules.GetManager<ISequenceTypeModuleInstance, SequenceTypeModuleManagement>();
			return manager.Get(fileType);
		}

		public ISequenceExecutor CreateSequenceExecutor(ISequence sequence)
		{
			SequenceTypeModuleManagement manager =
				Modules.GetManager<ISequenceTypeModuleInstance, SequenceTypeModuleManagement>();
			var sequenceTypeFactory = manager.GetFactory(sequence);
			if (sequenceTypeFactory != null) {
				ISequenceExecutor sequenceExecutor = sequenceTypeFactory.CreateExecutor();
				if (sequenceExecutor == null)
					throw new InvalidOperationException("No executor exists for sequence of type " + sequence.GetType() + ".");
				return sequenceExecutor;
			}
			return null;
		}

		public bool IsValidSequenceFileType(string fileType)
		{
			SequenceTypeModuleManagement manager =
				Modules.GetManager<ISequenceTypeModuleInstance, SequenceTypeModuleManagement>();
			return manager.IsValidSequenceFileType(fileType);
		}

		internal static DataContractSerializer GetSequenceTypeDataSerializer(ISequenceTypeModuleInstance sequenceTypeModule)
		{
			if (sequenceTypeModule == null) return null;
			if (sequenceTypeModule.Descriptor == null) return null;
			if (sequenceTypeModule.Descriptor.ModuleDataClass == null) return null;

			return new DataContractSerializer(sequenceTypeModule.Descriptor.ModuleDataClass, _GetAllModuleDataTypes());
		}

		private static IEnumerable<Type> _GetAllModuleDataTypes()
		{
			return
				ApplicationServices.GetTypesOfModules().SelectMany(Modules.GetDescriptors).Select(x => x.ModuleDataClass).Where(
					x => x != null);
		}
	}
}