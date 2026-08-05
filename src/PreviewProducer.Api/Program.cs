var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => Results.Text("preview-from-feature-branch"));
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

await app.RunAsync();
