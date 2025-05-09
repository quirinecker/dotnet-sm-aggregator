using shared;
using TwitchLib.Api.Helix.Models.Streams.GetStreams;
using TwitchLib.Api;
using System.Text.Json;
using TwitchLib.Api.Helix.Models.Users.GetUsers;

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
		if (IsDryRun && File.Exists("./streams.json") && File.Exists("./users.json"))
		{
			string streamsContent = File.ReadAllText("./streams.json");
			string usersContent = File.ReadAllText("./users.json");
			return new FetchResult {
				StreamsResponses = JsonSerializer.Deserialize<GetStreamsResponse[]>(streamsContent),
				UsersResponse = JsonSerializer.Deserialize<GetUsersResponse[]>(usersContent)
			};
		}

		var topGamesRespose = await Api.Helix.Games.GetTopGamesAsync();

		var fetchStreamTasks = topGamesRespose.Data
			.Select(game => Api.Helix.Streams.GetStreamsAsync(gameIds: [game.Id], first: 100))
			.ToList();

		var streamsResult = new List<GetStreamsResponse>();

		foreach (var task in fetchStreamTasks) {
			streamsResult.Add(await task);
		}

		var fetchUserTasks = streamsResult.Select(gameStreams => Api.Helix.Users.GetUsersAsync(
			ids: [.. gameStreams.Streams.Select(stream => stream.UserId)]
        ));

		var usersResult = new List<GetUsersResponse>();

		foreach (var task in fetchUserTasks) {
			usersResult.Add(await task);
		}

		var fetchResult = new FetchResult {
			StreamsResponses = streamsResult.ToArray(),
			UsersResponse = usersResult.ToArray()
		};

		if (!(File.Exists("./streams.json") || File.Exists("./users.json")))
		{
			File.WriteAllText("./streams.json", JsonSerializer.Serialize(fetchResult.StreamsResponses));
			File.WriteAllText("./users.json", JsonSerializer.Serialize(fetchResult.UsersResponse));
		}

		return fetchResult;
	}

}
