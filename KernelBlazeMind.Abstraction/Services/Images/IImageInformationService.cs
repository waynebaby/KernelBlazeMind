using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace KernelBlazeMind.Abstraction.Services.Images
{
    public interface IImageExifExtractionService
    {
        Task<IDictionary<string,string>> ExtractExifAsync(Stream stream);

    }
}
