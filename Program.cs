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

        if (usersResponse.Users != null && usersResponse.Users.Length > 0)
        {
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
        else
        {
            Console.WriteLine("Could not retrieve user information.");
        }
    }
		
}
