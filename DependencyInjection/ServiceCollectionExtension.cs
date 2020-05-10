﻿using BusinessLogic;
using BusinessLogic.Abstractions;
using Cross;
using Cross.Abstractions;
using Cross.Security;
using Data.Abstractions;
using Data.DataAccess.Context;
using Data.DataAccess.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace DependencyInjection
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddDataService(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<SqlServerDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
            services.AddScoped<IMyContext>(provider => provider.GetRequiredService<SqlServerDbContext>());
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            return services;
        }

        public static IServiceCollection AddCrossService(this IServiceCollection services)
        {
            services.AddScoped<IUtility, Utility>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            return services;
        }

        public static IServiceCollection AddBusinessLogicService(this IServiceCollection services)
        {
            services.AddSingleton<BusinessLogicUtility>();
            services.AddScoped<ISecurityProvider, SecurityProvider>();
            services.AddScoped<IUserAuthenticator,BusinessLogicUserManager>();
            services.AddScoped<IBusinessLogicUserManager, BusinessLogicUserManager>();
            return services;
        }
    }
}