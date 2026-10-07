# DDP Controller Host Name Support (VIX-3605)

## Overview

The DDP controller setup form (`src/Vixen.Modules/Controller/DDP/DDPSetup.cs`) only accepts a dotted
IPv4 address. `DDPSetup.Address` is typed as `System.Net.IPAddress`, and both the getter and the
constructor silently fall back to `IPAddress.Loopback` whenever the text in the box does not parse as
an IP — so a host name typed into the box is simply discarded. `DDPData.Address`
(`src/Vixen.Modules/Controller/DDP/DDPData.cs`) is a persisted `string`, but nothing ever puts anything
other than an IP-literal string into it today. There's already a breadcrumb for this exact gap in
`DDP.cs` (`OpenConnection()`): a commented-out line reading
`//_udpClient.Connect(_data Hostname, DDP_PORT);  //Switch to add Hostname support`.

The Streaming ACN (E1.31) controller already solves the "IP address or host name" problem in its
Unicast destination dialog (`src/Vixen.Modules/Controller/E131/UnicastForm.cs`): a pair of radio
buttons lets the user choose "IP Address" or "Network Name", and only the matching text box is
enabled. This document specifies bringing equivalent behavior to the DDP setup form, with two
deliberate improvements over the E131 precedent (see [Differences from the E131 Pattern](#differences-from-the-e131-pattern)):
resolution happens once, at entry time, rather than being deferred to every controller start, and the
original host name is preserved as its own persisted field so the form can redisplay it later.

## Requirements

- The DDP setup form must let the user choose, via two mutually exclusive radio buttons ("IP Address" /
  "Host Name"), whether the text box underneath is interpreted as a literal IP address or as a DNS host
  name — modeled on `UnicastForm`'s `ipRadio` / `networkNameRadio` pair.
- Only one text box is enabled/visible at a time, matching the existing `updateChecked()` swap pattern
  in `UnicastForm.cs`.
- When the user selects "IP Address", the existing IP validation behavior is unchanged (the box must
  contain a value parseable by `IPAddress.TryParse`).
- When the user selects "Host Name" and clicks OK, the entered host name must be resolved to an IPv4
  address (`Dns.GetHostAddresses`, preferring the first `AddressFamily.InterNetwork` result — see the
  IPv4-selection note below).
  - If resolution succeeds, the resolved `IPAddress` is what gets written to `DDPData.Address` (the
    field DDP's runtime connection and `GetNetworkConfiguration()` already consume) — never the raw
    host name string.
  - If resolution fails (unknown host, network error, no IPv4 result), the dialog must not close.
    Show a `MessageBoxForm` error using the current constructor-based pattern (not the older
    `MessageBoxForm.msgIcon = ...` static-field style found in `E131/SetupForm.cs`), e.g.:

    ```csharp
    MessageBoxForm mbf = new MessageBoxForm($"Could not resolve host '{hostName}'.", "DDP Setup Error",
        MessageBoxButtons.OK, SystemIcons.Error);
    mbf.ShowDialog(this);
    ```

    and return focus to the host name box so the user can correct or cancel.
- The resolved-from host name itself must be persisted separately from the resolved IP, in a new
  `DDPData.HostName` (nullable `string`) field, so that reopening the setup form can restore the user's
  original entry instead of just showing the IP it resolved to.
- When the setup form is constructed:
  - If `DDPData.HostName` is set (non-null/non-empty), the "Host Name" radio is selected and that text
    box is pre-filled with `HostName`.
  - Otherwise the "IP Address" radio is selected and the IP text box is pre-filled with `Address`
    (existing behavior, unchanged).
- When the user chooses "IP Address" and clicks OK, `DDPData.HostName` must be cleared (set to `null`)
  so a stale host name doesn't cause the form to misrepresent the current configuration on next open.
- `DDP.cs` (`OpenConnection`, `GetNetworkConfiguration`, `HostInfo`) continue to operate purely on
  `DDPData.Address` (already-resolved IP) and require no behavior change — resolution is a setup-time
  concern only, not a runtime one. The commented-out hostname line in `OpenConnection()` should be
  removed as part of this change since it's superseded by resolve-at-setup-time.
- `DDPData.Clone()` must copy the new `HostName` field along with `Address`.
- Existing saved profiles (which have no `HostName` value) must continue to load correctly — the field
  defaults to `null`/empty, which the form already treats as "show IP Address mode".
- Add unit test coverage in `src/Vixen.Tests/` for the new resolution/persistence logic (successful
  resolution, failed resolution keeps dialog open, mode-switch clears `HostName`, `Clone()` round-trips
  `HostName`). Existing IP-only entry behavior must be covered to prove it still works unchanged.
- Whenever the DDP controller (re)establishes its network connection — `DDP.cs`'s `OpenConnection()`,
  called from `Start()`, `Resume()`, and immediately after a successful `Setup()` — if the controller was
  configured via a host name (`DDPData.HostName` is non-null/non-empty), the host name must be re-resolved
  to an IPv4 address using the same resolution logic as the setup dialog. If the freshly resolved address
  differs from the currently stored `Address`, `Address` must be updated automatically, with no user
  intervention required, so the controller keeps working if the host's DHCP-assigned IP address changes
  over time.
- If re-resolution fails (host temporarily unreachable, DNS server down, etc.), the controller must log a
  warning using the existing logging pattern already present in `OpenConnection()`
  (`Logging.Warn(LogTag + "...")`) and continue using the last known-good `Address` rather than failing the
  connection outright — a transient DNS hiccup must not prevent the controller from running with the
  address it last successfully resolved.
- This runtime re-validation only applies when `HostName` is populated. Controllers configured with a
  literal IP address (`HostName` is `null`) are unaffected and behave exactly as they do today — no
  resolution attempt is made for them.

## Runtime Host Re-Validation

Setup-time resolution (above) only captures the IP address at the moment the user clicks OK. If the
controller's host is later reassigned a different address by DHCP, a purely setup-time approach would
require the user to manually reopen the setup dialog and re-save to pick up the change. To avoid that,
`DDP.cs`'s `OpenConnection()` — the method that opens the actual UDP socket, already called on every
`Start()`, `Resume()`, and post-`Setup()` reconnect — re-resolves `HostName` (when present) every time it
runs, before opening the socket. This is a lightweight, on-demand check (no background polling thread, no
timer): it happens exactly as often as the controller already (re)connects today, which is the natural and
sufficient cadence for this problem (a light show typically starts/stops/resumes at well-defined times, not
continuously). Resolution failures are logged and non-fatal, matching this method's existing behavior for
other connection failures.

## Differences from the E131 Pattern

The E131 `UnicastForm`/`E131ModuleDataModel` pattern was reviewed as the starting point but has two
characteristics this spec intentionally does not copy:

1. **E131 stores one ambiguous string and resolves at every controller start.**
   `E131ModuleDataModel.Unicast` holds either an IP literal or a host name in a single field, and
   `E131OutputPlugin` calls `Dns.GetHostAddresses(_data.Unicast)` synchronously on every `Setup()`/start,
   with no caching. Per the ticket requirements, DDP instead resolves once when the user accepts the
   setup dialog and stores the resolved IP in `Address` — `DDP.cs`'s runtime path never performs DNS
   resolution.
2. **E131's `UnicastForm.IpAddrText` setter is effectively dead/unreliable.** No call site in the
   codebase ever assigns to it (the form is always constructed blank via `new UnicastForm()`), and if it
   were used, it sets the text boxes based on `IPAddress.TryParse` but never updates
   `ipRadio.Checked`/`networkNameRadio.Checked` to match, leaving `updateChecked()` to read stale radio
   state. The new `DDPSetup` constructor must decide box contents *and* which radio is checked together,
   driven by whether `DDPData.HostName` is populated (see Requirements above), so this bug is not
   reintroduced.

Also note the IPv4-selection quirk in E131's `Dns.GetHostAddresses` loops (they iterate without
`break`, so they keep the *last* IPv4 result rather than the first if a host has multiple A records).
The new DDP resolution logic should select the *first* matching IPv4 address instead, since that's the
more conventional/expected behavior and there's no reason to reproduce the existing quirk.

## Shared Resolution Utility

The host-name-to-IPv4 resolution logic is implemented once, as a public utility, rather than as a
DDP-specific class: `Utilities.HostNameResolver` (`src/Vixen.Common/Utilities/HostNameResolver.cs`, in the
existing `Vixen.Common/Utilities` project, namespace `Utilities`, alongside other shared helpers like
`ElementTemplateHelper`). This lets the DDP module consume it now, and lets a future ticket migrate the
E131 controller's own ad hoc `Dns.GetHostAddresses` call sites (`E131OutputPlugin.cs`) onto the same
mechanism instead of maintaining two separate implementations of the same "resolve a host name to an
IPv4 address" behavior — including E131's own "last IPv4 wins" quirk described above, which that future
migration would also fix by switching to the shared, first-IPv4-wins implementation. Migrating E131 itself
is explicitly out of scope for this ticket; this section only records where the shared piece lives so that
future work has an obvious, already-tested place to plug into.

## Data Model Changes

`DDPData` (`src/Vixen.Modules/Controller/DDP/DDPData.cs`) gains one new `[DataMember]`:

```csharp
[DataMember]
public string HostName { get; set; }
```

`Address` keeps its existing meaning and type (`string`, IP-literal, `IPAddress.Loopback.ToString()`
default) — no existing consumers of `DDPData.Address` change.

## Guidelines

- Use the project skills `dotnet-best-practices`, `csharp-async`, `csharp-docs`, and
  `dotnet-design-pattern-review` when implementing this change.
- Add or update XML doc comments on `DDPData.HostName`, `DDPData.Address`, and any new/changed public or
  protected members on `DDPSetup`, per the `csharp-docs` skill — treat stale or missing docs as defects.
- DNS resolution is a blocking call (`Dns.GetHostAddresses`); follow the `csharp-async` skill's guidance
  on whether/how to keep the setup dialog responsive during resolution (e.g. a wait cursor) without
  introducing unnecessary complexity for what is a single one-shot, user-initiated lookup.
- Use `.agents/PLANS.md` conventions for the execution plan that implements this spec.
- The first implementation milestone must include creating a JIRA issue update to VIX-3605 with the
  finalized requirements, high-level design, acceptance criteria, and testing steps captured in this
  document — only after this spec is fully accepted.
- Call out any risks or open questions discovered during planning (e.g. behavior when a host name
  resolves to multiple IPv4 addresses, or when DNS is temporarily unavailable at setup time) in the
  execution plan.
