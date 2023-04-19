using Template.DbApi.AcceptanceTests;

namespace SpecFlowProject1.StepDefinitions;

[Binding]
public sealed class HealthcheckStepDefinitions
{
    // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

    private readonly CoreTestContext _testContext;

    public HealthcheckStepDefinitions(CoreTestContext testContext) // use it as ctor parameter
    {
        _testContext = testContext;
    }

    [When("We call the healthcheck endpoint")]
    public async Task WeCallTheHealthcheckEndpoint()
    {
        var fullUrl = Path.Combine(_testContext.Uri, "_system/health");
        using var client = new HttpClient();
        _testContext.Response = await client.GetAsync(fullUrl);
    }

    [When("We call the ping endpoint")]
    public async Task WeCallThePingEndpoint()
    {
        var fullUrl = Path.Combine(_testContext.Uri, "_system/ping");
        using var client = new HttpClient();
        _testContext.Response = await client.GetAsync(fullUrl);
    }

    [When("We call the metrics endpoint")]
    public async Task WeCallTheMetricsEndpoint()
    {
        var fullUrl = Path.Combine(_testContext.Uri, "_system/metrics");
        using var client = new HttpClient();
        _testContext.Response = await client.GetAsync(fullUrl);
    }

    [When("We call the swagger endpoint")]
    public async Task WeCallTheSwaggerEndpoint()
    {
        var fullUrl = Path.Combine(_testContext.Uri, "swagger/v1/swagger.json");
        using var client = new HttpClient();
        _testContext.Response = await client.GetAsync(fullUrl);
    }
}
