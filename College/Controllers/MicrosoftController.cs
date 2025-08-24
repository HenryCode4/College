using CollegeApp.MyLogging;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace College.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors(PolicyName = "AllowOnlyMicrosoft")]
    public class MicrosoftController : ControllerBase
    {
        private readonly IMyLogger _myLogger;
        public MicrosoftController(IMyLogger myLogger)
        {
            _myLogger = myLogger;
        }
        [HttpGet]
        public ActionResult Index()
        {
            _myLogger.Log("Index method started");

            return Ok();
        }
    }
}
