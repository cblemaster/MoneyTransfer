using CommunityToolkit.Mvvm.ComponentModel;
using MoneyTransfer.UI.MAUI.Services;

namespace MoneyTransfer.UI.MAUI.PageModels
{
    public class PageModelBase : ObservableObject
    {
        protected readonly IDataService _dataService;

        public PageModelBase(IDataService dataService)
        {
            _dataService = dataService;
        }
    }
}
