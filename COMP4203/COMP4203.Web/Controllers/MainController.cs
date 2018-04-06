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

		        graphDataList = SessionDataListToGraphDataList(sessionDataList, executionNumber);

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

	    private List<GraphData> SessionDataListToGraphDataList(
		    List<List<SessionData>> sessionDataList,
		    int executionNumber)
	    {
			List<GraphData> graphDataList = new List<GraphData>();
			GraphData aeedGraphData = new GraphData() {YAxisTitle = "AEED Value", Executions = executionNumber, XAxisExecutionsNumber = Enumerable.Range(1, executionNumber).ToList()};
			GraphData nroGraphData = new GraphData() {YAxisTitle = "NRO Value", Executions = executionNumber, XAxisExecutionsNumber = Enumerable.Range(1, executionNumber).ToList()};
		    GraphData bddGraphData = new GraphData() {YAxisTitle = "BDD Value", Executions = executionNumber, XAxisExecutionsNumber = Enumerable.Range(1, executionNumber).ToList()};
			GraphData pdrGraphData = new GraphData() {YAxisTitle = "PDR Value", Executions = executionNumber, XAxisExecutionsNumber = Enumerable.Range(1, executionNumber).ToList()};

		    for (var executionNum = 0; executionNum < sessionDataList.Count; executionNum++)
		    {
			    foreach (var sessionData in sessionDataList[executionNum])
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

		    foreach (var graphData in graphDataList)
		    {
			    double averageDsr = graphData.YAxisValuesDsr.Where(x => x != -1).Average();
			    double averageSadsr = graphData.YAxisValuesSadsr.Where(x => x != -1).Average();
			    double averageMsadsr = graphData.YAxisValuesMsadsr.Where(x => x != -1).Average();

			    graphData.AverageDsr = averageDsr;
			    graphData.AverageSadsr = averageSadsr;
			    graphData.AverageMsadsr = averageMsadsr;
		    }

		    return graphDataList;
	    }

        [HttpGet, Route("api/main/reset")]
        public void Reset()
        {
			
        }
    }
}