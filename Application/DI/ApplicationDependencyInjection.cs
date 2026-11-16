using DatabasePlugin.Application.Dispatchering.Commands;
using DatabasePlugin.Application.Dispatchering.Dispatchers;
using DatabasePlugin.Application.Dispatchering.Handlers;
using DatabasePlugin.Application.Dto;
using DatabasePlugin.Application.Context;
using DatabasePlugin.Application.Repositories.History;
using DatabasePlugin.Application.Repositories.Roles;
using DatabasePlugin.Application.Repositories.Users;
using DatabasePlugin.Application.Services.Authorization;
using DatabasePlugin.Application.Services.History;
using DatabasePlugin.Application.Services.Users;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sudzekai.LinCtrl.Core;
using Sudzekai.LinCtrl.Hosting.Application;
using IRoleRepository = DatabasePlugin.Application.Repositories.Roles.IRoleRepository;
using IUserRepository = DatabasePlugin.Application.Repositories.Users.IUserRepository;

namespace DatabasePlugin.Application.DI
{
    public static class ApplicationDependencyInjection
    {
        extension(IServiceCollection services)
        {
            public void AddApplicationServices()
            {
                services.AddSingleton<DatabaseSettings>();
                services.AddSingleton<IDbContext, AppDbContext>();
                services.AddSingleton<IUserRepository, UserRepository>();
                services.AddSingleton<IRoleRepository, RoleRepository>();
                services.AddSingleton<IHistoryRepository, HistoryRepository>();
                services.AddLinCtrlHashService();
                services.AddSingleton<SystemUserInitializer>();
                services.AddSingleton<UserService>();
                services.AddSingleton<HistoryService>();
                services.AddSingleton<AuthorizationService>();
                services.AddSingleton<IDispatcher>(provider =>
                {
                    try
                    {
                        provider.GetRequiredService<SystemUserInitializer>().Initialize();
                    }
                    catch (Exception ex)
                    {
                        provider.GetRequiredService<ILogger<Dispatcher>>().LogError(ex, "Ошибка инициализации плагина базы данных");
                        throw;
                    }

                    return new Dispatcher(provider);
                });

                services.AddTransient<ICommandHandler<GetVersionCommand, string>, GetVersionCommandHandler>();
                services.AddTransient<ICommandHandler<GetRolesCommand, RoleView[]>, GetRolesCommandHandler>();
                services.AddTransient<ICommandHandler<GetRoleCommand, RoleView>, GetRoleCommandHandler>();
                services.AddTransient<ICommandHandler<GetUsersCommand, UserView[]>, GetUsersCommandHandler>();
                services.AddTransient<ICommandHandler<GetUserCommand, UserView>, GetUserCommandHandler>();
                services.AddTransient<ICommandHandler<CreateUserCommand, UserView>, CreateUserCommandHandler>();
                services.AddTransient<ICommandHandler<UpdateUserCommand, UserView>, UpdateUserCommandHandler>();
                services.AddTransient<ICommandHandler<BlockUserCommand, object?>, BlockUserCommandHandler>();
                services.AddTransient<ICommandHandler<GetHistoryCommand, HistoryView[]>, GetHistoryCommandHandler>();
                services.AddTransient<ICommandHandler<GetHistoryEntryCommand, HistoryView>, GetHistoryEntryCommandHandler>();
                services.AddTransient<ICommandHandler<CreateHistoryEntryCommand, HistoryView>, CreateHistoryEntryCommandHandler>();
                services.AddTransient<ICommandHandler<UpdateHistoryEntryCommand, object?>, UpdateHistoryEntryCommandHandler>();
                services.AddTransient<ICommandHandler<DeleteHistoryEntryCommand, object?>, DeleteHistoryEntryCommandHandler>();
                services.AddTransient<ICommandHandler<AuthorizeCommand, (long Id, ClientRole Role)>, AuthorizeCommandHandler>();
            }
        }
    }
}
