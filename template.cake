﻿///////////////////////////////////////////////////////////////////////////////
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

string versionNumber;
string fullPackageName;


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
		var installResult = StartProcess("dotnet", @"new install ./template/ --force");
		if (installResult != 0)
			throw new ApplicationException($"Failed installation ({installResult})");
	});
	
Task("__CreateProjectAndTest")
	.Does(() => {

		Information("Cleaning folders...");
		if (System.IO.Directory.Exists(@"./bin/template-proj"))
			System.IO.Directory.Delete(@"./bin/template-proj", true);

		Information("Creating Template Instance...");
		var createResult = StartProcess("dotnet", @"new Template.TestedLibrary --output ./bin/template-proj --ProjectName CakeTest");
		if (createResult != 0)
			throw new ApplicationException($"Failed create ({createResult})");
			
		// TODO: Run cake BuildAndTest instead.
		Information("Testing...");
		DotNetTest(@"./bin/template-proj/CakeTest.sln");
	});

Task("__UninstallTemplate")
	.Does(() => {
		var removeResult = StartProcess("dotnet", @"new uninstall ./template/");
		if (removeResult != 0)
			throw new ApplicationException($"Failed create ({createResult})");
	});

Task("VersionInfo")
	.Does(() => {
		var version = GitVersion();
		Information(SerializeJsonPretty(version));
		
		versionNumber = version.SemVer;
		fullPackageName = $"{packageName}.{versionNumber}.nupkg";

		Information($"Full package Name: {fullPackageName}");
	});

Task("InstallAndTestTemplate")
	.IsDependentOn("__InstallTemplate")
	.IsDependentOn("__CreateProjectAndTest")
	.IsDependentOn("__UninstallTemplate");

Task("PackAndPushTemplate")
	.IsDependentOn("__PackageArgsCheck")
	.IsDependentOn("VersionInfo")
	.IsDependentOn("InstallAndTestTemplate")
	.Does(() => {

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
		DotNetPack("Template.TestedLibrary.csproj", packSettings);

		Information("Pushing...");
		var pushSettings = new DotNetNuGetPushSettings
		{
			Source = packageSource,
			ApiKey = apiKey
		};
		DotNetNuGetPush($"artifacts/{fullPackageName}", pushSettings);
	});

Task("Default")
    .IsDependentOn("InstallAndTestTemplate");

RunTarget(target);