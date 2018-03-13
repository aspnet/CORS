@Library('dotnet-ci') _

simpleNode('OSX10.12','latest') {
    stage ('Checking out source') {
        checkout scm
    }
    stage ('Build') {
        sh 'export DOTNET_CLI_TELEMETRY_OPTOUT=true; export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=true; ./build.sh'
    }
}
