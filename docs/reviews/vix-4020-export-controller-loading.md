# VIX-4020: Export controller loading design review

## Evidence

JIRA VIX-4020 reports a roughly 14-second delay entering the Export Wizard controller stage, a temporary Test controller, and no loading indication. The supplied timeline snapshot, C:\Dev\Snapshots\MarkBar\VIX-4020-initial-capture.dtp, shows 16,204.1 ms of elapsed own time beneath System.Net.NameResolution.dll. Its caller chain is BulkExportControllersStage.StageStart, ExportProfile.SyncronizeControllerInfo, Export.CreateControllerInfo, Controller's constructor, E131OutputPlugin.GetNetworkConfiguration, and DetermineIp. This is waiting time, not 16 seconds of CPU consumption. Six SocketException events report "No such host is known" at approximately 2.7-second intervals. A null-argument exception also occurs in this path. A Main-thread-only waiting tree for thread 40924 between 2.4 and 18.8 seconds attributes 16,197 ms to name resolution.

Controller list population itself consumes only about 36.5 ms of running time in the capture. Its displayed fields are Name, Channels, IsActive, and Index. The eager network configuration in the Controller constructor is unnecessary for this display. BulkExportControllersStage.Designer.cs explicitly inserts a ListViewItem named Test.

## Recommendation

Separate the controller metadata snapshot used for selection from explicit network configuration preparation. Retain eager behavior for existing core callers by preserving the current constructor and CreateControllerInfo signatures and adding overloads. Wizard profile synchronization uses the metadata-only overload. Do not perform DNS from property getters: that would silently move the freeze into capability checks on another wizard page.

Prepare the selected controllers' network configuration on a worker only when the export requests a universe configuration file. The summary stage already provides an asynchronous StageStart boundary, but readiness, error, cancellation, and re-entry must be explicit. Publish the prepared state on the UI thread before computing the universe-support warning or allowing export to start. Display loading feedback during actual network preparation. Ordinary sequence export should require no controller address resolution. Remove the designer Test row.

This is a separation-of-responsibilities change within the existing export snapshot, not a new dependency-injection framework or a controller-module interface redesign. Apply the project dotnet-best-practices, csharp-async, and csharp-docs skills during implementation. Keep the existing WinForms wizard as legacy UI maintenance; Catel ViewModels are not involved.

## Contracts and review risks

HasNetworkSupport and ControllerNetworkConfiguration remain explicit snapshot values. Before network preparation they must not be interpreted as completed capability results. All universe consumers must run after preparation. Preserve the existing E1.31 hostname resolution, IPv4 selection, multicast handling, throttling, universe numbering, and failure fallback. Do not introduce a global DNS cache or arbitrary timeout that changes exported addresses.

Cloning and profile serialization need regression coverage. Runtime source-controller references and readiness flags must not become DataMembers. Profile synchronization already rebuilds current runtime metadata while retaining saved active flags and order; use that existing path after deserialization. Clone's current comment that all members are value types is already inaccurate because configuration is a reference; update only documentation affected by the new snapshot behavior.

The shared Controller type is used by the legacy export dialog and universe writers as well as the wizard. Gortex reports a lower-bound impact of 65 affected symbols, including ExportProfileCloneTests. Preserve current eager defaults and validate both old callers and metadata-only callers. Do not refactor unrelated types out of Export.cs merely to apply general style guidance.

## Validation gate

Use a fake controller module to count configuration requests. Metadata construction, selection, reordering, and cloning must issue zero requests; explicit preparation must produce the same configuration as eager construction. Test non-network controllers, multicast and unicast configurations, multiple universes, inactive controllers, and provider failures without relying on DNS or controller hardware.

Build tests with full Visual Studio MSBuild before dotnet test --no-build, because the solution has C++/CLI dependencies. Reproduce with the original large profile and collect a new trace. The decisive result is no DetermineIp beneath the controller-selection stage and no network wait on the UI thread; do not claim an observed speedup until that capture exists.
