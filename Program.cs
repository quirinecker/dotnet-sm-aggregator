// See https://aka.ms/new-console-template for more information

using TwitchLib.Api.Interfaces;
using TwitchLib.Api;

class Program {
	static async Task Main(string[] args) {
		const string clientId = "g79z2bul0nr87ju79sf7jdtc43vbw2";
		const string clientSecret = "ldhb631jod1fstu0vf4fenu7d6ak0o";
		var twitchChannels = new List<string> {"papaplatte"};


		ITwitchAPI api = new TwitchAPI();
		api.Settings.ClientId = clientId;
		api.Settings.Secret = clientSecret;

		var usersResponse = await api.Helix.Users.GetUsersAsync(logins: twitchChannels);

		var gamesResponse = await api.Helix.Games.GetGamesAsync(gameNames: new List<string> { "Minecraft" });

		var topGamesRespose = await api.Helix.Games.GetTopGamesAsync();

        if (usersResponse.Users == null || usersResponse.Users.Length == 0) {
            Console.WriteLine("Could not retrieve user information.");
            return;
        }

        var gameIds = topGamesRespose.Data.Select(game => game.Id);
        var fetchTasks = new List<Task<TwitchLib.Api.Helix.Models.Streams.GetStreams.GetStreamsResponse>>();
        
		foreach (var game in topGamesRespose.Data)
		{
            fetchTasks.Add(api.Helix.Streams.GetStreamsAsync(gameIds: [game.Id], first: 100));
		}

        var responses = await Task.WhenAll(tasks: fetchTasks);

        if (responses == null) {
            Console.WriteLine("something went wrong oder so");
            return; 
        }

        var count = 0;
        foreach (var response in responses) {
            foreach (var stream in response.Streams) {
                Console.WriteLine($"({count}) {stream.GameName}: {stream.Title}");
                count++;
            }
            Console.WriteLine("==============================");
        }

        Console.WriteLine($"total: {count}");
        
        var papaplatteUser = usersResponse.Users.FirstOrDefault(u => u.Login.ToLower() == "papaplatte");

        if (papaplatteUser != null)
        {
            string userId = papaplatteUser.Id;
            // Get channel information using the user ID (broadcaster ID)
            var channelInfoResponse = await api.Helix.Channels.GetChannelInformationAsync(broadcasterId: userId);
            Console.WriteLine("\nChannel Information:");
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(channelInfoResponse.Data, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
        }
        else
        {
            Console.WriteLine($"Could not find user information for channel: papaplatte");
        }
    }	
}
