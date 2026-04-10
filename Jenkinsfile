pipeline {
    agent any

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
