using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module {
	class ModuleDataMover : ITypedDataMover {
		public string TypeOfData {
			get { return "Module data"; }
		}

		public bool CanHandle(object obj) {
			return obj is IModuleDataContainer;
		}

		public void Copy(object source, object destination, bool overwrite) {
			IModuleDataContainer moduleDataSource = source as IModuleDataContainer;
			IModuleDataContainer moduleDataDestination = destination as IModuleDataContainer;
			IModuleDataModel[] data;

			if(!overwrite) {
				// Only copy data for modules that do not already exist.
				data = moduleDataSource.ModuleDataSet.GetData().Except(moduleDataDestination.ModuleDataSet.GetData(), new ModuleDataModelEqualityComparer()).ToArray();
			} else {
				data = moduleDataSource.ModuleDataSet.GetData().ToArray();
				foreach(IModuleDataModel dataModel in data) {
					moduleDataDestination.ModuleDataSet.Remove(dataModel.ModuleTypeId);
				}
			}

			foreach(IModuleDataModel dataModel in data) {
				moduleDataDestination.ModuleDataSet.Add(dataModel);
			}
		}

		public void Move(object source, object destination, bool overwrite) {
			IModuleDataContainer moduleDataSource = source as IModuleDataContainer;
			IModuleDataContainer moduleDataDestination = destination as IModuleDataContainer;

			Copy(source, destination, overwrite);
			foreach(IModuleDataModel dataModel in moduleDataSource.ModuleDataSet.GetData().ToArray()) {
				moduleDataSource.ModuleDataSet.Remove(dataModel.ModuleTypeId);
			}
		}
	}

	class ModuleDataModelEqualityComparer : IEqualityComparer<IModuleDataModel> {
		public bool Equals(IModuleDataModel x, IModuleDataModel y) {
			return x.ModuleTypeId == y.ModuleTypeId;
		}

		public int GetHashCode(IModuleDataModel obj) {
			return obj.ModuleTypeId.GetHashCode();
		}
	}
}
