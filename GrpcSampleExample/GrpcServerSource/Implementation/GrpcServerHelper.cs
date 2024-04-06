using Grpc.Core;
using Grpc.Core.Interceptors;
using GrpcChatSample.Server;
using GrpcCommonSource;
using GrpcServerSource.Interfaces;
using GrpcServerSource.Services;
using GrpcServerSource.Utilities;
using System.ComponentModel.Composition;
using System.IO;

namespace GrpcServerSource.Implementation
{
    [Export(typeof(IGrpcServerHelper))]
    internal class GrpcServerHelper : IGrpcServerHelper
    {
        #region Fields declaration
        [Import]
        private Logger m_logger = null;
        private Grpc.Core.Server? m_server=null;
        private bool isServerRunning = false;
        [Import]
        private ChatGrpcService? m_service = null;

        [Import]
        private GreetGrpcService? m_greetGrpcservice = null;
        #endregion
        public bool CheckServerStatus()
        {
           return isServerRunning;
        }

        public void StartServer()
        {
            if (!isServerRunning)
            {
                // Locate required files and set true to enable SSL
                var secure = false;

                if (secure)
                {
                    // secure
                    var clientCACert = File.ReadAllText(@"C:\localhost_client.crt");
                    var serverCert = File.ReadAllText(@"C:\localhost_server.crt");
                    var serverKey = File.ReadAllText(@"C:\localhost_serverkey.pem");
                    var keyPair = new KeyCertificatePair(serverCert, serverKey);
                    var credentials = new SslServerCredentials(new[] { keyPair }, clientCACert, SslClientCertificateRequestType.RequestAndRequireAndVerify);

                    // Client authentication is an option. You can remove it as follows if you only need SSL.
                    //var credentials = new SslServerCredentials(new[] { keyPair });

                    m_server = new Grpc.Core.Server
                    {
                        Services =
                    {
                        Chat.BindService(m_service)
                            .Intercept(new ClientIdLogger()) // 2nd
                            .Intercept(new IpAddressAuthenticator()), // 1st
                        Greeter.BindService(m_greetGrpcservice)
                            .Intercept(new ClientIdLogger()) // 2nd
                            .Intercept(new IpAddressAuthenticator()) // 1st
                    },
                        Ports =
                    {
                        new ServerPort("localhost", 50052, credentials)
                    }
                    };
                }
                else
                {
                    // insecure
                    m_server = new Grpc.Core.Server
                    {
                        Services =
                    {
                        Chat.BindService(m_service)
                            .Intercept(new ClientIdLogger()) // 2nd
                            .Intercept(new IpAddressAuthenticator()), // 1st
                    
                        Greeter.BindService(m_greetGrpcservice)
                            .Intercept(new ClientIdLogger()) // 2nd
                            .Intercept(new IpAddressAuthenticator()) // 1st
                    },
                        Ports =
                    {
                        new ServerPort("localhost", 50052, ServerCredentials.Insecure)
                    }
                    };
                }

                m_server.Start();
                m_logger.Info("Started.");
                isServerRunning = true;
            }
        }

        public void StopServer()
        {
            if(m_server != null)
            {
                if (isServerRunning)
                {
                    m_server.ShutdownTask.Wait();
                    isServerRunning = false;    
                }
            }
        }
    }
}
