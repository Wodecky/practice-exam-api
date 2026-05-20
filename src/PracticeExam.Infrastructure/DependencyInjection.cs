using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        return services;
    }
}
