using System;
using System.Collections.Generic;
using System.Web.Http;
using COMP4203.Web.Models;

namespace COMP4203.Web.Controllers
{
    public class TestController : ApiController
    {
	    [HttpGet, Route("api/test/gettestlist")]
	    public List<Test> GetTestList([FromBody] int aNumber)
	    {
			return new List<Test>
			{
				new Test
				{
					Id = Guid.NewGuid(),
					Name = $"Name {aNumber + 1}",
					Number = aNumber + 1
				},
				new Test
				{
					Id = Guid.NewGuid(),
					Name = $"Name {aNumber + 2}",
					Number = aNumber + 2
				},
				new Test
				{
					Id = Guid.NewGuid(),
					Name = $"Name {aNumber + 3}",
					Number = aNumber + 3
				}
			};
	    }
    }
}