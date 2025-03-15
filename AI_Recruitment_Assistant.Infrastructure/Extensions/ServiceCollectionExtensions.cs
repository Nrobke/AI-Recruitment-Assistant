
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using AI_Recruitment_Assistant.Infrastructure.Persistence;
using AI_Recruitment_Assistant.Domain.Repositories;
using AI_Recruitment_Assistant.Application.Abstractions.Services;
using AI_Recruitment_Assistant.Infrastructure.Services;
using System.Text;
using AI_Recruitment_Assistant.Infrastructure.Repositories;
using AI_Recruitment_Assistant.Infrastructure.Services.Background;
using CustomerAppDashboard.Infrastructure.Authentication;
using AI_Recruitment_Assistant.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Identity;
using AI_Recruitment_Assistant.Infrastructure.Authentication;
using AI_Recruitment_Assistant.Domain.Entities;
using MerchantAppBackend.Infrastructure.Authentication;
using AI_Recruitment_Assistant.Domain.Repositories.UnitOfWork;
using System.Net.Http.Headers;
using MerchantAppBackend.Infrastructure.Seed;
using AI_Recruitment_Assistant.Infrastructure.Seed;

namespace AI_Recruitment_Assistant.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("merchantAppConn");
        services.AddDbContext<ApplicationDbContext>(opts => opts.UseNpgsql(connectionString));

        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ICalendarService, GoogleCalendarService>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddScoped<ITokenStorageService, MemoryTokenStorage>();
        services.AddScoped<IGeminiService, GeminiService>();
        services.AddRepositories();
        services.AddAuthenticationInternal(configuration);
        services.AddAuthorizationInternal();
        services.AddBackgroundService();
        services.AddCacheServices();
        services.AddHttpClient("GeminiClient", client =>
        {
            client.BaseAddress = new Uri(configuration["Gemini:BaseUrl"]!);
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        });
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();
        services.AddScoped<IRepository, Repository>();
        services.AddScoped<IJobRepository, JobRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    private static void AddAuthenticationInternal(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
           .AddJwtBearer(o =>
           {
               o.RequireHttpsMetadata = false;
               o.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!)),
                   ValidIssuer = configuration["Jwt:Issuer"],
                   ValidAudience = configuration["Jwt:Audience"],
                   ClockSkew = TimeSpan.Zero
               };
           });

        services.AddHttpContextAccessor();

        services.AddSingleton<ITokenProvider, TokenProvider>();
        services.AddScoped<SignInManager<User>, CustomSignInManager<User>>();
    }
    private static void AddAuthorizationInternal(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            //options.AddPolicies();
        });

    }

    private static void AddBackgroundService(this IServiceCollection services)
    {
        services.AddQuartz(q =>
        {
            q.AddJob<ProcessExpiredJobsJob>(opts => opts.WithIdentity("ProcessExpiredJobs"));
            q.AddTrigger(opts => opts
                .ForJob("ProcessExpiredJobs")
                .WithIdentity("ProcessExpiredJobsTrigger")
                .WithCronSchedule("0 0 * * * ?"));
        });

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });
        
        //services.AddTransient<IJobScheduler, QuartzJobScheduler>();
    }

    private static void AddCacheServices(this IServiceCollection services)
    {
        services.AddMemoryCache();
    }
}
