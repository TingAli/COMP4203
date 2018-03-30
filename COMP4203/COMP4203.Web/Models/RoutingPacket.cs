using System.Collections.Generic;

namespace COMP4203.Web.Models
{
    public class RoutingPacket
    {
        private List<MobileNode> nodeRoute;

		public RoutingPacket()
		{
			nodeRoute = new List<MobileNode>();
		}

		public List<MobileNode> GetNodeRoute() => nodeRoute;

        public RoutingPacket Copy()
        {
            RoutingPacket packet = new RoutingPacket();
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

        public bool RouteCompare(RoutingPacket route)
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
    }
}
