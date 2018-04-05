using System.Collections.Generic;

namespace COMP4203.Web.Models
{
    public class Route
    {
        private List<MobileNode> nodeRoute;

        private double sdp;

		public Route()
		{
			nodeRoute = new List<MobileNode>();
            sdp = 0;
		}

        // Use to retrieve route's SDP
        public double getSDP()
        {
            return sdp;
        }

        // Used to calculate route's SDP
        public double CalcSDP()
        {
            // Calculated as selfishness level times ac 
            // For each route, calculate the average battery level
            // Convert the average battery level of a route into a percentage and multiply it by the route's ac 
            double avgBatteryLevel = 0;
            double avgAltruismCoefficient = 0;
            foreach (MobileNode node in this.nodeRoute)
            {
                avgBatteryLevel += node.GetBatteryLevel();
                avgAltruismCoefficient += node.GetAltruismCoefficient();
            }
            avgAltruismCoefficient = (avgAltruismCoefficient / this.nodeRoute.Count) / 100;
            avgBatteryLevel = (avgBatteryLevel / this.nodeRoute.Count) / 100;
            sdp = avgAltruismCoefficient * avgBatteryLevel;
            return sdp;
        }

        public List<MobileNode> GetNodeRoute() => nodeRoute;

        public Route Copy()
        {
            Route packet = new Route();
            foreach (MobileNode node in nodeRoute)
            {
                packet.AddNodeToRoute(node);
            }
            return packet;
        }

        public void AddNodeToRoute(MobileNode node)
        {
            nodeRoute.Add(node);
        }

        public void AddNodesToRoute(List<MobileNode> nodes)
        {
            nodeRoute.AddRange(nodes);
        }

        public bool IsInRouteAlready(MobileNode node)
        {
            return nodeRoute.Contains(node);
        }

        public bool RouteCompare(Route route)
        {
            if (nodeRoute.Count != route.GetNodeRoute().Count)
            {
                return false;
            } else
            {
                for (int i = 0; i < nodeRoute.Count; i++)
                {
                    if (!nodeRoute[i].Equals(route.GetNodeRoute()[i]))
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public int GetRouteLength()
        {
            return nodeRoute.Count;
        }
        
        public string GetRouteAsString()
        {
            string output = "";
            foreach (MobileNode node in nodeRoute)
            {
                output += node.GetNodeID() + " ";
            }
            return output;
        }

        public double GetTransmissionTime()
        {
            double transmissionTime = 0;
            for (int i = 0; i < nodeRoute.Count - 1; i++)
            {
                transmissionTime += nodeRoute[i].GetTransmissionTimeToNode(nodeRoute[i+1]);
            }
            return transmissionTime;
        }
    }
}
