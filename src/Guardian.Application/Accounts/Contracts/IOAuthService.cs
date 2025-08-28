using Guardian.Application.Wrappers;

namespace Guardian.Application.Accounts.Contracts
{
    public interface IOAuthService
    {
        public Task<Response<string>> GetAccessTokenAsync();
    }
}