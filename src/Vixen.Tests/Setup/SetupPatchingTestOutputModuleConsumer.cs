using Vixen.Module.Controller;
using Vixen.Sys.Output;

namespace Vixen.Tests.Setup;

/// <summary>
/// Supplies an inert controller module consumer for in-memory output controller tests.
/// </summary>
internal sealed class SetupPatchingTestOutputModuleConsumer(IControllerModuleInstance module) : IOutputModuleConsumer<IControllerModuleInstance>
{
	/// <summary>
	/// Gets the module identifier used by the test controller.
	/// </summary>
	public Guid ModuleId { get; } = Guid.NewGuid();

	/// <summary>
	/// Gets the module instance identifier used by the test controller.
	/// </summary>
	public Guid ModuleInstanceId { get; } = Guid.NewGuid();

	/// <summary>
	/// Gets the controller module instance.
	/// </summary>
	public IControllerModuleInstance Module { get; } = module;

	/// <summary>
	/// Gets the test update interval.
	/// </summary>
	public int UpdateInterval => 0;

	/// <summary>
	/// Gets the update signaler, which is not used by these tests.
	/// </summary>
	public IOutputDeviceUpdateSignaler UpdateSignaler => null!;

	/// <summary>
	/// Gets whether the test controller supports named outputs.
	/// </summary>
	public bool SupportsNamedOutputs => false;

	/// <summary>
	/// Gets whether the test controller is running.
	/// </summary>
	public bool IsRunning => false;

	/// <summary>
	/// Gets whether the test controller is paused.
	/// </summary>
	public bool IsPaused => false;

	/// <summary>
	/// Gets whether the test controller requires setup.
	/// </summary>
	public bool HasSetup => false;

	/// <summary>
	/// Leaves output names unchanged for the test controller.
	/// </summary>
	public void NameOutputs()
	{
	}

	/// <summary>
	/// Starts no hardware for the in-memory test controller.
	/// </summary>
	public void Start()
	{
	}

	/// <summary>
	/// Stops no hardware for the in-memory test controller.
	/// </summary>
	public void Stop()
	{
	}

	/// <summary>
	/// Pauses no hardware for the in-memory test controller.
	/// </summary>
	public void Pause()
	{
	}

	/// <summary>
	/// Resumes no hardware for the in-memory test controller.
	/// </summary>
	public void Resume()
	{
	}

	/// <summary>
	/// Indicates that no setup dialog is available for the in-memory controller.
	/// </summary>
	/// <returns><see langword="false" /> because no setup dialog is available.</returns>
	public bool Setup() => false;
}
