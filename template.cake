///////////////////////////////////////////////////////////////////////////////
// ADDINS
///////////////////////////////////////////////////////////////////////////////
#addin nuget:?package=Cake.Json&version=7.0.0

///////////////////////////////////////////////////////////////////////////////
// TOOLS
///////////////////////////////////////////////////////////////////////////////
#tool dotnet:?package=GitVersion.Tool&version=5.12.0

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");


Task("__TestTemplate")
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

		DotNetTest(@"./bin/template-proj/CakeTest.sln");
	});

Task("PackAndPushTemplate")
	.IsDependentOn("__TestTemplate")
	.Does(() => {
		
		// Will just set the version number of the build in the build server to the FullSemVer.
		GitVersion(new GitVersionSettings
		{
			OutputType = GitVersionOutput.BuildServer
		});

		// Will just return the version number variables as JSON, which will be parsed by the Cake alias and then returned.
		var version = GitVersion(new GitVersionSettings
		{
			OutputType = GitVersionOutput.Json
		});
		Information(SerializeJsonPretty(version));

		Information("Loading git version...");
		version = GitVersion();
		Information("Writing..");
		Information(SerializeJsonPretty(version));

		Information("Setting up parameters...");
		var versionNumber = version.SemVer;
		var packageName = $"TestTemplate.{versionNumber}.nupkg";
		var source = "https://nuget.pkg.github.com/TristanRhodes/index.json";
		var key = "{KEY}";

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
		DotNetPack("template.csproj", packSettings);


		https://gitversion.net/docs/usage/cli/installation

		Information("Pushing...");
		var pushSettings = new DotNetNuGetPushSettings
		{
			Source = source,
			ApiKey = key
		};
		DotNetNuGetPush($"artifacts/{packageName}", pushSettings);
	});

Task("Default")
    .IsDependentOn("__TestTemplate");

RunTarget(target);