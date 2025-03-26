using KernelBlazeMind.Abstraction.Embeddings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KernelBlazeMind.Core.Embeddings
{
    public class LMStudioEmbeddingClient : IEmbeddingClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _model;

        public LMStudioEmbeddingClient(string baseUrl, string apiKey, string model)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            _model = model;
        }

        public async Task<List<float>> GenerateEmbeddingAsync(string input)
        {
            var payload = new
            {
                input = input,
                model = _model
            };

            var jsonContent = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("embeddings", content);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            var parsed = JsonSerializer.Deserialize<EmbeddingResponse>(responseString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString
            });

            return parsed?.Data?[0]?.Embedding ?? new List<float>();
        }
    }

    public class EmbeddingResponse
    {
        [JsonPropertyName("data")]
        public List<EmbeddingData> Data { get; set; }
    }

    public class EmbeddingData
    {
        [JsonPropertyName("embedding")]
        public List<float>? Embedding { get; set; }

        [JsonPropertyName("index")]
        public int Index { get; set; }
    }
}
