@echo off

if [%1] == [] (
  set TARGET=
) else (
  set TARGET=-Target %1
)

powershell.exe -File .\build.ps1 --settings_skipverification=true %TARGET%
