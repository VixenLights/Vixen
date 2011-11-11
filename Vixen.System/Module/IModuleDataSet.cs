using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Vixen.Module {
    public interface IModuleDataSet {
		void GetModuleTypeData(IModuleInstance module);
		void GetModuleInstanceData(IModuleInstance module);
		IModuleDataModel RetrieveTypeData(IModuleDescriptor descriptor);
		IModuleDataModel RetrieveInstanceData(IModuleInstance instance);
		void AddModuleTypeData(IModuleInstance instance);
		void AddModuleInstanceData(IModuleInstance instance);
		void RemoveModuleTypeData(Guid moduleTypeId);
		void RemoveModuleInstanceData(Guid moduleTypeId, Guid moduleInstanceId);
		bool Contains(Guid moduleTypeId);
		bool Contains(Guid moduleTypeId, Guid moduleInstanceId);
		IEnumerable<Guid> GetModuleTypes();
		IEnumerable<T> GetInstances<T>() where T : class, IModuleInstance;
		IEnumerable<IModuleDataModel> GetData();
        string Serialize();
        void Deserialize(string xmlText);
		void Clear();
		IModuleDataSet Clone();
		void Clone(IModuleDataSet sourceDataSet);
		IModuleDataModel CloneTypeData(IModuleInstance sourceModule);
		IModuleDataModel CloneInstanceData(IModuleInstance sourceModule, IModuleInstance destinationModule);
		XElement ToXElement();
	}
}
