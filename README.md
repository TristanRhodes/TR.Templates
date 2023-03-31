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
Run: `dotnet cake template.cake --Target=InstallAndTestTemplate --ApiKey={key}`

## To package and publish
Run: `dotnet cake template.cake --Target=PackAndPushTemplate --ApiKey={key}`

Note - currently this only pushes to my personal package feed.

## Install and Create New Project
Setup package feed:
`dotnet nuget add source https://nuget.pkg.github.com/TristanRhodes/index.json?apikey={apikey} --name TR.Packages`

Note: Tried this with username password
`--username {your-github-user} --password {your-github-password}` but this does not work on the auth side. Would like this so that anyone can access public package feeds.

For latest version:
Run: `dotnet new install TestTemplate --nuget-source TR.Packages --force'

For specific version:
Run: `dotnet new install TestTemplate::0.1.0 --nuget-source TR.Packages --force'

For some reason this hits the url:
https://nuget.pkg.github.com/TristanRhodes/index.json/FindPackagesById()?id='TestTemplate'&semVerLevel=2.0.0

Then returns 405 (Method Not Allowed) and we're not getting a search result from the nuget feed. This is not an auth problems though.

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





