using System;

namespace Vixen.Module
{
	public interface IModuleInstance : IDisposable
	{
		/// <summary>
		/// Module-supplied id of the module.
		/// </summary>
		Guid TypeId { get; }

		/// <summary>
		/// System-supplied id of the instance of the module.
		/// </summary>
		Guid InstanceId { get; set; }

		/// <summary>
		/// Module's non-static data model object.
		/// </summary>
		IModuleDataModel ModuleData { get; set; }

		/// <summary>
		/// Module's static data model object.
		/// </summary>
		IModuleDataModel StaticModuleData { get; set; }

		/// <summary>
		/// System-supplied reference to the descriptor for this instance.
		/// </summary>
		IModuleDescriptor Descriptor { get; set; }

		/// <summary>
		/// Creates a deep copy of the module instance.
		/// </summary>
		/// <returns></returns>
		IModuleInstance Clone();
	}
}