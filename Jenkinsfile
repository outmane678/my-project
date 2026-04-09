pipeline {
    agent any 

    environment{
        DOTNET = "C:\\Program Files\\dotnet\\dotnet.exe"
        DEPLOY_DIR = "C:\\inetpub\\wwwroot\\MonApp" 
    }

    stages {
        stage('Checkout'){   
            steps {
                git branch: 'main', url: 'https://github.com/outmane678/my-project.git'
            }
        }
        stage('Restore'){
            steps {
                bat '"%DOTNET%" restore'
            }
        }
        stage('Build'){
            steps {
                bat '"%DOTNET%" build --configuration Release'
            }

            }

        stage('Test'){
            steps {
                bat '"%DOTNET%" test'
            }
        }

        stage('Publish & Deploy'){
                steps {
                    bat '"%DOTNET%" publish -c Release -o "%DEPLOY_DIR%"'
                }
        }

        stage('Restart IIS'){
            steps {
                bat 'iisreset'
            }
        }
    }
    post{
        success {
            echo 'Build and deployment successful!'
        }
        failure {
            echo 'Build or deployment failed.'
        }
    }
}