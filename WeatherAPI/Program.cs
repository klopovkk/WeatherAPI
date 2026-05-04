using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/openapi.json", async context =>
{ 
    context.Response.Redirect("/swagger/v1/swagger.json");
});

app.MapScalarApiReference(options =>
{
    options
        .WithTitle("Weather API Scalar Docs")
        .WithOpenApiRoutePattern("/swagger/{documentName}/swagger.json");
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
