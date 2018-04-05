using System;
using System.Collections.Generic;
using System.Linq;
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
            sim.SendMessageSADSR(sim.GetMessages()[0], new SessionData(0), 500);
        }

        [HttpGet, Route("api/main/run/{nodeNumber}/{messageNumber}/{simSpeedNumber}/{nodeRange}/{pureSelfishNodeNumber}/{partialSelfishNodeNumber}/{executionNumber}/{tabIndex}")]
        public List<GraphData> RunTest(
	        int nodeNumber,
	        int messageNumber,
	        int simSpeedNumber,
	        int tabIndex,
            int nodeRange,
            int pureSelfishNodeNumber,
            int partialSelfishNodeNumber,
	        int executionNumber)
        {
	        if (executionNumber > 1)
	        {
		        List<GraphData> graphDataList;
				List<List<SessionData>> sessionDataList = new List<List<SessionData>>();
		        int currentTabIndex = tabIndex;

				for (int currentExecutionNumber = 0; currentExecutionNumber < executionNumber; currentExecutionNumber++)
				{
					List<SessionData> currentExecutionSessionDataList = new List<SessionData>();
					List<bool> tabIndexesDone = new List<bool> { false, false, false };
					bool isAllTabsDone = false;

					simulationEnvironment.GenerateRandomNodes(nodeNumber, nodeRange, pureSelfishNodeNumber, partialSelfishNodeNumber);
					simulationEnvironment.GenerateRandomMessages(messageNumber);

			        while (!isAllTabsDone)
			        {
				        if (tabIndexesDone.Any(x => x == false))
				        {
							Hub.Clients.All.resetCanvas(currentTabIndex);

							controller.PopulateNodesOnCanvas(simulationEnvironment.GetNodes(), currentTabIndex);
							var sessionData = simulationEnvironment.RunSimulation(simSpeedNumber, currentTabIndex);
					        currentExecutionSessionDataList.Add(sessionData);

					        tabIndexesDone[currentTabIndex] = true;

					        if (tabIndexesDone.Any(x => x == false))
					        {
						        currentTabIndex = tabIndexesDone.FindIndex(x => x == false);
					        }
				        }
				        else
				        {
					        isAllTabsDone = true;
				        }
			        }

					sessionDataList.Add(currentExecutionSessionDataList);
		        }

		        graphDataList = SessionDataListToGraphDataList(sessionDataList);

		        return graphDataList;
	        }
	        else
	        {
		        simulationEnvironment.GenerateRandomNodes(nodeNumber, nodeRange, pureSelfishNodeNumber, partialSelfishNodeNumber); // Generate Random Nodes //<remove once lock fixed
		        simulationEnvironment.GenerateRandomMessages(messageNumber); // Generate Random Messages //<remove once lock fixed
		        controller.PopulateNodesOnCanvas(simulationEnvironment.GetNodes(), tabIndex); // Populate Nodes on Canvas //<remove once lock fixed
		        simulationEnvironment.RunSimulation(simSpeedNumber, tabIndex); // Run Simulation on Environment

				return new List<GraphData>();
	        }
        }

	    public List<GraphData> SessionDataListToGraphDataList(List<List<SessionData>> sessionDataList)
	    {
			List<GraphData> graphDataList = new List<GraphData>();
			GraphData aeedGraphData = new GraphData() {YAxisTitle = "AEED Value"};
			GraphData nroGraphData = new GraphData() {YAxisTitle = "NRO Value"};
		    GraphData bddGraphData = new GraphData() {YAxisTitle = "BDD Value"};
			GraphData pdrGraphData = new GraphData() {YAxisTitle = "PDR Value"};

		    for (var executionNumber = 0; executionNumber < sessionDataList.Count; executionNumber++)
		    {
			    foreach (var sessionData in sessionDataList[executionNumber])
			    {
				    if (sessionData.tabIndex == 0)
				    {
						aeedGraphData.YAxisValuesDsr.Add(sessionData.CalculateAverageEndToEndDelay());
					    nroGraphData.YAxisValuesDsr.Add(sessionData.CalculateNormalizedRoutingOverhead());
					    bddGraphData.YAxisValuesDsr.Add(sessionData.CalculateBatteryDepletionDeviation());
					    pdrGraphData.YAxisValuesDsr.Add(sessionData.CalculatePacketDeliveryRatio());
				    }
					else if (sessionData.tabIndex == 1)
				    {
					    aeedGraphData.YAxisValuesSadsr.Add(sessionData.CalculateAverageEndToEndDelay());
					    nroGraphData.YAxisValuesSadsr.Add(sessionData.CalculateNormalizedRoutingOverhead());
					    bddGraphData.YAxisValuesSadsr.Add(sessionData.CalculateBatteryDepletionDeviation());
					    pdrGraphData.YAxisValuesSadsr.Add(sessionData.CalculatePacketDeliveryRatio());
				    }
					else if (sessionData.tabIndex == 2)
				    {
					    aeedGraphData.YAxisValuesMsadsr.Add(sessionData.CalculateAverageEndToEndDelay());
					    nroGraphData.YAxisValuesMsadsr.Add(sessionData.CalculateNormalizedRoutingOverhead());
					    bddGraphData.YAxisValuesMsadsr.Add(sessionData.CalculateBatteryDepletionDeviation());
					    pdrGraphData.YAxisValuesMsadsr.Add(sessionData.CalculatePacketDeliveryRatio());
				    }
			    }
		    }

			graphDataList.Add(aeedGraphData);
		    graphDataList.Add(nroGraphData);
		    graphDataList.Add(bddGraphData);
		    graphDataList.Add(pdrGraphData);

		    return graphDataList;
	    }

        [HttpGet, Route("api/main/reset")]
        public void Reset()
        {
			
        }
    }
}