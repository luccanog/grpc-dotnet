using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Dotnet.Protos;

namespace Grpc.Dotnet.Server.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }

        public override async Task<Empty> SayHeloStream(IAsyncStreamReader<HelloRequest> requestStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                var msg = requestStream.Current;
                _logger.LogInformation(msg.Name);
            }

            return new Empty();
        }

        public override async Task SayHelloHandleErrorStream(
            IAsyncStreamReader<HelloRequest> requestStream,
            IServerStreamWriter<ErrorMessage> responseStream,
            ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                var msg = requestStream.Current;
                _logger.LogInformation(msg.Name);

                if (msg.Name.Length < 4)
                {
                    await responseStream.WriteAsync(new ErrorMessage()
                    {
                        Message = $"{msg.Name} name length is lower than 4 chars."
                    });
                }
            }
        }
    }
}