namespace shared;

using TwitchLib.Api.Helix.Models.Streams.GetStreams;

public static class Util
{
	static void PrintResult(GetStreamsResponse[] responses)
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

