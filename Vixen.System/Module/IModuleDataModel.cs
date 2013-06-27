using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module
{
	public interface IModuleDataModel
	{
		/// <summary>
		/// Module type that the data model belongs to.
		/// </summary>
		Guid ModuleTypeId { get; set; }

		/// <summary>
		/// Module data set that the data model was created from.
		/// </summary>
		IModuleDataSet ModuleDataSet { get; set; }

		/// <summary>
		/// The InstanceId of the of module that the data model belongs to.
		/// </summary>
		Guid ModuleInstanceId { get; set; }

		/// <summary>
		/// Performs a deep cloning of the data object.
		/// </summary>
		/// <returns></returns>
		IModuleDataModel Clone();
	}
}