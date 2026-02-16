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
                var connectionString = configuration.GetValue<string>("AzureBlobStorage:ConnectionString")
                    ?? throw new InvalidOperationException("Azure Blob Storage connection string is not configured.");
                var containerName = configuration.GetValue<string>("AzureBlobStorage:ContainerName")
                    ?? throw new InvalidOperationException("Azure Blob Storage container name is not configured.");
                var baseUrl = configuration.GetValue<string>("AzureBlobStorage:BaseUrl")
                    ?? throw new InvalidOperationException("Azure Blob Storage base URL is not configured.");

                return new AzureBlobStorageService(connectionString, containerName, baseUrl);
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
