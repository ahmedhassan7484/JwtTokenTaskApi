using JwtTokenTask.Services;
using JwtTokenTask.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JwtTokenTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authServices;
        public AuthController(IAuthServices authServices)
        {
            _authServices = authServices;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody]RegesterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authServices.RegesterAsyc(model);
            if (!result.IsAuthontocated)
            {
                return BadRequest(result.Massage);    
            }
            return Ok(result);
        }
        [HttpPost("token")]
        public async Task<IActionResult> GetTokenAsyc([FromBody]TokenRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authServices.GetTokenAsyc(model);
            if (!result.IsAuthontocated)
            {
                return BadRequest(result.Massage);    
            }
            return Ok(result);
        }
    }
}
