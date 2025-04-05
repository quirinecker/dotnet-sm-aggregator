// See https://aka.ms/new-console-template for more information

using TwitchLib.Api.Interfaces;
using TwitchLib.Api;

class Program {
	static async Task Main(string[] args) {
		const string clientId = "";
		const string clientSecret = "";
		var twitchChannels = new List<string> { "FunkyGamer_12" };


		ITwitchAPI api = new TwitchAPI();
		api.Settings.ClientId = clientId;
		api.Settings.Secret = clientSecret;

		var response = await api.Helix.Users.GetUsersAsync(logins: twitchChannels);

		if (response.Users.Count() < 1) {
			Console.WriteLine("No users found");
		}

		var firstResult = response.Users.First();
		var email = firstResult.Email == null ? "no email" : firstResult.Email;
		Console.WriteLine($"Found {firstResult.Id}");

		var followers = await api.Helix.Users.GetUsersFollowsAsync(toId: firstResult.Id, first: 1);

		Console.WriteLine($"{twitchChannels.First()} has {followers.Follows.Count()} followers: ");
		foreach (var follower in followers.Follows) {
			Console.WriteLine($"{follower.FromUserName}");
		}
	}
}
