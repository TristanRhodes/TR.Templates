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
		var result = StartProcess("dotnet", "pack");
		if (result != 0)
			throw new ApplicationException($"Failed pack ({result})");

		// TODO: Push to package feed
	});

Task("Default")
    .IsDependentOn("__TestTemplate");

RunTarget(target);