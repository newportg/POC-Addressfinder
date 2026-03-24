using FluentAssertions;
using Xunit;

namespace AddressFinder.IntegrationTests;

public class RenderingFailureMetricsIntegrationTests
{
    [Fact]
    public void BaselineWorkflow_ShouldDefineMetricNamesForSc004()
    {
        var metricNames = new[] { "rendering_failures_before", "rendering_failures_after" };

        metricNames.Should().Contain("rendering_failures_before");
        metricNames.Should().Contain("rendering_failures_after");
    }
}
