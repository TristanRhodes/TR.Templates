### Message Template.DbApi

## Template Project
This project was created from a template.

## Overview

This project contains a postgres DB, an API, and a DB up project to handle DB migrations.

* Keycloak and SSO
* `Ping`, `Health`, and `Metrics` endpoints.
* Unit Tests, Acceptance Tests and Benchmarks with packaged reports.
* Export swagger defintions.

## To Use as local template

From local project root.

Install: `dotnet new install . --force`

Create New: `dotnet new Template.DbApi --ProjectName={ProjectName}`

Uninstall: `dotnet new uninstall .`


### To Run
Run docker-compose project in VS

OR

Run `docker compose -f docker-compose.infra.yml up` 
Launch apps:
* Template.DbApi.API
* Template.DbApi.DbUp


### Docker Pack and Push

for the purpose of getting started, I am using `ghcr.io/TristanRhodes` and the default `USERNAME`

`dotnet cake --Target=DockerPackAndPush --ContainerRegistry=ghcr.io/TristanRhodes --ContainerRegistryToken={token} --ContainerRegistryUserName=USERNAME`