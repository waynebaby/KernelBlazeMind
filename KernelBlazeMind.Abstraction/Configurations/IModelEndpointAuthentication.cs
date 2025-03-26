using KernelBlazeMind.Abstraction.Authentications;
using System.Threading.Tasks;

namespace KernelBlazeMind.Abstraction.Configurations
{
    public interface IModelEndpointAuthentication
    {
        Task AuthenticateAsync(); 
    }
    public interface IModelEndpointAuthentication<TConfiguration, TAuthenticationTarget> : IModelEndpointAuthentication 
        where TConfiguration : IModelEndpointAuthenticationConfiguration
        where TAuthenticationTarget : class

    {
        public TConfiguration Configuration { get; }
        public TAuthenticationTarget? Target { get; }
    }
}

