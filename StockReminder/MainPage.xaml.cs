using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.AspNet.SignalR.Client;
using System.Net.Http;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using System.Threading.Tasks;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace StockReminder
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<string> Messages { get; } = new ObservableCollection<string>();
        private HubConnection _hubConnection;
        private const string ServerURI = "https://stockmonitor.azurewebsites.net/signalr";
        private IHubProxy _hubProxy;

        public MainPage()
        {
            this.InitializeComponent();
            Messages.Add("Connecting...");
            _hubConnection = new HubConnection(ServerURI);
            _hubConnection.Closed += HubConnectionClosed;
            _hubProxy = _hubConnection.CreateHubProxy("ChatHub");
            _hubProxy.On<string, string>("BroadcastMessage", OnMessageReceived);

            try
            {
                _hubConnection.Start();
            }
            catch (HttpRequestException ex)
            {
                Messages.Add(ex.Message);
            }
            Messages.Add("client connected");
        }

        public void OnMessageReceived(string name, string message)
        {
            Task.Run(() => OnMessageReceivedAsync(name, message));
        }

        public async Task OnMessageReceivedAsync(string name, string message)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
             {
                 Messages.Add($"{name}: {message}");
             });

            //_messagingService.ShowMessage($"{name}: {message}");
        }

        private void HubConnectionClosed()
        {
            Messages.Add("Hub connection closed");
        }
    }
}
