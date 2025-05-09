using System.Diagnostics;
using System.Text.Json;

class Config
{
	public required List<string> Exec { get; set; }
	public required List<string> ExecWarmup { get; set; }
	public required List<string> Stages { get; set; }
}

class Program
{
	static void Main(string[] args)
	{
		if (args.Length != 1)
		{
			Console.WriteLine("Usage: benchmark-cli <config>");
			return;
		}

		var config = ReadConfig(args[0]);

		if (config == null)
		{
			Console.WriteLine("Could not read config file. It might not be in the required format or does not exist");
			return;
		}

		var intermediates = config.Stages;
		var results = new List<Tuple<List<int>, long>>();

		for (var i = 0; i < 10; i++)
		{
			var (executable, arguments) = ParseExec(config.ExecWarmup);
			RunProcess(executable, arguments);
		}

		for (var i = 0; i < 10; i++)
		{
			var (executable, arguments) = ParseExec(config.Exec);
			var result = RunProcess(executable, arguments);
			Console.WriteLine($"time: {result.Item2}");
			results.Add(result);
		}

		Tuple<List<int>, long> accumulator = Tuple.Create(new List<int>(), 0L);
		var finalResult = results.Aggregate(accumulator, (acc, y) =>
		{
			foreach (var (item, index) in y.Item1.Select((item, index) => (item, index)))
			{
				if (acc.Item1.Count() <= index)
				{
					acc.Item1.Add(0);
				}
				acc.Item1[index] += item;
			}

			return Tuple.Create(acc.Item1, acc.Item2 + y.Item2);
		});

		var averagedResult = Tuple.Create(finalResult.Item1.Select(x => (float)x / 10).ToList(), (float)finalResult.Item2 / 10);

		Console.WriteLine($"Avg Time: {averagedResult.Item2}");

		if (intermediates == null)
		{
			return;
		}

		foreach (var (result, intermediate) in averagedResult.Item1.Zip(intermediates))
		{
			Console.WriteLine($"{intermediate}: {result}");
		}
	}

	static (string, List<string>) ParseExec(List<string> exec)
	{
		return (exec.First(), exec.Skip(1).ToList());
	}

	static Config? ReadConfig(string path)
	{
		string jsonString = File.ReadAllText(path);
		return JsonSerializer.Deserialize<Config>(jsonString);
	}

	static Tuple<List<int>, long> RunProcess(string exec, List<string> arguments)
	{
		var stopWatch = new Stopwatch();
		stopWatch.Start();
		var process = ComposeProcess(exec, arguments);
		process.Start();
		process.WaitForExit();
		stopWatch.Stop();

		return Tuple.Create(ParseOutput(process.StandardOutput.ReadToEnd()), stopWatch.ElapsedMilliseconds);
	}

	static List<int> ParseOutput(string output)
	{
		if (!output.Contains(","))
		{
			return [];
		}
		JsonSerializer.Serialize(output);
		var segments = output.Split(",").Select(int.Parse);
		return segments.ToList();
	}

	static Process ComposeProcess(string exec, List<string> arguments)
	{
		return new Process
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = exec,
				Arguments = string.Join(" ", arguments),
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = false,
			}
		};
	}
}
