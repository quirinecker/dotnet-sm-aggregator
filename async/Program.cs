// See https://aka.ms/new-console-template for more information

using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Streams.GetStreams;
using System.Diagnostics;
using System.Text.Json;

class Program
{

	private TwitchAPI api = new TwitchAPI();

	static async Task Main(string[] args)
	{
		var program = new Program();
		await program.run();
	}

	async Task run()
	{
		const string clientId = "g79z2bul0nr87ju79sf7jdtc43vbw2";
		const string clientSecret = "ldhb631jod1fstu0vf4fenu7d6ak0o";

		api.Settings.ClientId = clientId;
		api.Settings.Secret = clientSecret;

		var stopWatch = new Stopwatch();

		var topGamesRespose = await api.Helix.Games.GetTopGamesAsync();

		var fetchTasks = topGamesRespose.Data
			.Select(game => api.Helix.Streams.GetStreamsAsync(gameIds: [game.Id], first: 100))
			.ToList();

		await meassure("async", fetchTasks);
	}

	async Task meassure(string choice, List<Task<GetStreamsResponse>> fetchTasks)
	{
		var stopWatch = new Stopwatch();

		stopWatch.Start();
		if (choice == "sync")
		{
			await GetSync(fetchTasks);
		}
		else if (choice == "async")
		{
			var result = await GetAsync(fetchTasks);
			Console.WriteLine(JsonSerializer.Serialize(result));
		}
		stopWatch.Stop();

		Console.Write($"{stopWatch.ElapsedMilliseconds}");
	}

	async Task<GetStreamsResponse[]> GetSync(List<Task<GetStreamsResponse>> fetchTasks)
	{
		var responses = new List<GetStreamsResponse>();
		foreach (var task in fetchTasks)
		{
			responses.Add(await task);
		}

		return responses.ToArray();
	}
	async Task<GetStreamsResponse[]> GetAsync(List<Task<GetStreamsResponse>> fetchTasks)
	{
		var responses = await Task.WhenAll(tasks: fetchTasks);
		return responses;
	}

	void PrintResult(GetStreamsResponse[] responses)
	{
		var count = 0;
		foreach (var response in responses)
		{
			foreach (var stream in response.Streams)
			{
				Console.WriteLine($"({count}) {stream.GameName}: {stream.Title}");
				count++;
			}
			Console.WriteLine("==============================");
		}

		Console.WriteLine($"total: {count}");
	}

}
