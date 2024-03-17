using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using MoneyTransfer.Core.Services.Data;
using MoneyTransfer.Core.Services.User;
using MoneyTransfer.UI.MAUI.PageModels;
using MoneyTransfer.UI.MAUI.Pages;

namespace MoneyTransfer.UI.MAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })

                .Services
                    .AddSingleton<AppShell>()
                    .AddSingleton<AppShellModel>()
                    .AddSingleton<IDataService, HttpDataService>()
                    .AddSingleton<IUserService, HttpUserService>()
                    .AddTransient<AccountDetailsPageModel>()
                    .AddTransient<AccountDetailsPage>()
                    .AddTransient<CompletedTransfersPageModel>()
                    .AddTransient<CompletedTransfersPage>()
                    .AddTransient<LogInPageModel>()
                    .AddTransient<LogInPage>()
                    .AddTransient<LogOutPageModel>()
                    .AddTransient<LogOutPage>()
                    .AddTransient<PendingTransfersPageModel>()
                    .AddTransient<PendingTransfersPage>()
                    .AddTransient<RegisterPageModel>()
                    .AddTransient<RegisterPage>()
                    .AddTransient<RequestTransferPageModel>()
                    .AddTransient<RequestTransferPage>()
                    .AddTransient<SendTransferPageModel>()
                    .AddTransient<SendTransferPage>()
                    .AddTransient<TransferDetailsPageModel>()
                    .AddTransient<ApproveTransferRequestPageModel>()
                    .AddTransient<RejectTransferRequestPageModel>()
                    ;

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
