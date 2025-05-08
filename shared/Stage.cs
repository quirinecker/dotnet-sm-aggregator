namespace shared;
using System.Diagnostics;

public abstract class Stage<T>
{
	public string Name { get; init; }
	abstract public Task<T> Run();

	public Stage(string name)
	{
		Name = name;
	}

	public async Task<(long, T)> Meassure()
	{
		var stopwatch = new Stopwatch();
		stopwatch.Start();
		var result = await Run();
		stopwatch.Stop();
		return (stopwatch.ElapsedMilliseconds, result);
	}
}
