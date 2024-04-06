using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcServerSource.Interfaces
{
    public interface IGrpcServerHelper
    {
        void StartServer();
        void StopServer();
        bool CheckServerStatus();
    }
}
