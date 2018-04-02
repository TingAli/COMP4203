using COMP4203.Web.Controllers;
using COMP4203.Web.Controllers.Hubs;
using System;
using System.Collections.Generic;

namespace COMP4203.Web.Models
{
	public class MobileNode
    {
        static double TRANSMIT_COST = 0.02;
        static double RECEIVE_PROCESS_COST = 0.01;

        public string FillColour { get; set; }
        public int BorderWidth { get; set; }
        public string StrokeColour { get; set; }
        public int Radius { get; set; }

        static int nodeCount = 0;
        static int range = 200;
        private int nodeID;
        public double BatteryLevel;
        public int CenterX, CenterY;
        private Dictionary<int, List<RoutingPacket>> knownRoutes;
        public Guid Id;
        public int CanvasIndex;

        public MobileNode()
        {
            nodeID = nodeCount++;
            BatteryLevel = 100;
            CenterX = CenterY = 0;
            knownRoutes = new Dictionary<int, List<RoutingPacket>>();
            Id = Guid.NewGuid();
            CanvasIndex = -1;
            BorderWidth = 2;
            StrokeColour = "#FFFFFF";
            Radius = 10;
        }

        public MobileNode(int x, int y, int bLevel)
        {
            nodeID = ++nodeCount;
            BatteryLevel = 100;
            CenterX = x;
            CenterY = y;
            BatteryLevel = bLevel; knownRoutes = new Dictionary<int, List<RoutingPacket>>();
            Id = Guid.NewGuid();
            CanvasIndex = -1;
            BorderWidth = 2;
            StrokeColour = "#FFFFFF";
            Radius = 10;
        }

        public int GetNodeID()
        {
            return nodeID;
        }

        public double GetBatteryLevel()
        {
            return BatteryLevel;
        }

        public int GetXPosition()
        {
            return CenterX;
        }

        public int GetYPosition()
        {
            return CenterY;
        }
        
        public void Print()
        {
            new OutputPaneController().PrintToOutputPane("Note", "Node #" + nodeID + " - Battery Level: " + BatteryLevel +
                " - Location: " + CenterX + "," + CenterY);
        }

        public void PrintNodesWithinRange(SimulationEnvironment env)
        {
            foreach (MobileNode n in env.GetNodes())
            {
                if (!this.Equals(n))
                {
                    if (IsWithinRangeOf(n))
                    {
                        new OutputPaneController().PrintToOutputPane("Node_Range", "Node " + n.GetNodeID() + " is within range. Distance: " + GetDistance(n));
                    }
                    else
                    {
                        new OutputPaneController().PrintToOutputPane("Node_Range", "Node " + n.GetNodeID() + " is not within range. Distance: " + GetDistance(n));
                    }
                }
            }
        }

        public double GetDistance(MobileNode node)
        {
            return Math.Sqrt((Math.Pow((CenterX - node.CenterX), 2)) + (Math.Pow((CenterY - node.CenterY), 2)));
        }

        public void TransmitPacket()
        {
            BatteryLevel -= TRANSMIT_COST;
            if (BatteryLevel < 0) { BatteryLevel = 0; }
        }

        public void ReceiveProcessPacket()
        {
            BatteryLevel -= RECEIVE_PROCESS_COST;
            if (BatteryLevel < 0) { BatteryLevel = 0; }
        }

        public bool IsWithinRangeOf(MobileNode node)
        {
            return (GetDistance(node) < 200);
        }

        public List<MobileNode> GetNodesWithinRange(SimulationEnvironment env)
        {
            List<MobileNode> nodes = new List<MobileNode>();
            foreach (MobileNode node in env.GetNodes())
            {
                if (IsWithinRangeOf(node) && !node.Equals(this) && node.BatteryLevel > 0)
                {
                    nodes.Add(node);
                }
            }
            return nodes;
        }

        public List<RoutingPacket> RouteDiscoveryDSR(MobileNode destNode, SimulationEnvironment env, SessionData sData)
        {
            new OutputPaneController().PrintToOutputPane("DSR", "Performing Route Discovery from Node " + nodeID + " to Node " + destNode.GetNodeID() + ".");
            RoutingPacket rPacket = new RoutingPacket();
            List<RoutingPacket> routes = DSRDicovery(destNode, env, rPacket, sData);
            if (knownRoutes.ContainsKey(destNode.GetNodeID()))
            {
                foreach (RoutingPacket r in routes)
                {
                    bool exists = false;
                    foreach (RoutingPacket r2 in knownRoutes[destNode.GetNodeID()])
                    {
                        if (r2.RouteCompare(r))
                        {
                            exists = true;
                            break;
                        }
                    }
                    if (!exists)
                    {
                        knownRoutes[destNode.GetNodeID()].Add(r);
                    }
                }
            }
            else
            {
                knownRoutes.Add(destNode.GetNodeID(), routes);
            }
            return routes;
        }

        private List<RoutingPacket> DSRDicovery(MobileNode destNode, SimulationEnvironment env, RoutingPacket route, SessionData sData)
        {
            List<RoutingPacket> routes = new List<RoutingPacket>();

            if (knownRoutes.ContainsKey(destNode.GetNodeID()))
            {
                foreach (RoutingPacket r in knownRoutes[destNode.GetNodeID()])
                {
                    RoutingPacket r2 = route.Copy();
                    r2.AddNodesToRoute(r.GetNodeRoute());
                    routes.Add(r2);
                }
                return routes;
            }

            List<MobileNode> nodesWithinRange = GetNodesWithinRange(env);
            if (nodesWithinRange.Count == 0 && !destNode.Equals(this)) { return null; }

            foreach (MobileNode node in nodesWithinRange)
            {
                // If node isn't in route yet...
                if (!route.IsInRouteAlready(node))
                {
                    // If node is the destination node...
                    if (node.Equals(destNode))
                    {
                        //Obtaining all possible routes
                        RoutingPacket rPacket = route.Copy();
                        rPacket.AddNodeToRoute(this); // Adding nodes to route
                        rPacket.AddNodeToRoute(node);
                        routes.Add(rPacket); // Adding all possible routes
                        new OutputPaneController().PrintToOutputPane("DSR", string.Format("Sending RREQ from Node {0} to Node {1}.", nodeID, node.GetNodeID()));
                        env.TransmitData(this, node, 15, env.RREQ_COLOUR);
                        sData.numControlPackets++;
                        new OutputPaneController().PrintToOutputPane("DSR", string.Format("Sending RREP from Node {0} to Node {1}.", node.GetNodeID(), nodeID));
                        env.TransmitData(node, this, 15, env.RREP_COLOUR);
                        sData.numControlPackets++;
                    }
                    else
                    {
                        RoutingPacket rPacket = route.Copy();
                        rPacket.AddNodeToRoute(this);
                        new OutputPaneController().PrintToOutputPane("DSR", string.Format("Sending RREQ from Node {0} to Node {1}.", nodeID, node.GetNodeID()));
                        env.TransmitData(this, node, 15, env.RREQ_COLOUR);
                        sData.numControlPackets++;
                        routes.AddRange(node.DSRDicovery(destNode, env, rPacket, sData)); // Recursive call
                    }
                }
            }
            foreach (RoutingPacket r in routes)
            {
                if (r.GetNodeRoute().Contains(destNode))
                {
                    List<MobileNode> rList = r.GetNodeRoute();
                    for (int i = 0; i < rList.Count; i++)
                    {
                        if (rList[i] == this && i != 0)
                        {
                            new OutputPaneController().PrintToOutputPane("DSR", string.Format("Sending RREP from Node {0} to Node {1}.", nodeID, rList[i-1].GetNodeID()));
                            env.TransmitData(this, rList[i - 1], 15, env.RREP_COLOUR);
                            sData.numControlPackets++;
                        }
                    }
                    
                }
            }
            return routes;
        }

        public List<RoutingPacket> GetRoutesToNode(MobileNode node)
        {
            // If there are no known routes for this destination, return null.
            if (!knownRoutes.ContainsKey(node.GetNodeID())) { return null; }
            // Otherwise, return the list of known routes.
            return knownRoutes[node.GetNodeID()];
        }

        public RoutingPacket GetBestRouteDSR(MobileNode node)
        {
            List<RoutingPacket> routes = GetRoutesToNode(node);
            if (routes == null) { return null; }
            if (routes.Count == 0) { return null; }
            int lowestValue = 99999999;
            int lowestIndex = -1;
            for (int i = 0; i < routes.Count; i++)
            {
                int rLength = routes[i].GetRouteLength();
                if (rLength < lowestValue)
                {
                    lowestValue = rLength;
                    lowestIndex = i;
                }
            }
            return routes[lowestIndex];
        }
    }
}
