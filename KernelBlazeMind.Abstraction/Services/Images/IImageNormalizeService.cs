using System;
using System.IO;
using System.Threading.Tasks;

namespace KernelBlazeMind.Abstraction.Services.Images
{
    public interface IImageNormalizeService
    { 
         Task<MemoryStream> NormalizeAsync(string path);


    }
}
