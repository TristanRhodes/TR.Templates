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

var packageSource = Argument<string>("Source", null)      // Input from cmd args to Cake
	?? EnvironmentVariable<string>("INPUT_SOURCE", null); // Input from GHA to Cake

var apiKey = Argument<string>("ApiKey", null)		      // Input from cmd args to Cake
	?? EnvironmentVariable<string>("INPUT_APIKEY", null); // Input from GHA to Cake

var packageName = Argument<string>("PackageName", null) 
	?? EnvironmentVariable<string>("INPUT_PACKAGENAME", null) // Input from GHA to Cake
	?? "Template.TestedLibrary";
	
var versionNumber = Argument<string>("VersionOverride", null)   // Input from cmd args to Cake 
	?? EnvironmentVariable<string>("INPUT_VERSIONOVERRIDE", null); // Input from GHA to Cake

string fullPackageName;

string[] templates = new [] 
{
	"Template.TestedLibrary",
	"Template.DbApi",
};

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
	
Task("__InstallTemplate")
	.Does(() => {
		Information("Installing Template...");
		var installResult = StartProcess("dotnet", "new install ./templates/ --force");
		if (installResult != 0)
			throw new ApplicationException($"Failed installation ({installResult})");
	});
	
Task("__CreateProjectAndTest")
	.Does(() => {

		foreach(var template in templates)
		{
			Information("Cleaning folders...");
			if (System.IO.Directory.Exists(@"./bin/template-proj"))
				System.IO.Directory.Delete(@"./bin/template-proj", true);

			Information("Creating Template Instance...");
			var createResult = StartProcess("dotnet", $@"new {template} --output ./bin/template-proj --ProjectName CakeTest");
			if (createResult != 0)
				throw new ApplicationException($"Failed create ({createResult})");
			
			// TODO: Run cake BuildAndTest instead.
			Information("Testing...");
			DotNetTest(@"./bin/template-proj/CakeTest.sln");
		}
	});

Task("__UninstallTemplate")
	.Does(() => {
		var removeResult = StartProcess("dotnet", @"new uninstall ./templates/");
		if (removeResult != 0)
			throw new ApplicationException($"Failed remove ({removeResult})");
	});

Task("__PackTemplates")
	.Does(() => {
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
		DotNetPack("Template.TestedLibrary.csproj", packSettings);
	});

Task("__PushTemplatePack")
	.Does(() => {
		Information("Pushing...");
		var pushSettings = new DotNetNuGetPushSettings
		{
			Source = packageSource,
			ApiKey = apiKey
		};
		DotNetNuGetPush($"artifacts/{fullPackageName}", pushSettings);
	});

Task("__VersionInfo")
	.Does(() => {

		if (string.IsNullOrEmpty(versionNumber))
		{
			var version = GitVersion();
			Information("GitVersion Info: " + SerializeJsonPretty(version));
			versionNumber = version.SemVer;
		}

		Information("Version Number: " + versionNumber);
	});

Task("InstallAndTestTemplate")
	.IsDependentOn("__InstallTemplate")
	.IsDependentOn("__CreateProjectAndTest")
	.IsDependentOn("__UninstallTemplate");

Task("PackAndPushTemplate")
	.IsDependentOn("__PackageArgsCheck")
	.IsDependentOn("__VersionInfo")
	.IsDependentOn("InstallAndTestTemplate")
	.IsDependentOn("__PackTemplates")
	.IsDependentOn("__PushTemplatePack");

Task("Default")
	.IsDependentOn("__VersionInfo")
    .IsDependentOn("__PackTemplates");

RunTarget(target);