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

var packageSource = Argument<string>("Source", null) ?? 
	EnvironmentVariable<string>("INPUT_SOURCE", null); // Input from GHA to Cake

var apiKey = Argument<string>("ApiKey", null) ?? 
	EnvironmentVariable<string>("INPUT_APIKEY", null); // Input from GHA to Cake

///////////////////////////////////////////////////////////////////////////////
// Setup / Teardown
///////////////////////////////////////////////////////////////////////////////
Setup(context =>
{
    // Executed BEFORE the first task.
});

Teardown(context =>
{
    // Executed AFTER the last task.
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

Task("VersionInfo")
	.Does(() => {
		var version = GitVersion();
		Information(SerializeJsonPretty(version));
	});

Task("InstallAndTestTemplate")
	.Does(() => {
		
		Information("Installing Template...");
		var installResult = StartProcess("dotnet", @"new install ./templates/TestedLibrary --force");
		if (installResult != 0)
			throw new ApplicationException($"Failed installation ({installResult})");

		Information("Cleaning folders...");
		if (System.IO.Directory.Exists(@"./bin/template-proj"))
			System.IO.Directory.Delete(@"./bin/template-proj", true);

		Information("Creating Template Instance...");
		var createResult = StartProcess("dotnet", @"new tr/tested-library --output ./bin/template-proj --ProjectName CakeTest");
		if (createResult != 0)
			throw new ApplicationException($"Failed create ({createResult})");
			
		Information("Testing...");
		DotNetTest(@"./bin/template-proj/CakeTest.sln");
	});

Task("PackAndPushTemplate")
	.IsDependentOn("__PackageArgsCheck")
	.IsDependentOn("InstallAndTestTemplate")
	.Does(() => {

		Information("Loading git version...");
		var version = GitVersion();

		Information("Writing...");
		Information(SerializeJsonPretty(version));

		Information("Setting up parameters...");
		var versionNumber = version.SemVer;
		var packageName = $"TestTemplate.{versionNumber}.nupkg";

		// https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-pack
		Information("Packing...");
		var settings = new DotNetMSBuildSettings
		{
			PackageVersion = versionNumber
		};

		var packSettings = new DotNetPackSettings
		{
			Configuration = "Release",
			OutputDirectory = "./artifacts/",
			MSBuildSettings = settings
		};
		DotNetPack("Template.TestLibrary.csproj", packSettings);

		Information("Pushing...");
		var pushSettings = new DotNetNuGetPushSettings
		{
			Source = packageSource,
			ApiKey = apiKey
		};
		DotNetNuGetPush($"artifacts/{packageName}", pushSettings);
	});

Task("Default")
    .IsDependentOn("InstallAndTestTemplate");

RunTarget(target);