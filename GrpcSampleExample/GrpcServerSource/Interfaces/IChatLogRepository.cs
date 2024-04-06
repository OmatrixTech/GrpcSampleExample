using GrpcCommonSource;

namespace GrpcServerSource.Interfaces
{
    public interface IChatLogRepository
    {
        void Add(ChatLog chatLog);
        IEnumerable<ChatLog> GetAll();
    }
}
