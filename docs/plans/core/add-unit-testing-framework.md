# Add Unit Testing Framework to Vixen (xUnit + Moq)

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds. This document must be maintained in accordance with `.agents/PLANS.md`.


## Purpose / Big Picture

Vixen currently has no automated tests. After this change, a developer can run `dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release` from the repo root and observe 10 green tests covering three core utility classes. The test project sits in the solution alongside all other projects, builds cleanly in the CI pipeline, and serves as an on-ramp for all future test authorship. A contributor who has never written a test in this codebase can read the seed files and understand the expected structure and naming conventions.


## Progress

- [x] Create JIRA issue in the VIX project to track this work (VIX-3920)
- [x] Add xunit, xunit.runner.visualstudio, Microsoft.NET.Test.Sdk, and Moq entries to `Directory.Packages.props`
- [x] Create `src/Vixen.Tests/` directory and subdirectories
- [x] Create `src/Vixen.Tests/Directory.Build.props` (imports parent, overrides output path)
- [x] Create `src/Vixen.Tests/Vixen.Tests.csproj`
- [x] Create `src/Vixen.Tests/Utility/NamingUtilitiesTests.cs`
- [x] Create `src/Vixen.Tests/Common/XYZTests.cs`
- [x] Create `src/Vixen.Tests/Data/Value/RGBValueTests.cs`
- [x] Run `dotnet sln add src/Vixen.Tests/Vixen.Tests.csproj` from repo root
- [x] Edit `Vixen.sln` to replace the six `Any CPU` platform entries for Vixen.Tests with `x64` targets
- [x] Run `dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release` and confirm 10 tests pass (12 passed)


## Surprises & Discoveries

- xunit 2.x does not inject global usings automatically; all test files need `using Xunit;` explicitly.
- The plan's decision to use `<Private>false</Private>` on the Vixen.Core project reference caused `Vixen.Core.dll` to be absent from the test output folder, making all tests fail at runtime with `FileNotFoundException`. The Copy Local = No convention applies to production modules deploying to the shared `Release\Output` folder; test projects must copy dependencies locally. The attribute was removed from `Vixen.Tests.csproj`.
- `dotnet test` without `-p:SolutionDir=...` triggers a failing post-build copy step in Vixen.Core (`*Undefined*\Release Notes.txt`). Always pass `-p:SolutionDir="C:\Dev\Vixen\\"` when building or testing the project in isolation.
- The three seed test files contain 12 tests total (3 + 6 + 3), not 10 as stated in the plan — the plan count was off by 2.


## Decision Log

- Decision: Use `xunit.v3 3.2.2`, `xunit.runner.visualstudio 3.1.5`, `Microsoft.NET.Test.Sdk 17.12.0`, `Moq 4.20.72`.
  Rationale: xUnit v2 is deprecated/maintenance-only; v3 is the current supported version. `xunit.runner.visualstudio` 3.x is required alongside `xunit.v3` for `dotnet test` / VSTest discovery — v3 does not embed its own adapter.
  Date/Author: 2026-05-26 / planning (updated post-implementation)

- Decision: The local `src/Vixen.Tests/Directory.Build.props` must explicitly import `../Directory.Build.props` before overriding output paths.
  Rationale: MSBuild stops ascending the directory tree at the first `Directory.Build.props` it finds. If the local file does not import the parent, the test project loses `TargetFramework`, `Platforms`, `Nullable`, `ImplicitUsings`, and all other shared settings.
  Date/Author: 2026-05-26 / planning

- Decision: Do NOT add `UseWindowsForms=true` in the test project.
  Rationale: `net10.0-windows` (inherited from parent props) is sufficient; `System.Drawing.Color` arrives transitively through Vixen.Core's `UseWindowsForms=true`. Adding it again is redundant and may cause SDK conflicts in headless CI.
  Date/Author: 2026-05-26 / planning

- Decision: Project reference to Vixen.Core uses `<Private>false</Private>` and no `ExcludeAssets` attribute.
  Rationale: CLAUDE.md states "Copy Local = No and Include Assets = None on project references." `Private=false` is the MSBuild spelling of Copy Local = No. `ExcludeAssets=None` is the default when the attribute is absent, so it is omitted.
  Date/Author: 2026-05-26 / planning

- Decision: Grayscale assertions use exact equality, not tolerance-based comparison.
  Rationale: `_BasicGrayscaleLuma` casts to `byte` by truncation. Red (255,0,0): `255*0.3 = 76.5` truncated → 76. White (255,255,255): `76.5 + 150.45 + 28.05 = 255.0` → 255. Black → 0. These are deterministic integer results.
  Date/Author: 2026-05-26 / planning

- Decision: Override `BaseOutputPath` and `OutputPath` in `src/Vixen.Tests/Directory.Build.props` to point to the project's own `bin/` instead of the shared `Release\Output\`.
  Rationale: `dotnet test` resolves the test DLL relative to the project's standard `bin/$(Configuration)/$(TargetFramework)/` path. If test binaries land in the shared output folder, `dotnet test` cannot find them.
  Date/Author: 2026-05-26 / planning

- Decision: Create a JIRA Task in project VIX to track this work before implementation begins.
  Rationale: All Vixen development work is tracked at http://bugs.vixenlights.com. Branch names and commit messages reference the issue key (e.g., VIX-XXXX). Record the assigned key here once the issue is created.
  JIRA issue: VIX-3920
  Date/Author: 2026-05-26 / planning


## Outcomes & Retrospective

Completed 2026-05-26. `dotnet test` reports 12 passed, 0 failed, 0 skipped. Test binary lands in `src/Vixen.Tests/bin/Release/` as intended. Three deviations from the plan were required: explicit `using Xunit;` in all test files, removal of `<Private>false</Private>` on the Vixen.Core reference, and passing `-p:SolutionDir` when running outside the full solution build. All tracked under VIX-3920.


## Context and Orientation

Vixen is a .NET 10 WPF desktop application at `C:\Dev\Vixen`. All ~150 source projects live under `src/` and share a single `src/Directory.Build.props` that sets the target framework (`net10.0-windows`), enables nullable reference types and implicit usings, pins output to `$(SolutionDir)$(Configuration)\Output`, and declares `Platforms=x86;x64`. There is no `Any CPU` build — only `x64` (and `x86` for legacy compatibility). Every project's `Any CPU.ActiveCfg` entries in the solution file must redirect to `x64` targets; `dotnet sln add` generates these entries pointing to `Any CPU` by default, so a manual fix is required after running the command.

NuGet packages are managed centrally: every package version is declared in `Directory.Packages.props` at the repo root. A local `<PackageReference>` node names the package but omits the version; MSBuild reads the version from the root props. To add a new package, add a `<PackageVersion>` entry to the root props first.

The three classes under test live in `src/Vixen.Core/`:

`Vixen.Utility.NamingUtilities` in `src/Vixen.Core/Utility/NamingUtilities.cs` — a static class with one public method, `Uniquify(HashSet<string> names, string name)`. Returns `name` unchanged if not in the set. If the name IS in the set, appends " - 2", " - 3", etc. (using `string.Format("{0} - {1}", originalName, counter++)`) until a unique name is found. The do-while loop condition checks `names.Contains(name)` after each increment; when the result is NOT in the set, `unique` is false and the loop exits.

`Common.Controls.ColorManagement.ColorModels.XYZ` in `src/Vixen.Core/Common/ColorSpaces.cs` — a public struct. The static method `ClipValue(double value, double min, double max)` returns `min` for NaN, negative infinity, or any value below `min`; returns `max` for positive infinity or any value above `max`; otherwise returns `value` unchanged.

`Vixen.Data.Value.RGBValue` in `src/Vixen.Core/Data/Value/RGBValue.cs` — a public struct with public static methods `ConvertToGrayscale(Color color)` and `GetGrayscaleLevel(Color color)`. The formula is `(byte)(R*0.3 + G*0.59 + B*0.11)`; `GetGrayscaleLevel` returns the `.R` component of the resulting grayscale color. The `Color` type is `System.Drawing.Color`, available on `net10.0-windows`.


## Plan of Work

The work proceeds in five ordered phases.

**Phase 1: Central package registration.** Open `C:\Dev\Vixen\Directory.Packages.props` and add four `<PackageVersion>` entries inside the existing `<ItemGroup>` block. Insert them in alphabetical order alongside the other packages (exact content in Concrete Steps).

**Phase 2: Test project directory and files.** Create `src/Vixen.Tests/` and subdirectories `Utility/`, `Common/`, `Data/Value/`. Write four files: `Directory.Build.props`, `Vixen.Tests.csproj`, and the three seed test files. Exact file content is in the Artifacts section.

**Phase 3: Solution integration.** Run `dotnet sln add src/Vixen.Tests/Vixen.Tests.csproj` from the repo root. Then open `Vixen.sln`, find the six `Any CPU` platform configuration lines for the newly added project GUID, and change the right-hand side of each line to reference `x64` targets (exact diff shown in Concrete Steps).

**Phase 4: Build verification.** Run `dotnet build src/Vixen.Tests/Vixen.Tests.csproj -c Release`. Confirm the build succeeds with zero errors. Test binaries should be at `src/Vixen.Tests/bin/Release/net10.0-windows/Vixen.Tests.dll` — NOT in `Release\Output\`.

**Phase 5: Test execution.** Run `dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release` and confirm 10 tests pass, 0 fail.


## Concrete Steps

All commands run from `C:\Dev\Vixen` unless stated otherwise.

**Step 0 — Create JIRA issue**

Use the `jira` skill (`.agents/skills/jira/SKILL.md`) to create a new Task in the VIX project at http://bugs.vixenlights.com. The issue should capture the full scope of this plan so the work is tracked alongside other Vixen development. Use the following field values:

- Project key: `VIX`
- Issue type: `Task`
- Summary: `Add xUnit unit testing framework with seed tests`
- Description (Markdown):

      Adds a new Vixen.Tests project to the solution, establishing xUnit + Moq as the standard testing framework
      and providing 10 seed tests across three pure-logic classes as a repeatable pattern for future contributors.

      Scope:
      - Register xunit 2.9.2, xunit.runner.visualstudio 2.8.2, Microsoft.NET.Test.Sdk 17.12.0, and Moq 4.20.72
        in the central Directory.Packages.props.
      - Create src/Vixen.Tests/ with its own Directory.Build.props (inherits shared props, overrides output path
        to bin/ so dotnet test can discover tests).
      - Seed test classes: NamingUtilitiesTests (3 tests), XYZTests (6 tests), RGBValueTests (3 tests).
      - Add project to Vixen.sln and correct Any CPU → x64 platform entries per solution conventions.

      Acceptance: dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release reports 10 passed, 0 failed.

      ExecPlan: docs/plans/add-unit-testing-framework.md

After creation, record the assigned issue key (e.g., VIX-XXXX) in the Decision Log below so it can be referenced in branch names and commit messages.

**Step 1 — Edit Directory.Packages.props**

Inside the single `<ItemGroup>` in `C:\Dev\Vixen\Directory.Packages.props`, add these four lines in alphabetical order among the existing entries:

    <PackageVersion Include="Microsoft.NET.Test.Sdk" Version="17.12.0" />
    <PackageVersion Include="Moq" Version="4.20.72" />
    <PackageVersion Include="xunit" Version="2.9.2" />
    <PackageVersion Include="xunit.runner.visualstudio" Version="2.8.2" />

**Step 2 — Create directories**

    mkdir src\Vixen.Tests
    mkdir src\Vixen.Tests\Utility
    mkdir src\Vixen.Tests\Common
    mkdir src\Vixen.Tests\Data\Value

**Step 3 — Create src/Vixen.Tests/Directory.Build.props**

    <Project>
        <Import Project="..\Directory.Build.props" />

        <PropertyGroup>
            <BaseOutputPath>$(MSBuildThisFileDirectory)bin\</BaseOutputPath>
            <OutputPath>$(BaseOutputPath)$(Configuration)\</OutputPath>
        </PropertyGroup>
    </Project>

The `<Import>` must be the first element. Without it, `TargetFramework` and all other shared settings are lost.

**Step 4 — Create src/Vixen.Tests/Vixen.Tests.csproj**

    <Project Sdk="Microsoft.NET.Sdk">

        <PropertyGroup>
            <RootNamespace>Vixen.Tests</RootNamespace>
            <IsPackable>false</IsPackable>
            <IsPublishable>false</IsPublishable>
        </PropertyGroup>

        <ItemGroup>
            <PackageReference Include="Microsoft.NET.Test.Sdk" />
            <PackageReference Include="Moq" />
            <PackageReference Include="xunit" />
            <PackageReference Include="xunit.runner.visualstudio">
                <PrivateAssets>all</PrivateAssets>
                <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
            </PackageReference>
        </ItemGroup>

        <ItemGroup>
            <ProjectReference Include="..\Vixen.Core\Vixen.Core.csproj">
                <Private>false</Private>
            </ProjectReference>
        </ItemGroup>

    </Project>

**Step 5 — Create seed test files** (exact content in Artifacts section)

- `src/Vixen.Tests/Utility/NamingUtilitiesTests.cs`
- `src/Vixen.Tests/Common/XYZTests.cs`
- `src/Vixen.Tests/Data/Value/RGBValueTests.cs`

**Step 6 — Add project to solution**

    dotnet sln Vixen.sln add src/Vixen.Tests/Vixen.Tests.csproj

Expected output:

    Project `src/Vixen.Tests/Vixen.Tests.csproj` added to the solution.

**Step 7 — Fix Any CPU platform entries in Vixen.sln**

Search `Vixen.sln` for the GUID of the newly added project. In `GlobalSection(ProjectConfigurationPlatforms)`, find the six lines with that GUID that contain `Any CPU` on the right-hand side. Replace each right-hand side so the block reads:

    {NEWGUID}.Debug|Any CPU.ActiveCfg = Debug|x64
    {NEWGUID}.Debug|Any CPU.Build.0 = Debug|x64
    {NEWGUID}.Deploy|Any CPU.ActiveCfg = Debug|x64
    {NEWGUID}.Deploy|Any CPU.Build.0 = Debug|x64
    {NEWGUID}.Release|Any CPU.ActiveCfg = Release|x64
    {NEWGUID}.Release|Any CPU.Build.0 = Release|x64

Note: the Deploy configuration maps to `Debug|x64`, matching the established pattern for other projects in the solution. Do not change the left-hand side of any line.

**Step 8 — Build verification**

    dotnet build src/Vixen.Tests/Vixen.Tests.csproj -c Release

Expected:

    Build succeeded.
        0 Warning(s)
        0 Error(s)

**Step 9 — Run tests**

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release

Expected:

    Passed!  - Failed:     0, Passed:    10, Skipped:     0, Total:    10, Duration: ...


## Validation and Acceptance

The change is accepted when all of the following are true:

1. `dotnet build src/Vixen.Tests/Vixen.Tests.csproj -c Release -p:SolutionDir="C:\Dev\Vixen\"` succeeds with zero errors and zero warnings.
2. `dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release -p:SolutionDir="C:\Dev\Vixen\"` reports 10 passed, 0 failed, 0 skipped.
3. The test binary is present at `src/Vixen.Tests/bin/Release/net10.0-windows/Vixen.Tests.dll` and NOT at `Release\Output\Vixen.Tests.dll`.
4. The full solution build `msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Release` still succeeds (the test project must not break the overall build).

Expected individual test names and their assertions:

NamingUtilitiesTests (3 tests):
- `Uniquify_NameNotInSet_ReturnsNameUnchanged` — result equals input name unchanged
- `Uniquify_NameAlreadyInSet_ReturnsNameWithSuffix2` — result equals "foo - 2"
- `Uniquify_Name2AlsoInSet_ReturnsNameWithSuffix3` — result equals "foo - 3"

XYZTests (6 tests):
- `ClipValue_ValueInRange_ReturnsValue` — ClipValue(50.0, 0.0, 100.0) == 50.0
- `ClipValue_ValueBelowMin_ReturnsMin` — ClipValue(-1.0, 0.0, 100.0) == 0.0
- `ClipValue_ValueAboveMax_ReturnsMax` — ClipValue(101.0, 0.0, 100.0) == 100.0
- `ClipValue_NaN_ReturnsMin` — ClipValue(double.NaN, 0.0, 100.0) == 0.0
- `ClipValue_NegativeInfinity_ReturnsMin` — ClipValue(double.NegativeInfinity, 0.0, 100.0) == 0.0
- `ClipValue_PositiveInfinity_ReturnsMax` — ClipValue(double.PositiveInfinity, 0.0, 100.0) == 100.0

RGBValueTests (3 tests):
- `GetGrayscaleLevel_Black_Returns0` — GetGrayscaleLevel(Color.FromArgb(0,0,0)) == 0
- `GetGrayscaleLevel_White_Returns255` — GetGrayscaleLevel(Color.FromArgb(255,255,255)) == 255
- `GetGrayscaleLevel_Red_Returns76` — GetGrayscaleLevel(Color.FromArgb(255,0,0)) == 76


## Idempotence and Recovery

Running `dotnet sln add` a second time for a project already in the solution produces `Project already exists in the solution` and changes nothing — safe to re-run.

If `Directory.Packages.props` already contains a `<PackageVersion>` entry for any of the four packages, do not add a duplicate — MSBuild will emit an error on duplicate `Include` values.

If the build fails with `TargetFramework` undefined, the `Directory.Build.props` inside `src/Vixen.Tests/` is missing its `<Import>` of the parent. Add the import as the first element in `<Project>`.

If `dotnet test` reports "no test assemblies found", verify the test binary is in `src/Vixen.Tests/bin/` (not `Release\Output\`). If binaries are in the wrong location, both `BaseOutputPath` and `OutputPath` must be overridden in the local `Directory.Build.props`.


## Artifacts and Notes

**src/Vixen.Tests/Utility/NamingUtilitiesTests.cs**

    // =============================================================================
    // VIXEN TEST AUTHORING GUIDE
    //
    // Every test class follows the AAA (Arrange, Act, Assert) pattern:
    //   Arrange  — set up objects and data the method under test needs.
    //   Act      — call the method under test exactly once.
    //   Assert   — verify one observable outcome.
    //
    // Naming convention: MethodName_Condition_ExpectedBehavior
    //
    // One assertion per test keeps failure messages pinpoint-accurate.
    // Use Assert.Equal(expected, actual) — expected value comes first.
    //
    // For classes that need mocked dependencies, inject them with Moq:
    //   var mock = new Mock<IMyService>();
    //   mock.Setup(s => s.DoThing()).Returns("value");
    //   var sut = new MyClass(mock.Object);
    // =============================================================================

    using Vixen.Utility;

    namespace Vixen.Tests.Utility;

    public class NamingUtilitiesTests
    {
        [Fact]
        public void Uniquify_NameNotInSet_ReturnsNameUnchanged()
        {
            // Arrange
            var names = new HashSet<string> { "alpha", "beta" };
            var name = "gamma";

            // Act
            var result = NamingUtilities.Uniquify(names, name);

            // Assert
            Assert.Equal("gamma", result);
        }

        [Fact]
        public void Uniquify_NameAlreadyInSet_ReturnsNameWithSuffix2()
        {
            // Arrange
            var names = new HashSet<string> { "foo" };
            var name = "foo";

            // Act
            var result = NamingUtilities.Uniquify(names, name);

            // Assert
            Assert.Equal("foo - 2", result);
        }

        [Fact]
        public void Uniquify_Name2AlsoInSet_ReturnsNameWithSuffix3()
        {
            // Arrange
            var names = new HashSet<string> { "foo", "foo - 2" };
            var name = "foo";

            // Act
            var result = NamingUtilities.Uniquify(names, name);

            // Assert
            Assert.Equal("foo - 3", result);
        }
    }


**src/Vixen.Tests/Common/XYZTests.cs**

    using Common.Controls.ColorManagement.ColorModels;

    namespace Vixen.Tests.Common;

    public class XYZTests
    {
        [Fact]
        public void ClipValue_ValueInRange_ReturnsValue()
        {
            var result = XYZ.ClipValue(50.0, 0.0, 100.0);
            Assert.Equal(50.0, result);
        }

        [Fact]
        public void ClipValue_ValueBelowMin_ReturnsMin()
        {
            var result = XYZ.ClipValue(-1.0, 0.0, 100.0);
            Assert.Equal(0.0, result);
        }

        [Fact]
        public void ClipValue_ValueAboveMax_ReturnsMax()
        {
            var result = XYZ.ClipValue(101.0, 0.0, 100.0);
            Assert.Equal(100.0, result);
        }

        [Fact]
        public void ClipValue_NaN_ReturnsMin()
        {
            var result = XYZ.ClipValue(double.NaN, 0.0, 100.0);
            Assert.Equal(0.0, result);
        }

        [Fact]
        public void ClipValue_NegativeInfinity_ReturnsMin()
        {
            var result = XYZ.ClipValue(double.NegativeInfinity, 0.0, 100.0);
            Assert.Equal(0.0, result);
        }

        [Fact]
        public void ClipValue_PositiveInfinity_ReturnsMax()
        {
            var result = XYZ.ClipValue(double.PositiveInfinity, 0.0, 100.0);
            Assert.Equal(100.0, result);
        }
    }


**src/Vixen.Tests/Data/Value/RGBValueTests.cs**

    using System.Drawing;
    using Vixen.Data.Value;

    namespace Vixen.Tests.Data.Value;

    public class RGBValueTests
    {
        // Grayscale formula: (byte)(R * 0.3 + G * 0.59 + B * 0.11)
        // Cast truncates (does not round):
        //   Black  (0,0,0)       -> 0
        //   White  (255,255,255) -> (byte)(76.5 + 150.45 + 28.05) = 255
        //   Red    (255,0,0)     -> (byte)(76.5) = 76

        [Fact]
        public void GetGrayscaleLevel_Black_Returns0()
        {
            // Arrange
            var color = Color.FromArgb(0, 0, 0);

            // Act
            var level = RGBValue.GetGrayscaleLevel(color);

            // Assert
            Assert.Equal((byte)0, level);
        }

        [Fact]
        public void GetGrayscaleLevel_White_Returns255()
        {
            // Arrange
            var color = Color.FromArgb(255, 255, 255);

            // Act
            var level = RGBValue.GetGrayscaleLevel(color);

            // Assert
            Assert.Equal((byte)255, level);
        }

        [Fact]
        public void GetGrayscaleLevel_Red_Returns76()
        {
            // Arrange — luma = 255*0.3 = 76.5, truncated to byte = 76
            var color = Color.FromArgb(255, 0, 0);

            // Act
            var level = RGBValue.GetGrayscaleLevel(color);

            // Assert
            Assert.Equal((byte)76, level);
        }
    }


## Interfaces and Dependencies

Package versions to add to `C:\Dev\Vixen\Directory.Packages.props`:

    Microsoft.NET.Test.Sdk        17.12.0
    Moq                            4.20.72
    xunit                          2.9.2
    xunit.runner.visualstudio      2.8.2

Public APIs under test with their exact namespaces and signatures:

In `src/Vixen.Core/Utility/NamingUtilities.cs`, namespace `Vixen.Utility`:

    public class NamingUtilities
    {
        public static string Uniquify(HashSet<string> names, string name)
    }

In `src/Vixen.Core/Common/ColorSpaces.cs`, namespace `Common.Controls.ColorManagement.ColorModels`:

    public struct XYZ
    {
        public static double ClipValue(double value, double min, double max)
    }

In `src/Vixen.Core/Data/Value/RGBValue.cs`, namespace `Vixen.Data.Value`:

    public struct RGBValue : IIntentDataType
    {
        public static Color ConvertToGrayscale(Color color)
        public static byte GetGrayscaleLevel(Color color)
    }

`System.Drawing.Color` is available on `net10.0-windows`; the test project receives it transitively through the Vixen.Core project reference. The test project SDK is `Microsoft.NET.Sdk` (not WPF or WinForms). `net10.0-windows` is inherited from `src/Directory.Build.props` via the local `Directory.Build.props` import chain.

### Critical files to create or modify

- `C:\Dev\Vixen\Directory.Packages.props` — add 4 package version entries
- `C:\Dev\Vixen\Vixen.sln` — fix 6 Any CPU → x64 platform entries after dotnet sln add
- `C:\Dev\Vixen\src\Vixen.Tests\Directory.Build.props` — new file
- `C:\Dev\Vixen\src\Vixen.Tests\Vixen.Tests.csproj` — new file
- `C:\Dev\Vixen\src\Vixen.Tests\Utility\NamingUtilitiesTests.cs` — new file
- `C:\Dev\Vixen\src\Vixen.Tests\Common\XYZTests.cs` — new file
- `C:\Dev\Vixen\src\Vixen.Tests\Data\Value\RGBValueTests.cs` — new file
