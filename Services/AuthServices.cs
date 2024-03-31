using JwtTokenTask.Helper;
using JwtTokenTask.Models;
using JwtTokenTask.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtTokenTask.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly UserManager<ApplicationUser> _user;
        private readonly JwtClass _jwtClass;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthServices(UserManager<ApplicationUser> user, IOptions<JwtClass> jwtClass , RoleManager<IdentityRole> roleManager)
        {
            _user = user;
            _jwtClass = jwtClass.Value;
            _roleManager = roleManager;
        }

        public async Task<AuthModel> GetTokenAsyc(TokenRequestModel model)
        {
            var authModel=new AuthModel();
            var user = await _user.FindByEmailAsync(model.Email);
           
            if (user == null || !await _user.CheckPasswordAsync(user, model.Password))
            {
                authModel.Massage = "Email Or Password Incorcet";
                return authModel;
            }
            var getRoles = await _user.GetRolesAsync(user);
            var jwtST = await CreateJwtToken(user);
            authModel.IsAuthontocated = true;
            authModel.Email=user.Email;
            authModel.ExpireOn = jwtST.ValidTo;
            authModel.Token= new JwtSecurityTokenHandler().WriteToken(jwtST);
            authModel.Roles = getRoles.ToList();
            authModel.UserName = user.UserName;
            return authModel;

        }

        public async Task<AuthModel> RegesterAsyc(RegesterModel model)
        {
            if (await _user.FindByEmailAsync(model.Email) is not null)
                return new AuthModel { Massage = "Email Is Exist" };
            if (await _user.FindByNameAsync(model.UserName) is not null)
                return new AuthModel { Massage = "Name Is Exist" };

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
            };
            var result=await _user.CreateAsync(user,model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += error.Description;
                }
                return new AuthModel { Massage=errors};
            }
            await _user.AddToRoleAsync(user,"User");
            var jwtST =await CreateJwtToken(user);
            return new AuthModel
            {
                IsAuthontocated=true,
                Email = user.Email,
                UserName = user.UserName,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtST),
                Roles = new List<string> { "User" },
                ExpireOn = jwtST.ValidTo
            };
        }


        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _user.GetClaimsAsync(user);
            var roles = await _user.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtClass.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtClass.Issuer,
                audience: _jwtClass.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwtClass.DurationInDays),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
        public async Task<string> AddRoleAsync(AddRoleModel model)
        {
            var user = await _user.FindByIdAsync(model.UserId);

            if (user is null || !await _roleManager.RoleExistsAsync(model.Role))
                return "Invalid user ID or Role";

            if (await _user.IsInRoleAsync(user, model.Role))
                return "User already assigned to this role";

            var result = await _user.AddToRoleAsync(user, model.Role);

            return result.Succeeded ? string.Empty : "Sonething went wrong";
        }
    }
}
