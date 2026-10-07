# Technical Specification: VIX-3983 - Marks Bar Wheel Navigation Parity

**Target File Path:** `docs\sequencer\vix-3983-marks-bar-wheel-navigation.md`  
**Status:** Approved for Execution Planning

## 1. Refined Requirements

- **Functional Overview:** When the pointer is over the Marks Bar, horizontal panning and horizontal zooming must feel identical to performing the same gesture over the timeline grid. The implementation must produce the same visible viewport changes, use the same shared timeline state, and preserve existing Mark drag, resize, and auto-scroll behavior.

- **Detailed Requirements List:**
  - Handle the Marks Bar's inherited `MouseWheel` and `MouseHWheel` inputs; the latter is the existing native horizontal-wheel route provided by `TimelineControlBase`.
  - Route only horizontal pan and horizontal zoom behavior from the Marks Bar. Do not forward the full event to `TimelineControl.OnMouseWheel`.
  - Shift-wheel, including Ctrl+Shift-wheel and Shift+Alt-wheel, pans horizontally by the same amount and in the same direction as the timeline grid.
  - Ctrl-wheel without Shift zooms the horizontal timeline axis by the same scale and with the same pointer anchoring as the timeline grid.
  - Native horizontal-wheel input pans horizontally by the same amount and in the same direction as the timeline grid.
  - Plain vertical wheel input over the Marks Bar must not newly enable vertical scroll. Ctrl+Alt row-height adjustment and Ctrl+Shift row-height adjustment must not be enabled over the Marks Bar.
  - Preserve the existing `TimelineControl.OnMouseWheel` behavior unless a focused implementation investigation proves it must be changed to attain the approved user-visible parity.
  - Do not modify `src/Vixen.Common/Controls/TimeLineControl/MarksBar.cs`.
  - Do not duplicate pan calculations with `MarksBar.VisibleTimeSpan`; all viewport work belongs to `TimelineControl` and uses its `VisibleTimeSpan`.
  - Do not add explicit viewport clamping. `TimelineControlBase.VisibleTimeStart` already clamps to zero and to `max(TotalTime - VisibleTimeSpan, 0)`.
  - Use tabs and LF line endings. No public or protected APIs, serialized data, configuration, timers, async work, WPF code, or Catel ViewModel code may be added.

- **Data Model & State Changes:** No persistent state changes are required. `TimelineControl`, `Grid`, `MarksBar`, `Ruler`, and `Waveform` already share one `TimeInfo`; changing `TimelineControl.VisibleTimeStart` or zoom state refreshes the timeline surfaces through that shared object.

## 2. Technical Architecture & Impact

- **Implementation Strategy:** `TimelineControl` owns the navigation policy. It will subscribe to both relevant inherited Marks Bar events during panel construction and unsubscribe before disposing the Marks Bar. Small, dedicated handlers will inspect only the needed gesture information and call shared TimelineControl pan/zoom helpers. If pointer-relative zoom is enabled, the handler will translate the Marks Bar event point into TimelineControl coordinates before using the established `ZoomTime` logic.

- **Mathematical / Logical Formulas:**
  - Pan displacement is `VisibleTimeSpan.Scale(-(delta / 1200.0))` for the standard vertical-wheel pan route.
  - Existing native horizontal-wheel pan uses `VisibleTimeSpan.Scale(0.10)` for positive delta and `VisibleTimeSpan.Scale(-0.10)` for negative delta. The Marks Bar must call the same shared helper so both surfaces stay aligned.
  - Standard Ctrl zoom uses `1.0 - delta / 1200.0`, and must retain the current `ZoomToMousePosition` choice between `ZoomTime` and `Zoom`.
  - Shift takes precedence over Ctrl for Marks Bar vertical-wheel gesture selection: Shift combinations pan, while Ctrl without Shift zooms.

- **Component Impact Matrix:**

  | Component | Change | Runtime effect |
  |---|---|---|
  | `TimelineControl.cs` | Event subscription lifecycle and shared pan/zoom routing helpers. | Marks Bar receives matching navigation behavior. |
  | `TimelineControlBase.cs` | No intended code change; supplies `MouseHWheel`. | Existing native horizontal-wheel message support remains the common event source. |
  | `Grid_Mouse.cs` | May be refactored only to use the shared native-horizontal pan helper. | Grid and Marks Bar retain equal movement math. |
  | `MarksBar.cs` | No change. | Existing mark interactions remain intact. |
  | `MarksBarMouseWheelTests.cs` | New focused unit tests. | Guards gesture dispatch, proportional movement, zoom, and bounds. |

## 3. Acceptance Criteria

- Given the pointer is over either the grid or the Marks Bar, when Shift-wheel is used with a negative or positive delta, then both surfaces produce the same horizontal `VisibleTimeStart` displacement and bounds behavior.
- Given Ctrl+Shift-wheel or Shift+Alt-wheel is used over either surface, when the wheel is moved, then both surfaces pan and do not change row height.
- Given Ctrl-wheel without Shift is used over either surface, when the wheel is moved, then both surfaces apply the same horizontal zoom scale and the same pointer-relative anchoring when that option is enabled.
- Given a native horizontal-wheel message over either surface, when it has positive or negative delta, then both surfaces pan by the same amount and direction.
- Given plain wheel or Ctrl+Alt wheel over the Marks Bar, when the wheel is moved, then the Marks Bar does not start vertical scrolling or row-height changes.
- Given the viewport is at either horizontal boundary, when panning would go beyond it, then the existing `VisibleTimeStart` property clamps it to zero or the latest possible visible start.
- Given a partial or high-resolution wheel delta, when vertical Shift-wheel pans, then movement remains proportional to `delta / 1200.0`.
- Given the timeline is disposed, when Marks Bar disposal begins, then every event subscribed by TimelineControl is detached first.

## 4. Test Plan

- **Automated Testing Strategy:** Create `src/Vixen.Tests/Sequencer/MarksBarMouseWheelTests.cs` using `[Collection(TimelineControlTestCollection.Name)]`. Expose only the narrow internal test seams required to invoke Marks Bar-equivalent pan and zoom dispatch without synthesizing WinForms input. Cover Shift `-120` and `+120`, Shift+Alt, Ctrl+Shift, Ctrl-only zoom, no modifiers, native horizontal-wheel positive and negative deltas, left and right bounds, and a partial/high-resolution delta. Assert the resulting viewport or zoom state against the equivalent grid helper behavior rather than duplicating formula expectations in multiple tests.

- **Manual / Verification Testing:** Open a long sequence and compare navigation while the pointer is over the grid and over the Marks Bar. Test Shift, Ctrl, Ctrl+Shift, Shift+Alt, and a mouse tilt wheel if available. Confirm marks can still be dragged and resized, and repeat the VIX-3944 Marks Bar auto-scroll regression scenario.

- **Performance & Regression Boundaries:** The handlers must only route a small number of event values to existing synchronous state changes. They must not allocate long-lived state, start background work, or duplicate event subscriptions. Build `Vixen_Tests` using full x64 MSBuild, run focused `MarksBarMouseWheelTests` with `dotnet test --no-build`, then run the complete `Vixen.Tests` suite.

Is this specification approved? Once approved, we can proceed to trigger the code execution plan (execplan).
