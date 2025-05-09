using TwitchLib.Api;

class Program
{

	private TwitchAPI api = new TwitchAPI();
	private bool isDryRun = false;
	private bool noPrint = false;

	static async Task Main(string[] args)
	{
		var program = new Program
		{
			isDryRun = args.Contains("--dry-run"),
			noPrint = args.Contains("--no-print"),

		};

		await program.run();
	}

	async Task run()
	{
		SetCredentials();

		var fetchStage = new FetchStage(api, isDryRun);
		var (fetchTime, fetchResult) = await fetchStage.Meassure();


		if (fetchResult is null)
		{
			Console.WriteLine("Fetching failed");
			return;
		}

		var processStage = new ProcessStage(fetchResult);
		var (processTime, processResult) = await processStage.Meassure();
		//var processStreamResult = processResult.streamResult;
		//var processUserResult = processResult.userResult;

		if (!this.noPrint) {
			var printStage = new PrintStage(processResult);
			await printStage.Run();
		}

		// Console.WriteLine(processResult.gameStreamTime.Count());
		// foreach(var result in processResult.gameStreamTime.AsEnumerable())  {
		// 	Console.WriteLine($"{result.Key}: {result.Value.Item2}");
		// }
		//Console.WriteLine(processStreamResult.streamCount);
		//Console.WriteLine(processStreamResult.matureContent);

		//Console.WriteLine($"{(double) processStreamResult.matureContent / processStreamResult.streamCount}");
		//Console.WriteLine(JsonSerializer.Serialize(processUserResult.streamerTypeDistribution));
		//Console.WriteLine(JsonSerializer.Serialize(processUserResult.userTypeDistribution));

		Console.WriteLine($"{fetchTime},{processTime}");
	}

	void SetCredentials()
	{
		const string clientId = "g79z2bul0nr87ju79sf7jdtc43vbw2";
		const string clientSecret = "ldhb631jod1fstu0vf4fenu7d6ak0o";

		api.Settings.ClientId = clientId;
		api.Settings.Secret = clientSecret;
	}
}
