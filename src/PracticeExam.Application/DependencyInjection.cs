using Microsoft.Extensions.DependencyInjection;
using PracticeExam.Application.Exams;

namespace PracticeExam.Application;

/// <summary>
/// Composition root for the Application layer. Register use cases, validators,
/// and pipeline behaviors here — nothing that touches infrastructure.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<GetExamsHandler>();
        services.AddScoped<GetExamByIdHandler>();

        return services;
    }
}
