namespace SpecFlowProject1.StepDefinitions;

[Binding]
public sealed class HealthcheckStepDefinitions
{
    // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

    private readonly FeatureContext _featureContext;
    public HealthcheckStepDefinitions(FeatureContext featureContext) // use it as ctor parameter
    {
        _featureContext = featureContext;
    }

    [Given("We have a Application api")]
    public void GivenTheSecondNumberIs()
    {
        _featureContext["Uri"] = "http://localhost:5160/";
    }

    [Given("We have a Simulator api")]
    public void WhenTheTwoNumbersAreAdded()
    {
        _featureContext["Uri"] = "http://localhost:5284/";
    }

    [Given("We have a Consumer api")]
    public void ThenTheResultShouldBe()
    {
        _featureContext["Uri"] = "http://localhost:5159/";
    }

    [When("We call the healthcheck endpoint")]
    public async Task WeCallTheHealthcheckEndpoint()
    {
        var fullUrl = Path.Combine((string)_featureContext["Uri"], "_system/health");
        using var client = new HttpClient();
        _featureContext["Result"] = await client.GetAsync(fullUrl);
    }

    [When("We call the ping endpoint")]
    public async Task WeCallThePingEndpoint()
    {
        var fullUrl = Path.Combine((string)_featureContext["Uri"], "_system/ping");
        using var client = new HttpClient();
        _featureContext["Result"] = await client.GetAsync(fullUrl);
    }

    [When("We call the metrics endpoint")]
    public async Task WeCallTheMetricsEndpoint()
    {
        var fullUrl = Path.Combine((string)_featureContext["Uri"], "_system/metrics");
        using var client = new HttpClient();
        _featureContext["Result"] = await client.GetAsync(fullUrl);
    }

    [Then("The response should be 200 OK")]
    public void TheResultShouldBeOk()
    {
        var result = _featureContext["Result"] as HttpResponseMessage;
        
        result = result.Should().NotBeNull()
            .And.Subject;
        result.EnsureSuccessStatusCode();
    }
}
