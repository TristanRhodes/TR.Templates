using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.DbApi.AcceptanceTests.StepDefinitions;

[Binding]
public class CommonStepDefinitions
{
    private readonly CoreTestContext _testContext;

    public CommonStepDefinitions(CoreTestContext testContext)
    {
        _testContext = testContext;
    }

    [Given("We have a Application api")]
    public void WeHaveAnApplicationApi()
    {
        _testContext.Uri = "http://localhost:5160/";
    }

    [Then("The response should be 200 OK")]
    public void TheResultShouldBeOk()
    {
       _testContext.Response
            .Should().NotBeNull()
            .And
            .Subject.EnsureSuccessStatusCode();
    }
}
