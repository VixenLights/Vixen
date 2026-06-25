# Universal Live Preview

## Overview

The WebServer module currently owns live element control logic (turn on/off elements, clear active
effects) tied to its own `LiveContext`. This functionality is useful beyond the WebServer — for
example, the State Property module needs to trigger live element state changes without taking a
dependency on the WebServer. Rather than duplicating the logic, this feature extracts it into a
shared `LivePreview` app module that manages a registry of named `LiveContext` instances, and
introduces a universal `Vixen.Messages` project so any module can publish or subscribe to
live-control broadcast messages with a single, lightweight dependency.

Consumers that need isolation (e.g. the WebServer running its own named context `"Web Server"`)
can target a specific context by name. Consumers that do not care simply omit the context name and
use the module's default context.

---

## New Projects

### `Common.Messages` (`src/Vixen.Common/Messages/`)

A universal message project inside `Vixen.Common`, alongside the existing `Broadcast` project.
Project file is `Messages.csproj` with root namespace `Common.Messages`, nested in the
`Vixen.Common` solution folder — the same convention as `Common.Broadcast`.

**Rules:**
- Contains only record DTOs and channel-name string constants.
- No business logic, no service references, no Vixen framework dependencies.
- Any Vixen module may reference this project to send or receive any supported message type.

**Contents:**

| Type | Purpose |
|---|---|
| `ElementState` | Moved from `WebServer.Model`; carries `Id`, `Duration`, `Color`, `Intensity` |
| `TurnOnElementMessage` | Wraps a single `ElementState` plus an optional `ContextName` |
| `TurnOnElementsMessage` | Wraps `IReadOnlyList<ElementState>` plus an optional `ContextName` |
| `TurnOffElementMessage` | Wraps a single element `Guid` plus an optional `ContextName` |
| `TurnOffElementsMessage` | Wraps `IReadOnlyList<ElementState>` plus an optional `ContextName` |
| `ClearActiveEffectsMessage` | Optional `ContextName`; clears the targeted or default context |
| `LivePreviewChannels` | Static class with channel-name string constants for each message type |

`ContextName` is `null` by default on all message types. A `null` value means "use the LivePreview
default context."

---

### `Vixen.Modules.App.LivePreview`

A new App module (`VixenModules.App.LivePreview`) that owns the single shared `LiveContext` and
exposes live element control as a service.

**Lifecycle:**
- Auto-started by Vixen core on application load — the user does not need to enable it in
  Application Modules.
- Creates and starts a default `LiveContext` named `"LivePreview"` on startup.
- Maintains a registry of named `LiveContext` instances. Named contexts are created on first use
  and released when the module stops.
- All contexts are stopped and released when the module stops.

**Context registry:**

```csharp
// Returns the named context, creating and starting it if it does not yet exist.
// Passing null or omitting the name returns the default "LivePreview" context.
LiveContext GetOrCreateContext(string? contextName = null);
```

**Service interface (`ILivePreviewService`):**

All methods accept an optional `contextName` parameter. `null` targets the default context.

```csharp
void TurnOnElement(Guid id, int seconds, double intensity, string color, string? contextName = null);
void TurnOnElements(IEnumerable<ElementState> states, string? contextName = null);
void TurnOffElement(Guid id, string? contextName = null);
void TurnOffElements(IEnumerable<ElementState> states, string? contextName = null);
void ClearActiveEffects(string? contextName = null);
```

**Broadcast subscription:**
The module subscribes to all five message types from `Vixen.Messages` via the existing
`Broadcast<T>` infrastructure. Incoming messages are dispatched to the corresponding
`ILivePreviewService` method, forwarding the message's `ContextName` (which may be `null`).
If the module is not running, broadcast messages are silently ignored.

---

## WebServer Refactor

- `ElementsHelper` is refactored to delegate to `ILivePreviewService` instead of calling
  `Module.LiveContext` directly. All calls pass `contextName: "Web Server"` so the WebServer's
  operations execute against its own isolated context.
- The live-control methods (`TurnOnElement`, `TurnOnElements`, `TurnOffElement`,
  `TurnOffElements`, `ClearActiveEffects`) are removed from `ElementsHelper`; the class becomes a
  thin adapter that resolves `ILivePreviewService` and forwards calls with the `"Web Server"` context
  name.
- `ElementState` references in the WebServer are updated to use the type from `Vixen.Messages`.
- WebServer continues to own its own REST/SignalR surface; no query helpers
  (`SearchElements`, `GetElements`, `GetChildElements`, `GetParentElements`) move out of WebServer.
- `Module.LiveContext` (the static property) is removed; the WebServer no longer creates or holds a
  `LiveContext` — the `"Web Server"` context is created on first use by `LivePreview`'s registry.

---

## State Property Integration

When the State Property activates or deactivates an element state, it publishes a
`TurnOnElementMessage` or `TurnOffElementMessage` (from `Vixen.Messages`) on the appropriate
`LivePreviewChannels` channel. The message carries the intensity, color, and duration values from
the State Property's configuration. The State Property references only `Vixen.Messages` — it has
no dependency on `Vixen.Modules.App.LivePreview`.

---

## Constraints

- `Vixen.Messages` must remain logic-free. If a consumer needs transformation, it does so locally
  before publishing.
- The query surface of `ElementsHelper` (search, hierarchy traversal) is out of scope for this
  ticket.
- The experimental `Effect(ElementEffect)` method in `ElementsHelper` is out of scope and is not
  promoted to a broadcast message type.
- All new public and protected APIs must carry XML documentation comments per project standards.
