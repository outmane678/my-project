// -----------------------------------------------------------------------------
// Nommage : user_account_service (repo) vs user-account-service (IIS / URL)
// - Dépôt Git : my-project-main/user_account_service/ + namespaces C# user_account_service.*
// - Fichier .csproj : user-account-service.csproj (assembly)
// - Après publish sous MonApp : dossier user-account-service (tirets)
// - URL + PathBase + alias IIS : /user-account-service (tirets uniquement côté web)
// Ne pas créer sous IIS un alias user_account_service si le code utilise PathBase /user-account-service
// -----------------------------------------------------------------------------
pipeline {
    agent any

    // Si le webhook GitHub → Jenkins n’est pas configuré, Jenkins vérifie le dépôt toutes les ~3 min.
    // Déclenchement immédiat au push : GitHub → Settings → Webhooks → URL .../github-webhook/ + cocher le trigger dans le job Jenkins.
    triggers {
        pollSCM('H/3 * * * *')
    }

    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        DEPLOY_DIR = "C:\\inetpub\\wwwroot\\MonApp"
        TEMP_DIR = "C:\\Temp\\MonApp"
    }

    stages {
        stage('Restore') {
            steps {
                bat 'dotnet restore "Projet.sln"'
            }
        }

        stage('Build') {
            steps {
                bat 'dotnet build "Projet.sln" --configuration Release --no-restore'
            }
        }

        stage('Test') {
            steps {
                bat 'dotnet test "Projet.sln" --configuration Release --no-build --logger "console;verbosity=normal" --results-directory "TestResults" --logger "trx;LogFileName=test-results.trx"'
            }
            post {
                always {
                    archiveArtifacts artifacts: 'TestResults/**', allowEmptyArchive: true
                }
            }
        }

        stage('Stop IIS') {
            steps {
                bat 'iisreset /stop'
            }
        }

        stage('Publish & Deploy') {
            steps {
                bat 'if not exist "%TEMP_DIR%" mkdir "%TEMP_DIR%"'
                bat 'dotnet publish "my-project-main\\dotnet_app\\dotnet_app.csproj" -c Release -o "%TEMP_DIR%\\dotnet_app"'
                bat 'dotnet publish "my-project-main\\user_account_service\\user-account-service.csproj" -c Release -o "%TEMP_DIR%\\user-account-service"'
                bat 'dotnet publish "my-project-main\\WebApplication1\\WebApplication1.csproj" -c Release -o "%TEMP_DIR%\\WebApplication1"'
                bat '''
                    if exist "%DEPLOY_DIR%" rd /s /q "%DEPLOY_DIR%"
                    mkdir "%DEPLOY_DIR%"
                    xcopy /E /Y /I "%TEMP_DIR%\\*" "%DEPLOY_DIR%\\"
                    if exist "%DEPLOY_DIR%\\web.config" del /F /Q "%DEPLOY_DIR%\\web.config"
                    if not exist "%DEPLOY_DIR%\\user-account-service\\web.config" (
                        echo ERREUR: publish user-account-service incomplet ^(web.config manquant^)
                        exit /b 1
                    )
                    if not exist "%DEPLOY_DIR%\\dotnet_app\\web.config" (
                        echo ERREUR: publish dotnet_app incomplet
                        exit /b 1
                    )
                    if not exist "%DEPLOY_DIR%\\WebApplication1\\web.config" (
                        echo ERREUR: publish WebApplication1 incomplet
                        exit /b 1
                    )
                '''
            }
        }

        // Evite HTTP 500.35 : une appli ASP.NET Core in-process par pool. Pools séparés = fiable même si web.config ancien.
        stage('IIS app pools (500.35)') {
            steps {
                powershell '''
                    $ErrorActionPreference = "Stop"
                    $appcmd = Join-Path $env:SystemRoot "system32\\inetsrv\\appcmd.exe"
                    if (-not (Test-Path $appcmd)) { throw "appcmd introuvable (IIS non installe ?)" }

                    function Ensure-Pool([string] $name) {
                        & $appcmd list apppool /name.name:$name 2>$null | Out-Null
                        if ($LASTEXITCODE -ne 0) {
                            & $appcmd add apppool /name:$name /managedRuntimeVersion:""
                        }
                    }

                    Ensure-Pool "MonApp_AC_WebApplication1"
                    Ensure-Pool "MonApp_AC_dotnet_app"
                    Ensure-Pool "MonApp_AC_user_account"

                    & $appcmd set app "MonApp/WebApplication1" /applicationPool:MonApp_AC_WebApplication1
                    & $appcmd set app "MonApp/dotnet_app" /applicationPool:MonApp_AC_dotnet_app
                    & $appcmd set app "MonApp/user-account-service" /applicationPool:MonApp_AC_user_account
                '''
            }
        }

        stage('Restart IIS') {
            steps {
                bat 'iisreset /start'
            }
        }
    }

    post {
        success {
            echo 'Build and deployment successful!'
        }
        failure {
            echo 'Build or deployment failed.'
        }
    }
}
