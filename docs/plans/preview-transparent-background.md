# Plan: Transparent Background for Preview Viewer

## Context

The user wants the preview viewer window to optionally render on a fully transparent background so that a desktop application window positioned behind the preview is visible through the "off" (unlit) areas. This allows overlaying a live light show preview on top of another app for monitoring or demonstration purposes.

Both the GDI and OpenGL renderers exist. **OpenGL is the priority** (it is the most commonly used). The feature should work with both renderers; GDI is a lower-priority follow-on.

**Key technical answer: Yes, this is possible on Windows 10/11 via the Win32 Layered Window API (`WS_EX_LAYERED` + `SetLayeredWindowAttributes` with `LWA_COLORKEY`).** The transparency key color (black) makes any pixel rendered with that exact color transparent at the DWM compositor level. Since the preview background is black and lighting pixels are colored, this maps cleanly onto the existing rendering architecture with minimal changes.

> **Important behavior note:** Transparent areas will inherently be click-through — this is an unavoidable Windows API behavior of `LWA_COLORKEY`. Any pixel matching the key color is transparent to both rendering and mouse input. There is no way to have per-area visual transparency without click-through unless a much more complex `UpdateLayeredWindow` per-pixel alpha path is used.

---

## Architecture Overview

Both preview windows are **Windows Forms** (not WPF):
- `GDIPreviewForm` (inherits `BaseForm`) — uses `GDIControl` which renders via a FastPixel bitmap → `DrawImageUnscaled`
- `OpenGlPreviewForm` (inherits `Form`) — uses OpenTK `GLControl` with a VBO/shader pipeline

Neither form currently uses `WS_EX_LAYERED`. No existing transparency infrastructure to reuse.

---

## Implementation Plan

### Task 1 — Add Win32 P/Invoke helpers

Create a small static helper class (e.g., `WinApiTransparency.cs`) in the `VixenPreview` project with:

```csharp
using System.Runtime.InteropServices;

internal static class WinApiTransparency
{
    internal const int GWL_EXSTYLE = -20;
    internal const int WS_EX_LAYERED = 0x00080000;
    internal const int LWA_COLORKEY = 0x00000001;

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
}
```

---

### Task 2 — Add setting, context menu toggle, and window configuration to both forms

Apply identically to **`GDIPreviewForm.cs`** and **`OpenGlPreviewForm.cs`**:

1. Add `private bool _transparentBackground;` field.

2. Add `ConfigureTransparentBackground()`:
   ```csharp
   private void ConfigureTransparentBackground()
   {
       int style = WinApiTransparency.GetWindowLong(Handle, WinApiTransparency.GWL_EXSTYLE);
       if (_transparentBackground)
       {
           WinApiTransparency.SetWindowLong(Handle, WinApiTransparency.GWL_EXSTYLE,
               style | WinApiTransparency.WS_EX_LAYERED);
           WinApiTransparency.SetLayeredWindowAttributes(Handle, 0x000000, 0,
               WinApiTransparency.LWA_COLORKEY);
       }
       else
       {
           WinApiTransparency.SetWindowLong(Handle, WinApiTransparency.GWL_EXSTYLE,
               style & ~WinApiTransparency.WS_EX_LAYERED);
       }
   }
   ```
   Using `SetWindowLong` at runtime (rather than overriding `CreateParams`) avoids the need to recreate the form handle when toggling the setting.

3. Add "Transparent Background" to `HandleContextMenu()` — same pattern as the existing boolean items (checkmark icon when active, click toggles `_transparentBackground`, calls `ConfigureTransparentBackground()` + `SaveWindowState()`).

4. Extend `SaveWindowState()` and `RestoreWindowState()`:
   - Save: `xml.PutSetting(..., "{name}/TransparentBackground", _transparentBackground)`
   - Restore: `_transparentBackground = xml.GetSetting(..., "{name}/TransparentBackground", false)`
   - Call `ConfigureTransparentBackground()` at the end of `RestoreWindowState()`, alongside `ConfigureStatusBar()`, `ConfigureBorders()`, `ConfigureAlwaysOnTop()`.

   GDI form prefix: `Preview_{InstanceId}` — OpenGL form prefix: `OpenGLPreview_{InstanceId}`.

---

### Task 3 — GDI renderer: suppress background image in transparent mode

**`GDIControl.cs`**:
- Add `public bool TransparentBackground { get; set; }` property.
- In `CreateAlphaBackground()`: when `TransparentBackground == true`, skip drawing the background image (fall through to the black-fill branch). The existing `_backgroundAlphaImage` is already filled with `Color.Black` for the no-image path, so no pixel format changes are needed.

**`GDIPreviewForm.cs`** — `Reload()`:
- After setting `gdiControl.BackgroundAlpha` and `gdiControl.Background`, also assign `gdiControl.TransparentBackground = _transparentBackground`.

Because `GDIControl` already fills the frame buffer with black by default, and `LWA_COLORKEY = 0x000000` makes all black pixels transparent at the compositor, no changes to the FastPixel render loop or `OnPaint` are needed. Colored light pixels are unaffected.

---

### Task 4 — OpenGL renderer: suppress background quad and use black clear in transparent mode

**`OpenGlPreviewForm.cs`**:
- In the render loop (`OnRenderFrame` / frame update method), when `_transparentBackground == true`:
  - Call `GL.ClearColor(0f, 0f, 0f, 1f)` (solid black — matches the `LWA_COLORKEY` target color).
  - Skip the `_background.Draw(...)` call (the textured quad that renders the background image or the default black fill; skipping it avoids double-drawing and ensures the background remains pure black for the color key).
- When `_transparentBackground == false`: existing behavior unchanged.

**`Background.cs`** and shaders: no changes needed. The background object is simply not called when transparent mode is active.

**GLControl pixel format**: the `LWA_COLORKEY` approach operates on the DWM-composited window surface, not via framebuffer alpha, so the existing pixel format does not need alpha bits. No changes to `OpenGLPreviewForm.Designer.cs` are required.

---

## Files to Modify

| File | Change |
|---|---|
| New `WinApiTransparency.cs` | P/Invoke declarations for `GetWindowLong`, `SetWindowLong`, `SetLayeredWindowAttributes` |
| `GDIPreview/GDIPreviewForm.cs` | `_transparentBackground` field, `ConfigureTransparentBackground()`, context menu item, save/restore, set `gdiControl.TransparentBackground` in `Reload()` |
| `GDIPreview/GDIControl.cs` | `TransparentBackground` property; skip background image draw when set |
| `OpenGL/OpenGLPreviewForm.cs` | `_transparentBackground` field, `ConfigureTransparentBackground()`, context menu item, save/restore, skip `_background.Draw()` and set black clear in render loop |

No changes needed to `VixenPreviewData`, `VixenPreviewModuleInstance`, `Background.cs`, any shader, or `OpenGLPreviewForm.Designer.cs`.

---

## Known Limitations / Trade-offs

| Limitation | Detail |
|---|---|
| **Transparent areas are click-through** | Platform behavior of `LWA_COLORKEY` — unavoidable without switching to `UpdateLayeredWindow` (much greater complexity) |
| **Black lights become transparent** | Any element at exactly `Color.Black` (zero intensity / fully off) is visually transparent. This is correct behavior — "off" lights should be invisible. |
| **Background image incompatible** | The loaded background image is suppressed in transparent mode. The two modes are mutually exclusive. |
| **OpenGL driver variability** | `LWA_COLORKEY` on a layered window interacts with DWM. On Windows 10/11 (DWM always active) this is reliable. Should be tested on the target machines. |

---

## Verification

1. Build and launch Vixen. Open a sequence. Enable the OpenGL preview.
2. Position a non-Vixen window behind the preview (e.g., a browser).
3. Right-click the preview → enable **Transparent Background** → verify the background becomes transparent and the window behind is visible through the unlit areas.
4. Trigger a sequence with active lights → verify colored light pixels remain fully visible and opaque.
5. Right-click → disable **Transparent Background** → verify the solid black background returns.
6. Close and reopen the preview → verify the setting persisted (saved via `XMLProfileSettings`).
7. Repeat steps 2–6 with the **GDI renderer** (uncheck OpenGL in preview setup).
8. Click in a transparent area → click should pass to the window below. Click on a lit pixel → click is captured by the preview.
