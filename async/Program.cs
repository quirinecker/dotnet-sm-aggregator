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

		if (!this.noPrint) {
			var printStage = new PrintStage(processResult);
			await printStage.Run();
		}

		Console.WriteLine($"{fetchTime},{processTime}");
	}

	void SetCredentials()
	{
		const string clientId = "";
		const string clientSecret = "";

		api.Settings.ClientId = clientId;
		api.Settings.Secret = clientSecret;
	}
}
