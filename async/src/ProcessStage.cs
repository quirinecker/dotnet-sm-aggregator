using shared;
using TwitchLib.Api.Helix.Models.Streams.GetStreams;

public class ProcessResult 
{
    required public (string, long)[] avgGameViewer;


}


public class ProcessStage(FetchResult FetchedData) : Stage<ProcessResult>("process")
{
    public async override Task<ProcessResult> Run()
    {

        

        return new ProcessResult 
        {
            avgGameViewer = [("lol", 0)]
        };
    }
}