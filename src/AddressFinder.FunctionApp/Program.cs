using AddressFinder.FunctionApp.Domain.Services;
using AddressFinder.FunctionApp.Infrastructure.Catalog;
using AddressFinder.FunctionApp.Infrastructure.Telemetry;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Trace;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddSingleton<IEmbeddedMaskCatalogProvider, EmbeddedMaskCatalogProvider>();
        services.AddSingleton<MaskContentSafetyValidator>();
        services.AddSingleton<InputValidationService>();
        services.AddSingleton<MaskResolutionService>();
        services.AddSingleton<MaskResolutionTelemetry>();
        services.AddSingleton<PiiRedaction>();

        services.AddOpenTelemetry()
            .WithTracing(t => t.AddSource("AddressFinder.FunctionApp").AddAspNetCoreInstrumentation());
    })
    .Build();

host.Run();
