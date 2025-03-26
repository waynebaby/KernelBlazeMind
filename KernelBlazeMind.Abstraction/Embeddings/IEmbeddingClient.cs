using System.Collections.Generic;
using System.Threading.Tasks;

namespace KernelBlazeMind.Abstraction.Embeddings
{
    public interface IEmbeddingClient
    {
        Task<List<float>> GenerateEmbeddingAsync(string input);
    }
}