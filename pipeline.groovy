import org.dotnet.ci.pipelines.Pipeline

def windowsPipeline = Pipeline.createPipeline(this, 'build/buildpipeline/windows.groovy')
def linuxPipeline = Pipeline.createPipeline(this, 'build/buildpipeline/linux.groovy')
def osxPipeline = Pipeline.createPipeline(this, 'build/buildpipeline/osx.groovy')
String configuration = 'Release'

windowsPipeline.triggerPipelineOnPush(['Configuration':configuration])
linuxPipeline.triggerPipelineOnPush(['Configuration':configuration])
osxPipeline.triggerPipelineOnPush(['Configuration':configuration])
