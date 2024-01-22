using DatingAppSql21012024.Data;
using DatingAppSql21012024.Interfaces;
using DatingAppSql21012024.Services;

namespace DatingAppSql21012024.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddAplicationServices(
                            this IServiceCollection services,
                            IConfiguration config
                            )
    {
        services.AddCors();

        services.AddScoped<ITokenService, TokenService>();

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        services.AddScoped<IUserRepository, UserRepository>();

        //services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));

        //services.AddScoped<IPhotoService, PhotoService>();

        //services.AddScoped<LogUserActivity>();

        //services.AddScoped<ILikesRepository, LikesRepository>();
        //services.AddScoped<IMessageRepository, MessageRepository>();
        //services.AddScoped<IUserRepository, UserRepository>();


        return services;
    }
}
