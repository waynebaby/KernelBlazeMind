using KernelBlazeMind.Abstraction.Services.Images;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KernelBlazeMind.Core.Test
{
    [TestClass]
    public sealed class ImageLanguageServicesTest : TestBase
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
        public async Task TestNormalizeHeicFile()
        {
            var httpClient = new HttpClient();
            // Arrange
            var imageNormalizeService = ServiceProvider.GetService<IImageNormalizeService>();
            var heicFilePath = "Assets\\test.heic";

            var mstream = await imageNormalizeService.NormalizeAsync(heicFilePath);
            byte[] imageBytes; 
            imageBytes = mstream.ToArray();
            // Act
            using var skBitmap = SKBitmap.Decode(mstream);
            var imageFormat = skBitmap.ColorType == SKColorType.Bgra8888 ? "jpeg" : "unknown";

            // Assert
            Assert.AreEqual("jpeg", imageFormat);

    
            var base64Image = Convert.ToBase64String(imageBytes);

            var requestBody = new
            {
                model = "gemma-3-4b-it",
                messages = new object[]
                {
                    new { role = "system", content = @"
你是一个帮助识别图片的助手，
你不会返回任何问候语. 
你会事无巨细的描述图片的内容 包括物体 （颜色 材质) 光线(方向 颜色 氛围) 人物 （动作 容貌 穿着 情绪） 动物 （颜色 种类 质地 皮毛） 等等，包括你能想象到的所有内容
每一个内容后面要增加详细描
如果上面有文字 要识别文字 如果不是中文 要给予翻译
并且提供简短的
艺术象征性
构图分析
色调分析
情绪分析
作为素材可能适合的使用场景
整体评价" },

                    new { role = "user", content = new  object []
                            {new {type="text",text="请识别"},
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
            //Assert.IsTrue(responseBody.Contains("circle"), "should mention it is circle");
            Console.WriteLine(responseBody);
            // Additional assertions can be added here to validate the response content 
        }


    }
}
