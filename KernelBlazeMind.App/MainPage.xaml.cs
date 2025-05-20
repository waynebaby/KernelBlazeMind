using CommunityToolkit.Maui.Views;
using System.Threading.Tasks;
using System.Reactive.Linq;
using MVVMSidekick.ViewModels;
using MVVMSidekick.Views;
using MVVMSidekick.Common;
using MVVMSidekick_MAUI;
namespace KernelBlazeMind.App
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            WebSearchPopup = new WebSearch();

            this.Loaded += MainPage_Loaded;
        }

   

        public WebSearch WebSearchPopup { get; private set; }
        private void MainPage_Loaded(object? sender, EventArgs e)
        {
            MVVMSidekick.EventRouting.EventRouter.Instance.GetEventChannel<string>()
                     .Where(x =>
                     x.Sender is Button)
                     .Where(x =>
                        x.EventName == nameof(Button.Clicked))
                     .Where(x =>
                        x.EventData == "OpenWebSearch")
                     .SubscribeOnDispatcher(this.Dispatcher,
                        async e =>
                            await this.ShowPopupAsync(this.WebSearchPopup))
                     .DisposeWhenUnload(this);


        }

 

    }
}
