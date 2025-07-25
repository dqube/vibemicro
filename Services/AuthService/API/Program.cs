using AuthService.API.Endpoints;
using AuthService.Application.Extensions;
using AuthService.Domain.Extensions;
using AuthService.Infrastructure.Extensions;
using BuildingBlocks.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddAuthServiceDomain();
builder.Services.AddAuthServiceApplication();
builder.Services.AddAuthServiceInfrastructure(builder.Configuration);

// Add API services from BuildingBlocks (includes all middleware, auth, validation, etc.)
builder.Services.AddApi(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline using BuildingBlocks
app.UseApi(app.Environment);

// Map AuthService specific endpoints
app.MapAuthenticationEndpoints();
app.MapUserEndpoints();
app.MapTokenEndpoints();

// Health check endpoints are automatically mapped by BuildingBlocks
// Available at: /health, /health/ready, /health/live

app.Run(); 