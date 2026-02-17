using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Data.Interceptors;
using Shared.Services;


namespace Catalog
{
    public static class CatalogModule
    {
        public static IServiceCollection AddCatalogModule(this IServiceCollection services,
            IConfiguration configuration)
        {
            
        //Application Use Case services

        // Data - Infrastructure services 

        var connectionString = configuration.GetConnectionString("Database");

            services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
            services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

            services.AddDbContext<CatalogDbContext>((sp, options)=>
            {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                options.UseNpgsql(connectionString);
            }
             );

            services.AddScoped<IDataSeeder, CatalogDataSeeder>();

            services.AddScoped<IImageService>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var containerName = configuration.GetValue<string>("AzureBlobStorage:ContainerName")
                    ?? throw new InvalidOperationException("Azure Blob Storage container name is not configured.");
                var baseUrl = configuration.GetValue<string>("AzureBlobStorage:ServiceUri")
                    ?? throw new InvalidOperationException("Azure Blob Storage URL is not configured.");

                return new AzureBlobStorageService(containerName, baseUrl);
            });

            return services;
        }

        public static IApplicationBuilder UseCatalogModule(this IApplicationBuilder app)
        {
            //Use Data - Infrastructure services 
            app.UseMigration<CatalogDbContext>();

            return app;
        }

    }
}
