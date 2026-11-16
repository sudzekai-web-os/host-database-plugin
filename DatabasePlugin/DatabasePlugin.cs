using DatabasePlugin.Application.DI;
using Sudzekai.LinCtrl.Abi.Managed;
using Microsoft.Extensions.DependencyInjection;
using Sudzekai.LinCtrl.Assembly;
using Sudzekai.LinCtrl.Core;

namespace DatabasePlugin
{
    [PluginAssembly]
    public partial class DatabasePlugin : IPluginAssembly
    {
        public void Configure(AssemblyHostBuilder builder)
        {
            LinCtrlContext.AddTypeInfoResolver(DatabaseJsonContext.Default);

            builder.Services.AddApplicationServices();
            builder.Services.AddTransient<IRequestHandler, GetVersionHandler>();
            builder.Services.AddTransient<IRequestHandler, GetRolesHandler>();
            builder.Services.AddTransient<IRequestHandler, GetRoleHandler>();
            builder.Services.AddTransient<IRequestHandler, GetUsersHandler>();
            builder.Services.AddTransient<IRequestHandler, GetUserHandler>();
            builder.Services.AddTransient<IRequestHandler, CreateUserHandler>();
            builder.Services.AddTransient<IRequestHandler, UpdateUserHandler>();
            builder.Services.AddTransient<IRequestHandler, BlockUserHandler>();
            builder.Services.AddTransient<IRequestHandler, GetHistoryHandler>();
            builder.Services.AddTransient<IRequestHandler, GetHistoryEntryHandler>();
            builder.Services.AddTransient<IRequestHandler, CreateHistoryEntryHandler>();
            builder.Services.AddTransient<IRequestHandler, UpdateHistoryEntryHandler>();
            builder.Services.AddTransient<IRequestHandler, DeleteHistoryEntryHandler>();
        }
    }
}
