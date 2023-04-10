# Readme

This project was created from a template.

## Build Output

All build artifacts are output to the `artifacts` folder in the project root directory.

- Benchmark Results: `artifacts/TestedLibrary.Benchmark`
- Test Results: `artifacts/TestedLibrary.Tests.trx`
- Package Output: `artifacts/TestedLibrary.{version}.nupkg`

## Cake

To run cake steps: `dotnet cake --Target=PackAndPush --Source=https://www.myget.org/F/tr-public/api/v3/index.json --ApiKey={apiKey}`