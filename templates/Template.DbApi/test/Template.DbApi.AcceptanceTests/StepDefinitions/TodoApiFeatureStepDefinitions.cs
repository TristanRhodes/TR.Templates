using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Net.Http.Json;
using TechTalk.SpecFlow.CommonModels;

namespace SpecFlowProject1.StepDefinitions;

[Binding]
public sealed class TodoApiFeatureStepDefinitions
{
    // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

    private readonly FeatureContext _featureContext;
    public TodoApiFeatureStepDefinitions(FeatureContext featureContext) // use it as ctor parameter
    {
        _featureContext = featureContext;
    }

    [When("We create task '(.*)'")]
    public async Task WeCreateATodoItem(string text)
    {
        var fullUrl = Path.Combine((string)_featureContext["Uri"], "TodoList");
        using var client = new HttpClient();
        var payload = new
        {
            Title = text,
            Description = text,
            DueDate = DateTime.Today
        };
        var result = await client.PostAsJsonAsync(fullUrl, payload);

        var json = await result.Content.ReadAsStringAsync();
        _featureContext["NewTask"] = JsonConvert.DeserializeObject<dynamic>(json);
        _featureContext["Result"] = result;
    }

    [When("We get our TodoList")]
    public async Task WeGetOurTodoList()
    {
        var fullUrl = Path.Combine((string)_featureContext["Uri"], "TodoList");
        using var client = new HttpClient();

        var result = await client.GetAsync(fullUrl);

        var json = await result.Content.ReadAsStringAsync();
        _featureContext["TaskList"] = JsonConvert.DeserializeObject<dynamic>(json);
        _featureContext["Result"] = result;

    }

    [Then("The result contains the created recordId")]
    public async Task WeCallThePingEndpoint()
    {
        var item = (dynamic)_featureContext["NewTask"];
        var id = item.itemId;

        var match = ((JArray)_featureContext["TaskList"])
            .SingleOrDefault(i => i["itemId"] == id);

        match.Should().NotBeNull();
    }
}
