@echo off
set folder=%~dp0
shift
"%folder%\..\python.exe" -m easyinstall %*