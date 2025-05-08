using shared;
using TwitchLib.Api.Helix.Models.Streams.GetStreams;
using TwitchLib.Api;
using System.Text.Json;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
using TwitchLib.Api.Helix.Models.Channels.GetChannelInformation;

public class FetchResult {
	public required GetStreamsResponse[] StreamsResponses;
	public required GetUsersResponse[] UsersResponse;
}

public class FetchStage : Stage<FetchResult?>
{

	public TwitchAPI Api { get; init; }
	public bool IsDryRun { get; init; }

	public FetchStage(TwitchAPI api, bool isDryRun) : base("Fetch")
	{
		Api = api;
		IsDryRun = isDryRun;
	}

	public override async Task<FetchResult?> Run()
	{
		if (IsDryRun && File.Exists("./mock.json"))
		{
			string content = File.ReadAllText("./mock.json");
			return JsonSerializer.Deserialize<FetchResult>(content);
		}

		var topGamesRespose = await Api.Helix.Games.GetTopGamesAsync();

		var fetchStreamTasks = topGamesRespose.Data
			.Select(game => Api.Helix.Streams.GetStreamsAsync(gameIds: [game.Id], first: 100))
			.ToList();

		var streamResult = await Task.WhenAll(tasks: fetchStreamTasks);

		var fetchUserTasks = streamResult.Select(gameStreams => Api.Helix.Users.GetUsersAsync(
			ids: [.. gameStreams.Streams.Select(stream => stream.UserId)]
        ));

		var usersResult = await Task.WhenAll(tasks: fetchUserTasks);

		var fetchResult = new FetchResult {
			StreamsResponses = streamResult,
			UsersResponse = usersResult
		};

		if (!File.Exists("./mock.json")) 
		{
			File.WriteAllText("./mock.json", JsonSerializer.Serialize(fetchResult));
		}

		return fetchResult;
	}

}
