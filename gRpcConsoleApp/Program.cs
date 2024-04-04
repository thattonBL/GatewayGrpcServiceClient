using Grpc.Core;
using Grpc.Net.Client;
using GatewayGrpcService.Protos;

namespace gRpcConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Grpc Client here.....!!!");
            var channel = GrpcChannel.ForAddress("http://localhost:62620");
            int Count = 0;
            var watch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                var client = new GatewayGrpcService.Protos.GatewayGrpcService.GatewayGrpcServiceClient(channel);

                using var call = client.GetGatewayMessages(new RequestParams(), deadline: DateTime.UtcNow.AddMinutes(10));
                await foreach (var each in call.ResponseStream.ReadAllAsync())
                {
                    Console.WriteLine(String.Format("New Order Receieved from {0}-{1},Order ID = {2}, Unit Price ={3}, Ship Date={4}", each.Author, each.Title, each.StorageLocationCode, each.Shelfmark, each.ReaderName));
                    Count++;
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.DeadlineExceeded)
            {
                Console.WriteLine("Service timeout.");
            }
            watch.Stop();

            Console.WriteLine($"Stream ended: Total Records:{Count.ToString()} in {watch.Elapsed.TotalMinutes} minutes and {watch.Elapsed.TotalSeconds} seconds.");
            Console.Read();
        }
    }
}
