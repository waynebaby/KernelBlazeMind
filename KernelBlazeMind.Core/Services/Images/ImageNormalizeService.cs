using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using KernelBlazeMind.Abstraction.Services.Images;
using Openize.Heic;
using Openize.Heic.Decoder;
using SkiaSharp;
using static System.Net.Mime.MediaTypeNames;

namespace KernelBlazeMind.Core.Services.Images
{
    public class ImageNormalizeService : IImageNormalizeService
    {
        public async Task<MemoryStream> NormalizeAsync(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty", nameof(path));
            }

            var extension = Path.GetExtension(path).ToLowerInvariant();
            if (extension == ".heic" || extension == ".heif")
            {
                return await ConvertHeicToJpegAsync(path);
            }
            else
            {
                return await GetMemoryStreamAsync(path);
            }
        }

        private async Task<MemoryStream> ConvertHeicToJpegAsync(string path)
        {

            using (var heicStream = File.OpenRead(path))
            {
                if (HeicImage.CanLoad(heicStream))
                {

                    var jpegStream = new MemoryStream();
                    var heicImage = HeicImage.Load(heicStream);
                    int[] pixels = heicImage.GetInt32Array(PixelFormat.Argb32);
                    using (var bitmap = new SKBitmap((int)heicImage.Width, (int)heicImage.Height))
                    {
                        // Create a pointer to the pixel data
                        var handle = GCHandle.Alloc(pixels, GCHandleType.Pinned);
                        try
                        {
                            bitmap.SetPixels(handle.AddrOfPinnedObject());

                            // Encode the bitmap to JPEG format
                            using (var image = SKImage.FromBitmap(bitmap))
                            using (var data = image.Encode(SKEncodedImageFormat.Jpeg, 100))
                                data.SaveTo(jpegStream);
                        }
                        finally
                        {
                            handle.Free();
                        }

                    }


                    jpegStream.Position = 0;
                    return await Task.FromResult(jpegStream);
                }
                else
                {
                    throw new BadImageFormatException("Invalid HEIC file");

                }

            }
        }

        private async Task<MemoryStream> GetMemoryStreamAsync(string path)
        {
            var memoryStream = new MemoryStream();
            using (var fileStream = File.OpenRead(path))
            {
                await fileStream.CopyToAsync(memoryStream);
            }
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
