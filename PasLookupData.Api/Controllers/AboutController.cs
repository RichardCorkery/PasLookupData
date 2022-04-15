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

            //ToDo: Only include swagger link if not Production
            var message = $@"App Name: {appName}<br/><br/>
                             Build: {buildVersion}<br/><br/>
                             Owner: PasLookupData.Api Developers<br/><br/>
                            <a href=""/swagger"">Swagger API Documentation</a>";

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
