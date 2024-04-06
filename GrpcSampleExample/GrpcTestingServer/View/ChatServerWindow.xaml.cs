using System.Windows;
using GrpcTestingServer.ViewModel;

namespace GrpcTestingServer.View
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ChatServerWindow : Window
    {
        public ChatServerWindow()
        {
            InitializeComponent();

            var viewModel = new ChatServerWindowViewModel();
            viewModel.SubscribeLogger();
            DataContext = viewModel;
        }
    }
}
