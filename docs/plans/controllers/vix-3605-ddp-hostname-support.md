# DDP Controller Host Name Support (VIX-3605)

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds. Maintain this document in accordance with `.agents/PLANS.md` (the repository's rules for execution plans, found at that exact path from the repository root).

This plan implements the design captured in `docs/controllers/vix-3605-ddp-hostname-support.md` (read that file first if any requirement below seems ambiguous; this plan repeats everything from it that is relevant to implementation, but that document is the source of truth for the *product* decision, this document is the source of truth for *how to build it*). Anyone picking up this plan needs no other document to succeed.

## Purpose / Big Picture

Today, the DDP controller ("Distributed Display Protocol", a simple UDP-based lighting control protocol used by controllers like WLED and Falcon Player) can only be configured with a literal IPv4 address such as `192.168.1.50`. The setup dialog for a DDP controller (right-click a DDP controller in Display Setup, choose "Properties") is `src/Vixen.Modules/Controller/DDP/DDPSetup.cs`. Its `Address` property is typed as `System.Net.IPAddress`, and if the text the user types does not parse as a literal IP address, the property silently discards it and substitutes `127.0.0.1` (loopback) instead — so today, typing a network host name like `wled-porch.local` into that box is accepted by the text box but thrown away the moment the dialog is read.

After this plan is complete, a user can right-click a DDP controller, choose "Properties", select a new "Host Name" radio button, type a DNS host name (e.g. `wled-porch` or `wled-porch.local`) instead of an IP address, and click OK. The dialog will look up that host name's IPv4 address (using the same underlying mechanism as typing `ping wled-porch` at a command prompt) and, if found, use that resolved address for all of DDP's actual network communication — exactly as if the user had typed the IP address directly. If the user reopens the Properties dialog later, it will show "Host Name" selected with the original host name text still in the box (not the IP address it resolved to), so the user's original intent is preserved and editable. If the host name cannot be resolved (typo, DHCP hasn't assigned it yet, no network), the dialog stays open and shows an error message box, exactly like other validation failures elsewhere in this codebase (see `MessageBoxForm` usage in `src/Vixen.Modules/Controller/E131/SetupForm.cs`), so the user can correct their entry instead of silently losing their configuration.

This mirrors a feature the Streaming ACN / E1.31 controller (`src/Vixen.Modules/Controller/E131/`) already has in its "add Unicast destination" dialog: a pair of radio buttons for "IP Address" vs. "Network Name". This plan's design deliberately does not copy E131's implementation verbatim — see "Differences from the E131 precedent" in the Context and Orientation section below for exactly what is different and why.

Beyond the one-time setup-dialog resolution, this plan also makes the DDP controller keep working automatically if a host-name-configured controller's IP address changes later (for example, a router reassigns a new DHCP lease to the physical DDP device days after the user configured it). Every time the controller (re)connects — starting the show, resuming from pause, or reconfiguring via Setup — it re-resolves the host name in the background of that existing reconnect step and silently updates the stored IP address if it has changed, with no dialog, no user action, and no interruption required. If the host name temporarily fails to resolve (e.g. the network is briefly down), the controller logs a warning and keeps using the last address that worked, rather than failing outright.

You can observe this working in two ways once the plan is complete:

1. Automated tests: running `dotnet test src/Vixen.Tests/Vixen.Tests.csproj --filter "FullyQualifiedName~DDP"` shows new passing tests for host-name resolution (a real, always-resolvable name — `localhost` — succeeds; a name guaranteed never to resolve — `nonexistent.invalid`, using the `.invalid` top-level domain, which is permanently reserved by RFC 2606 to never resolve on any real network, making the test deterministic and independent of any live network connection — fails cleanly without throwing) and for `DDPData.Clone()` round-tripping the new `HostName` field.
2. Manual verification against the running application: build and launch Vixen, add a DDP controller (or open an existing one's Properties), select "Host Name", type `localhost`, click OK, reopen Properties, and observe "Host Name" is still selected with `localhost` still in the box (not `127.0.0.1`). Then type an unresolvable name like `this-does-not-exist.invalid` and observe an error message box appears and the dialog does not close.
3. Runtime re-validation: configure a controller with a host name, then edit your system's `hosts` file (or equivalent local DNS override) to point that host name at a different IP address, then start/resume the show (or trigger any action that calls `DDP.cs`'s `OpenConnection()`) and check the NLog log output for a line showing the address was updated to the new resolved value — without reopening the Properties dialog.

## Progress

- [x] (2026-07-03) Milestone 1: JIRA issue. Already satisfied — [VIX-3605](https://vixenlights.atlassian.net/browse/VIX-3605) ("DDP Controller should allow for host name instead of just IP address") already existed before this plan was written and has been updated with the finalized requirements and acceptance criteria from `docs/controllers/vix-3605-ddp-hostname-support.md`. No further JIRA field updates are needed before starting code work; only the closeout note in Milestone 8 remains.
- [x] (2026-07-03) Milestone 2: Branch. Already satisfied — the repository's current branch is already `VIX-3605` (confirmed via `git status` before this plan was written), so no new branch needs to be created. All commits for this plan go on this existing branch, prefixed `VIX-3605` per the project's convention (see recent commits like `VIX-3776 Increase mark limit to 10000`).
- [ ] Milestone 3: `DDPHostNameResolver` — a small, pure, unit-testable static class that wraps `Dns.GetHostAddresses` and picks the first IPv4 result. Not started.
- [ ] Milestone 4: `DDPData.HostName` field, `Clone()` update, XML doc comments. Not started.
- [ ] Milestone 5: `DDPSetup` UI and code-behind changes (radio buttons, second text box, OK-button validation/resolution flow, error message box, constructor pre-fill logic). Not started.
- [ ] Milestone 6: `DDP.cs` wiring (`Setup()` reads `setup.HostName`; remove the stale commented-out hostname breadcrumb line in `OpenConnection()`; `OpenConnection()` re-resolves `HostName` on every (re)connect and auto-updates `Address`, logging a warning and falling back to the last known-good address on resolution failure). Not started.
- [ ] Milestone 7: Apply project skills (`csharp-docs`, `csharp-async`, `dotnet-best-practices`, `dotnet-design-pattern-review`) and address findings. Not started.
- [ ] Milestone 8: Manual end-to-end verification against the running application and JIRA closeout note. Not started.

## Surprises & Discoveries

*(Fill in as work progresses.)*

## Decision Log

- Decision: Extract DNS resolution into a small internal static class, `DDPHostNameResolver` (`src/Vixen.Modules/Controller/DDP/DDPHostNameResolver.cs`), rather than calling `Dns.GetHostAddresses` directly inside `DDPSetup`'s code-behind.
  Rationale: `DDPSetup` is a `System.Windows.Forms.Form` subclass. There is no existing precedent anywhere in `Vixen.Tests` for unit-testing a WinForms `Form` directly (no STA-thread test infrastructure exists in this test project today — confirmed by grep, zero matches for `STAThread`/`ApartmentState` in `src/Vixen.Tests`), and this plan does not want to introduce that infrastructure just for this feature. This mirrors the exact boundary the `docs/plans/vix-3933-element-tags-api.md` ExecPlan already established for this codebase: UI/orchestration classes (there, `VixenSystem`; here, `DDPSetup`) are verified manually, while the pure logic they call is pulled out into a small class that unit tests can exercise directly with no forms, no static global state, and no mocking required.
  Date/Author: 2026-07-03, planning session.
- Decision: `DDPHostNameResolver` is `internal`, not `public`, with `InternalsVisibleTo` granted to `Vixen.Tests` (added to `DDP.csproj`, following the exact pattern already used in `src/Vixen.Modules/App/FPPClient/FPPClient.csproj`).
  Rationale: This class is an implementation detail of the DDP module's setup dialog, not something any other assembly should call. Module assemblies in this codebase (`Vixen.Modules/Controller/DDP`) are loaded dynamically as plugins (`EnableDynamicLoading` — see `FPPClient.csproj` for the same setting) and their public surface should stay minimal. Keeping it `internal` and using the already-established `InternalsVisibleTo` mechanism (rather than making it `public` purely to make it testable) matches how `Vixen.Core`'s `ElementTagService` validation helper was kept internal for the same reason (see the Decision Log of `docs/plans/vix-3933-element-tags-api.md`).
  Date/Author: 2026-07-03, planning session.
- Decision: The resolver selects the *first* IPv4 (`AddressFamily.InterNetwork`) address returned by `Dns.GetHostAddresses`, using `Array.Find`/LINQ `.FirstOrDefault(...)`, not the *last* one.
  Rationale: `docs/controllers/vix-3605-ddp-hostname-support.md` explicitly calls out that E131's two `Dns.GetHostAddresses` call sites (`src/Vixen.Modules/Controller/E131/E131OutputPlugin.cs`, around lines 445-467 and 660-671) iterate over every returned address without a `break`, so they end up keeping the *last* IPv4 match rather than the first if a host has multiple `A` records. The design spec deliberately chose not to reproduce that quirk for DDP, since "first match" is the more conventional and expected behavior and there is no reason to copy an accidental quirk.
  Date/Author: 2026-07-03, design spec / planning session.
- Decision: Use two hard-coded test host names — `localhost` (always resolves, to `127.0.0.1` or `::1`, without any network access) and a name under the `.invalid` top-level domain (e.g. `this-host-does-not-exist.invalid`, permanently reserved by RFC 2606 to never resolve on any real DNS server) — for the resolver's unit tests, instead of mocking `Dns.GetHostAddresses`.
  Rationale: `System.Net.Dns`'s resolution methods are static and not virtual/interface-based, so they cannot be mocked with Moq (already a dependency of `Vixen.Tests`) without introducing a wrapper interface purely for testability — an abstraction this small feature does not need. `localhost` and the `.invalid` TLD are both guaranteed-deterministic regardless of network connectivity or DNS server configuration, so the tests remain reliable in CI and offline. If `localhost` resolves only to IPv6 (`::1`) in some environment and the IPv4-only assertion fails, note that in Surprises & Discoveries and fall back to asserting only "resolution succeeds and returns a non-null address" for that specific case — but on Windows (this project's only target platform, see `CLAUDE.md`), `localhost` reliably includes `127.0.0.1` in its `Dns.GetHostAddresses` results.
  Date/Author: 2026-07-03, planning session.
- Decision: `DDPSetup`'s existing `buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK` designer-level auto-close binding (`DDPSetup.Designer.cs` line 41: `this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;`) must be removed and replaced with an explicit `Click` handler, mirroring how `E131/UnicastForm.Designer.cs` wires its own OK button (`okButton.Click += okButton_Click;`, with **no** `DialogResult` property set on the button itself — `UnicastForm.cs`'s `okButton_Click` sets `DialogResult = DialogResult.OK; Close();` itself, only after the user's input is accepted).
  Rationale: A WinForms button whose `DialogResult` property is set directly closes the dialog immediately on click, before any code can run to validate or resolve the input. Host-name resolution must be able to fail and keep the dialog open (per the design spec's explicit requirement), which is impossible with the auto-close binding. This is not a new pattern for this codebase — E131's own dialog already avoids the auto-close binding for exactly this reason.
  Date/Author: 2026-07-03, planning session.
- Decision: `DDP.cs`'s `OpenConnection()` re-resolves `DDPData.HostName` (when populated) every time it runs — i.e. on every `Start()`, `Resume()`, and post-`Setup()` reconnect — rather than adding a background timer/polling thread to watch for DNS changes continuously.
  Rationale: The user asked for the controller to "validate that the host still resolves to the specified IP address so if the host DNS changes the IP can be updated without user intervention." `OpenConnection()` already runs at every point where a fresh connection is meaningful (show start, resume from pause, re-applying setup changes), which is a sufficient and natural cadence for this — light shows do not run continuously for days without ever stopping/starting, and a background polling thread would add complexity (timer lifecycle, thread-safety around `_data.Address`, disposal) that this feature does not need. This keeps the change additive and reuses an existing call site rather than introducing new infrastructure, consistent with the project's "don't add abstractions beyond what the task requires" guidance.
  Date/Author: 2026-07-03, planning session (added after initial plan review; see the Purpose section's second paragraph, added in the same revision).
- Decision: On re-resolution failure inside `OpenConnection()`, log a warning (`Logging.Warn(LogTag + ...)`, the exact pattern already used a few lines below for UDP connect failures) and continue using the last known-good `_data.Address`, rather than aborting the connection attempt.
  Rationale: The user explicitly asked for "error logging if the resolution fails," and a transient DNS failure (network blip, DNS server briefly down) should not take down a running or resuming show that was working fine a moment ago on its last-known address. This mirrors E131's own existing resolution-failure handling (`E131OutputPlugin.cs`'s `Setup()`/`DetermineIp()`, both of which log a warning and continue rather than throwing), so it is also the established "common pattern" the user referred to.
  Date/Author: 2026-07-03, planning session.
- Decision: `OpenConnection()` only calls `DDPHostNameResolver.TryResolveToIPv4` when `_data.HostName` is non-null/non-empty; controllers configured with a literal IP address perform no resolution attempt and are unaffected.
  Rationale: Calling DNS resolution on a value that is already a literal IP address (e.g. `Dns.GetHostAddresses("192.168.1.50")`) is unnecessary — `IPAddress.TryParse`-validated literals don't change out from under the user, unlike DNS-backed names — and would add a needless network/system call to the hot path of every controller start for controllers that never opted into host-name mode.
  Date/Author: 2026-07-03, planning session.
- Decision: Do not reuse E131's `IpTextBox` control (`src/Vixen.Modules/Controller/E131/Controls/IPTextBox.cs`, a `TextBox` subclass that overrides `CreateParams` to use the native Windows `SysIPAddress32` masked-input window class). `DDPSetup`'s IP address box remains the plain `System.Windows.Forms.TextBox` it already is today (`textBoxIPAddress`), validated with `IPAddress.TryParse` exactly as it is today.
  Rationale: `IpTextBox` is a `public` class, but it lives in the E131 module's own project (`src/Vixen.Modules/Controller/E131/E131.csproj`), and `DDP.csproj` has no project reference to it today (confirmed by reading `DDP.csproj`, which references only `Vixen.Common/Controls`, `Vixen.Common/Resources`, and `Vixen.Core`). Adding a cross-module project reference between two independently-loadable controller plugins, purely to reuse one small `TextBox` subclass, is a bigger architectural change than this feature calls for and was not requested. The plain `TextBox` is also what DDP's IP box already uses today with no reported issues, so this plan does not change that part of the existing behavior.
  Date/Author: 2026-07-03, planning session.

## Outcomes & Retrospective

*(Write this section once all milestones are complete.)*

## Context and Orientation

**What DDP is.** DDP ("Distributed Display Protocol") is a simple lighting-control protocol transmitted over UDP, implemented in `src/Vixen.Modules/Controller/DDP/DDP.cs` (`namespace VixenModules.Output.DDP`, class `DDP : ControllerModuleInstanceBase`). This is a "Controller" module — one of Vixen's plugin types described in `CLAUDE.md` — meaning it's the code that actually sends lighting data out over the network to physical hardware. `ControllerModuleInstanceBase` is a base class defined in `Vixen.Core` that every controller module (DDP, E131, and others) inherits from.

**The three files this plan touches, and what each currently does.**

`src/Vixen.Modules/Controller/DDP/DDPData.cs` is the persisted configuration for one DDP controller instance — a small `[DataContract]` class with one `[DataMember]` today:

    [DataContract]
    public class DDPData : ModuleDataModelBase
    {
        [DataMember]
        public string Address { get; set; }

        public DDPData()
        {
            Address = IPAddress.Loopback.ToString();
        }

        public override IModuleDataModel Clone()
        {
            DDPData result = new DDPData();
            result.Address = Address;
            return result;
        }
    }

`Address` is a `string` holding an IP address's text form (e.g. `"192.168.1.50"`), and it is what gets saved to disk when a Vixen profile is saved. `ModuleDataModelBase` is a base class in `Vixen.Core` providing the `IModuleDataModel` contract every module's saved-settings class implements — the mechanism Vixen uses to persist per-module configuration inside a saved show profile.

`src/Vixen.Modules/Controller/DDP/DDPSetup.cs` and its designer-generated companion `DDPSetup.Designer.cs` together form the WinForms "Properties" dialog a user sees when they configure a DDP controller. (This project uses WinForms, not WPF, for controller setup dialogs — despite `CLAUDE.md`'s general description of Vixen as a WPF application, individual controller/effect modules like this one can and do use WinForms `Form` subclasses; do not be surprised by `using System.Windows.Forms;` here.) Today, `DDPSetup`'s constructor takes a `DDPData` and immediately tries to parse its `Address` as an `IPAddress`; if parsing fails for any reason it silently substitutes `IPAddress.Loopback`. The dialog has one `GroupBox` ("Configuration") containing a `Label` ("IP Address:") and one plain `TextBox`, `textBoxIPAddress` (see `DDPSetup.Designer.cs` for its exact `Location`/`Size`: `Point(138, 44)`, `Size(160, 27)`, inside `groupBox1` at `Point(16, 19)`, `Size(385, 103)`). The dialog exposes one public property, `Address` (type `System.Net.IPAddress`), whose getter re-parses `textBoxIPAddress.Text` (falling back to loopback on failure) and whose setter writes `value.ToString()` into the text box. The OK button, `buttonOK`, has its `DialogResult` property set directly to `DialogResult.OK` in the designer (`DDPSetup.Designer.cs` line 41), which means clicking it closes the dialog immediately with no code running first — there is no `Click` handler on it today.

`src/Vixen.Modules/Controller/DDP/DDP.cs` is the runtime controller class. Its `Setup()` method (used when the user right-clicks the controller and chooses Properties) is:

    public override bool Setup()
    {
        Logging.Trace(LogTag + "Setup()");
        _isRunning = false;
        DDPSetup setup = new DDPSetup(_data);
        if (setup.ShowDialog() == DialogResult.OK) {
            if (setup.Address != null)
                _data.Address = setup.Address.ToString();
            OpenConnection();
            return true;
        }
        _isRunning = true;
        return false;
    }

`OpenConnection()` (around line 203) opens the actual UDP socket used to send lighting data, calling `_udpClient.Connect(_data.Address, DDP_PORT)`. It already contains a commented-out breadcrumb left by a previous developer specifically about this feature:

    try {
        _udpClient = new UdpClient(); //doesn't work when in class constructor
        _udpClient.Connect(_data.Address, DDP_PORT);
        //_udpClient.Connect(_data Hostname, DDP_PORT);  //Switch to add Hostname support
    }
    catch {
        Logging.Warn(LogTag + "DDP: Failed connect to host");
        return false;
    }

Note that `UdpClient.Connect(string, int)` (the .NET base class library method, not something in this codebase) already accepts a host name directly and resolves it internally — but this plan does **not** rely on that, because the design spec requires resolution to happen once, at setup time in the dialog, with the resolved IP address stored in `DDPData.Address` for all runtime use. `DDP.cs`'s own network code therefore needs **no behavior change** in this plan beyond removing the now-superseded commented-out line — it keeps calling `_udpClient.Connect(_data.Address, ...)` with what is now always a plain IP-address string, exactly as it does today. `GetNetworkConfiguration()` (around line 253, used for FPP-export style features) similarly keeps using `IPAddress.TryParse(_data.Address, ...)` unchanged, because `_data.Address` will always contain a resolved IP-literal string, never a host name, after this plan.

**The E131 precedent this plan is modeled on, and exactly what is different (and why).** `src/Vixen.Modules/Controller/E131/UnicastForm.cs` and `UnicastForm.Designer.cs` implement a small dialog with two mutually exclusive `RadioButton` controls, `ipRadio` ("IP Address") and `networkNameRadio` ("Network Name"), and two `TextBox` controls occupying the *same screen location* (`Point(45, 18)`), only one of which is `Enabled` and frontmost (`BringToFront()`) at a time — swapped by a private `updateChecked()` method called from both radio buttons' `CheckedChanged` event. This plan reuses that same UI shape (radio buttons + overlapping text boxes) for `DDPSetup`, but with three deliberate differences, already recorded above in the Decision Log and repeated here for a reader who skips that section:

1. E131 stores one ambiguous string (`E131ModuleDataModel.Unicast`) that might be an IP literal or a host name, and resolves it fresh via `Dns.GetHostAddresses` every single time the controller starts (`E131OutputPlugin.cs`, inside `Setup()`), with no caching, blocking synchronously on the calling thread. This plan instead resolves **once**, when the user clicks OK in the setup dialog, and stores the already-resolved IP address in `DDPData.Address` — `DDP.cs`'s runtime path (`OpenConnection()`, `GetNetworkConfiguration()`) performs no DNS resolution at all, before or after this change.
2. E131's `Dns.GetHostAddresses` loops keep the *last* IPv4 match, not the first (a loop without `break`). This plan's resolver keeps the *first* IPv4 match.
3. E131's `UnicastForm.IpAddrText` property setter is effectively dead code (no call site anywhere in the codebase ever assigns to it — the form is always constructed via `new UnicastForm()` with no pre-population) and, if it were used, has a latent bug: it decides which text box to populate based on `IPAddress.TryParse`, but never updates `ipRadio.Checked`/`networkNameRadio.Checked` to match, so `updateChecked()` (which reads the *current* checked state) would show the wrong mode. This plan's `DDPSetup` constructor must decide the pre-filled text *and* which radio button is checked together, driven by whether `DDPData.HostName` (the new field this plan adds) is populated, so this bug is not reintroduced.

**Testing conventions.** `src/Vixen.Tests/Vixen.Tests.csproj` is an xunit v3 + Moq test project. Run it from the repository root with `dotnet test src/Vixen.Tests/Vixen.Tests.csproj`. Tests follow an AAA (Arrange-Act-Assert) convention with method names of the shape `MethodName_Condition_ExpectedBehavior` — see the header comment in `src/Vixen.Tests/Utility/NamingUtilitiesTests.cs` for the canonical example already in this codebase. `Vixen.Tests.csproj` currently has `ProjectReference` entries for `Vixen.Core`, two `Vixen.Common` sub-projects, and three modules (`FPPClient`, `ExportWizard`, `LivePreview`, `Spiral`) — it does **not** yet reference the DDP controller project, so this plan must add that reference (Milestone 3).

**Error dialog convention.** `Common.Controls.MessageBoxForm` (`src/Vixen.Common/Controls/MessageBoxForm.cs`) is the shared error/message dialog used throughout the controller modules. It has two constructors; this plan uses the newer, preferred one:

    public MessageBoxForm(string messageBoxData, string messageBoxTitle, MessageBoxButtons buttons, Icon icon)

used like:

    MessageBoxForm mbf = new MessageBoxForm("Could not resolve host 'foo'.", "DDP Setup Error", MessageBoxButtons.OK, SystemIcons.Error);
    mbf.ShowDialog(this);

Do **not** use the older `MessageBoxForm.msgIcon = SystemIcons.Error; new MessageBoxForm(message, title, false, false);` pattern seen throughout `E131/SetupForm.cs` — that static-field-based style is being phased out in favor of the constructor-parameter style shown above (this was an explicit correction from the user during this plan's design phase; the design spec document was updated to match and this plan follows the corrected version).

## Plan of Work

### Milestone 3 — `DDPHostNameResolver` and its unit tests

Create `src/Vixen.Modules/Controller/DDP/DDPHostNameResolver.cs`:

    using System.Linq;
    using System.Net;
    using System.Net.Sockets;

    namespace VixenModules.Output.DDP
    {
        /// <summary>
        /// Resolves a DNS host name to an IPv4 address for use by the DDP controller setup dialog.
        /// </summary>
        internal static class DDPHostNameResolver
        {
            /// <summary>
            /// Attempts to resolve <paramref name="hostName"/> to an IPv4 address.
            /// </summary>
            /// <param name="hostName">A DNS host name, e.g. "wled-porch" or "wled-porch.local". Must not be null or empty.</param>
            /// <param name="resolvedAddress">
            /// When this method returns <see langword="true"/>, the first IPv4 (<see cref="AddressFamily.InterNetwork"/>)
            /// address found for <paramref name="hostName"/>. When this method returns <see langword="false"/>,
            /// <see langword="null"/>.
            /// </param>
            /// <returns><see langword="true"/> if an IPv4 address was found; otherwise <see langword="false"/>.</returns>
            internal static bool TryResolveToIPv4(string hostName, out IPAddress resolvedAddress)
            {
                resolvedAddress = null;
                if (string.IsNullOrWhiteSpace(hostName))
                {
                    return false;
                }

                IPAddress[] addresses;
                try
                {
                    addresses = Dns.GetHostAddresses(hostName);
                }
                catch (SocketException)
                {
                    return false;
                }

                resolvedAddress = addresses.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
                return resolvedAddress != null;
            }
        }
    }

Note the `catch (SocketException)` is deliberately narrower than E131's bare `catch` — `Dns.GetHostAddresses` documents `SocketException` as the exception it throws for resolution failures; a bare `catch` would also silently swallow unrelated bugs (e.g. a `NullReferenceException` from a future code change), which the `dotnet-best-practices` skill (applied in Milestone 7) would flag. If manual testing in Milestone 8 discovers a different exception type is actually thrown in some environment, widen this catch and record the discovery in Surprises & Discoveries with evidence (the exact exception type and a repro) before widening it.

Add the project reference and `InternalsVisibleTo` grant needed for testing. In `src/Vixen.Tests/Vixen.Tests.csproj`, add, alongside the existing module references:

    <ProjectReference Include="..\Vixen.Modules\Controller\DDP\DDP.csproj" />

In `src/Vixen.Modules/Controller/DDP/DDP.csproj`, add a new `ItemGroup` (following the exact pattern already used in `src/Vixen.Modules/App/FPPClient/FPPClient.csproj`):

    <ItemGroup>
        <AssemblyAttribute Include="System.Runtime.CompilerServices.InternalsVisibleToAttribute">
            <_Parameter1>Vixen.Tests</_Parameter1>
        </AssemblyAttribute>
    </ItemGroup>

Create `src/Vixen.Tests/Controller/DDP/DDPHostNameResolverTests.cs` (new folder — no `Vixen.Tests/Controller` folder exists yet; create it) with tests:

- `TryResolveToIPv4_LocalHost_ReturnsTrueAndAnIPv4Address` — arrange: hostname `"localhost"`; act: call `DDPHostNameResolver.TryResolveToIPv4`; assert: returns `true`, `resolvedAddress` is not null, and `resolvedAddress.AddressFamily == AddressFamily.InterNetwork`.
- `TryResolveToIPv4_ReservedInvalidTld_ReturnsFalse` — arrange: hostname `"this-host-does-not-exist.invalid"`; act/assert: returns `false` and `resolvedAddress` is null.
- `TryResolveToIPv4_NullOrEmptyHostName_ReturnsFalse` — a `[Theory]` with `[InlineData(null)]` and `[InlineData("")]` and `[InlineData("   ")]`, asserting `false` is returned without throwing.

Run `dotnet test src/Vixen.Tests/Vixen.Tests.csproj --filter "FullyQualifiedName~DDPHostNameResolver"` from the repository root and confirm all new tests pass with `0` failed. Then run `msbuild Vixen.sln -m -t:restore -t:Build -p:Configuration=Debug` from the repository root and confirm the solution still builds with no new warnings.

### Milestone 4 — `DDPData.HostName`

Modify `src/Vixen.Modules/Controller/DDP/DDPData.cs` to add one new member and update `Clone()`:

    [DataContract]
    public class DDPData : ModuleDataModelBase
    {
        /// <summary>
        /// Gets or sets the resolved IP address used to communicate with the controller, in dotted-quad
        /// string form (e.g. "192.168.1.50"). This is always an IP-literal string, never a host name — if
        /// the user configured the controller using a host name, <see cref="HostName"/> holds that original
        /// entry and this property holds the IP address it most recently resolved to.
        /// </summary>
        [DataMember]
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the DNS host name the user entered in the setup dialog, or <see langword="null"/>
        /// if the controller is configured with a literal IP address instead. When set, the setup dialog
        /// pre-selects "Host Name" mode and pre-fills this value the next time it is opened.
        /// </summary>
        [DataMember]
        public string HostName { get; set; }

        public DDPData()
        {
            Address = IPAddress.Loopback.ToString();
        }

        public override IModuleDataModel Clone()
        {
            DDPData result = new DDPData();
            result.Address = Address;
            result.HostName = HostName;
            return result;
        }
    }

Because `HostName` is a new `[DataMember]` on an existing `[DataContract]` and is a reference type with no `[DataMember(IsRequired = true)]` attribute, `DataContractSerializer` (the mechanism `ModuleDataModelBase`-derived classes are persisted with elsewhere in this codebase) deserializes it as `null` when reading a profile saved before this change — this is exactly the "existing saved profiles must continue to load correctly" requirement from the design spec, and requires no extra code to satisfy; it falls out of how `DataContractSerializer` already handles added optional members.

Acceptance for this milestone: add a small unit test, `src/Vixen.Tests/Controller/DDP/DDPDataTests.cs`, with a test `Clone_CopiesHostNameAlongsideAddress` that constructs a `DDPData`, sets both `Address` and `HostName`, calls `Clone()`, and asserts the clone's `Address` and `HostName` both equal the originals and that the clone is a distinct object reference from the original. Run `dotnet test src/Vixen.Tests/Vixen.Tests.csproj --filter "FullyQualifiedName~DDPData"` and confirm it passes. `msbuild Vixen.sln -m -t:restore -t:Build -p:Configuration=Debug` must still succeed.

### Milestone 5 — `DDPSetup` UI and code-behind

**Designer changes (`DDPSetup.Designer.cs`).** Inside `groupBox1`, alongside the existing `label1` ("IP Address:") and `textBoxIPAddress`, add:

- `radioButtonIPAddress` (`RadioButton`, `Text = "IP Address"`, `Checked = true` by default, positioned above the existing label/text box row, e.g. `Location = new Point(24, 20)`).
- `radioButtonHostName` (`RadioButton`, `Text = "Host Name"`, positioned to the right of `radioButtonIPAddress` on the same row, e.g. `Location = new Point(140, 20)`).
- `textBoxHostName` (`TextBox`, same `BorderStyle.FixedSingle` and same `Size`/`Location` as `textBoxIPAddress` — i.e. occupying the exact same screen position, `Point(138, 44)`, `Size(160, 27)` — so only one of the two text boxes is visible/frontmost at a time, mirroring E131's `UnicastForm.Designer.cs` layout technique exactly). Set `Enabled = false` initially since IP Address mode is the default. Give it placeholder text such as `"myhost.local"` (E131's equivalent `networkNameTextBox` uses `"myComputer"` as its placeholder).
- Move `label1`'s existing text/position if needed so the new radio-button row does not visually overlap it; increase `groupBox1.Size` height slightly (e.g. from `Size(385, 103)` to `Size(385, 130)`) and the form's overall size/`MinimumSize` to fit the extra row, keeping the OK/Cancel button `Anchor`/positions relative to the bottom of the (now taller) form — adjust their `Location.Y` down by the same amount the group box grew.
- Remove the line `this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;` (per the Decision Log entry above) and add `this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);` in its place inside `InitializeComponent()`.
- Wire `radioButtonIPAddress.CheckedChanged` and `radioButtonHostName.CheckedChanged` both to a new handler, `Radio_CheckedChanged` (naming chosen to match E131's identical `Radio_CheckedChanged` handler name for the same shape of event, for consistency across the two sibling controller modules).

Add the four new fields (`radioButtonIPAddress`, `radioButtonHostName`, `textBoxHostName`) to the designer's private-field declarations at the bottom of the partial class, alongside the existing `label1`/`textBoxIPAddress` fields.

**Code-behind changes (`DDPSetup.cs`).** Add `using System.Drawing;` (needed for `SystemIcons.Error`, not currently imported by this file) alongside the existing `using System.Net;` and `using System.Windows.Forms;`. Replace the constructor and `Address` property, and add a `HostName` property, a `buttonOK_Click` handler, and an `UpdateModeControls` helper:

    private IPAddress _resolvedAddress;
    private string _resolvedHostName;

    public DDPSetup(DDPData data)
    {
        InitializeComponent();
        ThemeUpdateControls.UpdateControls(this);

        if (!string.IsNullOrEmpty(data.HostName))
        {
            radioButtonHostName.Checked = true;
            textBoxHostName.Text = data.HostName;
        }
        else
        {
            radioButtonIPAddress.Checked = true;
            if (IPAddress.TryParse(data.Address, out var result))
            {
                textBoxIPAddress.Text = result.ToString();
            }
            else
            {
                textBoxIPAddress.Text = IPAddress.Loopback.ToString();
            }
        }
        UpdateModeControls();
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IPAddress Address => _resolvedAddress ?? IPAddress.Loopback;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string HostName => _resolvedHostName;

    private void UpdateModeControls()
    {
        if (radioButtonHostName.Checked)
        {
            textBoxHostName.Enabled = true;
            textBoxHostName.BringToFront();
            textBoxIPAddress.Enabled = false;
        }
        else
        {
            textBoxIPAddress.Enabled = true;
            textBoxIPAddress.BringToFront();
            textBoxHostName.Enabled = false;
        }
    }

    private void Radio_CheckedChanged(object sender, EventArgs e)
    {
        UpdateModeControls();
    }

    private void buttonOK_Click(object sender, EventArgs e)
    {
        if (radioButtonHostName.Checked)
        {
            string hostName = textBoxHostName.Text.Trim();
            if (DDPHostNameResolver.TryResolveToIPv4(hostName, out var resolved))
            {
                _resolvedAddress = resolved;
                _resolvedHostName = hostName;
            }
            else
            {
                MessageBoxForm mbf = new MessageBoxForm(
                    $"Could not resolve host name '{hostName}' to an IP address. Check the spelling and your network connection, or switch to IP Address mode.",
                    "DDP Setup Error", MessageBoxButtons.OK, SystemIcons.Error);
                mbf.ShowDialog(this);
                textBoxHostName.Focus();
                return;
            }
        }
        else
        {
            if (!IPAddress.TryParse(textBoxIPAddress.Text, out var ip))
            {
                ip = IPAddress.Loopback;
            }
            _resolvedAddress = ip;
            _resolvedHostName = null;
        }

        DialogResult = DialogResult.OK;
        Close();
    }

    private void groupBoxes_Paint(object sender, PaintEventArgs e)
    {
        ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
    }

(`groupBoxes_Paint` is the existing, unmodified handler already in this file — shown here only so the surrounding structure is clear; do not change it.)

Note `_resolvedHostName` being set back to `null` in the IP-address branch is exactly what satisfies the design spec's "when the user chooses IP Address mode and clicks OK, `HostName` must be cleared" requirement — `DDP.cs`'s `Setup()` (Milestone 6) will assign `_data.HostName = setup.HostName;` unconditionally, so a `null` here means the persisted `HostName` becomes `null` too.

Acceptance for this milestone: `msbuild Vixen.sln -m -t:restore -t:Build -p:Configuration=Debug` succeeds with no new errors or warnings. This milestone has no automated tests of its own (see the Decision Log's rationale for why `DDPSetup` itself is not unit-tested) — its behavior is covered by Milestone 8's manual verification.

### Milestone 6 — `DDP.cs` wiring, including runtime host re-validation

In `src/Vixen.Modules/Controller/DDP/DDP.cs`, update `Setup()`:

    public override bool Setup()
    {
        Logging.Trace(LogTag + "Setup()");
        _isRunning = false;
        DDPSetup setup = new DDPSetup(_data);
        if (setup.ShowDialog() == DialogResult.OK) {
            if (setup.Address != null)
                _data.Address = setup.Address.ToString();
            _data.HostName = setup.HostName;
            OpenConnection();
            return true;
        }
        _isRunning = true;
        return false;
    }

Update `OpenConnection()` to remove the now-superseded commented-out line (per the design spec's explicit requirement) and, before opening the socket, re-resolve `_data.HostName` when it is populated, auto-updating `_data.Address` if the resolved value has changed, and logging (not throwing) on failure:

    private bool OpenConnection()
    {
        Logging.Trace(LogTag + "DDP: OpenConnection()");

        // start off by closing the connection
        CloseConnection();

        if (_data.Address == null) {
            Logging.Warn(LogTag + "DDP: Trying to connect with a null IP address.");
            return false;
        }

        if (!string.IsNullOrEmpty(_data.HostName))
        {
            if (DDPHostNameResolver.TryResolveToIPv4(_data.HostName, out var resolvedAddress))
            {
                string resolvedText = resolvedAddress.ToString();
                if (resolvedText != _data.Address)
                {
                    Logging.Info(LogTag + $"DDP: Host '{_data.HostName}' now resolves to {resolvedText} (was {_data.Address}); updating address.");
                    _data.Address = resolvedText;
                }
            }
            else
            {
                Logging.Warn(LogTag + $"DDP: Could not resolve host name '{_data.HostName}'; continuing with last known address {_data.Address}.");
            }
        }

        try {
            _udpClient = new UdpClient(); //doesn't work when in class constructor
            _udpClient.Connect(_data.Address, DDP_PORT);
        }
        catch {
            Logging.Warn(LogTag + "DDP: Failed connect to host");
            return false;
        }

        return true;
    }

This satisfies the design spec's "Runtime Host Re-Validation" requirement: `OpenConnection()` is already called from `Start()`, `Resume()`, and the end of `Setup()` (unchanged call sites — no new call sites are introduced), so re-resolution happens at exactly those existing points, with no new timer, background thread, or polling loop. The `resolvedText != _data.Address` comparison is a plain ordinal string comparison — both sides are always `IPAddress.ToString()` output in the same dotted-quad form, so this is safe and avoids logging/writing on every single reconnect when nothing has actually changed. Because `_data` is the live `DDPData` instance also referenced by `ModuleData` (see `DDP.ModuleData` property), updating `_data.Address` here immediately and automatically reflects in whatever gets persisted the next time the profile is saved — no separate save-back step is needed. `HostInfo()`/`LogTag` (used in every log line above) already interpolate `_data.Address`, so log lines naturally show the fresh address after an update.

No other changes to `DDP.cs` are needed — `GetNetworkConfiguration()` already operates purely on `_data.Address` (unchanged), and this milestone deliberately does not add any re-resolution there: `GetNetworkConfiguration()` is a point-in-time query (used for FPP export), not a connection-establishment path, so it reads whatever `_data.Address` currently holds, which `OpenConnection()` keeps fresh on every reconnect.

Add a unit test in `src/Vixen.Tests/Controller/DDP/DDPHostNameResolverTests.cs` (created in Milestone 3) is sufficient for the pure resolution logic; `OpenConnection()`'s orchestration of that logic against `_data` is a `DDP`-internal method on a class with hardware/socket side effects (`UdpClient`) and is verified manually in Milestone 8, not unit tested — consistent with this plan's established boundary (see the Decision Log entry on why `DDPSetup` itself isn't unit tested either).

Acceptance: `msbuild Vixen.sln -m -t:restore -t:Build -p:Configuration=Debug` succeeds with no new errors or warnings.

### Milestone 7 — Project skills pass

Every new or modified public/internal member from Milestones 3-6 (`DDPHostNameResolver.TryResolveToIPv4`, `DDPData.HostName`, `DDPSetup.Address`, `DDPSetup.HostName`, and the modified `DDPData.Clone()`/`DDP.Setup()`) needs XML doc comments per `CLAUDE.md`'s "XML Docs" section — read `.agents/skills/csharp-docs/SKILL.md` first, since it has project-specific conventions beyond generic XML-doc guidance, and apply it to anything Milestones 3-6 left under-documented (the sketches above already include doc comments for the new public/internal members, but re-check them against the skill's exact conventions once it's loaded, e.g. `<remarks>` usage, wording style). Then run the `csharp-async` skill (confirm no async code was introduced or should have been — `Dns.GetHostAddresses` is a blocking call used synchronously by design, since this is a single one-shot, user-initiated lookup from a modal dialog's OK button, not a background or hot-path operation; the skill may still flag whether an async alternative like `Dns.GetHostAddressesAsync` with UI responsiveness (e.g. a wait cursor) would be better — if so, weigh that against introducing `async void` in a WinForms click handler, which is itself a pattern generally discouraged, and record the resulting decision in the Decision Log rather than silently picking one), `dotnet-best-practices`, and `dotnet-design-pattern-review` against the full diff, and address every finding (or record in the Decision Log why a finding was deliberately not addressed, following the precedent set in `docs/plans/vix-3933-element-tags-api.md`'s Milestone 6).

Acceptance: `dotnet test src/Vixen.Tests/Vixen.Tests.csproj` shows all tests (existing plus new) passing, `0` failed. `msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug` succeeds with no new warnings.

### Milestone 8 — Manual verification and JIRA closeout

Build the full solution (`msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug`) and launch `Vixen.Application` (the built executable lands under `Debug/Output/`, per `CLAUDE.md`'s "Build" section). Open or create a profile with a DDP controller (Display Setup, add a new controller of type "DDP"). Right-click it, choose "Properties", and verify:

1. The dialog opens with "IP Address" selected by default (for a new controller) and the IP text box showing `127.0.0.1`.
2. Click "Host Name", type `localhost`, click OK. The dialog closes without error.
3. Reopen Properties on the same controller. Verify "Host Name" is selected and the text box shows `localhost` (not `127.0.0.1` or any other resolved IP).
4. Click "Host Name" again, type `this-host-does-not-exist.invalid`, click OK. Verify a `MessageBoxForm` error dialog appears (title "DDP Setup Error", an error icon, an OK button) and that clicking its OK button returns focus to the Properties dialog, which remains open with focus back on the host name text box.
5. Click "IP Address", verify the IP text box shows some value (from the last successful resolution or the default), click OK. Reopen Properties and verify "IP Address" mode is now selected (proving `HostName` was cleared).
6. Save the profile, close and reopen Vixen, reopen the same profile, and verify the controller's Properties dialog still reflects whichever mode/value was last saved (proving persistence round-trips correctly through a full save/load cycle, not just in-memory during one session).
7. Runtime re-validation: with the controller configured in "Host Name" mode (e.g. pointing at a hostname you control, or a hostname added to your machine's `hosts` file, such as `C:\Windows\System32\drivers\etc\hosts`, resolving to some IP address `A`), start the show (or otherwise trigger `Start()`/`Resume()`), then check the NLog log output and confirm no "now resolves to" message appears (since nothing changed) and the controller connects normally. Then edit the `hosts` file entry to point the same hostname at a different IP address `B`, stop and start the show again (or resume it), and verify the log now shows a line like `Host '<name>' now resolves to B (was A); updating address.` and that `DDPData.Address` (visible by reopening Properties — it will be in "Host Name" mode still, but you can also confirm indirectly via the log line) reflects `B`. Finally, temporarily point the hostname at nothing resolvable (remove the `hosts` entry, assuming the name doesn't otherwise resolve on your network) and restart the show; verify a `Logging.Warn` line appears ("Could not resolve host name ... continuing with last known address ...") and the controller continues attempting to connect using the last good address rather than crashing or hanging. Revert your `hosts` file changes when done.

Update the JIRA issue [VIX-3605](https://vixenlights.atlassian.net/browse/VIX-3605) with a short comment summarizing what was implemented and the final `dotnet test` results. Per this project's established convention (see the memory this project's assistant maintains: JIRA issue status is not manually transitioned when finishing work — the PR/merge process handles transitions), do **not** call `mcp__atlassian__transitionJiraIssue` on this issue. Update this ExecPlan's `Outcomes & Retrospective` section with what shipped, any deviations from this plan, and any lessons learned.

## Concrete Steps

Run all commands from the repository root, `C:\Dev\Vixen`. The branch `VIX-3605` already exists and is already checked out — do not create a new branch.

1. Create/edit the files listed in Milestones 3-6 using an editor; there is no code-generation step.
2. After Milestone 3: `dotnet test src/Vixen.Tests/Vixen.Tests.csproj --filter "FullyQualifiedName~DDP"` — expect the new `DDPHostNameResolverTests` to pass, `0` failed.
3. After Milestone 4: `dotnet test src/Vixen.Tests/Vixen.Tests.csproj --filter "FullyQualifiedName~DDPData"` — expect the new `DDPDataTests` to pass, `0` failed.
4. After each milestone: `msbuild Vixen.sln -m -t:restore -t:Build -p:Configuration=Debug` — expect a clean build with no new warnings.
5. Before Milestone 8: `msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug`, then launch the built `Vixen.exe` under `Debug/Output/` and follow Milestone 8's manual steps.
6. Commit after each milestone with messages prefixed `VIX-3605`, e.g. `VIX-3605 Add DDPHostNameResolver and unit tests`.

## Validation and Acceptance

- `dotnet test src/Vixen.Tests/Vixen.Tests.csproj` reports all existing tests still passing plus the new `DDPHostNameResolverTests` and `DDPDataTests`, `0` failed.
- `msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug` succeeds with no new errors or warnings attributable to this change.
- In the running application: entering a resolvable host name and clicking OK persists the resolved IP into `DDPData.Address` and the host name into `DDPData.HostName`; reopening the Properties dialog shows "Host Name" mode with the original text; entering an unresolvable host name shows an error message box and keeps the dialog open; switching back to "IP Address" mode and clicking OK clears `DDPData.HostName` to `null`.
- A profile saved before this change (with no `HostName` value in its XML) continues to load without error, and its DDP controller's Properties dialog opens in "IP Address" mode as before.
- For a controller configured in "Host Name" mode: when the host's DNS/`hosts`-file resolution changes between two connection attempts (e.g. `Start()` then a later `Resume()`), `OpenConnection()` automatically updates `DDPData.Address` to the newly resolved value and logs an informational message, with no dialog and no user action; when resolution fails, `OpenConnection()` logs a warning and continues using the previous address rather than failing outright. Controllers configured with a literal IP address perform no resolution attempt at all.

## Idempotence and Recovery

Every code change in this plan is a straightforward additive edit to existing files; none of it depends on any prior partial state on disk. If a milestone's build fails, fix the compile error and rebuild — there is no migration or one-time data transformation step anywhere in this plan. `DDPHostNameResolver.TryResolveToIPv4` performs no caching and no mutation of shared state, so it is safe to call repeatedly (e.g. if a user clicks OK, gets a resolution failure, and clicks OK again after fixing a typo). If manual verification in Milestone 8 reveals unexpected behavior, do not hand-edit a real saved profile's XML to investigate — copy it to a scratch location first, exactly as `docs/plans/vix-3933-element-tags-api.md`'s own Idempotence and Recovery section recommends for the same class of problem.

## Artifacts and Notes

Design source of truth for the product/behavior decisions this plan implements: `docs/controllers/vix-3605-ddp-hostname-support.md` in this repository.

JIRA issue for this plan: [VIX-3605](https://vixenlights.atlassian.net/browse/VIX-3605) ("DDP Controller should allow for host name instead of just IP address"), already updated with the finalized requirements and acceptance criteria before this plan was written.

Reserved test host names used in Milestone 3's unit tests, for reference: `localhost` (always resolves locally, no network required) and any name under the `.invalid` top-level domain (permanently reserved by RFC 2606 to never resolve).

## Interfaces and Dependencies

In `VixenModules.Output.DDP` (`src/Vixen.Modules/Controller/DDP/`), define:

    internal static class DDPHostNameResolver
    {
        internal static bool TryResolveToIPv4(string hostName, out IPAddress resolvedAddress);
    }

Extend the existing `DDPData`:

    [DataContract]
    public class DDPData : ModuleDataModelBase
    {
        [DataMember]
        public string Address { get; set; }   // unchanged in meaning/type

        [DataMember]
        public string HostName { get; set; }  // new
    }

Extend the existing `DDPSetup` (constructor signature unchanged, `DDPSetup(DDPData data)`):

    public IPAddress Address { get; }  // was already public; now a computed/cached property instead of a live textbox re-parse
    public string HostName { get; }    // new

`DDP.csproj` gains an `InternalsVisibleTo` grant for `Vixen.Tests`, following the exact pattern already present in `src/Vixen.Modules/App/FPPClient/FPPClient.csproj`. `Vixen.Tests.csproj` gains a `ProjectReference` to `DDP.csproj`. Depends on nothing new externally — no new NuGet packages; `System.Net.Dns` and `System.Net.Sockets.SocketException` are both already part of the .NET base class library already referenced throughout this codebase (e.g. `E131OutputPlugin.cs` already uses `System.Net.Dns`).

`DDP` (`internal class DDP : ControllerModuleInstanceBase`, unchanged public surface) gains no new public members from the runtime re-validation behavior — `OpenConnection()` (already `private`) is extended in place to call `DDPHostNameResolver.TryResolveToIPv4` and mutate `_data.Address`/log, as shown in Milestone 6. No new call sites are added; `Start()`, `Resume()`, and `Setup()` all continue to call `OpenConnection()` exactly where they do today.
