#load "../template.cake"
///////////////////////////////////////////////////////////////////////////////
// ADDINS
///////////////////////////////////////////////////////////////////////////////
#addin nuget:?package=Cake.Json&version=7.0.1

///////////////////////////////////////////////////////////////////////////////
// TOOLS
///////////////////////////////////////////////////////////////////////////////
#tool dotnet:?package=GitVersion.Tool&version=5.12.0

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////
var target = Argument("target", "Default");

var configuration = Argument("configuration", "Release");

var packageSource = Argument<string>("Source", null)	  // Input from cmd args to Cake 
	?? EnvironmentVariable<string>("INPUT_SOURCE", null); // Input from GHA to Cake

var apiKey = Argument<string>("ApiKey", null)		      // Input from cmd args to Cake 
	?? EnvironmentVariable<string>("INPUT_APIKEY", null); // Input from GHA to Cake

string artifactsFolder = "./artifacts";
string versionNumber;
string fullPackageName;

string[] packages = new[] 
{
	"src/TestedLibrary/TestedLibrary.csproj"
};

string[] tests = new[] 
{
	"test/TestedLibrary.UnitTests/TestedLibrary.UnitTests.csproj"
};

string[] benchmarks = new[] 
{
	"test/TestedLibrary.Benchmark/TestedLibrary.Benchmark.csproj"
};

///////////////////////////////////////////////////////////////////////////////
// Setup / Teardown
///////////////////////////////////////////////////////////////////////////////
Setup(context =>
{
	// Clean artifacts
	if (System.IO.Directory.Exists(artifactsFolder))
		System.IO.Directory.Delete(artifactsFolder, true);
});

Teardown(context =>
{
    
});

///////////////////////////////////////////////////////////////////////////////
// Tasks
///////////////////////////////////////////////////////////////////////////////
Task("__PackageArgsCheck")
	.Does(() => {
		if (string.IsNullOrEmpty(packageSource))
			throw new ArgumentException("Source is required");

		if (string.IsNullOrEmpty(apiKey))
			throw new ArgumentException("ApiKey is required");
	});

Task("__UnitTest")
	.Does(() => {

		foreach(var test in tests)
		{
			var testName = System.IO.Path.GetFileNameWithoutExtension(test);

			var settings = new DotNetTestSettings
			{
				Configuration = configuration,
				ResultsDirectory = artifactsFolder
			};

			// Console log for build agent
			settings.Loggers.Add("console;verbosity=normal");
		
			// Logging for trx test report artifact
			settings.Loggers.Add($"trx;logfilename={testName}.trx");

			DotNetTest(test, settings);
		}
	});

Task("__Benchmark")
	.Does(() => {

		foreach(var benchmark in benchmarks)
		{
			var benchName = System.IO.Path.GetFileNameWithoutExtension(benchmark);

			var settings = new DotNetRunSettings
			{
				Configuration = "Release", 
				ArgumentCustomization = args => {
					return args
						.Append("--artifacts")
						.AppendQuoted(System.IO.Path.Combine(artifactsFolder, benchName));
				}
			};

			DotNetRun(benchmark, settings);
		}
	});

Task("__VersionInfo")
	.Does(() => {

		var version = GitVersion();
		Information(SerializeJsonPretty(version));
		versionNumber = version.SemVer;
	});

Task("BuildAndTest")
	.IsDependentOn("__UnitTest");

Task("BuildAndBenchmark")
	.IsDependentOn("__Benchmark");

Task("PackAndPush")
	.IsDependentOn("__PackageArgsCheck")
	.IsDependentOn("__VersionInfo")
	.IsDependentOn("__UnitTest")
	.IsDependentOn("__Benchmark")
	.Does(() => {

		var packagesFolder = System.IO.Path.Combine(artifactsFolder, "packages");

		foreach(var package in packages)
		{
			Information("$Packing {package}...");
			var settings = new DotNetMSBuildSettings
			{
				PackageVersion = versionNumber
			};

			var packSettings = new DotNetPackSettings
			{
				Configuration = "Release",
				OutputDirectory = packagesFolder,
				MSBuildSettings = settings
			};
			DotNetPack(package, packSettings);
		}
		
		var packedArtifacts = System.IO.Directory.EnumerateFiles(packagesFolder);
		foreach(var package in packedArtifacts)
		{
			Information($"Pushing {package}...");
			var pushSettings = new DotNetNuGetPushSettings
			{
				Source = packageSource,
				ApiKey = apiKey
			};
			DotNetNuGetPush(package, pushSettings);
		}
	});

Task("Default")
	.IsDependentOn("__UnitTest")
	.IsDependentOn("__Benchmark");

RunTarget(target);