@Library('dotnet-ci') _

simpleNode('Ubuntu16.04', 'latest-or-auto-docker') {
    stage ('Checking out source') {
        checkout scm
    }
    stage ('Build') {
        sh 'export DOTNET_CLI_TELEMETRY_OPTOUT=true; export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=true; ./build.sh'
    }
}
