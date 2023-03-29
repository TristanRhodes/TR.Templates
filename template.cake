///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");


Task("__TestTemplate")
	.Does(() => {

		var installResult = StartProcess("dotnet", @"new install .\templates\TestedLibrary --force");
		if (installResult != 0)
			throw new ApplicationException($"Failed installation ({installResult})");

		if (System.IO.Directory.Exists(@".\bin\template-proj"))
			System.IO.Directory.Delete(@".\bin\template-proj", true);

		var createResult = StartProcess("dotnet", @"new tr/tested-library --output .\bin\template-proj --ProjectName CakeTest");
		if (createResult != 0)
			throw new ApplicationException($"Failed create ({createResult})");

		DotNetTest(@".\bin\template-proj\CakeTest.sln");
	});

Task("PackageTemplate")
	.IsDependentOn("__TestTemplate")
	.Does(() => {
		// TODO: Figure out how to inject version number in here (then use git version)

		 var packSettings = new DotNetPackSettings
		 {
			 Configuration = "Release",
			 OutputDirectory = "./artifacts/"
		 };

		DotNetPack("template.csproj", packSettings);

		var versionNumber = "1.0.0";
		var packageName = $"TestTemplate.{versionNumber}.nupkg";
		var source = "https://nuget.pkg.github.com/TristanRhodes/index.json";
		var key = "{KEY}";

		var settings = new DotNetNuGetPushSettings
		{
			Source = source,
			ApiKey = key
		};

		DotNetNuGetPush($"artifacts/{packageName}", settings);
	});

Task("Default")
    .IsDependentOn("__TestTemplate");

RunTarget(target);