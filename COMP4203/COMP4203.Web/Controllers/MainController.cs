using System;
using System.Web.Http;
using COMP4203.Web.Controllers.Extend;
using COMP4203.Web.Controllers.Hubs;
using COMP4203.Web.Models;
using Newtonsoft.Json;

namespace COMP4203.Web.Controllers
{
    public class MainController : ApiControllerWithHub<MainHub>
    {
	    [HttpGet, Route("api/main/testdebug")]
	    public void TestDebug()
	    {
		    Hub.Clients.All.broadcastOutputMessage(JsonConvert.SerializeObject(new OutputMessage
		    {
				Id = Guid.NewGuid(),
				Tag = "Test",
				Message = "Test message..."
		    }));
	    }
    }
}