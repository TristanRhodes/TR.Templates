///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");


Task("__TestTemplate")
	.Does(() => {

		var installResult = StartProcess("dotnet", @"new install .\ --force");
		if (installResult != 0)
			throw new ApplicationException($"Failed installation ({installResult})");

		if (System.IO.Directory.Exists(@".\bin\template-proj"))
			System.IO.Directory.Delete(@".\bin\template-proj", true);

		var createResult = StartProcess("dotnet", @"new tr/tested-library --output .\bin\template-proj --ProjectName CakeTest");
		if (createResult != 0)
			throw new ApplicationException($"Failed create ({createResult})");

		DotNetTest(@".\bin\template-proj\CakeTest.sln");
	});

Task("__PackageTemplate")
		.IsDependentOn("__TestTemplate")
		.Does(() => {
			// TODO: Package and release template
		});

Task("Default")
    .IsDependentOn("__TestTemplate");

RunTarget(target);