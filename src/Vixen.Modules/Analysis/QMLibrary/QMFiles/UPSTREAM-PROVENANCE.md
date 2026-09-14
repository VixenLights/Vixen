# Embedded Bar/Beat source provenance

This document records the exact upstream material evaluated for the Vixen
Bar/Beat tracker update. It is intentionally kept beside the vendored
`QMFiles` payload so a future update can reproduce the source selection.

## Staged sources

The sources were staged outside the repository at
`C:\Temp\vamp-qm-upgrade` on 2026-09-14. No staging directory is used by the
Vixen build.

- VAMP Plugin SDK: `https://github.com/vamp-plugins/vamp-plugin-sdk.git`, tag
  `vamp-plugin-sdk-v2.10`, commit
  `67adfc2bf9486912a0fce5123cf54360ea2678bc`. Its `COPYING` file grants a
  permissive MIT-style license and also carries the embedded KissFFT notice.
- QM VAMP Plugins: `https://github.com/c4dm/qm-vamp-plugins.git`, tag
  `qm-vamp-plugins-v1.8.0`, commit
  `8f3c51f96eb5fb2d576d1cd6cdab3449d50c3225`. Its `COPYING` file is GNU GPL
  version 2.
- QM DSP: `https://github.com/c4dm/qm-dsp.git`, the exact `qm-dsp` lock-file
  revision `4d2a4a4e0c2dd0ddf07bf9d2a224ded912712714`. Its `COPYING` file is
  GNU GPL version 2.
- KissFFT: the `ext/kissfft` subtree of that QM DSP revision. Its `COPYING`
  file is the three-clause BSD license.

QM 1.8.0's `repoint-lock.json` additionally names a 12-character
Mercurial pin, `c42e50a5c297`, for VAMP. The Vixen update instead stages the
explicit VAMP 2.10 release tag above, as required by the update scope. The
`repoint.bat install` helper could not run in the staging environment because
neither supported Standard ML runtime (`sml` or `polyml`) was installed.
The exact QM DSP lock revision was therefore cloned directly. This is a
mechanical equivalent for that Git dependency; it is not a substitute for an
unpinned upstream branch.

When the sources are imported, retain their upstream `COPYING` files with the
corresponding material and preserve this notice. Review GPL distribution
obligations before shipping the resulting application.

## Bar/Beat direct-source inventory

`plugins/BarBeatTrack.cpp` from QM 1.8.0 has this direct compile closure. The
first group are the C++ translation units that must be compiled; the second
group are compiled C implementation units required by QM DSP's FFT wrapper.

    plugins/BarBeatTrack.cpp
    dsp/onsets/DetectionFunction.cpp
    dsp/onsets/PeakPicking.cpp
    dsp/phasevocoder/PhaseVocoder.cpp
    dsp/rateconversion/Decimator.cpp
    dsp/signalconditioning/DFProcess.cpp
    dsp/signalconditioning/Filter.cpp
    dsp/signalconditioning/FiltFilt.cpp
    dsp/tempotracking/DownBeat.cpp
    dsp/tempotracking/TempoTrackV2.cpp
    dsp/transforms/FFT.cpp
    maths/MathUtilities.cpp
    ext/kissfft/kiss_fft.c
    ext/kissfft/tools/kiss_fftr.c
    VAMP Plugin SDK src/vamp-sdk/RealTime.cpp

Headers needed by this closure include the matching `base`, `dsp`, and `maths`
directories from QM DSP; `plugins/BarBeatTrack.h`; VAMP's `vamp-sdk` headers;
and KissFFT's `kiss_fft.h`, `_kiss_fft_guts.h`, and `tools/kiss_fftr.h`.
`PeakPicking.h` is included by BarBeatTrack but is not directly instantiated;
its implementation remains part of the QM DSP closure because
`DetectionFunction` depends on it.

The imported project must define `_USE_MATH_DEFINES` and
`kiss_fft_scalar=double`, and add the QM DSP root plus both KissFFT include
directories. The current Vixen project already defines `_USE_MATH_DEFINES`.

The full upstream `QMVampPlugins.vcxproj` links `qm-dsp.lib` and OpenBLAS, but
that project builds every QM plugin. The direct Bar/Beat closure above does not
reference BLAS or OpenBLAS symbols.

## Native compile probe

On 2026-09-14, the source list above plus a native program that constructs
`BarBeatTracker(44100.0f)` was compiled and linked as an x64 executable with
Visual Studio 18's C++ toolchain. It used `/D_USE_MATH_DEFINES` and
`/Dkiss_fft_scalar=double`, then exited successfully after confirming the
plugin identifier is `qm-barbeattracker`. This proves the narrow
direct-source strategy remains viable and does not require an OpenBLAS binary.
