using GrpcServerSource.Utilities;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Reactive.Linq;
using System.Windows.Data;

namespace GrpcTestingServer.ViewModel
{
    public class ChatServerWindowViewModel
    {
        [Import]
        private Logger m_logger = null;

        public ObservableCollection<string> Logs { get; private set; } = new ObservableCollection<string>();

        public ChatServerWindowViewModel()
        {
            MefManager.Container.ComposeParts(this);
            BindingOperations.EnableCollectionSynchronization(Logs, new object());
        }

        public void SubscribeLogger()
        {
            m_logger.GetLogsAsObservable()
                .Subscribe((x) => Logs.Add(x));
        }
    }
}
