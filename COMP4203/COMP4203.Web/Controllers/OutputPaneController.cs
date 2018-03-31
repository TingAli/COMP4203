using COMP4203.Web.Controllers.Extend;
using COMP4203.Web.Controllers.Hubs;
using COMP4203.Web.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace COMP4203.Web.Controllers
{
    public class OutputPaneController : ApiControllerWithHub<MainHub>
    {
        public void PrintToOutputPane(String tag, String message)
        {
            Hub.Clients.All.broadcastOutputMessage(JsonConvert.SerializeObject(new OutputMessage
            {
                Id = Guid.NewGuid(),
                Tag = tag,
                Message = message
            }));
        }

        public void PrintArrow(MobileNode sourceNode, MobileNode destNode)
        {
            Hub.Clients.All.sendMessageBetweenTwoNodes(JsonConvert.SerializeObject(sourceNode), JsonConvert.SerializeObject(destNode));
        }

        public void UpdateBatteryLevel(MobileNode node)
        {
            Hub.Clients.All.updateBatteryLevel(JsonConvert.SerializeObject(node));
        }

        public void PopulateNodesDSR(List<MobileNode> nodes, int canvasIndex)
        {
            foreach (MobileNode node in nodes) {
                node.CanvasIndex = canvasIndex;
                node.FillColour = "#FF0000";
            }
            Hub.Clients.All.populateNodes(JsonConvert.SerializeObject(nodes));
        }
    }
}