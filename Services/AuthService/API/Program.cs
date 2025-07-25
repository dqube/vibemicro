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

// Add API services
builder.Services.AddApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Map endpoints
app.MapAuthenticationEndpoints();

app.Run(); 