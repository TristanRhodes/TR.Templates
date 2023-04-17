Feature: TodoApiFeatures

Feature test for the Todo list API.

@todo-list @api
Scenario: Create and View Todo item
	Given We have a Application api
	When We create task 'New Task'
	Then The response should be 200 OK
	When We get our TodoList
	Then The response should be 200 OK
	Then The result contains the created recordId
