using CollegeApp.MyLogging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace College.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DemoController : ControllerBase
    {
        private readonly IMyLogger _myLogger;

        public DemoController(IMyLogger myLogger)
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
