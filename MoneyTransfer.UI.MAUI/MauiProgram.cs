using Microsoft.Extensions.Logging;
using MoneyTransfer.UI.MAUI.PageModels;
using MoneyTransfer.UI.MAUI.Services;

namespace MoneyTransfer.UI.MAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })

                .Services
                    .AddSingleton<AppShell>()
                    .AddSingleton<IDataService, HttpDataService>()
                    .AddTransient<ApproveTransferRequestPageModel>()
                    .AddTransient<CompletedTransfersPageModel>()
                    .AddTransient<LogInPageModel>()
                    .AddTransient<LogOutPageModel>()
                    .AddTransient<PendingTransfersPageModel>()
                    .AddTransient<RegisterPageModel>()
                    .AddTransient<RejectTransferRequestPageModel>()
                    .AddTransient<RequestTransferPageModel>()
                    .AddTransient<SendTransferPageModel>()
                    .AddTransient<AccountDetailsPageModel>()
                    .AddTransient<TransferDetailsPageModel>()
                    .AddSingleton<AppShell>()
                    .AddSingleton<IDataService, HttpDataService>()
                    .AddTransient<ApproveTransferRequestPageModel>()
                    .AddTransient<CompletedTransfersPageModel>()
                    .AddTransient<LogInPageModel>()
                    .AddTransient<LogOutPageModel>()
                    .AddTransient<PendingTransfersPageModel>()
                    .AddTransient<RegisterPageModel>()
                    .AddTransient<RejectTransferRequestPageModel>()
                    .AddTransient<RequestTransferPageModel>()
                    .AddTransient<SendTransferPageModel>()
                    .AddTransient<AccountDetailsPageModel>()
                    .AddTransient<TransferDetailsPageModel>()
                    ;

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
