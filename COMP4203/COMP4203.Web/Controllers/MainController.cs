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
        ComponentController controller;

	    public MainController()
	    {
            controller = new ComponentController();
        }

        [HttpGet, Route("api/main/demo/{tabIndex}")]
        public void RunDemo(int tabIndex)
        {
            SimulationEnvironment sim = new SimulationEnvironment();
            // Add Test Nodes
            sim.AddNode(new MobileNode(100, 100, 30, 200));
            sim.AddNode(new MobileNode(200, 210, 30, 200));
            sim.AddNode(new MobileNode(210, 200, 30, 200));
            sim.AddNode(new MobileNode(298, 298, 30, 200));
            // Add Test Message
            sim.AddMessage(new Message(sim.GetNodes()[0], sim.GetNodes()[3]));
            // Print Simulation Nodes
            new ComponentController().PrintToOutputPane("Note", "Simulation Nodes:");
            foreach (MobileNode node in sim.GetNodes())
            {
                node.Print();
                node.PrintNodesWithinRange(sim);
            }
            /*new ComponentController().PopulateNodesDSR(sim.GetNodes(), tabIndex);
            sim.SendMessageDSR(sim.GetMessages()[0], new SessionData(), 2000);*/
            new ComponentController().PopulateNodesSADSR(sim.GetNodes(), tabIndex);
            sim.SendMessageSADSR(sim.GetMessages()[0], new SessionData(), 2000);
        }

        [HttpGet, Route("api/main/run/{nodeNumber}/{messageNumber}/{simSpeedNumber}/{nodeRange}/{tabIndex}")]
        public void RunTest(
	        int nodeNumber,
	        int messageNumber,
	        int simSpeedNumber,
	        int tabIndex,
            int nodeRange)
        {
            SimulationEnvironment sim = new SimulationEnvironment();
            sim.GenerateRandomNodes(nodeNumber, nodeRange);
            sim.GenerateRandomMessages(messageNumber);
            controller.PopulateNodesDSR(sim.GetNodes(), tabIndex);
            sim.RunSimulation(simSpeedNumber);
        }
    }
}