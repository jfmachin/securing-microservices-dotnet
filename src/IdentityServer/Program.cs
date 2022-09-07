using IdentityServer.Extensions;
using IdentityServerHost.Quickstart.UI;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var migrationsAssembly = Assembly.GetExecutingAssembly().GetName().Name;
var connectionString = builder.Configuration.GetConnectionString("sqlserver");

builder.Services.AddControllersWithViews();
builder.Services.AddIdentityServer()
    .AddTestUsers(TestUsers.Users)
    .AddConfigurationStore(opt => {
        opt.ConfigureDbContext = b => b.UseSqlServer(connectionString,
        sql => sql.MigrationsAssembly(migrationsAssembly));
    })
    .AddOperationalStore(opt => {
        opt.ConfigureDbContext = b => b.UseSqlServer(connectionString,
        sql => sql.MigrationsAssembly(migrationsAssembly));
    }).AddDeveloperSigningCredential();

var app = builder.Build().MigrateDatabase();

app.UseStaticFiles();
app.UseRouting();
app.UseIdentityServer();
app.UseAuthorization();

app.MapDefaultControllerRoute();
app.Run();