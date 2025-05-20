using System.Threading.Tasks;

namespace KernelBlazeMind.Abstraction.Authentication
{
    public interface IAuthenticationService<TState>
    {
        Task AuthenticateAsync();
        Task<bool> IsAuthenticatedAsync();
        TState State { get; }
    }
    public interface IAuthenticationService<TOptions, TState> : IAuthenticationService<TState>
    {
        public TOptions Options { get; }
    }
}

