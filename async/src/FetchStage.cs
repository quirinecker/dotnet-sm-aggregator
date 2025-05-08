using shared;
using TwitchLib.Api.Helix.Models.Streams.GetStreams;
using TwitchLib.Api;
using System.Text.Json;

public class FetchStage : Stage<GetStreamsResponse[]?>
{

	public TwitchAPI Api { get; init; }
	public bool IsDryRun { get; init; }

	public FetchStage(TwitchAPI api, bool isDryRun) : base("Fetch")
	{
		Api = api;
		IsDryRun = isDryRun;
	}

	public override async Task<GetStreamsResponse[]?> Run()
	{
		if (IsDryRun && File.Exists("../shared/mock.json"))
		{
			string content = File.ReadAllText("../shared/mock.json");
			return JsonSerializer.Deserialize<GetStreamsResponse[]>(content);
		}

		var topGamesRespose = await Api.Helix.Games.GetTopGamesAsync();

		var fetchTasks = topGamesRespose.Data
			.Select(game => Api.Helix.Streams.GetStreamsAsync(gameIds: [game.Id], first: 100))
			.ToList();

		var result = await Task.WhenAll(tasks: fetchTasks);

		if (!File.Exists("../shared/mock.json")) {
			File.WriteAllText("../shared/mock.json", JsonSerializer.Serialize(result));
		}

		return result;
	}

}
