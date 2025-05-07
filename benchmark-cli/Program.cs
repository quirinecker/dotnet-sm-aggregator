using System.Diagnostics;

class Program
{
	static void Main(string[] args)
	{
		if (args.Length == 0 || args.Length > 2) {
			Console.WriteLine("Usage: benchmark-cli <path-to-executable> <?intermediates>");
			return;
		}

		var exec_arg = args[0].Split(" ").ToList();
		var exec = exec_arg.First();
		var arguments = exec_arg.Skip(1).ToList();
		var results = new List<Tuple<List<int>, long>>();

		for (var i = 0; i < 10; i++) {
			var result = RunProcess(exec, arguments);
			Console.WriteLine($"time: {result.Item2}");
			results.Add(result);
		}

		Console.WriteLine($"Avg Time: {resu}")
	}

	static Tuple<List<int>, long> RunProcess(string exec, List<string> arguments) {
		var stopWatch = new Stopwatch();
		stopWatch.Start();
		var process = ComposeProcess(exec, arguments);
		process.Start();
		process.WaitForExit();
		stopWatch.Stop();

		return Tuple.Create(ParseOutput(process.StandardOutput.ReadToEnd()), stopWatch.ElapsedMilliseconds);
	}

	static List<int> ParseOutput(string output) {
		var segments = output.Split(",").Select(int.Parse);
		return segments.ToList();
	}

	static Process ComposeProcess(string exec, List<string> arguments) {
		return new Process {
			StartInfo = new ProcessStartInfo {
				FileName = exec,
				Arguments = string.Join(" ", arguments),
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = false,
			}
		};
	}
}
