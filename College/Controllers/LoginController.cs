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
            string audience = string.Empty;
            string issuer = string.Empty;
            byte[] key = null;
            if (model.Policy == "Google")
            {
                issuer = _configuration.GetValue<string>("GoogleIssuer");
                audience = _configuration.GetValue<string>("GoogleAudience");
                key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("JWTSecretforGoogle"));
            }
            else if (model.Policy == "Microsoft")
            {
                issuer = _configuration.GetValue<string>("MicrosoftIssuer");
                audience = _configuration.GetValue<string>("MicrosoftAudience");
                key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("JWTSecretforMicrosoft"));
            }
            else if(model.Policy == "Local")
            {
                issuer = _configuration.GetValue<string>("LocalIssuer");
                audience = _configuration.GetValue<string>("LocalAudience");
                key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("JWTSecretforLocal"));
            }


            if (model.Username == "admin" && model.Password == "password")
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor()
                {
                    Issuer = issuer,
                    Audience = audience,
                    Subject = new ClaimsIdentity(new Claim[]
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
