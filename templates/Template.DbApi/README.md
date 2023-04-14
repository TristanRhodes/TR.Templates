### Message Template.DbApi

## Overview

This project is a template project to demonstrate a queue ingestor, reading records from a stream and enriching them into a REST object that can be served up via an API.


### To Test Template Project
From root:

`dotnet test`


### To Test Template Generation
From root:
`dotnet cake template.cake`

### To Run
Run docker-compose project in VS

OR

Run `docker compose -f docker-compose.infra.yml up` 
Launch apps:
* Template.DbApi.API
* Template.DbApi.DbUp

### Updating Grafana Dashboards

Launch Grafana, make your changes to your graphs, then run:

`docker compose -f docker-compose.grafana-sync.yml up`

or

`dotnet cake --target="ExportGrafana"`

This will generate the json files required to provision all your dashboards in grafana.



## OUT OF DATE

## Brief

We have a system that allows clients to send anonymous messages to us from their sites regarding user interactions with entities in a session.

This is split into two operations:
* Session Start - Creates a new session bucket to add interactions to.
* Entity Interaction - An interaction with an entity.

### Objectives
* Collect the Session Start events.
* Collect all interaction events.
* Support 1000 event hits per second.
* Provide an API that exposes endpoints to:
	* View most popular events in a session.
	* Query most popular events in an account over time.

## Application Components

* Template.DbApi Consumer - Enriches and collates messages put on a queue and stores them as aggreate.
* Template.DbApi Api - API for configuring merchants and products, and querying sessions by sessionId.
* Template.DbApi Simulator - Generates messages into the ingestion queue. Can also be used as a load tester.

## Containers
| Container          | Image        | Ports              |
| ------------------ | ------------ | ------------------ |
| Template.DbApi.Consumer  | function app |                    |
| Template.DbApi.Api       | api project  | 5080               |
| Template.DbApi.Simulator | api project  | 6080               |
| Template.DbApi.Seq       | Seq          | 5030               |
| Template.DbApi.Queue     | RabbitMQ     | 5672 (admin 15672) |

## Dependencies
* Azure Development Tools
* Dotnet SDK 6.0

## Diagram
TODO

## Configuring RabbitMQ
The rabbit MQ container (Template.DbApi.Queue) uses the Management image of RabbitMQ which comes with the admin endpoints enabled. These can be reached via the [local management porta](http://localhost:15672) or [api](http://localhost:15672/api/index.html). 

For the purpose of this demo, we have exported a pre-configured exchange defintion and run our container off that.


## Ingested Messages

### Session Start Event
Raised when a new session is created from a particular account. We would expect this to come from a merchant site for example when you land on the home page, or add your first item to a basket.

```json
{
  "SessionId":1111,
  "AccountId":2222,
  "Timestamp":"2022-01-01T22:05:01"
}
```

### Interaction Event
Raised when a client interacts with an entity. For example a client adds an item to a basket or clicks a product.

```json
{
  "SessionId":1111,
  "AccountId":2222,
  "EntityId":3333,
  "Timestamp":"2022-01-01T22:05:01"
}
```

## Enriched Values

### Session
Contains information about the acccount and when the session was started.

```json
{
  "SessionId":1111,
  "AccountId":2222,
  "AccountName":"Name",
  "AccountHomePage":"Url",
  "Timestamp":"2022-01-01T22:05:01"
}
```

### Interaction
Contains information about an entity interaction and when the last time it happened.

```json
{
  "SessionId":1111,
  "EntityName":"Name",
  "EntityUrl":"Url",
  "EntityImage":"Url/image.jpg",
  "Timestamp":"2022-01-01T22:05:01"
}
```

Storage Requirements:
* Merchant Table (Id, Name, Url)
* Product Table (Id, Name, Description, Url)



