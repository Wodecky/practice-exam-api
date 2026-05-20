using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PracticeExam.Application.Common.Interfaces;
using PracticeExam.Infrastructure.Persistence;

namespace PracticeExam.Infrastructure;

/// <summary>
/// Composition root for the Infrastructure layer. Register concrete
/// implementations of Application abstractions here (persistence, external
/// services, etc.).
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PracticeExamDb")
            ?? throw new InvalidOperationException(
                "Connection string 'PracticeExamDb' is not configured.");

        services.AddDbContext<PracticeExamDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IExamRepository, ExamRepository>();

        return services;
    }
}
