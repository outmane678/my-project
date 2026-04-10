pipeline {
    agent any

    environment {
        DOTNET = "C:\\Program Files\\dotnet\\dotnet.exe"
        DEPLOY_DIR = "C:\\inetpub\\wwwroot\\MonApp"
        TEMP_DIR = "C:\\Temp\\MonApp"
    }

    stages {
        stage('Restore') {
            steps {
                bat '"%DOTNET%" restore "Projet.sln"'
            }
        }

        stage('Build') {
            steps {
                bat '"%DOTNET%" build "Projet.sln" --configuration Release --no-restore'
            }
        }

        stage('Test') {
            steps {
                bat '"%DOTNET%" test "my-project-main.Tests\\my-project-main.Tests.csproj" --configuration Release --no-build'
            }
        }

        stage('Stop IIS') {
            steps {
                bat 'iisreset /stop'
            }
        }

        stage('Publish & Deploy') {
            steps {
                bat '''
                    if not exist "%TEMP_DIR%\\dotnet_app" mkdir "%TEMP_DIR%\\dotnet_app"
                    if not exist "%TEMP_DIR%\\user_account_service" mkdir "%TEMP_DIR%\\user_account_service"
                    if not exist "%TEMP_DIR%\\WebApplication1" mkdir "%TEMP_DIR%\\WebApplication1"
                '''
                bat '"%DOTNET%" publish "my-project-main\\dotnet_app\\dotnet_app.csproj" -c Release -o "%TEMP_DIR%\\dotnet_app"'
                bat '"%DOTNET%" publish "my-project-main\\user_account_service\\user-account-service.csproj" -c Release -o "%TEMP_DIR%\\user_account_service"'
                bat '"%DOTNET%" publish "my-project-main\\WebApplication1\\WebApplication1.csproj" -c Release -o "%TEMP_DIR%\\WebApplication1"'
                bat '''
                    if not exist "%DEPLOY_DIR%\\dotnet_app" mkdir "%DEPLOY_DIR%\\dotnet_app"
                    if not exist "%DEPLOY_DIR%\\user_account_service" mkdir "%DEPLOY_DIR%\\user_account_service"
                    if not exist "%DEPLOY_DIR%\\WebApplication1" mkdir "%DEPLOY_DIR%\\WebApplication1"
                    xcopy /E /Y /I "%TEMP_DIR%\\dotnet_app\\*" "%DEPLOY_DIR%\\dotnet_app\\"
                    xcopy /E /Y /I "%TEMP_DIR%\\user_account_service\\*" "%DEPLOY_DIR%\\user_account_service\\"
                    xcopy /E /Y /I "%TEMP_DIR%\\WebApplication1\\*" "%DEPLOY_DIR%\\WebApplication1\\"
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
