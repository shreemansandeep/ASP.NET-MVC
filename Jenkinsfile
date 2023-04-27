pipeline{
  agent any
  
    stages{
      
      stage("Git Checkout"){
        steps{
              git credentialsId: 'GitHubCred', url: 'https://github.com/shreemansandeep/ASP.NET-MVC.git'
        }
      }
      
      stage("Docker Build image and Tag"){
            steps{
                  bat "dotnet restore"
            }
            }
      
    }
}
