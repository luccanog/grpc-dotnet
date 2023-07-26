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

        //Stream message
        var data = await Mock.GetData();

        await StreamCommunication(client, data);

        //Bidirectional stream
        await BiDirectionalStreamCommunication(client, data);

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    private static async Task BiDirectionalStreamCommunication(Greeter.GreeterClient client, IEnumerable<Mock.MockData> data)
    {
        var biStream = client.SayHelloHandleErrorStream();
        try
        {
            foreach (var item in data)
            {
                await biStream.RequestStream.WriteAsync(new HelloRequest { Name = item.Name });
                await Task.Delay(250);
            }
        }
        finally
        {
            await biStream.RequestStream.CompleteAsync();
        }

        while (await biStream.ResponseStream.MoveNext(new CancellationToken()))
        {
            Console.WriteLine("Error from gRPC server: " + biStream.ResponseStream.Current.Message);
        }
    }

    private static async Task StreamCommunication(Greeter.GreeterClient client, IEnumerable<Mock.MockData> data)
    {
        var stream = client.SayHeloStream();
        try
        {
            foreach (var item in data)
            {
                await stream.RequestStream.WriteAsync(new HelloRequest { Name = item.Name });
                await Task.Delay(250);
            }
        }
        finally
        {
            await stream.RequestStream.CompleteAsync();
        }
    }
}