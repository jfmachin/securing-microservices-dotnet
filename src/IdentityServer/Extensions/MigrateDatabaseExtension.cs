using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Extensions {
    public static class MigrateDatabaseExtension {
        public static WebApplication MigrateDatabase(this WebApplication host) {
            using (var scope = host.Services.CreateScope()) {
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                if (!context.Clients.Any()) {
                    foreach (var client in Config.Clients)
                        context.Clients.Add(client.ToEntity());
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any()) {
                    foreach (var resource in Config.IdentityResources)
                        context.IdentityResources.Add(resource.ToEntity());
                    context.SaveChanges();
                }

                if (!context.ApiScopes.Any()) {
                    foreach (var resource in Config.ApiScopes)
                        context.ApiScopes.Add(resource.ToEntity());
                    context.SaveChanges();
                }
            }
            return host;
        }
    }
}
