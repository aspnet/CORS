import org.dotnet.ci.pipelines.Pipeline

def windowsPipeline = Pipeline.createPipeline(this, 'windows.groovy')
def linuxPipeline = Pipeline.createPipeline(this, 'linux.groovy')
def osxPipeline = Pipeline.createPipeline(this, 'osx.groovy')
String configuration = 'Release'

[windowsPipeline, linuxPipeline, osxPipeline].each { pipeline ->
    pipeline.triggerPipelineOnEveryGithubPR(['Configuration':configuration])
    pipeline.triggerPipelineOnGithubPush(['Configuration':configuration])
}
