using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.AppServices.AppointmentServices;
using Services.AppServices.ServiceServices;
using Services.AppServices.UserRoleServices;
using Services.AppServices.UserServices;
using Services.Middlewares;
using System.Reflection;

namespace Services.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {

       
        services.AddScoped<IAppointmentService, AppointmentService>();
        services.AddScoped<IServiceService, ServiceService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserRoleService, UserRoleService>();
        services.AddScoped<ExceptionHandlingMiddleware>();

        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddHttpContextAccessor();

        return services;
    }
}