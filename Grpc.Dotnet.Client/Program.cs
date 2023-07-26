using Grpc.Dotnet.Protos;
using Grpc.Net.Client;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

using var channel = GrpcChannel.ForAddress("https://localhost:7256");
var client = new Greeter.GreeterClient(channel);

//Single message
var reply = await client.SayHelloAsync(new HelloRequest { Name = "GreeterClient" });
Console.WriteLine("Greeting: " + reply.Message);

//Stream message
var stream = client.SayHeloStream();
try
{
    var data = await GetMockData();

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

Console.WriteLine("Press any key to exit...");
Console.ReadKey();


static async Task<IEnumerable<MockData>> GetMockData()
{
    HttpClient httpClient = new HttpClient();

    var response = await httpClient.GetAsync("https://64c09be20d8e251fd1124065.mockapi.io/names");
    return await response.Content.ReadFromJsonAsync<IEnumerable<MockData>>();
}

record MockData
{

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}