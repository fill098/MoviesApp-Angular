using MovieApp.Api.Extensions;
using MoviesApp.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddDataAccess(builder.Configuration)
    .AddJwtAuthentication()
    .AddOpenApiDocumentation();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // /openapi/v1.json and the Scalar UI at /scalar
    app.MapApiDocs();
}

app.UseHttpsRedirection();

// Authentication (who are you?) must run before Authorization (are you allowed?)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
