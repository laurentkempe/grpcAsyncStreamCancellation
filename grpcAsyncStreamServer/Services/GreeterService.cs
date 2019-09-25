#region using

using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

#endregion

namespace grpcAsyncStreamServer
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;

        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override async Task SayHello(HelloRequest request, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
        {
            var contextCancellationToken = context.CancellationToken;

            foreach (var n in Enumerable.Range(1, 10))
            {
                if (contextCancellationToken.IsCancellationRequested) return;

                await responseStream.WriteAsync(new HelloReply
                {
                    Message = $"Hello {request.Name} {n}"
                });

                await Task.Delay(200);
            }
        }
    }
}