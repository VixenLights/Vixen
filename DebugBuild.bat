::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
::: Build an application with MSBuild
::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

@echo off
setlocal
title %~n0
  
  set MSBUILDPATH=C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319
  set MSBUILD=%MSBUILDPATH%\MSBuild.exe
    
  set SLNFILE=Vixen.msbuild
  
  set LOGGERPARAM=/logger:FileLogger,Microsoft.Build.Engine;logfile="%~n0.log"
  set TARGETPARAM=/target:IntegrationBuild
  set PROPERTYPARAM=/property:Configuration=Debug
  
  %MSBUILD% %SLNFILE% %LOGGERPARAM% %TARGETPARAM% %PROPERTYPARAM%
  
  
  pause
  
:Exit
  endlocal
  goto :EOF