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

var packageName = "TR.Templates";
	
var versionNumber = Argument<string>("VersionOverride", null)   // Input from cmd args to Cake 
	?? EnvironmentVariable<string>("INPUT_VERSIONOVERRIDE", null); // Input from GHA to Cake

string fullPackageName;

string[] templates = new [] 
{
	"Template.TestedLibrary",
	"Template.TestedApi",
};

public void MoveDirectory(string source, string target)
{
    var stack = new Stack<Folders>();
    stack.Push(new Folders(source, target));

    while (stack.Count > 0)
    {
        var folders = stack.Pop();
        System.IO.Directory.CreateDirectory(folders.Target);
        foreach (var file in System.IO.Directory.GetFiles(folders.Source, "*.*"))
        {
             string targetFile = System.IO.Path.Combine(folders.Target, System.IO.Path.GetFileName(file));
             if (System.IO.File.Exists(targetFile)) System.IO.File.Delete(targetFile);
             System.IO.File.Move(file, targetFile);
        }

        foreach (var folder in System.IO.Directory.GetDirectories(folders.Source))
        {
			// Don't copy the .git folder.
			if (folder.EndsWith(".git"))
				continue;

            stack.Push(new Folders(folder, 
				System.IO.Path.Combine(folders.Target, System.IO.Path.GetFileName(folder))));
        }
    }
}

public class Folders
{
    public string Source { get; private set; }
    public string Target { get; private set; }

    public Folders(string source, string target)
    {
        Source = source;
        Target = target;
    }
}

///////////////////////////////////////////////////////////////////////////////
// Setup / Teardown
///////////////////////////////////////////////////////////////////////////////
Setup(context =>
{
    // Executed BEFORE the first task.
});

Teardown(context =>
{
	var setOriginResult = StartProcess("git", $"remote set-url origin https://github.com/TristanRhodes/TR.Templates.git");
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
	
Task("__CloneTestedLibraryTemplate")
	.Does(() => {


		if (!System.IO.Directory.Exists(@"./staging"))
			System.IO.Directory.CreateDirectory(@"./staging");
	
		if (System.IO.Directory.Exists(@"./staging/Template.TestedLibrary"))
			System.IO.Directory.Delete(@"./staging/Template.TestedLibrary", true);
			
		Information("Setting credentials for Template.TestedLibrary...");
		var setOriginResult = StartProcess("git", $"remote set-url origin https://{apiKey}@github.com/TristanRhodes/Template.TestedLibrary.git");
		if (setOriginResult != 0)
			throw new ApplicationException($"Failed to set origin for Template.TestedLibrary.");

		Information("Cloning Template.TestedLibrary...");

		var args = new ProcessArgumentBuilder()
					.Append("clone https://{apiKey}@github.com/TristanRhodes/Template.TestedLibrary.git");

		var cloneSettings = new ProcessSettings
		{
			Arguments = args,
			WorkingDirectory = @"./staging"
		};

		var cloneResult = StartProcess("git", cloneSettings);
		if (cloneResult != 0)
			throw new ApplicationException($"Failed to clone Template.TestedLibrary.");
			
		MoveDirectory("staging/Template.TestedLibrary", "templates/Template.TestedLibrary");
	});

Task("__CloneTestedApiTemplate")
	.Does(() => {
			
		if (!System.IO.Directory.Exists(@"./staging"))
			System.IO.Directory.CreateDirectory(@"./staging");

		if (System.IO.Directory.Exists(@"./staging/Template.TestedApi"))
			System.IO.Directory.Delete(@"./staging/Template.TestedApi", true);
			
		var setOriginResult = StartProcess("git", $"remote set-url origin https://{apiKey}@github.com/TristanRhodes/Template.TestedApi.git");
		if (setOriginResult != 0)
			throw new ApplicationException($"Failed to set origin for Template.TestedApi.");
			
		Information("Cloning Template.TestedApi...");

		var args = new ProcessArgumentBuilder()
					.Append("clone https://github.com/TristanRhodes/Template.TestedApi.git");

		var cloneSettings = new ProcessSettings
		{
			Arguments = args,
			WorkingDirectory = @"./staging"
		};

		var cloneResult = StartProcess("git", cloneSettings);
		if (cloneResult != 0)
			throw new ApplicationException($"Failed to clone Template.TestedApi.");
			
		MoveDirectory("staging/Template.TestedApi", "templates/Template.TestedApi");
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
			Information("Testing Tempalte: " + template);

			Information("Cleaning folders...");
			if (System.IO.Directory.Exists(@"./bin/template-proj"))
				System.IO.Directory.Delete(@"./bin/template-proj", true);

			Information("Creating Template Instance...");
			var createResult = StartProcess("dotnet", $@"new {template} --output ./bin/template-proj --ProjectName CakeTest");
			if (createResult != 0)
				throw new ApplicationException($"Failed create ({createResult})");
			
			Information("Testing...");
			var settings = new ProcessSettings
			{
				WorkingDirectory = "./bin/template-proj/",
				Arguments = new ProcessArgumentBuilder()
						.Append("cake --Target=BuildAndTest")
			};

			StartProcess("dotnet", settings);
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
		DotNetPack("TR.Templates.csproj", packSettings);
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

		fullPackageName = $"{packageName}.{versionNumber}.nupkg";
		Information($"Full package Name: {fullPackageName}");
	});

Task("InstallAndTestTemplate")
	.IsDependentOn("__CloneTestedLibraryTemplate")
	.IsDependentOn("__CloneTestedApiTemplate")
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