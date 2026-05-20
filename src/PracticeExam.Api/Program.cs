using PracticeExam.Application;
using PracticeExam.Infrastructure;

const string corsPolicyName = "ClientApp";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Allowed origins come from configuration (Cors:AllowedOrigins). Production has
// none by default, so the API stays locked down until an origin is configured.
builder.Services.AddCors(options =>
    options.AddPolicy(corsPolicyName, policy =>
        policy
            .WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [])
            .AllowAnyHeader()
            .AllowAnyMethod()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors(corsPolicyName);

app.UseAuthorization();

app.MapControllers();

app.Run();

// Exposed so PracticeExam.Api.IntegrationTests can bootstrap the app via WebApplicationFactory<Program>.
public partial class Program;
