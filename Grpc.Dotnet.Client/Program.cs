using Grpc.Dotnet.Client;
using Grpc.Dotnet.Protos;
using Grpc.Net.Client;


internal class Program
{
    private static async Task Main(string[] args)
    {
        using var channel = GrpcChannel.ForAddress("https://localhost:7256");
        var client = new Greeter.GreeterClient(channel);

        //Single message
        var reply = await client.SayHelloAsync(new HelloRequest { Name = "GreeterClient" });
        Console.WriteLine("Greeting: " + reply.Message);

        var data = await Mock.GetData();

        //Stream message
        await StreamCommunication(client, data);

        //Bidirectional stream
        await BiDirectionalStreamCommunication(client, data);

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    private static async Task BiDirectionalStreamCommunication(Greeter.GreeterClient client, IEnumerable<Mock.MockData> data)
    {
        Console.WriteLine("***Bidirectional stream communication***");

        var biStream = client.SayHelloHandleErrorStream();

        var requestStreamTask = Task.Run(async () =>
        {
            try
            {
                foreach (var item in data)
                {
                    await biStream.RequestStream.WriteAsync(new HelloRequest { Name = item.Name });
                    await Task.Delay(500);
                }
            }
            finally
            {
                await biStream.RequestStream.CompleteAsync();
            }
        });

        var responseStreamTask = Task.Run(async () =>
        {
            while (await biStream.ResponseStream.MoveNext(new CancellationToken()))
            {
                Console.WriteLine("Error from gRPC server: " + biStream.ResponseStream.Current.Message);
            }
        });
        
        await Task.WhenAll(requestStreamTask, responseStreamTask);
    }

    private static async Task StreamCommunication(Greeter.GreeterClient client, IEnumerable<Mock.MockData> data)
    {
        var stream = client.SayHeloStream();
        try
        {
            foreach (var item in data)
            {
                await stream.RequestStream.WriteAsync(new HelloRequest { Name = item.Name });
            }
        }
        finally
        {
            await stream.RequestStream.CompleteAsync();
        }
    }
}