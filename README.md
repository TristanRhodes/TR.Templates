# TestedLibrary

This repo is aiming to become a small installable project template I can use to easily spin up new repos with tested, publishable class libraries, published as nuget packages.

### Templating

For more on templating, see here:

https://github.com/dotnet/templating

https://learn.microsoft.com/en-us/dotnet/core/tools/custom-templates


### To Test Template Generation
From root:
`dotnet cake template.cake`

### To Generate a template
Run:
`dotnet new tr/tested-library --output .\bin\template-proj --ProjectName {ProjectName}`