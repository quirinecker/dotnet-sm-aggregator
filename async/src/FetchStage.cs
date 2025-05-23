using shared;
using TwitchLib.Api.Helix.Models.Streams.GetStreams;
using TwitchLib.Api;
using System.Text.Json;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
using TwitchLib.Api.Helix.Models.Channels.GetChannelInformation;

public class FetchResult
{
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
			return new FetchResult
			{
				StreamsResponses = JsonSerializer.Deserialize<GetStreamsResponse[]>(streamsContent),
				UsersResponse = JsonSerializer.Deserialize<GetUsersResponse[]>(usersContent)
			};
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

		var fetchResult = new FetchResult
		{
			StreamsResponses = streamResult,
			UsersResponse = usersResult
		};

		if (!(File.Exists("./streams.json") || File.Exists("./users.json")))
		{
			File.WriteAllText("./streams.json", JsonSerializer.Serialize(fetchResult.StreamsResponses));
			File.WriteAllText("./users.json", JsonSerializer.Serialize(fetchResult.UsersResponse));
		}


		return fetchResult;
	}

}
