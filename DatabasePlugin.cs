using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sudzekai.LinCtrl.Assembly;
using Sudzekai.LinCtrl.Core;
using Sudzekai.LinCtrl.Hosting;

namespace DatabasePlugin
{
    [PluginAssembly]
    public partial class DatabasePlugin : IPluginAssembly
    {
        public void Configure(AssemblyHostBuilder builder)
        {
            LinCtrlContext.AddTypeInfoResolver(DatabaseJsonContext.Default);

            builder.Services.AddSingleton<DatabaseConnectionFactory>();
            builder.Services.AddSingleton<DatabaseRepository>();
            builder.Services.AddSingleton<PasswordHashService>();
            builder.Services.AddSingleton<SystemUserInitializer>();
            builder.Services.AddSingleton(services =>
            {
                try
                {
                    services.GetRequiredService<SystemUserInitializer>().Initialize();
                }
                catch (Exception ex)
                {
                    services.GetRequiredService<ILogger<DatabasePlugin>>().LogError(ex, "Database plugin initialization failed");
                    throw;
                }

                return new DatabaseDispatcher(services.GetRequiredService<DatabaseRepository>(),
                    services.GetRequiredService<PasswordHashService>());
            });
            builder.Services.AddDatabaseHandlers();
        }
    }
}
