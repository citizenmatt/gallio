@if not defined ECHO (echo off) else (echo %ECHO%)
REM
REM This script installs or uninstalls components.
REM

setlocal
set SRC_DIR=%~dp0
set BIN_DIR=%~dp0..\bin
set GACUTIL=%BIN_DIR%\gacutil.exe
if exist "%ProgramFiles(x86)%" (set GACUTIL40="%ProgramFiles(x86)%\Microsoft SDKs\Windows\v7.0A\Bin\NETFX 4.0 Tools\gacutil.exe") ELSE (set GACUTIL40="%PROGRAMFILES%\Microsoft SDKs\Windows\v7.0A\Bin\NETFX 4.0 Tools\gacutil.exe")
set ELEVATE=%BIN_DIR%\elevate.cmd
set REG=%BIN_DIR%\reg.exe

if "%~1"=="/x" (
    shift
) else (
    call "%ELEVATE%" "%~dpnx0" /x %*
    exit /b %ERRORLEVEL%
)

echo.
echo This script installs or uninstalls components built in the source tree
echo for local debugging purposes.
echo.
echo Choose the component to install or uninstall:
echo   1^) Install Gallio registry keys and plugins, including:
echo         - Reports
echo         - TestDriven.Net runner
echo   2^) Install Visual Studio 2008 addin.  ^(implies 1^)
echo   3^) Install Visual Studio 2010 addin.  ^(implies 1^)
echo.
echo   0^) Uninstall all components.
echo.

:PROMPT
set ANSWER=%~1
if not defined ANSWER set /P ANSWER=Choice? 
echo.

if "%ANSWER%"=="1" call :INSTALL_GALLIO & goto :OK
if "%ANSWER%"=="2" call :INSTALL_VISUALSTUDIO_ADDIN 9 & goto :OK
if "%ANSWER%"=="3" call :INSTALL_VISUALSTUDIO_ADDIN 10 & goto :OK

if "%ANSWER%"=="0" call :UNINSTALL_ALL & goto :OK
goto :PROMPT

:OK
pause
exit /b 0


REM Install Gallio and installable components.
:INSTALL_GALLIO
echo ** Install Gallio. **
echo Adding registry keys.
"%REG%" ADD "HKEY_LOCAL_MACHINE\Software\Gallio.org\Gallio\0.0" /V InstallationFolder /D "%SRC_DIR%Gallio\Gallio" /F >nul
echo.

echo Installing plugins.
call "%SRC_DIR%Gallio.Utility.bat" Setup /install /v:verbose 
echo.
exit /b 0


REM Uninstalls Gallio.
:UNINSTALL_GALLIO
echo ** Uninstall Gallio. **
echo Deleting registry keys.
"%REG%" DELETE "HKEY_LOCAL_MACHINE\Software\Gallio.org\Gallio\0.0" /V InstallationFolder /F 2>nul >nul
echo.

echo Uninstalling plugins.
call "%SRC_DIR%Gallio.Utility.bat" Setup /uninstall /v:verbose 
echo.
exit /b 0


REM Install Visual Studio addin.
:INSTALL_VISUALSTUDIO_ADDIN
call :INSTALL_GALLIO

echo ** Install Visual Studio v%~1.0 Addin **
call :SET_VISUALSTUDIO_ADDIN_VARS "%~1"
if errorlevel 1 exit /b 1

echo Adding registry keys.

REM Register Shell
"%REG%" ADD "%VS_PRODUCT_KEY%" /V Package /D "{9e600ffc-344d-4e6f-89c0-ded6afb42459}" /F >nul
"%REG%" ADD "%VS_PRODUCT_KEY%" /V UseInterface /T REG_DWORD /D "1" /F >nul

"%REG%" ADD "%VS_PACKAGE_KEY%" /VE /D "Gallio Shell Package" /F >nul
"%REG%" ADD "%VS_PACKAGE_KEY%" /V InprocServer32 /D "%SystemRoot%\system32\mscoree.dll" /F >nul
"%REG%" ADD "%VS_PACKAGE_KEY%" /V Class /D "Gallio.VisualStudio.Shell.Core.ShellPackage" /F >nul
"%REG%" ADD "%VS_PACKAGE_KEY%" /V CodeBase /D "%SHELL_BIN_DIR%\Gallio.VisualStudio.Shell%VS_VERSION%0.dll" /F >nul
"%REG%" ADD "%VS_PACKAGE_KEY%" /V ID /T REG_DWORD /D 1 /F >nul
"%REG%" ADD "%VS_PACKAGE_KEY%" /V MinEdition /D "Standard" /F >nul
"%REG%" ADD "%VS_PACKAGE_KEY%" /V ProductVersion /D "3.0" /F >nul
"%REG%" ADD "%VS_PACKAGE_KEY%" /V ProductName /D "Gallio" /F >nul
"%REG%" ADD "%VS_PACKAGE_KEY%" /V CompanyName /D "Gallio Project" /F >nul

"%REG%" ADD "%VS_ROOT_KEY%\AutoLoadPackages\{f1536ef8-92ec-443c-9ed7-fdadf150da82}" /V "{9e600ffc-344d-4e6f-89c0-ded6afb42459}" /T REG_DWORD /D "0" /F >nul

REM Register AddIn
"%REG%" ADD "%VS_ROOT_KEY%\AutomationOptions\LookInFolders" /V "%SHELL_BIN_DIR%" /D "Gallio" /F >nul

REM Register TIP
"%REG%" ADD "%VS_TEST_TYPE_KEY%" /V NameId /D "#100" /F >nul
"%REG%" ADD "%VS_TEST_TYPE_KEY%" /V TipProvider /D "Gallio.VisualStudio.Tip.GallioTipProxy, Gallio.VisualStudio.Tip%VS_VERSION%0.Proxy, Version=0.0.0.0, Culture=neutral, PublicKeyToken=eb9cfa67ee6ab36e" /F >nul
"%REG%" ADD "%VS_TEST_TYPE_KEY%" /V ServiceType /D "Gallio.VisualStudio.Tip.SGallioTestService, Gallio.VisualStudio.Tip%VS_VERSION%0.Proxy, Version=0.0.0.0, Culture=neutral, PublicKeyToken=eb9cfa67ee6ab36e" /F >nul

"%REG%" ADD "%VS_TEST_TYPE_KEY%\Extensions" /V .dll /T REG_DWORD /D "101" /F >nul
"%REG%" ADD "%VS_TEST_TYPE_KEY%\Extensions" /V .exe /T REG_DWORD /D "101" /F >nul

echo Installing Gallio.VisualStudio.Tip.Proxy proxy assembly into GAC.
if %VS_VERSION%==9 ("%GACUTIL%" /i "%PROXY_DLL%" /f) else (%GACUTIL40% /i "%PROXY_DLL%" /f)

call :RUN_VISUALSTUDIO_ADDIN_SETUP
echo.
exit /b 0


REM Uninstall Visual Studio addin.
:UNINSTALL_VISUALSTUDIO_ADDIN
echo ** Uninstall Visual Studio v%~1.0 Addin **
call :SET_VISUALSTUDIO_ADDIN_VARS "%~1"
if errorlevel 1 exit /b 1

echo Deleting registry keys.
"%REG%" DELETE "%VS_TEST_TYPE_KEY%" /F 2>nul >nul
"%REG%" DELETE "%VS_PRODUCT_KEY%" /F 2>nul >nul
"%REG%" DELETE "%VS_PACKAGE_KEY%" /F 2>nul >nul
"%REG%" DELETE "%VS_ROOT_KEY%\AutoLoadPackages\{f1536ef8-92ec-443c-9ed7-fdadf150da82}" /V "{9e600ffc-344d-4e6f-89c0-ded6afb42459}" /F 2>nul >nul
"%REG%" DELETE "%VS_ROOT_KEY%\AutomationOptions\LookInFolders" /V "%SHELL_BIN_DIR%" /F 2>nul >nul

echo Uninstalling Gallio.VisualStudio.Tip.Proxy proxy assembly from GAC.
if %VS_VERSION%==9 ("%GACUTIL%" /u "%PROXY_DLL%" 2>nul >nul) else (%GACUTIL40% /u "%PROXY_DLL%" 2>nul >nul)

call :RUN_VISUALSTUDIO_ADDIN_SETUP
echo.
exit /b 0

REM Helper: Run DEVENV.exe / setup
:RUN_VISUALSTUDIO_ADDIN_SETUP
echo Running devenv.exe /setup.
"%VS_INSTALL_DIR%\devenv.exe" /setup /nosetupvstemplates
exit /b 0


REM Helper: Set Visual Studio addin variables.
:SET_VISUALSTUDIO_ADDIN_VARS
set VS_VERSION=%~1
set VS_ROOT_KEY=HKLM\Software\Microsoft\VisualStudio\%VS_VERSION%.0
set VS_INSTALL_DIR=
for /F "tokens=1,2*" %%V in ('"%REG%" query %VS_ROOT_KEY% /V InstallDir') do (
    if "%%V"=="InstallDir" set VS_INSTALL_DIR=%%X
)
if not exist "%VS_INSTALL_DIR%" (
    echo Visual Studio version %VS_VERSION% was not found!
    exit /b 1
)

set VS_PRIVATE_ASSEMBLIES_DIR=%VS_INSTALL_DIR%\PrivateAssemblies

set VS_TEST_TYPE_KEY=%VS_ROOT_KEY%\EnterpriseTools\QualityTools\TestTypes\{F3589083-259C-4054-87F7-75CDAD4B08E5}
set VS_PRODUCT_KEY=%VS_ROOT_KEY%\InstalledProducts\Gallio
set VS_PACKAGE_KEY=%VS_ROOT_KEY%\Packages\{9e600ffc-344d-4e6f-89c0-ded6afb42459}

set SHELL_BIN_DIR=%SRC_DIR%\Extensions\VisualStudio\Gallio.VisualStudio.Shell\bin\v%VS_VERSION%.0
set PROXY_DLL=%SRC_DIR%\Extensions\VisualStudio\Gallio.VisualStudio.Tip.Proxy\bin\v%VS_VERSION%.0\Gallio.VisualStudio.Tip%VS_VERSION%0.Proxy.dll
exit /b 0


REM Uninstall all.
:UNINSTALL_ALL
call :UNINSTALL_GALLIO
call :UNINSTALL_VISUALSTUDIO_ADDIN 9
call :UNINSTALL_VISUALSTUDIO_ADDIN 10
exit /b 0
