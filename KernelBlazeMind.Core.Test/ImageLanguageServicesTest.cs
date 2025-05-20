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
����һ������ʶ��ͼƬ�����֣�
�㲻�᷵���κ��ʺ���. 
������޾�ϸ������ͼƬ������ �������� ����ɫ ����) ����(���� ��ɫ ��Χ) ���� ������ ��ò ���� ������ ���� ����ɫ ���� �ʵ� Ƥë�� �ȵȣ������������󵽵���������
ÿһ�����ݺ���Ҫ������ϸ��
������������� Ҫʶ������ ����������� Ҫ���跭��
�����ṩ��̵�
����������
��ͼ����
ɫ������
��������
��Ϊ�زĿ����ʺϵ�ʹ�ó���
��������" },

                    new { role = "user", content = new  object []
                            {new {type="text",text="��ʶ��"},
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
