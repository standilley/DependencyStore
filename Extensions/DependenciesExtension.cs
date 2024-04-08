﻿using DependencyStore.Repositories;
using DependencyStore.Repositories.Contracts;
using DependencyStore.Services;
using DependencyStore.Services.Contracts;
using Microsoft.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace DependencyStore.Extensions
{
    public static class DependenciesExtension
    {
        public static void AddSqlConnection(
            this IServiceCollection services,
            string connectionString)
        {
            services.AddScoped<SqlConnection>(x
                => new SqlConnection(connectionString));
        }
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<IPromoCodeRepository, PromoCodeRepository>();
        }
        public static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<IDeliveryFeeService, DeliveryFeeService>();
        }
        //public static void AddConfiguration(
        //    this IServiceCollection services,
        //    IConfiguration configuration
        //    )
        //{
        //    services.AddSingleton<Configuration>();
        //    configuration.GetConnectionString("Default");
        //}
    }
}
