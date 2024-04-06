using Grpc.Core;
using GrpcCommonSource;
using System.ComponentModel.Composition;

namespace GrpcServerSource.Services
{
    [Export]
    public class GreetGrpcService:Greeter.GreeterBase
    {
        
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }
    }
}
