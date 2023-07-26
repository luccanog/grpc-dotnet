using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Grpc.Dotnet.Client
{
    public static class Mock
    {
        public static async Task<IEnumerable<MockData>> GetData()
        {
            try
            {
                HttpClient httpClient = new HttpClient();

                var response = await httpClient.GetAsync("https://64c09be20d8e251fd1124065.mockapi.io/names");
                return await response.Content.ReadFromJsonAsync<IEnumerable<MockData>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to retrieve mocked data from API. Check if the https://mockapi.io host is alive.", ex.Message);
                throw;
            }
        }

        public record MockData
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; }
        }
    }
}
