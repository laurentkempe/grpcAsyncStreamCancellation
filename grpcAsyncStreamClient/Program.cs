#region using

using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using grpcAsyncStreamServer;

#endregion

namespace grpcAsyncStreamClient
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);

            var replies = client.SayHello(new HelloRequest {Name = "Laurent"});

            var tokenSource = new CancellationTokenSource();
            var n = 0;

            try
            {
                await foreach (var reply in replies.ResponseStream.ReadAllAsync(tokenSource.Token))
                {
                    Console.WriteLine(reply.Message);

                    if (++n == 5)
                    {
                        tokenSource.Cancel();
                    }
                }
            }
            catch (RpcException e) when (e.Status.StatusCode == StatusCode.Cancelled)
            {
                Console.WriteLine("Streaming was cancelled from the client!");
            }
        }
    }
}