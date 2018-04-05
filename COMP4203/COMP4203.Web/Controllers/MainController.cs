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
        SimulationEnvironment simulationEnvironment;

	    public MainController()
	    {
            controller = new ComponentController();
            simulationEnvironment = new SimulationEnvironment();
        }

        [HttpGet, Route("api/main/demo/{tabIndex}")]
        public void RunDemo(int tabIndex)
        {
            SimulationEnvironment sim = new SimulationEnvironment();
            // Add Test Nodes
            sim.AddNode(new MobileNode(100, 100, 100, 200));
            sim.AddNode(new MobileNode(200, 210, 100, 200));
            sim.AddNode(new MobileNode(210, 200, 15, 200));
            sim.AddNode(new MobileNode(298, 298, 15, 200));
            sim.AddNode(new MobileNode(200, 100, 100, 200));
            sim.AddNode(new MobileNode(150, 120, 100, 200));
            // Add Test Message
            sim.AddMessage(new Message(sim.GetNodes()[0], sim.GetNodes()[3]));
            // Print Simulation Nodes
            new ComponentController().PrintToOutputPane(OutputTag.TAG_NOTE, "Simulation Nodes:");
            foreach (MobileNode node in sim.GetNodes())
            {
                node.Print();
                node.PrintNodesWithinRange(sim);
            }
            new ComponentController().PopulateNodesOnCanvas(sim.GetNodes(), tabIndex);
            //sim.SendMessageDSR(sim.GetMessages()[0], new SessionData(), 2000);
            sim.SendMessageSADSR(sim.GetMessages()[0], new SessionData(), 500);
        }

        [HttpGet, Route("api/main/generate/{nodeNumber}/{messageNumber}/{simSpeedNumber}/{nodeRange}/{pureSelfishNodeNumber}/{partialSelfishNodeNumber}/{tabIndex}")]
        public void GenerateData(
            int nodeNumber,
            int messageNumber,
            int simSpeedNumber,
            int tabIndex,
            int nodeRange,
            int pureSelfishNodeNumber,
            int partialSelfishNodeNumber)
        {
            simulationEnvironment.GenerateRandomNodes(nodeNumber, nodeRange, pureSelfishNodeNumber, partialSelfishNodeNumber);
            simulationEnvironment.GenerateRandomMessages(messageNumber);
            controller.PopulateNodesOnCanvas(simulationEnvironment.GetNodes(), tabIndex);
        }

        [HttpGet, Route("api/main/run/{nodeNumber}/{messageNumber}/{simSpeedNumber}/{nodeRange}/{pureSelfishNodeNumber}/{partialSelfishNodeNumber}/{tabIndex}")]
        public void RunTest(
	        int nodeNumber,
	        int messageNumber,
	        int simSpeedNumber,
	        int tabIndex,
            int nodeRange,
            int pureSelfishNodeNumber,
            int partialSelfishNodeNumber)
        {
            simulationEnvironment.GenerateRandomNodes(nodeNumber, nodeRange, pureSelfishNodeNumber, partialSelfishNodeNumber);        // Generate Random Nodes //<remove once lock fixed
            simulationEnvironment.GenerateRandomMessages(messageNumber);             // Generate Random Messages //<remove once lock fixed
            controller.PopulateNodesOnCanvas(simulationEnvironment.GetNodes(), tabIndex); // Populate Nodes on Canvas //<remove once lock fixed
            simulationEnvironment.RunSimulation(simSpeedNumber, tabIndex);           // Run Simulation on Environment
        }

        [HttpGet, Route("api/main/reset")]
        public void Reset()
        {

        }
    }
}