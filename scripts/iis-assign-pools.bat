@echo off
setlocal
set APPCMD=%SystemRoot%\System32\inetsrv\appcmd.exe
if not exist "%APPCMD%" (
  echo ERREUR: IIS appcmd introuvable. Installez IIS + outils de gestion.
  exit /b 1
)

REM Pool dedie pour la racine du site (evite 2 applis Core sur le meme pool que les enfants)
call :addpool MonApp_AC_sitepool
call :addpool MonApp_AC_WebApplication1
call :addpool MonApp_AC_dotnet_app
call :addpool MonApp_AC_user_account

REM Racine MonApp : ne doit pas partager le pool "MonApp" avec les APIs si 500.35
"%APPCMD%" set app "MonApp/" /applicationPool:MonApp_AC_sitepool
if errorlevel 1 (
  echo ERREUR: set app MonApp/ — verifiez que le site s'appelle exactement MonApp dans IIS.
  exit /b 1
)

"%APPCMD%" set app "MonApp/WebApplication1" /applicationPool:MonApp_AC_WebApplication1
if errorlevel 1 (
  echo ERREUR: set app MonApp/WebApplication1
  exit /b 1
)

"%APPCMD%" set app "MonApp/dotnet_app" /applicationPool:MonApp_AC_dotnet_app
if errorlevel 1 (
  echo ERREUR: set app MonApp/dotnet_app
  exit /b 1
)

"%APPCMD%" set app "MonApp/user-account-service" /applicationPool:MonApp_AC_user_account
if not errorlevel 1 goto :userok
"%APPCMD%" set app "MonApp/user_account_service" /applicationPool:MonApp_AC_user_account
if errorlevel 1 (
  echo ERREUR: set app user-account-service ^(essayez alias IIS user-account-service ou user_account_service^)
  exit /b 1
)
:userok

echo Pools IIS OK pour MonApp.
exit /b 0

:addpool
"%APPCMD%" add apppool /name:%~1 /managedRuntimeVersion:"" 2>nul
exit /b 0
