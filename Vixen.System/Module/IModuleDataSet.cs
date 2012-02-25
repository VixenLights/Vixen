using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Vixen.IO;

namespace Vixen.Module {
    public interface IModuleDataSet {
		void AssignModuleTypeData(IModuleInstance module);
		IModuleDataModel GetTypeData(IModuleInstance module);
		void AssignModuleInstanceData(IModuleInstance module);
		IModuleDataModel GetInstanceData(IModuleInstance module);
		void RemoveModuleTypeData(IModuleInstance module);
		void RemoveModuleInstanceData(IModuleInstance module);
		void Clear();
		IModuleDataSet Clone();
		void Clone(IModuleDataSet sourceDataSet);
		void Serialize(IModuleDataModelCollectionSerializer dataModelCollectionSerializer);
		void Deserialize(IModuleDataModelCollectionSerializer dataModelCollectionSerializer);

		//void GetModuleTypeData(IModuleInstance module);
		//void GetModuleInstanceData(IModuleInstance module);
		//IModuleDataModel RetrieveTypeData(IModuleDescriptor descriptor);
		//IModuleDataModel GetInstanceData(IModuleInstance instance);
		//void AddModuleTypeData(IModuleInstance instance);
		//void AddModuleInstanceData(IModuleInstance instance);
		//void RemoveModuleTypeData(Guid moduleTypeId);
		//void RemoveModuleInstanceData(Guid moduleTypeId, Guid moduleInstanceId);
		//bool Contains(Guid moduleTypeId);
		//bool Contains(Guid moduleTypeId, Guid moduleInstanceId);
		//IEnumerable<Guid> GetModuleTypes();
		//IEnumerable<T> GetInstances<T>() where T : class, IModuleInstance;
		//IEnumerable<IModuleDataModel> GetData();
		//void Serialize(IModuleDataModelCollectionSerializer dataModelCollectionSerializer);
		//void Deserialize(IModuleDataModelCollectionSerializer dataModelCollectionSerializer);
		////string Serialize();
		////void Deserialize(string xmlText);
		//void Clear();
		//IModuleDataSet Clone();
		//void Clone(IModuleDataSet sourceDataSet);
		//IModuleDataModel CloneTypeData(IModuleInstance sourceModule);
		//IModuleDataModel CloneInstanceData(IModuleInstance sourceModule, IModuleInstance destinationModule);
		////XElement ToXElement();
	}
}
