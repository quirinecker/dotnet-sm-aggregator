// See https://aka.ms/new-console-template for more information

using TwitchLib.Api.Interfaces;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Streams.GetStreams;
using TwitchLib.Api.Helix.Models.Games;
using TwitchLib.PubSub.Models.Responses;
using System.Diagnostics;

class Program {

    private TwitchAPI api = new TwitchAPI();

	static async Task Main(string[] args) {
        var program = new Program();
        await program.run();
    }	

    async Task run() {
		const string clientId = "g79z2bul0nr87ju79sf7jdtc43vbw2";
		const string clientSecret = "ldhb631jod1fstu0vf4fenu7d6ak0o";

        api.Settings.ClientId = clientId;
		api.Settings.Secret = clientSecret;

		var setupStopWatch = new Stopwatch();
        setupStopWatch.Start();
		var topGamesRespose = await api.Helix.Games.GetTopGamesAsync();
        setupStopWatch.Stop();

        var fetchTasks = topGamesRespose.Data.Select(game => {
            return api.Helix.Streams.GetStreamsAsync(gameIds: [game.Id], first: 100);
        }).ToList();

        var syncStopWatch = new Stopwatch();
        syncStopWatch.Start();
		var syncResults = await GetSync(fetchTasks);
		//PrintResult(await GetSync(fetchTasks));
        syncStopWatch.Stop();
        
		
		var asyncStopWatch = new Stopwatch();
        asyncStopWatch.Start();
		var asyncResults = await GetAsync(fetchTasks);
        //PrintResult(await GetAsync(fetchTasks));
        asyncStopWatch.Stop();

		Console.WriteLine($"{syncResults.Count()}");
		Console.WriteLine($"{asyncResults.Count()}");

		Console.WriteLine($"SetUpTime: {setupStopWatch.ElapsedMilliseconds}");
		Console.WriteLine($"SyncTime: {syncStopWatch.ElapsedMilliseconds}");
		Console.WriteLine($"AsyncTime: {asyncStopWatch.ElapsedMilliseconds}");
    }

	async Task<GetStreamsResponse[]> GetSync(List<Task<GetStreamsResponse>> fetchTasks) {
        var responses = new List<GetStreamsResponse>();
		foreach (var task in fetchTasks)
		{
			responses.Add(await task);
		}

		return responses.ToArray();
	}
	async Task<GetStreamsResponse[]> GetAsync(List<Task<GetStreamsResponse>> fetchTasks) {
        var responses = await Task.WhenAll(tasks: fetchTasks);
		return responses;
	}

    void PrintResult(GetStreamsResponse[] responses) {
        var count = 0;
        foreach (var response in responses) {
            foreach (var stream in response.Streams) {
                Console.WriteLine($"({count}) {stream.GameName}: {stream.Title}");
                count++;
            }
            Console.WriteLine("==============================");
        }

        Console.WriteLine($"total: {count}");
    }

}
