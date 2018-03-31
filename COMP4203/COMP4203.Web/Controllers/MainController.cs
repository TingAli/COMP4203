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
	    public MainController()
	    {

        }

        [HttpGet, Route("api/main/demo/{tabIndex}")]
        public void RunDemo(int tabIndex)
        {
            SimulationEnvironment sim = new SimulationEnvironment();
            // Add Test Nodes
            sim.AddNode(new MobileNode(100, 100, 100));
            sim.AddNode(new MobileNode(200, 210, 100));
            sim.AddNode(new MobileNode(210, 200, 100));
            sim.AddNode(new MobileNode(298, 298, 100));
            // Add Test Message
            sim.AddMessage(new Message(sim.GetNodes()[0], sim.GetNodes()[3]));
            // Print Simulation Nodes
            new OutputPaneController().PrintToOutputPane("Note", "Simulation Nodes:");
            foreach (MobileNode node in sim.GetNodes())
            {
                node.Print();
                node.PrintNodesWithinRange(sim);
            }
            new OutputPaneController().PopulateNodesDSR(sim.GetNodes(), tabIndex);
            sim.SendMessageDSR(sim.GetMessages()[0]);
        }

        [HttpGet, Route("api/main/run/{nodeNumber}/{messageNumber}/{simSpeedNumber}/{tabIndex}")]
        public void RunTest(
	        int nodeNumber,
	        int messageNumber,
	        int simSpeedNumber,
	        int tabIndex)
        {
            SimulationEnvironment sim = new SimulationEnvironment();
            sim.GenerateRandomNodes(nodeNumber);
            sim.GenerateRandomMessages(messageNumber);
            new OutputPaneController().PopulateNodesDSR(sim.GetNodes(), tabIndex);
            sim.RunSimulation();
            
        }

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