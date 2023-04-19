using Template.DbApi.AcceptanceTests;

namespace SpecFlowProject1.StepDefinitions;

[Binding]
public sealed class HealthcheckStepDefinitions
{
    // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

    private readonly CoreTestContext _testContext;

    public HealthcheckStepDefinitions(CoreTestContext testContext)
    {
        _testContext = testContext;
    }

    [When("We call the healthcheck endpoint")]
    public async Task WeCallTheHealthcheckEndpoint()
    {
        using var client = _testContext.CreateClient();
        _testContext.Response = await client.GetAsync("_system/health");
    }

    [When("We call the ping endpoint")]
    public async Task WeCallThePingEndpoint()
    {
        using var client = _testContext.CreateClient();
        _testContext.Response = await client.GetAsync("_system/ping");
    }

    [When("We call the metrics endpoint")]
    public async Task WeCallTheMetricsEndpoint()
    {
        using var client = _testContext.CreateClient();
        _testContext.Response = await client.GetAsync("_system/metrics");
    }

    [When("We call the swagger endpoint")]
    public async Task WeCallTheSwaggerEndpoint()
    {
        using var client = _testContext.CreateClient();
        _testContext.Response = await client.GetAsync("swagger/v1/swagger.json");
    }
}
