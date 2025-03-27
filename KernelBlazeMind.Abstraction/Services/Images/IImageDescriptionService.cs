using System.IO;
using System.Threading.Tasks;

namespace KernelBlazeMind.Abstraction.Services.Images
{
    public interface IImageDescriptionService
    {
        Task<string> DescribeAsync(Stream stream);
    }
}
