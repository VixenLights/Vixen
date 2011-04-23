using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Vixen.Module {
    public interface IModuleDataSet {
		void GetModuleTypeData(IModuleInstance module);
		void GetModuleInstanceData(IModuleInstance module);
		void Add(IModuleDataModel module);
		void Remove(Guid moduleTypeId);
		void Remove(Guid moduleTypeId, Guid moduleInstanceId);
		bool Contains(Guid moduleTypeId);
		bool Contains(Guid moduleTypeId, Guid moduleInstanceId);
		IEnumerable<Guid> GetModuleTypes();
		IEnumerable<T> GetInstances<T>() where T : class, IModuleInstance;
		IEnumerable<IModuleDataModel> GetData();
        string Serialize();
        void SaveToParent(XElement parentNode);
        void Deserialize(string xmlText);
		void Clear();
		IModuleDataSet Clone();
		IModuleDataModel CloneTypeData(IModuleInstance sourceModule);
		IModuleDataModel CloneInstanceData(IModuleInstance sourceModule, IModuleInstance destinationModule);
	}
}
