using College.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace College.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public ActionResult<LoginResponseDTO> Login(LoginDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Please provide username and password");
            }

            LoginResponseDTO response = new() { Username = model.Username};

            if (model.Username == "admin" && model.Password == "password")
            {
                byte[] key = null;
                if (model.Policy == "Google")
                {
                    key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("JWTSecretforGoogle")); ;
                }
                else if (model.Policy == "Microsoft")
                {
                    key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("JWTSecretforMicrosoft")); ;
                }
                else if(model.Policy == "Local")
                {
                    key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("JWTSecretforLocal")); ;
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor()
                {
                    Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]
                    {
                        //username claim
                        new Claim(ClaimTypes.Name, model.Username),
                        new Claim(ClaimTypes.Role, "Admin"),
                    }),
                    Expires = DateTime.Now.AddHours(4),
                    SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                response.token = tokenHandler.WriteToken(token);
            }
            else
            {
                return Unauthorized("Invalid username and password");
            }
            return Ok(response);
        }
    }
}
