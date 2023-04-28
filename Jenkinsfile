pipeline{
  agent any
  
    stages{
      
      stage("Git Checkout"){
        steps{
              git credentialsId: 'GitHubCred', url: 'https://github.com/shreemansandeep/ASP.NET-MVC.git'
        }
      }
      
      stage('Restore packages'){
   steps{
      bat "dotnet restore"
     }
  }
  
   
   stage('Build'){
   steps{
      bat "dotnet build"
    }
 }
 
 stage('Test: Unit Test'){
   steps {
     bat "dotnet test"
     }
  }
       
   
   stage('Publish'){
     steps{
       bat "dotnet publish"
     }
}
      
    }
}
