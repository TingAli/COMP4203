using System;
using System.Collections.Generic;
using System.Web.Http;
using COMP4203.Web.Controllers.Extend;
using COMP4203.Web.Controllers.Hubs;
using COMP4203.Web.Models;
using Newtonsoft.Json;

namespace COMP4203.Web.Controllers
{
    public class MainController : ApiControllerWithHub<MainHub>
    {
	    [HttpGet, Route("api/testtwo/gettesttwolist")]
	    public void GetTestTwoList([FromUri] int aNumber)
	    {
		    List<Test> testTwoList = new List<Test>
		    {
			    new Test
			    {
				    Id = Guid.NewGuid(),
				    Name = $"Name {aNumber + 10}",
				    Number = aNumber + 10
			    },
			    new Test
			    {
				    Id = Guid.NewGuid(),
				    Name = $"Name {aNumber + 20}",
				    Number = aNumber + 20
			    },
			    new Test
			    {
				    Id = Guid.NewGuid(),
				    Name = $"Name {aNumber + 30}",
				    Number = aNumber + 30
			    }
		    };

		    Hub.Clients.All.broadcastTests(JsonConvert.SerializeObject(testTwoList));
	    }

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