using Microsoft.AspNetCore.Mvc;

namespace PasLookupData.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AboutController : ControllerBase
    {
        // GET: api/<AboutController>
        [HttpGet]
        public string Get()
        {
            return "About Controller";
        }
    }
}
