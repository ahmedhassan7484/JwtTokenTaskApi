using JwtTokenTask.ViewModel;

namespace JwtTokenTask.Services
{
    public interface IAuthServices
    {
        Task<AuthModel> RegesterAsyc(RegesterModel model);
        Task<AuthModel> GetTokenAsyc(TokenRequestModel model);
    }
}
