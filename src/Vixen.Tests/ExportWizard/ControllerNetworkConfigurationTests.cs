using System.Reflection;
using System.Runtime.CompilerServices;
using Moq;
using Vixen.Data.Flow;
using Vixen.Export;
using Vixen.Module.Controller;
using Vixen.Sys;
using Vixen.Sys.Managers;
using Vixen.Sys.Output;
using Vixen.Tests.Core;
using VixenModules.App.ExportWizard;
using Xunit;

namespace Vixen.Tests.ExportWizard;

[Collection(OutputControllerOutputIndexTestCollection.Name)]
public sealed class ControllerNetworkConfigurationTests : IDisposable
{
	private readonly OutputControllerManager _previousOutputControllers;
	private readonly OutputControllerManager _outputControllers;
	private readonly DataFlowManager _previousDataFlow;
	private readonly DataFlowManager _dataFlow;

	public ControllerNetworkConfigurationTests()
	{
		_previousOutputControllers = VixenSystem.OutputControllers;
		_previousDataFlow = VixenSystem.DataFlow;
		_outputControllers = new OutputControllerManager(
			new OutputDeviceCollection<OutputController>(), new OutputDeviceExecution<OutputController>());
		_dataFlow = new DataFlowManager();
		SetVixenSystemProperty(nameof(VixenSystem.OutputControllers), _outputControllers);
		SetVixenSystemProperty(nameof(VixenSystem.DataFlow), _dataFlow);
	}

	[Fact]
	public void MetadataConstructionAndClone_DoNotLoadNetworkConfiguration()
	{
		var (controller, module) = CreateOutputController("Metadata", 12);

		var snapshot = new Controller(controller, 4, initializeNetworkConfiguration: false);
		var clone = (Controller)snapshot.Clone();

		AssertMetadata(snapshot, controller, 4);
		AssertMetadata(clone, controller, 4);
		module.Verify(instance => instance.GetNetworkConfiguration(), Times.Never);
	}

	[Fact]
	public void ProfileSynchronizationAndClone_DoNotLoadNetworkConfiguration()
	{
		var (controller, module) = CreateOutputController("Profile", 8);
		_outputControllers.Add(controller);
		var profile = CreateMinimalProfile();

		profile.SyncronizeControllerInfo();
		var clone = (ExportProfile)profile.Clone();

		var snapshot = Assert.Single(profile.Controllers);
		AssertMetadata(snapshot, controller, 0);
		AssertMetadata(Assert.Single(clone.Controllers), controller, 0);
		module.Verify(instance => instance.GetNetworkConfiguration(), Times.Never);
	}

	[Fact]
	public void MetadataSnapshots_PreserveControllerIdentityCountsActiveStateAndOrder()
	{
		var (firstController, _) = CreateOutputController("First", 5);
		var (secondController, _) = CreateOutputController("Second", 9);
		_outputControllers.Add(firstController);
		_outputControllers.Add(secondController);
		var profile = CreateMinimalProfile();
		SetControllers(profile,
		[
			new Controller(secondController, 0, false) { IsActive = false },
			new Controller(firstController, 1, false)
		]);

		profile.SyncronizeControllerInfo();

		Assert.Collection(profile.Controllers,
			first =>
			{
				AssertMetadata(first, firstController, 1);
				Assert.True(first.IsActive);
			},
			second =>
			{
				AssertMetadata(second, secondController, 0, expectedActive: false);
				Assert.False(second.IsActive);
			});
	}

	[Fact]
	public void ExplicitPreparation_MatchesEagerConfigurationAndRunsOnlyOnce()
	{
		var configuration = new ControllerNetworkConfiguration
		{
			SupportsUniverses = true,
			ProtocolType = ProtocolTypes.sACN,
			TransmissionMethod = TransmissionMethods.Multicast,
			IpAddress = System.Net.IPAddress.Parse("239.1.2.3"),
			Universes =
			[
				new UniverseConfiguration { UniverseNumber = 17, Active = true, Size = 512 },
				new UniverseConfiguration { UniverseNumber = 18, Active = false, Size = 512 }
			]
		};
		var (controller, module) = CreateOutputController("Network", 512, configuration);
		var eager = new Controller(controller, 2);
		var prepared = new Controller(controller, 2, initializeNetworkConfiguration: false);

		Parallel.Invoke(prepared.InitializeNetworkConfiguration, prepared.InitializeNetworkConfiguration);

		Assert.True(prepared.HasNetworkSupport);
		Assert.Equal(eager.HasNetworkSupport, prepared.HasNetworkSupport);
		Assert.Equal(eager.ControllerNetworkConfiguration.ProtocolType, prepared.ControllerNetworkConfiguration.ProtocolType);
		Assert.Equal(eager.ControllerNetworkConfiguration.TransmissionMethod, prepared.ControllerNetworkConfiguration.TransmissionMethod);
		Assert.Equal(eager.ControllerNetworkConfiguration.IpAddress, prepared.ControllerNetworkConfiguration.IpAddress);
		Assert.Equal(eager.ControllerNetworkConfiguration.Universes.Select(x => (x.UniverseNumber, x.Active, x.Size)),
			prepared.ControllerNetworkConfiguration.Universes.Select(x => (x.UniverseNumber, x.Active, x.Size)));
		module.Verify(instance => instance.GetNetworkConfiguration(), Times.Exactly(2));
	}

	[Fact]
	public async Task ProfilePreparation_ProcessesOnlyActiveControllersAndHonorsCancellationBetweenControllers()
	{
		var (inactiveController, inactiveModule) = CreateOutputController("Inactive", 4);
		var firstConfiguration = new ControllerNetworkConfiguration { SupportsUniverses = true, Universes = [] };
		var (firstController, firstModule) = CreateOutputController("First", 5, firstConfiguration);
		var (secondController, secondModule) = CreateOutputController("Second", 6,
			new ControllerNetworkConfiguration { SupportsUniverses = true, Universes = [] });
		var profile = CreateMinimalProfile();
		SetControllers(profile,
		[
			new Controller(inactiveController, 0, false) { IsActive = false },
			new Controller(firstController, 1, false),
			new Controller(secondController, 2, false)
		]);
		using var cancellation = new CancellationTokenSource();
		firstModule.Setup(instance => instance.GetNetworkConfiguration()).Returns(() =>
		{
			cancellation.Cancel();
			return firstConfiguration;
		});

		await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
			profile.InitializeControllerNetworkConfigurationAsync(cancellation.Token));

		inactiveModule.Verify(instance => instance.GetNetworkConfiguration(), Times.Never);
		firstModule.Verify(instance => instance.GetNetworkConfiguration(), Times.Once);
		secondModule.Verify(instance => instance.GetNetworkConfiguration(), Times.Never);
	}

	[Fact]
	public void PreparationFailure_DoesNotMarkSnapshotReadyAndCanBeRetried()
	{
		var configuration = new ControllerNetworkConfiguration { SupportsUniverses = true, Universes = [] };
		var (controller, module) = CreateOutputController("Retry", 3, configuration);
		var snapshot = new Controller(controller, 0, initializeNetworkConfiguration: false);
		module.SetupSequence(instance => instance.GetNetworkConfiguration())
			.Throws(new InvalidOperationException("provider failure"))
			.Returns(configuration);

		Assert.Throws<InvalidOperationException>(snapshot.InitializeNetworkConfiguration);
		Assert.False(snapshot.HasNetworkSupport);
		 snapshot.InitializeNetworkConfiguration();

		Assert.True(snapshot.HasNetworkSupport);
		module.Verify(instance => instance.GetNetworkConfiguration(), Times.Exactly(2));
	}

	[Fact]
	public async Task ProfilePreparation_ReportsRemovedControllerSourceBeforeExportCanProceed()
	{
		var removedController = (Controller)RuntimeHelpers.GetUninitializedObject(typeof(Controller));
		typeof(Controller).GetProperty(nameof(Controller.Name))!.SetValue(removedController, "Removed controller");
		typeof(Controller).GetProperty(nameof(Controller.IsActive))!.SetValue(removedController, true);
		var profile = CreateMinimalProfile();
		SetControllers(profile, [removedController]);

		await Assert.ThrowsAsync<InvalidOperationException>(() =>
			profile.InitializeControllerNetworkConfigurationAsync(CancellationToken.None));
	}

	[Fact]
	public void StalePreparationCompletion_IsRejectedAfterProfileReplacementOrNavigation()
	{
		var preparedProfile = CreateMinimalProfile();
		var replacementProfile = CreateMinimalProfile();
		using var cancellation = new CancellationTokenSource();

		Assert.True(BulkExportSummaryStage.IsCurrentNetworkPreparation(
			7, 7, preparedProfile, preparedProfile, cancellation.Token, isDisposed: false));
		Assert.False(BulkExportSummaryStage.IsCurrentNetworkPreparation(
			7, 8, preparedProfile, preparedProfile, cancellation.Token, isDisposed: false));
		Assert.False(BulkExportSummaryStage.IsCurrentNetworkPreparation(
			7, 7, preparedProfile, replacementProfile, cancellation.Token, isDisposed: false));
		Assert.False(BulkExportSummaryStage.IsCurrentNetworkPreparation(
			7, 7, preparedProfile, preparedProfile, cancellation.Token, isDisposed: true));

		cancellation.Cancel();
		Assert.False(BulkExportSummaryStage.IsCurrentNetworkPreparation(
			7, 7, preparedProfile, preparedProfile, cancellation.Token, isDisposed: false));
	}

	[Fact]
	public void ControllerWithoutUniverseSupport_IsNotReportedAsUniverseCapable()
	{
		var configuration = new ControllerNetworkConfiguration { SupportsUniverses = false, Universes = [] };
		var (controller, module) = CreateOutputController("No universes", 2, configuration);
		var snapshot = new Controller(controller, 0, initializeNetworkConfiguration: false);

		snapshot.InitializeNetworkConfiguration();

		Assert.False(snapshot.HasNetworkSupport);
		Assert.Null(snapshot.ControllerNetworkConfiguration);
		module.Verify(instance => instance.GetNetworkConfiguration(), Times.Once);
	}

	[Fact]
	public void ControllerWithoutNetworkSupport_DoesNotRequestConfiguration()
	{
		var (controller, module) = CreateOutputController("Local", 2, supportsNetwork: false);
		var snapshot = new Controller(controller, 0, initializeNetworkConfiguration: false);

		snapshot.InitializeNetworkConfiguration();

		Assert.False(snapshot.HasNetworkSupport);
		Assert.Null(snapshot.ControllerNetworkConfiguration);
		module.Verify(instance => instance.GetNetworkConfiguration(), Times.Never);
	}

	public void Dispose()
	{
		SetVixenSystemProperty(nameof(VixenSystem.OutputControllers), _previousOutputControllers);
		SetVixenSystemProperty(nameof(VixenSystem.DataFlow), _previousDataFlow);
	}

	private static (OutputController Controller, Mock<IControllerModuleInstance> Module)
		CreateOutputController(string name, int channels, ControllerNetworkConfiguration? configuration = null,
			bool supportsNetwork = true)
	{
		var dataPolicy = new Mock<IDataPolicy>();
		var dataPolicyFactory = new Mock<IDataPolicyFactory>();
		dataPolicyFactory.Setup(factory => factory.CreateDataPolicy()).Returns(dataPolicy.Object);
		var module = new Mock<IControllerModuleInstance>();
		module.SetupGet(instance => instance.DataPolicyFactory).Returns(dataPolicyFactory.Object);
		module.SetupGet(instance => instance.SupportsNetwork).Returns(supportsNetwork);
		module.Setup(instance => instance.GetNetworkConfiguration()).Returns(configuration!);
		var consumer = new TestOutputModuleConsumer(module.Object);
		var outputs = new OutputCollection<CommandOutput>();
		for (var index = 0; index < channels; index++)
		{
			outputs.AddOutput(new CommandOutput(Guid.NewGuid(), $"Output {index + 1}", index));
		}
		var mediator = new OutputMediator<CommandOutput>(outputs, Mock.Of<IUpdatableOutputCount>());
		var controller = new OutputController(Guid.NewGuid(), name, mediator, Mock.Of<IHardware>(), consumer);
		return (controller, module);
	}

	private static ExportProfile CreateMinimalProfile()
	{
		var profile = (ExportProfile)Activator.CreateInstance(typeof(ExportProfile), nonPublic: true)!;
		typeof(ExportProfile).GetProperty(nameof(ExportProfile.SequenceFiles))!.SetValue(profile, new List<string>());
		typeof(ExportProfile).GetProperty(nameof(ExportProfile.Controllers))!
			.SetValue(profile, new List<Controller>(), BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);
		return profile;
	}

	private static void SetControllers(ExportProfile profile, List<Controller> controllers)
	{
		typeof(ExportProfile).GetProperty(nameof(ExportProfile.Controllers))!
			.SetValue(profile, controllers, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);
	}

	private static void AssertMetadata(Controller snapshot, OutputController source, int expectedIndex, bool expectedActive = true)
	{
		Assert.Equal(source.Id, snapshot.Id);
		Assert.Equal(source.Name, snapshot.Name);
		Assert.Equal(source.OutputCount, snapshot.Channels);
		Assert.Equal(expectedIndex, snapshot.Index);
		Assert.Equal(expectedActive, snapshot.IsActive);
	}

	private static void SetVixenSystemProperty(string propertyName, object value)
	{
		var property = typeof(VixenSystem).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static);
		property!.GetSetMethod(true)!.Invoke(null, [value]);
	}

	private sealed class TestOutputModuleConsumer(IControllerModuleInstance module) : IOutputModuleConsumer<IControllerModuleInstance>
	{
		public Guid ModuleId { get; } = Guid.NewGuid();
		public Guid ModuleInstanceId { get; } = Guid.NewGuid();
		public IControllerModuleInstance Module { get; } = module;
		public int UpdateInterval => 0;
		public IOutputDeviceUpdateSignaler UpdateSignaler => null!;
		public bool SupportsNamedOutputs => false;
		public bool IsRunning => false;
		public bool IsPaused => false;
		public bool HasSetup => false;
		public void NameOutputs() { }
		public void Start() { }
		public void Stop() { }
		public void Pause() { }
		public void Resume() { }
		public bool Setup() => false;
	}
}
