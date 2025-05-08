using TwitchLib.Api;
using System.Text.Json;

class Program
{

	private TwitchAPI api = new TwitchAPI();
	private bool isDryRun = false;

	static async Task Main(string[] args)
	{
		var program = new Program
		{
			isDryRun = args.Contains("--dry-run")
		};

		await program.run();
	}

	async Task run()
	{
		SetCredentials();

		var fetchStage = new FetchStage(api, true);
		var (fetchTime, fetchResult) = await fetchStage.Meassure();


		if (fetchResult is null)
		{
			Console.WriteLine("Fetching failed");
			return;
		}

		//Console.WriteLine(fetchResult.UsersResponse);
		//Console.WriteLine(fetchResult.StreamsResponses);
		Console.WriteLine(JsonSerializer.Serialize(fetchResult.StreamsResponses.Take(1)));
		Console.WriteLine(JsonSerializer.Serialize(fetchResult.UsersResponse.Take(1)));

		var processStage = new ProcessStage(fetchResult);
	}

	void SetCredentials()
	{
		const string clientId = "g79z2bul0nr87ju79sf7jdtc43vbw2";
		const string clientSecret = "ldhb631jod1fstu0vf4fenu7d6ak0o";

		api.Settings.ClientId = clientId;
		api.Settings.Secret = clientSecret;
	}
}
