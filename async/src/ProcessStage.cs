using System.Collections;
using System.Text.Json;
using Microsoft.VisualBasic;
using shared;
using TwitchLib.Api.Helix.Models.Streams.GetStreams;
using TwitchLib.Api.Helix.Models.Users.Internal;
using TwitchStream = TwitchLib.Api.Helix.Models.Streams.GetStreams.Stream;
using TwitchUser = TwitchLib.Api.Helix.Models.Users.GetUsers.User;

public class StreamProcessResult 
{

    // --- Data for specific game ---
    required public (string, long, Dictionary<string, long>)[] gameViewer {get; set;} //game, viewer per game (how many people are watching this game in total)
    // required public (string, long)[] avgGameViewer; //game, viewers (oben durch die games)
    required public Dictionary<string, (TwitchStream, double)> gameStreamTime; //how long is the game streamed in total 

    required public long matureContent;

    required public long streamCount;
    

}

public class UserProcessResult
{
    required public long[] userTypeDistribution; //Twtich Admin, global mod,... (4)
    required public long[] streamerTypeDistribution; //partner, affiliate, nothing

}

public class ProcessResult
{
    required public StreamProcessResult streamResult;
    required public UserProcessResult userResult;
}


public class ProcessStage(FetchResult FetchedData) : Stage<ProcessResult?>("process")
{
    public async override Task<ProcessResult?> Run()
    {
        var streamResults = await Task.WhenAll(FetchedData.StreamsResponses.Select(gameStreams => computeStreams(gameStreams.Streams)));
        var userResults = await Task.WhenAll(FetchedData.UsersResponse.Select(gameUsers => computeUsers(gameUsers.Users)));
        var firstStream = streamResults.First();
        var firstUser = userResults.First();
        if (firstStream == null) {
            return null;
        }

        var streamProcessResults = streamResults.Skip(1).Aggregate(firstStream, (acc, result) => {
            foreach(var entry in result.gameStreamTime.AsEnumerable()) {
                acc.gameStreamTime.Add(entry.Key, entry.Value);
            }

            return new StreamProcessResult {
                gameViewer = acc.gameViewer.Concat(result.gameViewer).ToArray(),
                gameStreamTime = acc.gameStreamTime,
                streamCount = acc.streamCount + result.streamCount,
                matureContent = acc.matureContent + result.matureContent
            };
        });

        var userProcessResult = userResults.Skip(1).Aggregate(firstUser, (acc, result) => {
            return new UserProcessResult {
                streamerTypeDistribution = acc.streamerTypeDistribution.Zip(result.streamerTypeDistribution, (a, b) => a + b).ToArray(), 
                userTypeDistribution = acc.userTypeDistribution.Zip(result.userTypeDistribution, (a,b) => a + b).ToArray()
            };
        });

        return new ProcessResult {
            streamResult = streamProcessResults,
            userResult = userProcessResult
        };
    }

    private async Task<StreamProcessResult?> computeStreams(TwitchStream[] streams)
    {
        // It's good practice to simulate async work if the method is marked async
        // but doesn't have any await calls. Or simply return Task.FromResult if it's purely synchronous.
        // However, for simplicity in this transformation, we can just let the compiler handle it.

        if (streams == null || !streams.Any())
        {
            // Decide how to handle empty streams. Return null, an empty ProcessResult, or throw.
            // For aggregation, returning an empty ProcessResult might be better than null.
            return new StreamProcessResult { 
                gameViewer = Array.Empty<(string, long, Dictionary<String, long>)>(),
                gameStreamTime = [],
                streamCount = 0,
                matureContent = 0
            };
        }

        // Simulate some processing if needed, or directly compute
        // For Task.Run, you'd wrap the synchronous part.
        // For a simple async method, you can just compute and return.
        // If there were actual async operations here (e.g., another API call), you would await them.

        await Task.Yield(); // Ensures it behaves as an async method, yielding control briefly.
                        // Or use Task.CompletedTask if no real async work after this point.

        var viewerCount = (long)streams.Sum(stream => stream.ViewerCount);
        var gameName = streams.First().GameName; // Assumes streams is not empty due to the check above
        

        var languagePerGame = new Dictionary<string, long>();
        var streamTime = new Dictionary<string, (TwitchStream, double)>();
        var streamCount = 0;
        var matureCount = 0;

        foreach (var stream in streams) {
            if (languagePerGame.ContainsKey(stream.Language)) {
                languagePerGame[stream.Language] = ++languagePerGame[stream.Language];
            } else {
                languagePerGame[stream.Language] = 1;
            }

            if (!streamTime.ContainsKey(stream.UserName)) {
                var streamDuration = DateTime.Now - stream.StartedAt;
                streamTime[stream.UserName] = (stream, streamDuration.TotalHours - 2);
            }

            if (stream.IsMature) {
                matureCount++;
            }

            streamCount++;
        }


        return new StreamProcessResult
        {
            gameViewer = [(gameName, viewerCount, languagePerGame)],
            gameStreamTime = streamTime,
            streamCount = streamCount,
            matureContent = matureCount
            // Initialize other fields as needed
        };
    }   

    private async Task<UserProcessResult?> computeUsers(TwitchUser[] users)
    {
        if (users == null || !users.Any())
        {
            return new UserProcessResult { 
                userTypeDistribution = [],
                streamerTypeDistribution = []

            };
        }
        await Task.Yield();

        var twitchAdminCount = 0;
        var globalModCount = 0;
        var twitchStaffCount = 0;
        var normalUserCount = 0;

        var affiliateCount = 0;
        var partnerCount = 0;
        var normalStreamerCount = 0;

        foreach(var user in users) {
            if (user.Type == "admin" ) {
                twitchAdminCount++;
            } else if ( user.Type =="global_mod") {
                globalModCount++;
            } else if (user.Type == "staff") {
                twitchStaffCount++;
            } else {
                normalUserCount++;
            }

            if (user.BroadcasterType == "affiliate") {
                affiliateCount++;
            } else if (user.BroadcasterType == "partner") {
                partnerCount++;
            } else {
                normalStreamerCount++;
            }
        }

        return new UserProcessResult
        {
            userTypeDistribution = [twitchAdminCount, globalModCount, twitchStaffCount, normalUserCount],
            streamerTypeDistribution = [affiliateCount, partnerCount, normalStreamerCount]
        };
    }
}