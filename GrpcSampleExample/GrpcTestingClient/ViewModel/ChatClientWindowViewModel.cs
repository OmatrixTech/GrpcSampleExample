using Google.Protobuf.WellKnownTypes;
using GrpcClientSource;
using GrpcCommonSource;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace GrpcTestingClient
{
    public class ChatClientWindowViewModel : BindableBase, IDisposable
    {
        private readonly ChatServiceClient m_chatService = new ChatServiceClient();
        private readonly GreetServiceClient m_GreetService = new GreetServiceClient();
        public ObservableCollection<string> ChatHistory { get; } = new ObservableCollection<string>();
        private readonly object m_chatHistoryLockObject = new object();

        public string Name
        {
            get { return m_name; }
            set { SetProperty(ref m_name, value); }
        }
        private string m_name = "anonymous";

        private bool disposedValue;

        public DelegateCommand<string> WriteCommand { get; }
        public DelegateCommand ShowResultCommand { get; }
        public ChatClientWindowViewModel()
        {
            BindingOperations.EnableCollectionSynchronization(ChatHistory, m_chatHistoryLockObject);

            WriteCommand = new DelegateCommand<string>(WriteCommandExecute);
            ShowResultCommand = new DelegateCommand(ShowResultCommandExecute);
            StartReadingChatServer();
        }
        private async void ShowResultCommandExecute()
        {
            var cts = new CancellationTokenSource();
            var inputReq = new HelloRequest { Name = "Demo" };
            var response = m_GreetService.SayHelloMethod(inputReq);
            ChatHistory.Add($"{DateTime.Now.ToString("HH:mm:ss")} {"SayHello"}: {""}{response.Message}");
        }
        private void StartReadingChatServer()
        {
            var cts = new CancellationTokenSource();
            _ = m_chatService.ChatLogs()
                .ForEachAsync((x) => ChatHistory.Add($"{x.At.ToDateTime().ToString("HH:mm:ss")} {x.Name}: {x.Content}"), cts.Token);

            App.Current.Exit += (_, __) => cts.Cancel();
        }

        private async void WriteCommandExecute(string content)
        {
            await m_chatService.Write(new ChatLog
            {
                Name = m_name,
                Content = content,
                At = Timestamp.FromDateTime(DateTime.Now.ToUniversalTime()),
            });
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    m_chatService.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
