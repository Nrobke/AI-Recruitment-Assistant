using AI_Recruitment_Assistant.Api.Extensions;
using AI_Recruitment_Assistant.Api.IdentityEndpoints;
using AI_Recruitment_Assistant.Application.Extensions;
using AI_Recruitment_Assistant.Domain.Entities;
using AI_Recruitment_Assistant.Infrastructure.Extensions;
using MerchantAppBackend.Infrastructure.Seed;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddPresentation();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHttpClient();

var app = builder.Build();

var scope = app.Services.CreateScope();
var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
await seeder.Seed();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGroup("api/identity")
    .WithTags("Identity")
    .MapCustomIdentityApi<User>();

app.UseAuthorization();

app.MapControllers();

app.Run();
