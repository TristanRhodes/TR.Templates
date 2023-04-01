# Tested Library Template

This repo is aiming to become a small installable project template I can use to easily spin up new repos with tested, publishable class libraries, published as nuget packages.

## Disclaimer
This is a work in progress.

## Tools
* GitVersion - https://gitversion.net/
* Github Actions - https://docs.github.com/en/actions
* Github Packages - https://docs.github.com/en/packages
* Build.cake - https://cakebuild.net/
* Dotnet Custom Templates (MSBuild) - https://learn.microsoft.com/en-us/dotnet/core/tools/custom-templates

## Secrets Policy

TODO: Token generation, storage and injection into GHA.

## To build and test project
Run: `dotnet cake template.cake --Target=InstallAndTestTemplate --Source={Source} --ApiKey={key}`

## To package and publish
Run: `dotnet cake template.cake --Target=PackAndPushTemplate --Source={Source} --ApiKey={key}`

## Package Feed:
Packages currently publish to my personal feed by default: https://www.myget.org/F/tr-public/api/v3/index.json


## Install and Create New Project
Setup package feed:
`dotnet nuget add source https://www.myget.org/F/tr-public/api/v3/index.json --name TR.Packages`

For latest version:
`dotnet new install TestTemplate --nuget-source TR.Packages --force`

For specific version:
`dotnet new install TestTemplate::0.1.0 --nuget-source TR.Packages --force`

## GitVersion

In order for GitVersion to work properly in a repository, it needs a tag to base the original commit marker off.

You achieve this by tagging your repository with `{Major}.{Minor}.0` and letting the commit counter increment with each PR / Merge.

## NOTES

### Dotnet Templating

https://github.com/dotnet/templating

### To Test Template Generation
Run: `dotnet cake template.cake`

### To Package the template
Run: `dotnet pack`

### To Generate a template
Run: `dotnet new tr/tested-library --output .\bin\template-proj --ProjectName {ProjectName}`

### For packaging to Github NuGet:

Create Personal access token: 
https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/creating-a-personal-access-token

https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-nuget-registry





