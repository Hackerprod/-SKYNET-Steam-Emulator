@echo off
setlocal

set "ROOT=%~dp0"
set "OUT=%ROOT%Publish"

echo === Building SKYNET server ===
dotnet publish "%ROOT%SKYNET server\SKYNET server.csproj" -c Release -r win-x64 --self-contained true -o "%OUT%\Server"
if errorlevel 1 goto :error

echo === Building SKYNET Steam Client ===
dotnet publish "%ROOT%SKYNET Steam Client\SKYNET Steam Client.csproj" -c Release -o "%OUT%\Client"
if errorlevel 1 goto :error

echo.
echo Done. Output in "%OUT%"
goto :eof

:error
echo.
echo Build failed.
exit /b 1
