using UnitConverter.Api.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ConversionService>();
builder.Services.AddRazorPages();

var app = builder.Build();

app.UseStaticFiles();
app.MapRazorPages();

app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }));
app.MapGet("/api/categories", (ConversionService service) => Results.Ok(service.Categories));
app.MapGet("/api/units/{category}", (string category, ConversionService service) =>
{
    try { return Results.Ok(service.GetUnits(category)); }
    catch (ArgumentException ex) { return Results.BadRequest(new { error = ex.Message }); }
});
app.MapGet("/api/convert", (double value, string category, string from, string to, ConversionService service) =>
{
    try { return Results.Ok(service.Convert(value, category, from, to)); }
    catch (ArgumentException ex) { return Results.BadRequest(new { error = ex.Message }); }
});

app.Run();
