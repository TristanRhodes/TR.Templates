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


Task("__TestTemplate")
	.Does(() => {
		
		Information("Running dotnet commands....");
		var result = StartProcess("dotnet");
		if (result != 0)
			throw new ApplicationException($"Failed ({result})");

		Information("Installing Template...");
		var installResult = StartProcess("dotnet", @"new install .\templates\TestedLibrary --force");
		if (installResult != 0)
			throw new ApplicationException($"Failed installation ({installResult})");

		Information("Cleaning folders...");
		if (System.IO.Directory.Exists(@".\bin\template-proj"))
			System.IO.Directory.Delete(@".\bin\template-proj", true);

		Information("Creating Template Instance...");
		var createResult = StartProcess("dotnet", @"new tr/tested-library --output .\bin\template-proj --ProjectName CakeTest");
		if (createResult != 0)
			throw new ApplicationException($"Failed create ({createResult})");

		DotNetTest(@".\bin\template-proj\CakeTest.sln");
	});

Task("PackAndPushTemplate")
	.IsDependentOn("__TestTemplate")
	.Does(() => {

		var version = GitVersion();
		Information(SerializeJsonPretty(version));

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