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
                  sh 'docker image build -t $JOB_NAME:v1.$BUILD_ID .'
                  sh 'docker image tag $JOB_NAME:v1.$BUILD_ID dockersandheep/$JOB_NAME:v1.$BUILD_ID'
            }
            }
      
    }
}
