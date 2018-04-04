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
    public class ComponentController : ApiControllerWithHub<MainHub>
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

        public void PrintArrow(MobileNode sourceNode, MobileNode destNode, string colour)
        {
            Hub.Clients.All.sendMessageBetweenTwoNodes(JsonConvert.SerializeObject(sourceNode), JsonConvert.SerializeObject(destNode), colour);
        }

        public void UpdateBatteryLevel(MobileNode node)
        {
            Hub.Clients.All.updateBatteryLevel(JsonConvert.SerializeObject(node));
        }

        public void PopulateNodesOnCanvas(List<MobileNode> nodes, int canvasIndex)
        {
            foreach (MobileNode node in nodes) {
                node.CanvasIndex = canvasIndex;
                node.FillColour = "#FF0000";
            }
            Hub.Clients.All.populateNodes(JsonConvert.SerializeObject(nodes));
        }

        public void MarkTransferAsComplete(int tabIndex)
        {
            Hub.Clients.All.completeRun(tabIndex);
        }
    }
}