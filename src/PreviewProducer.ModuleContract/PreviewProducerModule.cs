using Aspire.Hosting;
using Aspire.Hosting.ModularAppHosts;

namespace PreviewProducer.ModuleContract;

[GenerateDistributedApplicationModule(Name, Version = "2")]
public static partial class PreviewProducerModule
{
    public const string Name = "preview-producer";
    public const string ApiResourceName = "preview-producer-api";
    public const string SidecarResourceName = "preview-producer-sidecar";
    public const string ApiImage =
        "ghcr.io/shirubasoft/aspire-modular-apphosts-preview-producer/api";

    public static void Define(IDistributedApplicationModuleBuilder module)
    {
        module.AddContainer(ApiResourceName, ApiImage)
            .Configure(api => api
                .WithHttpEndpoint(targetPort: 8080, name: "http")
                .WithExternalHttpEndpoints()
                .WithHttpHealthCheck("/health"));

        module.AddContainer(SidecarResourceName, "nginx", "1.29-alpine")
            .Configure(sidecar => sidecar
                .WithHttpEndpoint(targetPort: 80, name: "http")
                .WithHttpHealthCheck("/"));
    }
}
