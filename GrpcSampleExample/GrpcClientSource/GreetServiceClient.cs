using Grpc.Core;
using Grpc.Core.Interceptors;
using GrpcCommonSource;
using System.IO;


namespace GrpcClientSource
{
    public class GreetServiceClient : IDisposable
    {
        private readonly Greeter.GreeterClient m_client;
        private readonly Channel? m_channel; // If you create multiple client instances, the Channel should be shared.
        private readonly CancellationTokenSource m_cts = new CancellationTokenSource();
        private bool disposedValue;

        public GreetServiceClient()
        {
            // Locate required files and set true to enable SSL
            var secure = false;

            if (secure)
            {
                // create secure channel
                var serverCACert = File.ReadAllText(@"C:\localhost_server.crt");
                var clientCert = File.ReadAllText(@"C:\localhost_client.crt");
                var clientKey = File.ReadAllText(@"C:\localhost_clientkey.pem");
                var keyPair = new KeyCertificatePair(clientCert, clientKey);
                //var credentials = new SslCredentials(serverCACert, keyPair);

                // Client authentication is an option. You can remove it as follows if you only need SSL.
                var credentials = new SslCredentials(serverCACert);
                m_channel = new Channel("localhost", 50052, credentials);
                m_client = new Greeter.GreeterClient(
                    m_channel
                    .Intercept(new ClientIdInjector()) // 2nd
                    .Intercept(new HeadersInjector())); // 1st
            }
            else
            {
                // create insecure channel
                m_channel = new Channel("localhost", 50052, ChannelCredentials.Insecure);
                m_client = new Greeter.GreeterClient(
                    m_channel
                    .Intercept(new ClientIdInjector()) // 2nd
                    .Intercept(new HeadersInjector())); // 1st
            }
        }
        public HelloReply SayHelloMethod(HelloRequest helloRequest)
        {
            return m_client.SayHello(helloRequest);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    m_cts.Cancel(); // without cancel active calls, ShutdownAsync() never completes...
                    m_channel.ShutdownAsync().Wait();
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
