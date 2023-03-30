# TestedLibrary

This repo is aiming to become a small installable project template I can use to easily spin up new repos with tested, publishable class libraries, published as nuget packages.

### Templating

For more on templating, see here:

https://github.com/dotnet/templating

https://learn.microsoft.com/en-us/dotnet/core/tools/custom-templates


### To Test Template Generation
Run: `dotnet cake template.cake`

### To Package a template
Run: `dotnet pack`

### To Generate a template
Run: `dotnet new tr/tested-library --output .\bin\template-proj --ProjectName {ProjectName}`

### For packaging to Github NuGet:

Create Personal access token: https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/creating-a-personal-access-token

https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-nuget-registry

See `NuGet` tab: https://github.com/features/packages

See section tab: https://docs.github.com/en/packages/quickstart

dotnet cake template.cake --Target=PackageTemplate

#### To package and publish
To pack and publish: `dotnet cake template.cake --Target=PackAndPushTemplate --ApiKey={key}`

Note - currently this only pushes to my personal package feed.