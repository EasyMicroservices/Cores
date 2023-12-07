using EasyMicroservices.Cores.AspCoreApi;
using EasyMicroservices.Cores.AspEntityFrameworkCoreApi.Interfaces;
using EasyMicroservices.Cores.Tests.DatabaseLogics.Database.Entities;
using EasyMicroservices.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EasyMicroservices.Cores.AspCore.Tests.Controllers
{
    public class UserController : SimpleQueryServiceController<UserEntity, UserEntity, UserEntity, UserEntity, long>
    {
        public UserController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        [Authorize]
        [HttpGet]
        public MessageContract AuthorizeError()
        {
            return true;
        }

        [Authorize]
        [HttpPost]
        public MessageContract PostAuthorizeError(string value)
        {
            return true;
        }

        [HttpGet]
        public MessageContract CheckHasAccess()
        {
            return true;
        }

        [HttpGet]
        public MessageContract CheckHasNoAccess()
        {
            return true;
        }

        [HttpGet]
        [AllowAnonymous]
        public MessageContract<string> Login()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("VGhpc0lzQVNlY3JldEtleUZvckp3dEF1dGhlbnRpY2F0aW9u=");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new List<Claim>() { new Claim(ClaimTypes.Role, "EndUser") }),
                Expires = DateTime.UtcNow.AddSeconds(1000),
                Issuer = "https://github.com/easymicroservices",
                Audience = "easymicroservices",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }

        [HttpGet]
        [AllowAnonymous]
        public MessageContract<string> AsCheckedResult()
        {
            MessageContract<string> msg = FailedReasonType.Incorrect;
            msg.GetCheckedResult();
            return "success";
        }

        [HttpGet]
        public MessageContract InternalError()
        {
            throw new Exception("Internal Error!");
        }
    }
}
