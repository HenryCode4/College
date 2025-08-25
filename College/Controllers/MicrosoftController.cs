using CollegeApp.MyLogging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace College.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[EnableCors(PolicyName = "AllowOnlyMicrosoft")]
    [Authorize(AuthenticationSchemes = "LoginForMocrosoftUSers")]
    public class MicrosoftController : ControllerBase
    {
        private readonly IMyLogger _myLogger;
        public MicrosoftController(IMyLogger myLogger)
        {
            _myLogger = myLogger;
        }

        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        public ActionResult Index()
        {
            _myLogger.Log("This is microsoft endpoint");

            return Ok();
        }
    }
}
