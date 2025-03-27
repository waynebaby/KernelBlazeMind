using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using OpenAI;
using System.ClientModel;
using System.Buffers.Text;
using KernelBlazeMind.Core.Embeddings;
using System.IO;
using Microsoft.Maui.Graphics.Skia;
using Microsoft.Maui.Graphics;

using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Skia;
using System.IO;
using System.Threading.Tasks;
using SkiaSharp;
using OpenAI.Chat;
namespace KernelBlazeMind.Core.Test
{
    [TestClass]
    public sealed class LocalEmbeddingTest: TestBase
    {
      

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            // This method is called once for the test class, before any tests of the class are run.
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            // This method is called once for the test class, after all tests of the class are run.
        }

        [TestInitialize]
        public void TestInit()
        {
            // This method is called before each test method.
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // This method is called after each test method.
        }

        [TestMethod]
        public async Task TestLocalEmbedding()
        {
            var httpClient = new HttpClient();
            var requestBody = new
            {
                model = "text-embedding-granite-embedding-278m-multilingual",
                input = "Some text to embed"
            };

            var json = System.Text.Json.JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("http://127.0.0.1:1234/v1/embeddings", content);

            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(responseBody, "Response body should not be null");
            Console.WriteLine(responseBody);
            // Additional assertions can be added here to validate the response content
        }

        [TestMethod]
        public async Task TestLocalEmbeddingWithClient()
        {
            var client = new LMStudioEmbeddingClient("http://127.0.0.1:1234/v1/", "something", "text-embedding-granite-embedding-278m-multilingual");
            var response = await client.GenerateEmbeddingAsync("Some text to embed asdasdasdasd ");
            var response2 = await client.GenerateEmbeddingAsync("Some text to embed asdasdasdasd 2312312312 asdasd ");

            Assert.IsNotNull(response, "Response should not be null");
            Assert.IsTrue(response.Count > 0, "Embeddings should not be empty");
            Assert.IsFalse(response.SequenceEqual(response2), "Embeddings should not be same");
            Console.WriteLine(string.Join(", ", response));

            Console.WriteLine(response.Count);
            // Additional assertions can be added here to validate the response content
        }

        [TestMethod]
        public async Task TestLocalChat()
        {
            var httpClient = new HttpClient();
            var requestBody = new
            {
                model = "gemma-3-27b-it",
                messages = new[]
                {
                    new { role = "system", content = "Always answer in rhymes. Today is Thursday" },
                    new { role = "user", content = "What day is it today?" }
                },
                temperature = 0.7,
                max_tokens = -1,
                stream = false
            };

            var json = System.Text.Json.JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("http://localhost:1234/v1/chat/completions", content);

            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(responseBody, "Response body should not be null");
            Console.WriteLine(responseBody);
            // Additional assertions can be added here to validate the response content
        }

        [TestMethod]
        public async Task TestLocalChatWithPicture()
        {
            var httpClient = new HttpClient();
            var stream = await CreatePictureCircleAsync();
            byte[] imageBytes;
            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                imageBytes = memoryStream.ToArray();
            }
            var base64Image = Convert.ToBase64String(imageBytes);

            var requestBody = new
            {
                model = "gemma-3-27b-it",
                messages = new object[]
                {
                    new { role = "system", content = "You are an AI that can recognize simple shapes." },
                    new { role = "user", content = new  object []
                            {new {type="text",text="What shape is this?"},
                                new { type= "image_url",
                        image_url =new{ url=$"data:image/jpeg;base64,{base64Image}"} } } ,

                    }
                },
                temperature = 0.7,
                max_tokens = -1,
                stream = false,
            };

            var json = System.Text.Json.JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("http://localhost:1234/v1/chat/completions", content);

            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(responseBody, "Response body should not be null");
            Assert.IsTrue(responseBody.Contains("circle"), "should mention it is circle");
            Console.WriteLine(responseBody);
            // Additional assertions can be added here to validate the response content 
        }
        [TestMethod]
        public async Task TestLocalChatWithPictureUsingOpenAISDK()
        {
            var stream = await CreatePictureCircleAsync();
            byte[] imageBytes;
            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                imageBytes = memoryStream.ToArray();
            }
            var base64Image = Convert.ToBase64String(imageBytes);

            var openAiClient = new OpenAIClient(new ApiKeyCredential("any"), new OpenAIClientOptions { Endpoint = new Uri("http://localhost:1234/v1/") });

            var chatClient = openAiClient.GetChatClient("gemma-3-27b-it");

            var response = await chatClient.CompleteChatAsync(
                new SystemChatMessage("You are an AI that can recognize simple shapes."),
                new UserChatMessage(ChatMessageContentPart.CreateTextPart("What shape is this?"), ChatMessageContentPart.CreateImagePart(new Uri($"data:image/jpeg;base64,{base64Image}")))
            );

            Assert.IsNotNull(response, "Response should not be null");



            foreach (var message in response.Value.Content)
            {
                if (message.Kind==ChatMessageContentPartKind.Text)
                {
                    Assert.IsTrue(message.Text.Contains("circle"), "should mention it is circle");
                    Console.WriteLine(message.Text );

                }
            }


            //   Assert.IsTrue(responseBody.Contains("circle"), "should mention it is circle");
            // Additional assertions can be added here to validate the response content
        }

        private async Task<Stream> CreatePictureCircleAsync()
        {
            int width = 200;
            int height = 200;
            var memoryStream = new MemoryStream();
            // Create a new bitmap with the specified width and height
            using (var bitmap = new SKBitmap(width, height))
            {
                // Create a canvas to draw on the bitmap
                using (var canvas = new SKCanvas(bitmap))
                {
                    // Clear the canvas with white color
                    canvas.Clear(SKColors.White);

                    // Create a paint object to define the circle's appearance
                    var paint = new SKPaint
                    {
                        Color = SKColors.Black,
                        StrokeWidth = 1,
                        IsStroke = true,
                        Style = SKPaintStyle.Stroke
                    };

                    // Draw the circle on the canvas
                    canvas.DrawCircle(width / 2, height / 2, (width - 20) / 2, paint);
                }

                // Encode the bitmap to PNG format and save it to the memory stream
                using (var image = SKImage.FromBitmap(bitmap))
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                {
                    data.SaveTo(memoryStream);
                }
            }

            memoryStream.Position = 0;
            return await Task.FromResult(memoryStream);
        }
    }

}