using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.ModularAppHosts;

namespace PreviewProducer.ModuleContract;

[GenerateDistributedApplicationModule(Name, Version = "2")]
public static partial class PreviewProducerModule
{
    public const string Name = "preview-producer";
    public const string ApiResourceName = "preview-producer-api";
    public const string SidecarResourceName = "preview-producer-sidecar";
    public const string Repository = "shirubasoft/aspire-modular-apphosts-preview-producer";

    public static void Define(IDistributedApplicationModuleBuilder module)
    {
        module.WithRepository(Repository);
        module.RequiresRepository();
        module.AddResource<ProjectResource>(ApiResourceName, context =>
            context.ApplicationBuilder
                .AddProject(
                    context.ResourceName,
                    Path.Combine(
                        context.RepositoryPath,
                        "src",
                        "PreviewProducer.Api",
                        "PreviewProducer.Api.csproj"))
                .WithHttpEndpoint(name: "http")
                .WithExternalHttpEndpoints()
                .WithHttpHealthCheck("/health"));

        module.AddContainer(SidecarResourceName, "nginx", "1.29-alpine")
            .Configure(sidecar => sidecar
                .WithHttpEndpoint(targetPort: 80, name: "http")
                .WithHttpHealthCheck("/"));
    }
}
