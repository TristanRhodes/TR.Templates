# Tested Library Template

This repo is aiming to become a small installable project template I can use to easily spin up new repos with tested, publishable class libraries, published as nuget packages.

## Disclaimer
This is a work in progress.

## Tools
* GitVersion - https://gitversion.net/
* Github Actions - https://docs.github.com/en/actions
* Github Packages - https://docs.github.com/en/packages (Actually now publishing to myget)
* Cake Build - https://cakebuild.net/
* Dotnet Pack - https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-pack
* Dotnet Custom Templates (MSBuild) - https://learn.microsoft.com/en-us/dotnet/core/tools/custom-templates

## Secrets Policy

TODO: Token generation, storage and injection into GHA.

## To build and test project
Run: `dotnet cake template.cake --Target=InstallAndTestTemplate`

## To package and publish
Run: `dotnet cake template.cake --Target=PackAndPushTemplate --Source={Source} --ApiKey={key}`

## Package Feed:
Packages currently publish to my personal feed by default: https://www.myget.org/F/tr-public/api/v3/index.json

## Install and Create New Project
Setup package feed:
`dotnet nuget add source https://www.myget.org/F/tr-public/api/v3/index.json --name TR.Packages`

Install Template Package:
`dotnet new install TR.Templates --nuget-source TR.Packages --force`

Create new project:
`dotnet new Template.TestedLibrary --ProjectName={MyProjectName}`

### Forking
If you want to fork this and push your template to your own package feed, you'll need to configure the following in your repo:

* Create your personal NuGet feed
* Generate / lookup the API key for feed
* Go to your repository `settings` => `Secrets and variables` => `Actions`
* Under `Variables`/`Repository Variables`, create `NUGET_SOURCE` and put your package source package URL here.
* Under `Secrets`, create `NUGET_APIKEY` and put your package source API key here.
* These feed into `Github Actions` and are injected into the `Cake Build` step.
* When the GHA run the `PackAndPushTemplate` your package will be generated and pushed to the feed.