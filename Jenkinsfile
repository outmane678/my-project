pipeline {
    agent any 

    environment{
        DOTNET = "C:\\Program Files\\dotnet\\dotnet.exe"
        DEPLOY_DIR = "C:\\inetpub\\wwwroot\\MonApp"
        TEMP_DIR = "C:\\Temp\\MonApp"
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

        stage('Stop IIS'){
            steps {
                // Arrête IIS pour libérer web.config
                bat 'iisreset /stop'
                // OU arrêter uniquement le site : 
                // bat 'appcmd stop site /site.name:"MonApp"'
            }
        }

        stage('Publish & Deploy'){
            steps {
                // Publier dans dossier temporaire
                bat '"%DOTNET%" publish -c Release -o "%TEMP_DIR%"'
                // Copier dans wwwroot
                bat 'xcopy /E /Y "%TEMP_DIR%\\*" "%DEPLOY_DIR%\\"'
            }
        }

        stage('Restart IIS'){
            steps {
                bat 'iisreset /start'
                // OU démarrer uniquement le site :
                // bat 'appcmd start site /site.name:"MonApp"'
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