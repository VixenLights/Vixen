# AGENTS.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## About Vixen

Vixen is a Windows desktop application (.NET 10, WPF) for creating and running animated light shows sequenced to music. It supports DMX, traditional and pixel lighting, and DIY controllers.

## Build

```bash
# Restore and build (Release)
msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Release

# Debug build
msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Debug
```

Build output lands in `/Release/Output/` (Release) or `/Debug/Output/` (Debug). Modules build into subdirectories named `Module.{ModuleType}.{ModuleName}/`.

Unit tests live in `src/Vixen.Tests/`. Run them with `dotnet test src/Vixen.Tests/Vixen.Tests.csproj`.

## Code Style

Defined in `src/.editorconfig`. Key rules:
- **Tabs** (not spaces) for indentation
- CRLF line endings
- PascalCase for types and members; `I`-prefix for interfaces
- Nullable and ImplicitUsings enabled; C# 12+ features enabled

Avoid reformatting unrelated code in a commit. If a reformat is necessary, put it in a separate commit clearly marked as such.
Research the codebase before editing. Never change code you haven't read.
Always use Windows (CRLF) line endings when inserting or updating code. Preserve \r\n line-breaks.

## Architecture

Vixen is a modular system

## Use Docs First

Before changing architecture, naming, setup flow, or prop-pipeline behavior, check the relevant files under `docs/` first 
and treat them as the primary repository reference unless the code clearly diverged and needs to be brought back into alignment.

## XML Docs

When modifying any public or protected C# API, update its XML documentation in the same change. This includes summary text,
parameter docs, return docs, remarks, and exception docs when behavior changes. Treat stale XML docs as defects, not cleanup.

Use the `csharp-docs` skill for changes that add or modify public
or protected C# classes, interfaces, methods, properties, or events.

### Project Layout

```
src/
  Vixen.Core/        # Core framework: module system, execution engine, data models
  Vixen.Application/ # Main WPF application (entry point, orchestrates everything)
  Vixen.Common/      # Shared utilities and controls (AudioPlayer, WpfPropertyGrid, etc.)
  Vixen.Modules/     # All pluggable modules (Effects, Controllers, Editors, etc.)
  Vixen.Installer/   # Installer projects
  Vixen.DeployBundle/# Deployment packaging
```

### Module (Plugin) System

Vixen is built around a descriptor-based plugin architecture. Every capability — effects, controllers, editors, output filters, previews — is a module.

**Three core interfaces in `Vixen.Core/Module/`:**
- `IModuleDescriptor` — static metadata: `TypeId` (GUID), author, version, `ModuleClass` (the concrete type to instantiate), declared `Dependencies`
- `IModuleInstance` — runtime instance: holds `InstanceId`, `ModuleData`, `StaticModuleData`
- `IModuleDataModel` — serializable per-instance configuration

**Module folder convention under `src/Vixen.Modules/{ModuleType}/{ModuleName}/`:**
```
MyEffect.cs              # IEffect implementation
MyEffectDescriptor.cs    # Inherits EffectModuleDescriptorBase
MyEffectData.cs          # Inherits ModuleDataModel (serialized state)
MyEffect.csproj
```

**Namespace convention:** `VixenModules.{ModuleType}.{ModuleName}`

**Module types** (each a subdirectory under `Vixen.Modules/`):
- `Effect` — visual effects rendered onto element timelines (50+ effects)
- `Editor` — UI editors (TimedSequenceEditor, LayerEditor, FixtureWizard)
- `App` — application-level modules (ColorGradients, Curves, Shows, WebServer, Modeling)
- `Controller` — hardware output drivers
- `OutputFilter` — per-channel output transforms (ColorBreakdown, DimmingCurve, etc.)
- `Property` — element properties (Color, Orientation, IntelligentFixture, etc.)
- `Preview` — preview renderers
- `Media` / `Timing` / `SequenceFilter` — supporting module types

### Effect Module Lifecycle

Effect modules inherit from `EffectModuleInstanceBase` and implement `IEffect`. Key methods:
- `PreRender()` — called once before rendering begins; set up data structures here.
- `Render()` — called per effect TimeSpan to produce command intents that the engine will execute.
- `GenerateVisualRepresentation()` — produces the thumbnail shown in the sequence editor if the standard intent rasterizer needs to be overridden
- Edits of an effect, run this lifecycle method to update the effect instance's data model.

### Adding Projects to the Solution

After running `dotnet sln add`, the generated `Any CPU` platform entries in `Vixen.sln` must be corrected — they default to `Any CPU` targets but this solution only builds `x86` and `x64`. For every newly added project, find its GUID in the `GlobalSection(ProjectConfigurationPlatforms)` block and ensure all three `Any CPU` entries point to `x64`:

```
{GUID}.Debug|Any CPU.ActiveCfg = Debug|x64
{GUID}.Debug|Any CPU.Build.0 = Debug|x64
{GUID}.Deploy|Any CPU.ActiveCfg = Deploy|x64
{GUID}.Deploy|Any CPU.Build.0 = Deploy|x64
{GUID}.Release|Any CPU.ActiveCfg = Release|x64
{GUID}.Release|Any CPU.Build.0 = Release|x64
```

**`dotnet sln add` creates spurious solution folders.** The command generates new `Project` entries of type `{2150E333...}` (Solution Folder) mirroring the filesystem path (e.g. `src`, `Vixen.Modules`, `App`) and nests the new project inside them. These duplicate folders must be removed manually:

1. Delete the auto-generated `Project`/`EndProject` blocks for the spurious folders.
2. In `GlobalSection(NestedProjects)`, update the new project's entry to point at the correct pre-existing solution folder GUID instead of the auto-generated one.

### Project References

Per README conventions:
- Use **project references**, not DLL references
- Set **Copy Local = No** and **Include Assets = None** on project references
- NuGet packages follow the same rule — add the package centrally (`Directory.Packages.props`), set **Exclude Assets = None** locally so binaries deploy only to the common output path

### Key Frameworks

- **Catel** (MVVM) + **Orchestra** — WPF application shell, view models, service locator
- **CommunityToolkit.Mvvm** — additional MVVM helpers
- **NLog** — logging throughout
- **MessagePack** — high-performance serialization
- **LiteDB** — embedded document store for custom props
- **NAudio** — audio playback

## Issue Tracker

Bugs and feature requests are tracked at <http://vixenlights.atlassian.net> (Jira). Commit messages and branch names reference ticket IDs (e.g., `VIX-3871`).

# ExecPlans

When writing complex features or significant refactors, use an ExecPlan (as described in .agents/PLANS.md) from design to implementation.

# Skills

Project-specific skills live under `.agents/skills/`. Always read the skill file from that directory before applying it — project versions contain extra rules and output conventions not present in system-level skill definitions.

Available skills:

| Skill | Path | When to use |
|---|---|---|
| `csharp-docs` | `.agents/skills/csharp-docs/SKILL.md` | Adding or modifying any public or protected C# API |
| `csharp-async` | `.agents/skills/csharp-async/SKILL.md` | Writing or reviewing async/await C# code |
| `dotnet-best-practices` | `.agents/skills/dotnet-best-practices/SKILL.md` | Reviewing or writing any C# code |
| `dotnet-design-pattern-review` | `.agents/skills/dotnet-design-pattern-review/SKILL.md` | Reviewing interfaces, types, or architecture proposals |
| `catel-mvvm` | `.agents/skills/catel-mvvm/SKILL.md` | Writing or reviewing WPF ViewModels, Views, or commands |
| `commit-msg` | `.agents/skills/commit-msg/SKILL.md` | Composing git commit messages |
| `summarize-changes` | `.agents/skills/summarize-changes/SKILL.md` | Summarizing a changeset or PR |
| `jira` | `.agents/skills/jira/SKILL.md` | Use when asked to "create JIRA ticket", "search JIRA", "update JIRA issue", "transition issue", "sprint planning", or "epic management". |

Task documents that say "use the X skill" always refer to the project version at `.agents/skills/X/SKILL.md`.
