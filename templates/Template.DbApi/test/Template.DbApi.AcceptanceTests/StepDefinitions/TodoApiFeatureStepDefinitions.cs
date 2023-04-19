using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Net.Http.Json;
using TechTalk.SpecFlow.CommonModels;
using Template.DbApi.AcceptanceTests;

namespace SpecFlowProject1.StepDefinitions;

[Binding]
public sealed class TodoApiFeatureStepDefinitions
{
    private readonly CoreTestContext _testContext;
    private readonly TodoListTestContext _todoListTestContext;

    public TodoApiFeatureStepDefinitions(
        CoreTestContext testContext, 
        TodoListTestContext todoListTestContext)
    {
        _testContext = testContext;
        _todoListTestContext = todoListTestContext;
    }

    [When("We create task '(.*)'")]
    public async Task WeCreateATodoItem(string text)
    {
        var payload = new
        {
            Title = text,
            Description = text,
            DueDate = DateTime.Today
        };

        using var client = _testContext.CreateClient();
        _testContext.Response = await client.PostAsJsonAsync("TodoList", payload);
    }

    [Then("The response should contain a new RecordId")]
    public async Task TheResponseShouldContainANewRecordId()
    {
        var json = await _testContext.Response.Content.ReadAsStringAsync();
        _todoListTestContext.NewTodoItem = JsonConvert.DeserializeObject<dynamic>(json);

        ((string)_todoListTestContext.NewTodoItem
            .itemId.Value)
            .Should().NotBeNull();
    }

    [When("We get our TodoList")]
    public async Task WeGetOurTodoList()
    {
        using var client = _testContext.CreateClient();
        _testContext.Response = await client.GetAsync("TodoList");

    }

    [Then("The response should contain a Todo List")]
    public async Task TheResponseShouldContainATodoList()
    {
        var json = await _testContext.Response.Content.ReadAsStringAsync();
        _todoListTestContext.TaskList = ((JArray)JsonConvert.DeserializeObject<dynamic>(json))
            .Select(j => (dynamic)j)
            .ToList();
    }

    [Then("The result contains the created recordId")]
    public async Task WeCallThePingEndpoint()
    {
        var item = _todoListTestContext.NewTodoItem;
        var id = item.itemId;

        var match = _todoListTestContext.TaskList
            .SingleOrDefault(i => i.itemId == id);

        ((object)match).Should().NotBeNull();
    }
}
