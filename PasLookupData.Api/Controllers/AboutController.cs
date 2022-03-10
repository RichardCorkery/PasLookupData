using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace PasLookupData.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AboutController : ControllerBase
    {
        // GET: api/about>
        [HttpGet]
        public ContentResult Get()
        {
            var appName = Assembly.GetExecutingAssembly().GetName().Name;
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            
            var buildVersion = $"{ version.Major:0000}.{ version.Minor:00}.{ version.Build:00}.{ version.Revision:00}";

            var message = $@"App Name: {appName}<br/><br/>
                             Build: {buildVersion}<br/><br/>
                             Owner: PasLookupData.Api Developers<br/><br/>";
                            //ToDo: Figure out if and what environment swagger should be enabled in Azure.  Once done the line below can be uncommented
                            //<a href=""/swagger"">Swagger API Documentation</a>";

            var contentResult = new ContentResult()
            {
                Content = message,
                StatusCode = StatusCodes.Status200OK,
                ContentType = "text/html"
            };
            return contentResult;
        }
    }
}
