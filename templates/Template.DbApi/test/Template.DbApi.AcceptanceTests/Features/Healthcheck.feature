Feature: Healthchecks

@healthcheck @api
Scenario: API Healthcheck
	Given We have a Application api
	When We call the ping endpoint
	Then The response should be 200 OK
	When We call the healthcheck endpoint
	Then The response should be 200 OK
	When We call the metrics endpoint
	Then The response should be 200 OK

@healthcheck @simulator
Scenario: Simulator healthcheck
	Given We have a Simulator api
	When We call the ping endpoint
	Then The response should be 200 OK
	When We call the healthcheck endpoint
	Then The response should be 200 OK
	When We call the metrics endpoint
	Then The response should be 200 OK

@healthcheck @consumer
Scenario: Consumer healthcheck
	Given We have a Consumer api
	When We call the ping endpoint
	Then The response should be 200 OK
	When We call the healthcheck endpoint
	Then The response should be 200 OK
	When We call the metrics endpoint
	Then The response should be 200 OK

