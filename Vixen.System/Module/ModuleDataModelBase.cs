using System;
using System.Runtime.Serialization;

namespace Vixen.Module {
	[DataContract(Namespace = "")]
	public abstract class ModuleDataModelBase : IModuleDataModel {
		/// <summary>
		/// Module type that the data model belongs to.
		/// </summary>
		[DataMember]
		public Guid ModuleTypeId { get; set; }
		/// <summary>
		/// Module data set that the data model was created from.
		/// </summary>
		public IModuleDataSet ModuleDataSet { get; set; }
		/// <summary>
		/// The InstanceId of the of module that the data model belongs to.
		/// </summary>
		[DataMember]
		public Guid ModuleInstanceId { get; set; }
		/// <summary>
		/// Performs a deep cloning of the data object.
		/// </summary>
		/// <returns></returns>
		abstract public IModuleDataModel Clone();
	}
}
