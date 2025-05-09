using shared;

public class PrintStage : Stage<PrintStage?>
{

	public ProcessResult ProcessResult;

	public PrintStage(ProcessResult result) : base("Print")
	{
		ProcessResult = result;
	}

	public override Task<PrintStage?> Run()
	{
		WriteStreamNumberStats();
		WriteUserTypeNumberStats();
		WriteStreamerTypeNumberStats();
		WriteStreamTimeStats();
		WriteStreamStats();

		return Task.FromResult(this);
	}

	public void WriteStreamNumberStats()
	{
		StreamProcessResult result = this.ProcessResult.streamResult;
		String output = "streamCount;matureCount";
		output += "\n";
		output += $"{result.streamCount};{result.matureContent}";
		File.WriteAllText("./stream_stats.csv", output);
	}

	public void WriteUserTypeNumberStats()
	{
		UserProcessResult result = this.ProcessResult.userResult;
		long twitchAdminCount = result.userTypeDistribution[0];
		long globalModCount = result.userTypeDistribution[1];
		long twitchStaffCount = result.userTypeDistribution[2];
		long normalUserCount = result.userTypeDistribution[3];
		String output = "twitchAdminCount;globalModCount;twitchStaffCount;normalUserCount";
		output += "\n";
		output += $"{twitchAdminCount};{globalModCount};{twitchStaffCount};{normalUserCount}";
		File.WriteAllText("./user_type_stats.csv", output);
	}

	public void WriteStreamerTypeNumberStats()
	{
		UserProcessResult result = this.ProcessResult.userResult;
		long affiliateCount = result.streamerTypeDistribution[0];
		long partnerCount = result.streamerTypeDistribution[1];
		long normalUser = result.streamerTypeDistribution[2];

		String output = "affiliateCount;partnerCount;normalStreamerCount";
		output += "\n";
		output += $"{affiliateCount};{partnerCount};{normalUser}";
		File.WriteAllText("./streamer_type_stats.csv", output);
	}

	public void WriteStreamTimeStats()
	{
		StreamProcessResult result = this.ProcessResult.streamResult;
		String output = "Streamer;Time";
		output += "\n";

		foreach (var timeResult in result.gameStreamTime)
		{
			output += $"{timeResult.Key};{timeResult.Value.Item2}";
			output += "\n";
		}

		File.WriteAllText("./stream_time_stats.csv", output);
	}

	public void WriteStreamStats()
	{
		StreamProcessResult result = this.ProcessResult.streamResult;
		String viewerOutput = "GameName;ViewerCount";
		viewerOutput += "\n";



		int index = 0;
		foreach (var gameStat in result.gameViewer)
		{
			viewerOutput += $"{gameStat.Item1};{gameStat.Item2}";
			viewerOutput += "\n";

			String languageOutput = "Language;Count";
			languageOutput += "\n";


			foreach (var language in gameStat.Item3)
			{
				languageOutput += $"{language.Key};{language.Value}";
				languageOutput += "\n";
			}

			File.WriteAllText($"./{gameStat.Item1.Replace(" ", "_")}_language_stats.csv", languageOutput);
			index++;
		}

		File.WriteAllText("./game_viewer_stats.csv", viewerOutput);
	}
}
