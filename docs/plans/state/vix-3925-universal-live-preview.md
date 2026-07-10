# VIX-3925: Universal Live Preview — Vixen.Messages and LivePreview Module

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`,
`Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.
Maintain this document in accordance with `.agents/PLANS.md`.

## Purpose / Big Picture

Today the WebServer module is the only part of Vixen that can turn physical lighting elements
on and off in real time (outside of running a full sequence). It does this through its own private
`LiveContext` — an execution context that accepts ad-hoc effect commands and immediately sends them
to controllers. No other part of Vixen can use this capability without depending on the WebServer,
which is wrong because the WebServer is an optional, user-enabled module.

After this change, any Vixen module — a future fixture tester, the State Property (when it is
created), a script — can activate or deactivate elements in real time by publishing a single
broadcast message, with no dependency on the WebServer. The new `LivePreview` App module owns the
live execution engine, manages named contexts so multiple consumers can operate independently, and
subscribes to broadcast messages. A new `Messages` project inside `Vixen.Common` (alongside the
existing `Broadcast` project) provides the message types and channel names so any module can
participate with one small reference.

Note: State Property does not yet exist. The broadcast infrastructure created here is designed so
that the State Property (a future ticket) only needs to reference `Vixen.Messages` and publish
messages — no changes to this feature will be required at that time.

**What you can see working after each milestone:**

- M1: `Vixen.Messages` builds cleanly as a standalone project.
- M2: Launch Vixen; attach a debugger or check the log — the `"LivePreview"` context appears in
  `VixenSystem.Contexts` without the user enabling anything.
- M3: The WebServer REST API (`PUT /api/element/{id}/on`) still turns on elements; the
  `Module.LiveContext` static property no longer exists in WebServer.

---

## Progress

- [x] (2026-06-01) M1: Create `Common.Messages` project (`src/Vixen.Common/Messages/`)
- [x] (2026-06-01) M2: Create `Vixen.Modules.App.LivePreview` module
- [x] (2026-06-01) M3: Refactor WebServer to use ILivePreviewService  - Out of Scope
- [x] (2026-06-01) M4: Write unit tests in Vixen.Tests

---

## Surprises & Discoveries

- Observation: `ClearActiveEffectsMessage.cs` was saved with the filename
  `ClearActiveElementsMessage.cs` (Elements instead of Effects). The class inside was correctly
  named. Caught during M1 validation; file renamed before marking complete.
  Evidence: `Glob` output listed `ClearActiveElementsMessage.cs`; class body showed
  `ClearActiveEffectsMessage`.

- Observation: The solution file `Any CPU` platform entries generated for the Messages project
  targeted `x86` instead of `x64`. Corrected per CLAUDE.md before marking M1 complete.

- Observation: `LivePreviewData.cs` was scaffolded with the class named `Data` (not matching the
  filename), while `Descriptor.cs` referenced `typeof(LivePreviewData)`. The class was renamed to
  `LivePreviewData` to resolve the mismatch.

- Observation: `LivePreviewService.cs` was scaffolded as `public interface LivePreviewService`
  instead of a class. Replaced with the full implementation.

- Observation: `SetLevel` requires two project references to build: `Effect.csproj` (provides
  `BaseEffect`, the base class) and `SetLevel.csproj`. Without `Effect.csproj`, the compiler
  cannot resolve `SetLevel`'s inheritance chain and the `EffectNode` constructor overloads.
  Both references were added to `LivePreview.csproj`.

- Observation: The initial M3 implementation made WebServer reference LivePreview directly and
  call ILivePreviewService. This was corrected to use Broadcast.Publish instead, removing the
  LivePreview project reference from WebServer entirely. WebServer now only references Messages
  and Broadcast — no direct dependency on the LivePreview module.

- Observation: The experimental `Effect(ElementEffect)` method in `ElementsHelper` called
  `Module.LiveContext` directly, which was removed in M3. Since the method is out of scope for
  broadcast/service promotion and has no known callers outside the WebServer REST API, it was
  removed along with its `ElementController` action rather than routing it to an ad-hoc context.


- Observation: `LiveContext` methods (TerminateNode, TerminateNodes, Execute, Clear) are non-virtual,
  so Moq cannot mock `LiveContext` directly. Resolved by introducing `ILiveContext` and
  `LiveContextAdapter` in the LivePreview project. Tests mock `ILiveContext`; production code
  uses `LiveContextAdapter` to delegate to the real `LiveContext`.

- Observation: Moq requires `[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]` in
  addition to `InternalsVisibleTo("Vixen.Tests")` to proxy internal interfaces at runtime.
  Both were added to `LivePreview.csproj`.
- Observation: All App modules are auto-loaded at startup via `Modules.PopulateRepositories()`
  → `AppModuleRepository.Add()` → `instance.Loading()`. No special "always-on" mechanism is
  needed; placing the DLL in the module directory is sufficient.

---

## Decision Log

- Decision: Use a single `Vixen.Messages` project for all broadcast message types rather than
  per-module message assemblies.
  Rationale: Prevents fragmentation; any module needs at most one reference to participate in any
  broadcast. The project is kept logic-free (DTOs + channel constants only) to avoid it becoming
  a god assembly.
  Date/Author: 2026-06-01 / Jeff Uchitjil

- Decision: LivePreview manages a registry of named `LiveContext` instances rather than a single
  shared one.
  Rationale: The WebServer creates its own `"Web Server"` context so that `ClearActiveEffects`
  from the WebServer does not clear effects triggered by other consumers. This also future-proofs
  the design for modules like State Property (not yet created) without requiring changes here.
  Date/Author: 2026-06-01 / Jeff Uchitjil

- Decision: `ContextName = null` on all message types means "use the default context."
  Rationale: Keeps the common case (consumers that don't care about isolation) zero-friction while
  enabling opt-in isolation for consumers that need it.
  Date/Author: 2026-06-01 / Jeff Uchitjil

- Decision: LivePreview auto-starts (Loading() sets up the default context) rather than requiring
  user enablement.
  Rationale: It is infrastructure, not a user-facing feature. Making it opt-in would silently break
  any module that publishes a broadcast message when the user hasn't enabled it.
  Date/Author: 2026-06-01 / Jeff Uchitjil

- Decision: Messages project lives at `src/Vixen.Common/Messages/` (project name `Messages`,
  root namespace `Common.Messages`) rather than as a standalone `src/Vixen.Messages/` project.
  Rationale: Mirrors the existing `Vixen.Common/Broadcast/` layout and keeps all shared
  infrastructure in one place. Consumers add one reference to the same area of the solution they
  already know. The `Common.Messages` namespace matches the `Common.Broadcast` convention.
  Date/Author: 2026-06-01 / Jeff Uchitjil

- Decision: Message types and ElementState use `record` rather than `class`.
  Rationale: dotnet-best-practices skill requires records for all DTOs and messages to enforce
  immutability and enable value-equality. This also allows callers to use the `with` expression
  to produce modified copies without mutating the original.
  Date/Author: 2026-06-01 / Jeff Uchitjil

- Decision: Introduce `IContextFactory` to abstract `VixenSystem.Contexts` access in
  `LivePreviewService`.
  Rationale: `VixenSystem.Contexts` is a static property returning a concrete `ContextManager`
  with no interface. Without this abstraction the service cannot be unit tested in isolation.
  The abstraction is `internal` — it is not part of the public API.
  Date/Author: 2026-06-01 / Jeff Uchitjil

- Decision: WebServer uses Broadcast.Publish rather than a direct ILivePreviewService reference.
  Rationale: The original plan said WebServer could reference LivePreview directly; during
  implementation this was revised because broadcast is the intended decoupled path for all
  consumers. WebServer now has no dependency on the LivePreview module assembly.
  Date/Author: 2026-06-01 / Jeff Uchitjil

- Decision: `ILivePreviewService` singleton exposed via `Module.Instance` static property.
  Rationale: Consistent with the WebServer's `Module.LiveContext` pattern already established in
  the codebase. ServiceLocator would be more decoupled but adds ceremony not justified by the
  current consumer count. Can be migrated later if needed.
  Date/Author: 2026-06-01 / Jeff Uchitjil

- Decision: Auto-start is inherent — no extra mechanism needed.
  Rationale: `Modules.PopulateRepositories()` calls `Loading()` on every discovered App module
  at startup. The LivePreview module starts automatically as long as its DLL is in the module
  output directory.
  Date/Author: 2026-06-01 / Jeff Uchitjil

- Decision: `LivePreview.csproj` references both `Effect.csproj` and `SetLevel.csproj`.
  Rationale: `SetLevel` inherits from `BaseEffect` in the `Effect` project. Without the base
  project reference the compiler cannot resolve the type hierarchy needed for `EffectNode`.
  Date/Author: 2026-06-01 / Jeff Uchitjil

- Decision: `TurnOnElement` / `TurnOnElements` are not covered by unit tests.
  Rationale: Constructing a valid `EffectNode` requires `VixenSystem.Nodes` and effect module
  infrastructure that cannot be initialised without a running Vixen environment. These paths are
  verified by the manual integration test (Validation step 3). If a test harness for VixenSystem
  is introduced in a future ticket, these tests can be added at that time.
  Date/Author: 2026-06-01 / Jeff Uchitjil

- Decision: Web server refactor is out of scope.
  Rationale: Since it requries more detailed responses the basic publish does not provide the details required. Will 
  consider it in future work.
  Date/Author: 2026-06-01 / Jeff Uchitjil

---

## Outcomes & Retrospective

*(Fill in at completion.)*

---

## Context and Orientation

### Key terms

**LiveContext** — an execution context (defined in `src/Vixen.Core/Execution/Context/LiveContext.cs`)
that accepts ad-hoc effect commands and executes them immediately on connected controllers. Unlike
a sequence context, it has no timeline; you push an `EffectNode` and it starts executing right
away. The `Clear()` method stops all executing effects.

**App module** — a Vixen plugin that runs at the application level rather than per-element. App
modules live under `src/Vixen.Modules/App/`. Each one has a `Descriptor` (static metadata), a
`Module` (runtime instance that inherits `AppModuleInstanceBase`), and optionally a `Data`
(serializable configuration). The runtime calls `Module.Loading()` on startup and
`Module.Unloading()` on shutdown.

**Broadcast** — the pub/sub helper in `src/Vixen.Common/Broadcast/Broadcast.cs`. It wraps the
MVVM Community Toolkit `WeakReferenceMessenger`. Messages are type-safe generics routed by a
channel name string. Publishers call `Broadcast.Publish<T>(channel, message)` and subscribers
call `Broadcast.Subscribe<T>(this, channel, callback)`.

**ContextManager** — `src/Vixen.Core/Sys/Managers/ContextManager.cs`, accessible via
`VixenSystem.Contexts`. Provides `CreateLiveContext(name)` and `ReleaseContext(context)`.

### Current state

The WebServer module (`src/Vixen.Modules/App/WebServer/`) owns:
- `Module.LiveContext` — a static `LiveContext?` property created in `_SetServerEnableState(true)`
  and released in `_SetServerEnableState(false)` / `Unloading()`.
- `Service/ElementsHelper.cs` — internal class with `TurnOnElement`, `TurnOnElements`,
  `TurnOffElement`, `TurnOffElements`, `ClearActiveEffects`, plus query helpers
  (`SearchElements`, `GetElements`, `GetChildElements`, `GetParentElements`).
- `Model/ElementState.cs` — DTO: `Id (Guid)`, `Duration (int)`, `Color (string)`,
  `Intensity (double)`.

No other module currently has access to live element control.

### Project layout after this change

    src/
      Vixen.Common/
        Broadcast/                             ← unchanged
        Messages/                             ← NEW: DTOs + channel constants (Messages.csproj)
      Vixen.Core/                              ← unchanged
      Vixen.Modules/
        App/
          LivePreview/                         ← NEW: LivePreview App module
          WebServer/                           ← MODIFIED: delegates to ILivePreviewService

---

## Plan of Work

### Milestone 1 — Create `Vixen.Common/Messages`

Create a new C# class library project at `src/Vixen.Common/Messages/Messages.csproj`. This
follows the same layout as `src/Vixen.Common/Broadcast/Broadcast.csproj`. Target
`net10.0-windows`. Set `<RootNamespace>Common.Messages</RootNamespace>` in the project file to
match the `Common.Broadcast` convention. Do not reference any other Vixen project. No external
references are needed.

**Skill: dotnet-best-practices** — per this skill, all DTOs and messages must be `record` types
(not classes) to enforce immutability and enable value-equality. Apply `sealed` to every record
and message type since none are intended for inheritance. Each type gets its own file, with the
filename matching the type name exactly.

**Skill: csharp-docs** — every public type and property in this project must have XML doc
comments before M1 is considered complete. Apply the skill when writing the files.

Create the following files under `src/Vixen.Common/Messages/`:

**`LivePreview/ElementState.cs`** — moved and namespace-updated from WebServer. Use a record so
that callers can clone and mutate via the `with` keyword.

    namespace Common.Messages.LivePreview;

    /// <summary>Represents the desired state for a single lighting element in a live preview operation.</summary>
    public sealed record ElementState
    {
        /// <summary>Gets or sets the unique identifier of the element node.</summary>
        public Guid Id { get; init; }

        /// <summary>Gets or sets the duration in seconds the element should remain on.</summary>
        public int Duration { get; init; }

        /// <summary>Gets or sets the HTML color string (e.g. <c>"#FF0000"</c>) for the element.</summary>
        public string Color { get; init; } = string.Empty;

        /// <summary>Gets or sets the intensity level from 0.0 (off) to 1.0 (full).</summary>
        public double Intensity { get; init; }
    }

**`LivePreview/TurnOnElementMessage.cs`**

    namespace Common.Messages.LivePreview;

    /// <summary>Broadcast message that requests a single element be turned on in a live preview context.</summary>
    public sealed record TurnOnElementMessage
    {
        /// <summary>Gets the desired state for the element.</summary>
        public ElementState State { get; init; } = new();

        /// <summary>Gets the name of the target live context, or <see langword="null"/> to use the default context.</summary>
        public string? ContextName { get; init; }
    }

**`LivePreview/TurnOnElementsMessage.cs`**

    namespace Common.Messages.LivePreview;

    /// <summary>Broadcast message that requests multiple elements be turned on in a live preview context.</summary>
    public sealed record TurnOnElementsMessage
    {
        /// <summary>Gets the desired states for each element.</summary>
        public IReadOnlyList<ElementState> States { get; init; } = [];

        /// <summary>Gets the name of the target live context, or <see langword="null"/> to use the default context.</summary>
        public string? ContextName { get; init; }
    }

**`LivePreview/TurnOffElementMessage.cs`**

    namespace Common.Messages.LivePreview;

    /// <summary>Broadcast message that requests a single element be turned off in a live preview context.</summary>
    public sealed record TurnOffElementMessage
    {
        /// <summary>Gets the unique identifier of the element node to turn off.</summary>
        public Guid ElementId { get; init; }

        /// <summary>Gets the name of the target live context, or <see langword="null"/> to use the default context.</summary>
        public string? ContextName { get; init; }
    }

**`LivePreview/TurnOffElementsMessage.cs`**

    namespace Common.Messages.LivePreview;

    /// <summary>Broadcast message that requests multiple elements be turned off in a live preview context.</summary>
    public sealed record TurnOffElementsMessage
    {
        /// <summary>Gets the desired states identifying the elements to turn off.</summary>
        public IReadOnlyList<ElementState> States { get; init; } = [];

        /// <summary>Gets the name of the target live context, or <see langword="null"/> to use the default context.</summary>
        public string? ContextName { get; init; }
    }

**`LivePreview/ClearActiveEffectsMessage.cs`**

    namespace Common.Messages.LivePreview;

    /// <summary>Broadcast message that requests all active effects be cleared from a live preview context.</summary>
    public sealed record ClearActiveEffectsMessage
    {
        /// <summary>Gets the name of the target live context, or <see langword="null"/> to use the default context.</summary>
        public string? ContextName { get; init; }
    }

**`LivePreview/LivePreviewChannels.cs`** — string constants for each channel. Consumers use these
to avoid magic strings.

    namespace Common.Messages.LivePreview;

    /// <summary>Defines the broadcast channel names used by the Live Preview messaging system.</summary>
    public static class LivePreviewChannels
    {
        /// <summary>Channel for <see cref="TurnOnElementMessage"/> messages.</summary>
        public const string TurnOnElement = "LivePreview.TurnOnElement";

        /// <summary>Channel for <see cref="TurnOnElementsMessage"/> messages.</summary>
        public const string TurnOnElements = "LivePreview.TurnOnElements";

        /// <summary>Channel for <see cref="TurnOffElementMessage"/> messages.</summary>
        public const string TurnOffElement = "LivePreview.TurnOffElement";

        /// <summary>Channel for <see cref="TurnOffElementsMessage"/> messages.</summary>
        public const string TurnOffElements = "LivePreview.TurnOffElements";

        /// <summary>Channel for <see cref="ClearActiveEffectsMessage"/> messages.</summary>
        public const string ClearActiveEffects = "LivePreview.ClearActiveEffects";
    }

Note: `IEnumerable<ElementState>` is replaced with `IReadOnlyList<ElementState>` on the batch
message types. This makes the contract explicit (callers provide a fully-materialised list, not
a lazy sequence) and avoids multiple-enumeration bugs.

Add the project to `Vixen.sln`. After running `dotnet sln add`, fix the solution file manually:
1. Correct all `Any CPU` platform entries to `x64` (see CLAUDE.md for the exact format).
2. Remove any spurious solution folder entries created by `dotnet sln add`.
3. Move the project's `NestedProjects` entry to point at the pre-existing `Vixen.Common` solution
   folder GUID (not the `src` folder) — the same folder that contains `Broadcast`.

Verify this milestone by running:

    msbuild src/Vixen.Common/Messages/Messages.csproj -t:Rebuild -p:Configuration=Release -p:Platform=x64

Expected: Build succeeded with 0 errors.

---

### Milestone 2 — Create `Vixen.Modules.App.LivePreview`

Create a new C# class library at `src/Vixen.Modules/App/LivePreview/LivePreview.csproj`.
Target `net10.0-windows`. Reference `Vixen.Core` and `Messages` (from
`src/Vixen.Common/Messages/Messages.csproj`) as project references (Copy Local = No, Include
Assets = None per CLAUDE.md conventions). Also reference the `Broadcast` project from
`src/Vixen.Common/Broadcast/`.

Use the FPPClient module (`src/Vixen.Modules/App/FPPClient/`) as a structural template.

**Skill: dotnet-best-practices** — use primary constructor syntax for any constructor-injected
dependencies in `LivePreviewService`. Use `sealed` on the service class. Apply structured NLog
logging (no string interpolation). Use `ArgumentNullException.ThrowIfNull` for guard checks.

**Skill: csharp-docs** — all public and protected members in `ILivePreviewService`,
`LivePreviewService`, `Module`, and `Descriptor` require XML doc comments before M2 is complete.

**Skill: dotnet-design-pattern-review** — after writing all M2 code, invoke this skill to review
the design. Write the review output to `docs/reviews/vix-3925-m2-design-review.md`. Address any
critical findings before proceeding to M3. Record the decision about any accepted trade-offs in
the Decision Log.

**Testability requirement:** `LivePreviewService` must not call `VixenSystem.Contexts` (a static
property) directly. `VixenSystem.Contexts` returns a `ContextManager` which is a concrete class
with no interface — this makes the service untestable in isolation. Instead, introduce a thin
`IContextFactory` interface:

    /// <summary>Abstracts live context creation so the service can be tested without VixenSystem.</summary>
    internal interface IContextFactory
    {
        /// <summary>Creates and registers a new live context with the given name.</summary>
        LiveContext Create(string name);

        /// <summary>Releases a previously created context.</summary>
        void Release(LiveContext context);
    }

Provide a real implementation `VixenContextFactory` that delegates to `VixenSystem.Contexts`.
`LivePreviewService` takes `IContextFactory` via its primary constructor. In tests, a mock or
stub implementation is supplied instead. This is the only abstraction needed for testability —
do not introduce additional indirection.

**`Descriptor.cs`**

    namespace VixenModules.App.LivePreview;

    public class Descriptor : AppModuleDescriptorBase
    {
        private static readonly Guid _typeId = new("{/* generate a new GUID */}");
        public override string TypeName => "Live Preview";
        public override Guid TypeId => _typeId;
        public override string Author => "Vixen Team";
        public override string Description => "Shared live element control service for broadcast-driven previews";
        public override string Version => "1.0";
        public override Type ModuleClass => typeof(Module);
        public override Type ModuleStaticDataClass => typeof(Data);
    }

**`Data.cs`** — no configuration needed at this stage; keep it minimal.

    [DataContract]
    public class Data : ModuleDataModelBase
    {
        public override IModuleDataModel Clone() => (IModuleDataModel)MemberwiseClone();
    }

**`ILivePreviewService.cs`** — the public interface other modules (like WebServer) will depend on.
Place it in the `VixenModules.App.LivePreview` namespace.

    public interface ILivePreviewService
    {
        void TurnOnElement(Guid id, int seconds, double intensity, string color, string? contextName = null);
        void TurnOnElements(IEnumerable<ElementState> states, string? contextName = null);
        void TurnOffElement(Guid id, string? contextName = null);
        void TurnOffElements(IEnumerable<ElementState> states, string? contextName = null);
        void ClearActiveEffects(string? contextName = null);
    }

**`LivePreviewService.cs`** — concrete implementation. This class owns the context registry and
translates service calls into LiveContext operations.

The registry is a `ConcurrentDictionary<string, LiveContext>` keyed by context name. The default
context name is the string `"LivePreview"`.

    GetOrCreateContext(string? contextName):
        key = contextName ?? "LivePreview"
        if not in registry:
            context = VixenSystem.Contexts.CreateLiveContext(key)
            context.Start()
            add to registry
        return registry[key]

For `TurnOnElement`: build an `EffectNode` wrapping a `SetLevel` effect with the given intensity
and color, with a `TimeSpan` of `seconds` duration. Call `context.Execute(effectNode)`.

For `TurnOffElement`: call `context.TerminateNode(id)`.

For `TurnOffElements`: call `context.TerminateNodes(states.Select(s => s.Id))`.

For `ClearActiveEffects`: call `context.Clear()`.

Study how the existing `ElementsHelper.TurnOnElement` constructs its `EffectNode` and `SetLevel`
effect — copy that logic exactly, do not invent a new approach. The key steps are:
1. Look up the `ElementNode` by id via `VixenSystem.Nodes.GetElementNode(id)`.
2. Create a `SetLevel` effect instance; set `IntensityLevel` and `Color`.
3. Wrap it in an `EffectNode` with the element's patched outputs and the requested duration.
4. Call `context.Execute(effectNode)`.

**`Module.cs`** — inherits `AppModuleInstanceBase`. In `Loading()`, create and start the default
context; subscribe to all five broadcast channels. In `Unloading()`, unsubscribe from all
channels and release all contexts in the registry.

    public class Module : AppModuleInstanceBase
    {
        private LivePreviewService? _service;

        public override IApplication Application { set { } }
        public override IModuleDataModel StaticModuleData { get => _data; set => _data = (Data)value; }
        private Data _data = new();

        public override void Loading()
        {
            _service = new LivePreviewService();
            _service.Initialize();          // creates the default "LivePreview" context
            _SubscribeToBroadcasts();
        }

        public override void Unloading()
        {
            _UnsubscribeFromBroadcasts();
            _service?.Dispose();
            _service = null;
        }
    }

Make `LivePreviewService` accessible as a singleton so WebServer (and other modules) can resolve
it. The simplest approach is a static `Instance` property on `Module` — follow the same pattern
used by other App modules (e.g., WebServer's `Module.LiveContext`). A more decoupled option is
to register it with `ServiceLocator` (Catel's service locator) in `Loading()`. Choose whichever
approach is consistent with how other App modules expose services; investigate by reading
`src/Vixen.Modules/App/WebServer/Module.cs` and one other App module before deciding. Record
the decision in the Decision Log.

Subscribe to broadcasts in `Loading()`:

    Broadcast.Subscribe<TurnOnElementMessage>(this, LivePreviewChannels.TurnOnElement,
        m => _service!.TurnOnElement(m.State.Id, m.State.Duration, m.State.Intensity, m.State.Color, m.ContextName));
    // ... similarly for all five channel types

Add the project to `Vixen.sln` and fix `Any CPU` → `x64` and spurious folder entries as in M1.

**Verify auto-start behavior:** Investigate whether `Loading()` is called automatically for all
registered App modules or whether the user must explicitly enable the module. Look at how
`VixenSystem` initializes App modules (search for `AppModuleDescriptorBase` consumers in
`src/Vixen.Core/`). If App modules require user opt-in, find the mechanism to mark a module as
always-on (check `ModuleImplementation` attributes or similar). Record the finding in the
Decision Log and implement accordingly.

Verify this milestone by building and launching Vixen. After startup, break in a debugger or add
a log statement in `Loading()`. Confirm:

    VixenSystem.Contexts.ToList()  // should contain a context named "LivePreview"

---

### Milestone 3 — Refactor WebServer to use ILivePreviewService

**Skill: dotnet-best-practices** — apply when writing the refactored `ElementsHelper`. Confirm
that the forwarding methods use expression-body syntax where appropriate and that no multi-class
files are introduced.

**Skill: csharp-docs** — update any XML doc comments on `ElementsHelper` public members that
change behaviour as a result of the refactor.

Add a project reference from `Vixen.Modules.App.WebServer` to `Vixen.Modules.App.LivePreview`
(or to an interface assembly if service resolution was done via ServiceLocator — see M2 Decision
Log).

In `src/Vixen.Modules/App/WebServer/Service/ElementsHelper.cs`:

1. Remove the five live-control methods (`TurnOnElement`, `TurnOnElements`, `TurnOffElement`,
   `TurnOffElements`, `ClearActiveEffects`).
2. Add a private field resolving `ILivePreviewService` — either via `Module.LivePreviewService`
   (static property) or via the service locator, whichever M2 established.
3. Add five thin forwarding methods that call the service with `contextName: "Web Server"`:

        public void TurnOnElement(Guid id, int seconds, double intensity, string color)
            => _livePreviewService.TurnOnElement(id, seconds, intensity, color, "Web Server");

4. Update all `using` statements that reference `WebServer.Model.ElementState` to use
   `Vixen.Messages.LivePreview.ElementState` instead. Check all files in the WebServer project
   for this type reference: `Module.cs`, `ElementsHelper.cs`, `Controllers/ElementController.cs`,
   and any model or DTO files.

In `src/Vixen.Modules/App/WebServer/Module.cs`:

1. Remove the `internal static LiveContext? LiveContext` property.
2. Remove the `VixenSystem.Contexts.CreateLiveContext("Web Server")` call and the corresponding
   `VixenSystem.Contexts.ReleaseContext(LiveContext)` call. The `"Web Server"` context is now
   created on first use by the LivePreview context registry.
3. Remove the `src/Vixen.Modules/App/WebServer/Model/ElementState.cs` file (it is now in
   `Vixen.Messages`).

Verify by building the full solution and running Vixen. Start the WebServer module. Make a REST
call:

    PUT http://localhost:8080/api/element/{valid-element-id}/on
    Body: { "duration": 5, "intensity": 1.0, "color": "#FFFFFF" }

Expected: the element lights up on connected controllers. Confirm in VixenSystem.Contexts that a
context named `"Web Server"` now exists (created by LivePreview on first use).

---

### Milestone 4 — Unit Tests in Vixen.Tests

The test project at `src/Vixen.Tests/Vixen.Tests.csproj` already exists and uses xunit.v3 + Moq.
Add project references to `Messages` (`src/Vixen.Common/Messages/Messages.csproj`) and
`Vixen.Modules.App.LivePreview` in its `.csproj`.
Do not add a reference to `Vixen.Modules.App.WebServer` — the WebServer is not under test here.

Create test files under `src/Vixen.Tests/LivePreview/`. One file per test class, filename
matching the class name. Follow the AAA (Arrange / Act / Assert) pattern throughout.

**Skill: dotnet-best-practices** — test method names must follow the form
`MethodName_Scenario_ExpectedBehavior`. Use `Moq` for all dependencies. No real `VixenSystem`
is available in unit tests; rely entirely on the `IContextFactory` mock introduced in M2.

**`LivePreviewChannelsTests.cs`** — verify the channel constants have the expected string values.
These act as a contract test: if someone renames a constant, the test breaks and forces a
deliberate decision.

    ChannelConstants_HaveExpectedValues()
        // Asserts each LivePreviewChannels constant equals its documented string literal.

**`ElementStateTests.cs`** — verify record behaviour of `ElementState`.

    ElementState_WithExpression_ProducesModifiedCopy()
        // Creates an ElementState, uses `with` to change one property, asserts the original is
        // unchanged and the copy has the new value.

**`LivePreviewServiceTests.cs`** — the main unit test class. Mock `IContextFactory` to return
a mock `LiveContext` (or a stub). Because `LiveContext` is a concrete class, check whether it can
be subclassed or mocked by Moq; if not, introduce a thin `ILiveContext` interface in the
LivePreview project and adapt accordingly. Record the decision in the Decision Log.

Tests to write:

    GetOrCreateContext_ReturnsDefaultContext_WhenContextNameIsNull()
        // Arrange: mock IContextFactory; Act: call GetOrCreateContext(null) twice;
        // Assert: factory.Create called once with "LivePreview", same instance returned both times.

    GetOrCreateContext_ReturnsNamedContext_WhenContextNameProvided()
        // Arrange: Act: call GetOrCreateContext("Web Server");
        // Assert: factory.Create called with "Web Server".

    GetOrCreateContext_ReturnsDifferentContexts_ForDifferentNames()
        // Arrange: Act: call GetOrCreateContext("A") and GetOrCreateContext("B");
        // Assert: two distinct Create calls, two distinct instances.

    ClearActiveEffects_ClearsOnlyTargetContext_WhenContextNameProvided()
        // Arrange: two mock contexts "A" and "B" pre-registered in the service;
        // Act: ClearActiveEffects("A");
        // Assert: context "A".Clear() called; context "B".Clear() NOT called.

    ClearActiveEffects_ClearsDefaultContext_WhenContextNameIsNull()
        // Arrange: default context pre-registered;
        // Act: ClearActiveEffects(null);
        // Assert: default context's Clear() called.

    TurnOffElement_CallsTerminateNode_WithCorrectId()
        // Arrange: mock context; Act: TurnOffElement(knownId, null);
        // Assert: context.TerminateNode(knownId) called.

    TurnOffElements_CallsTerminateNodes_WithCorrectIds()
        // Arrange: two ElementState records; Act: TurnOffElements(states, null);
        // Assert: context.TerminateNodes called with both ids.

Note on `TurnOnElement` tests: constructing an `EffectNode` requires `VixenSystem.Nodes` and
effect module infrastructure that cannot be unit tested without a running Vixen environment.
Do not attempt to unit test `TurnOnElement` or `TurnOnElements` at this stage — those paths are
covered by the manual integration test in Validation step 3.

Run all tests with:

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj

Expected: all new tests pass, zero failures.

---

## Concrete Steps

### M1 — Vixen.Common/Messages project

    # From repo root (C:\Dev\Vixen):
    dotnet new classlib -n Messages -o src/Vixen.Common/Messages --framework net10.0
    dotnet sln Vixen.sln add src/Vixen.Common/Messages/Messages.csproj -s Vixen.Common

Then manually edit `Messages.csproj` to add `<RootNamespace>Common.Messages</RootNamespace>`.

Then manually edit `Vixen.sln` to:
- Fix all `Any CPU` platform entries to `x64` per CLAUDE.md.
- Remove any spurious solution folder entries created by `dotnet sln add`.
- Set the `NestedProjects` entry to point at the `Vixen.Common` solution folder GUID
  (same folder that contains `Broadcast`), not a newly generated one.

Create all seven files described in M1 above.

Build check:

    msbuild src/Vixen.Common/Messages/Messages.csproj -t:Rebuild -p:Configuration=Release -p:Platform=x64
    # Expected: Build succeeded, 0 Error(s)

### M2 — LivePreview module

    dotnet new classlib -n LivePreview -o src/Vixen.Modules/App/LivePreview
    dotnet sln Vixen.sln add src/Vixen.Modules/App/LivePreview/LivePreview.csproj

Fix `Vixen.sln` as before. Create `Descriptor.cs`, `Data.cs`, `ILivePreviewService.cs`,
`LivePreviewService.cs`, `Module.cs`.

Full solution build check:

    msbuild Vixen.sln -m -t:Rebuild -p:Configuration=Debug -p:Platform=x64
    # Expected: Build succeeded, 0 Error(s)

Launch Vixen and confirm `"LivePreview"` context appears in VixenSystem.Contexts.

### M3 — WebServer refactor

Edit files as described in M3. Full solution build:

    msbuild Vixen.sln -m -t:Rebuild -p:Configuration=Debug -p:Platform=x64

Launch Vixen, enable WebServer, hit the element on/off endpoint, confirm behavior is unchanged.
Confirm `Module.LiveContext` no longer exists (grep for `Module.LiveContext` should return zero
results outside of git history).

### M4 — Unit tests

Add `Vixen.Messages` and `Vixen.Modules.App.LivePreview` references to `Vixen.Tests.csproj`.
Create test files as described in M4 above. Run:

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj --logger "console;verbosity=normal"
    # Expected: all tests pass, 0 failed.

---

## Validation and Acceptance

All acceptance criteria must be manually verified before the ticket is closed:

1. **Vixen.Messages builds standalone.** Run `msbuild src/Vixen.Messages/Vixen.Messages.csproj`
   with no other Vixen projects present. Must succeed with 0 errors.

2. **LivePreview auto-starts.** Launch Vixen. Without enabling any module, check that a context
   named `"LivePreview"` exists in `VixenSystem.Contexts`. This can be verified by adding a
   temporary log line in Module.Loading() or via debugger.

3. **WebServer still works.** Enable the WebServer module. Make a PUT request to turn an element
   on. Confirm the element activates. Confirm `Module.LiveContext` no longer exists in
   `src/Vixen.Modules/App/WebServer/Module.cs`. Confirm a `"Web Server"` context appears in
   `VixenSystem.Contexts`.

4. **Context isolation.** Turn on element A via the WebServer (`"Web Server"` context), then
   call `ClearActiveEffects` targeting the default context (null ContextName). Element A must
   remain on (its context was not cleared).

5. **Unit tests pass.** Run `dotnet test src/Vixen.Tests/Vixen.Tests.csproj`. All tests in
   `Vixen.Tests/LivePreview/` must pass with 0 failures.

6. **XML documentation.** All new public and protected members in `ILivePreviewService`,
   `LivePreviewService`, `Module`, `Descriptor`, and all types in `Vixen.Messages` carry XML doc
   comments. Verified by building with `<GenerateDocumentationFile>true</GenerateDocumentationFile>`
   in each new project and confirming no CS1591 warnings.

7. **Design review completed.** `docs/reviews/vix-3925-m2-design-review.md` exists and any
   critical findings from the `dotnet-design-pattern-review` skill have been addressed.

---

## Idempotence and Recovery

Each milestone produces an independently buildable increment. If a milestone is interrupted:
- M1: delete `src/Vixen.Common/Messages/` and re-create from scratch.
- M2: delete `src/Vixen.Modules/App/LivePreview/` and re-create.
- M3: restore `src/Vixen.Modules/App/WebServer/` from git (`git checkout -- src/Vixen.Modules/App/WebServer/`).

The solution file changes (M1, M2) can be reverted with `git checkout -- Vixen.sln`.

---

## Interfaces and Dependencies

### `IContextFactory` (internal, in `Vixen.Modules.App.LivePreview`)

    namespace VixenModules.App.LivePreview;

    internal interface IContextFactory
    {
        LiveContext Create(string name);
        void Release(LiveContext context);
    }

The production implementation `VixenContextFactory` delegates to `VixenSystem.Contexts`. Tests
supply a `Mock<IContextFactory>` that returns mock or stub `LiveContext` instances.

### `ILivePreviewService` (in `Vixen.Modules.App.LivePreview`)

    namespace VixenModules.App.LivePreview;

    public interface ILivePreviewService
    {
        void TurnOnElement(Guid id, int seconds, double intensity, string color, string? contextName = null);
        void TurnOnElements(IEnumerable<ElementState> states, string? contextName = null);
        void TurnOffElement(Guid id, string? contextName = null);
        void TurnOffElements(IEnumerable<ElementState> states, string? contextName = null);
        void ClearActiveEffects(string? contextName = null);
    }

### `LivePreviewChannels` (in `Common.Messages`, project `Messages`)

    namespace Common.Messages.LivePreview;

    public static class LivePreviewChannels
    {
        public const string TurnOnElement      = "LivePreview.TurnOnElement";
        public const string TurnOnElements     = "LivePreview.TurnOnElements";
        public const string TurnOffElement     = "LivePreview.TurnOffElement";
        public const string TurnOffElements    = "LivePreview.TurnOffElements";
        public const string ClearActiveEffects = "LivePreview.ClearActiveEffects";
    }

### Dependency graph (after this change)

    Messages  (src/Vixen.Common/Messages/, namespace Common.Messages)
        ← Vixen.Modules.App.LivePreview  (references Messages, Vixen.Core, Broadcast)
        ← Vixen.Modules.App.WebServer    (references LivePreview + Messages)
        ← Vixen.Tests                    (references Messages + LivePreview for unit tests)
        ← (future consumers — e.g. State Property — reference Messages ONLY)

    Broadcast (Vixen.Common/Broadcast)
        ← Vixen.Modules.App.LivePreview

### Key existing types to reuse

- `LiveContext` — `src/Vixen.Core/Execution/Context/LiveContext.cs`
  Methods used: `Execute(EffectNode)`, `TerminateNode(Guid)`, `TerminateNodes(IEnumerable<Guid>)`,
  `Clear(bool waitForReset)`, `Start()`.
- `ContextManager` — `src/Vixen.Core/Sys/Managers/ContextManager.cs`
  Methods used: `CreateLiveContext(string name)`, `ReleaseContext(IContext)`.
  Accessed via `VixenSystem.Contexts`.
- `Broadcast` — `src/Vixen.Common/Broadcast/Broadcast.cs`
  Methods used: `Publish<T>(string channel, T message)`,
  `Subscribe<T>(object source, string channel, Action<T> callback)`,
  `Unsubscribe<T>(object source, string channel)`.
- `AppModuleInstanceBase` — base class for all App module instances.
  Override `Loading()` and `Unloading()`.
- `AppModuleDescriptorBase` — base class for App module descriptors.
  Must provide a unique `TypeId` GUID.
- `ElementsHelper.TurnOnElement` — `src/Vixen.Modules/App/WebServer/Service/ElementsHelper.cs`
  Copy the EffectNode construction logic from this method into `LivePreviewService.TurnOnElement`.
