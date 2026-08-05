var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => Results.Text("baseline-from-main"));
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

await app.RunAsync();

