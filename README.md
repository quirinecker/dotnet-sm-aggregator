# Dotnet Async/Parallel Twitch Analysis

This project has 3 subprojects. 

* benchmark-cli
* sync
* async

Sync fetches and processed the Data synchrounously while Async does it asynchrounously. The Benchmark tool is for meassuring the performance.

## Getting Started Async

```
dotnet run --prject async
```

## Getting Started Sync

```
dotnet run --prject async
```

## Getting Started Benchmark CLI
```
dotnet run --prject benchmark-cli async.json
```
or
```
dotnet run --prject benchmark-cli sync.json
```

depending on the program you desire to benchmark.

## R Script

The R script in the project is for generating plots. Be ware it may need adjusting before running.
