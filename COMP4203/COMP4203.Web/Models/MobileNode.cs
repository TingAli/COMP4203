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
        public int range = 200;
        private int nodeID;
        public double BatteryLevel;
        public int CenterX, CenterY;
        private Dictionary<int, List<Route>> knownRoutes;
        public Guid Id;
        public int CanvasIndex;
        private int ac;
        private bool drop = false;

        private ComponentController controller;

        public MobileNode()
        {
            nodeID = nodeCount++;
            BatteryLevel = 100;
            CenterX = CenterY = 0;
            knownRoutes = new Dictionary<int, List<Route>>();
            Id = Guid.NewGuid();
            CanvasIndex = -1;
            BorderWidth = 2;
            StrokeColour = "#FFFFFF";
            Radius = 10;
            controller = new ComponentController();
            ac = 50;
        }

        public MobileNode(int x, int y, int bLevel, int range)
        {
            nodeID = ++nodeCount;
            BatteryLevel = 100;
            CenterX = x;
            CenterY = y;
            BatteryLevel = bLevel; knownRoutes = new Dictionary<int, List<Route>>();
            Id = Guid.NewGuid();
            CanvasIndex = -1;
            BorderWidth = 2;
            StrokeColour = "#FFFFFF";
            Radius = 10;
            this.range = range;
            controller = new ComponentController();
            ac = 50;
        }

        public int GetAC()
        {
            return ac;
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
            controller.PrintToOutputPane("Note", "Node #" + nodeID + " - Battery Level: " + BatteryLevel +
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
                        controller.PrintToOutputPane("Node_Range", "Node " + n.GetNodeID() + " is within range. Distance: " + GetDistance(n));
                    }
                    else
                    {
                        controller.PrintToOutputPane("Node_Range", "Node " + n.GetNodeID() + " is not within range. Distance: " + GetDistance(n));
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
        // Implement SA-DSR here 
        public List<Route> RouteDiscoverySADSR(MobileNode destNode, SimulationEnvironment env, SessionData sData, int delay)
        {
            Route rPacket = new Route();
            List<Route> routes = SADSRDiscovery(destNode, env, rPacket, sData, delay);
            return routes;
        }
        private List<Route> SADSRDiscovery(MobileNode destNode, SimulationEnvironment env, Route route, SessionData sData, int delay)
        {
            List<Route> routes = new List<Route>();

            if (knownRoutes.ContainsKey(destNode.GetNodeID()))
            {
                foreach (Route r in knownRoutes[destNode.GetNodeID()])
                {
                    Route r2 = route.Copy();
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
                        //Obtaining all possible route
                        if (this.PacketDrop() == true)
                        {
                            route.GetNodeRoute()[0].ac -= 5;
                            controller.PrintToOutputPane("SADSR", string.Format("RREQ dropped by node {0}", nodeID));
                            return null;
                        }
                        if (this.PacketDrop() == false)
                        {
                            route.GetNodeRoute()[0].ac += 5;
                            Route rPacket = route.Copy();
                            rPacket.AddNodeToRoute(this); // Adding nodes to route
                            rPacket.AddNodeToRoute(node);
                            routes.Add(rPacket); // Adding all possible routes
                            controller.PrintToOutputPane("SADSR", string.Format("Sending RREQ from Node {0} to Node {1}.", nodeID, node.GetNodeID()));
                            env.TransmitData(this, node, delay, env.RREQ_COLOUR);
                            sData.numControlPackets++;
                            controller.PrintToOutputPane("SADSR", string.Format("Sending RREP from Node {0} to Node {1}.", node.GetNodeID(), nodeID));
                            env.TransmitData(node, this, delay, env.RREP_COLOUR);
                            sData.numControlPackets++;
                        }
                    }
                    else
                    {
                        if (this.PacketDrop() == true)
                        {
                            route.GetNodeRoute()[0].ac -= 5;
                            controller.PrintToOutputPane("SADSR", string.Format("RREQ dropped by node {0}", nodeID));
                            return null;
                        }
                        if (this.PacketDrop() == false)
                        {
                            Route rPacket = route.Copy();
                            rPacket.AddNodeToRoute(this);
                            controller.PrintToOutputPane("SADSR", string.Format("Sending RREQ from Node {0} to Node {1}.", nodeID, node.GetNodeID()));
                            env.TransmitData(this, node, delay, env.RREQ_COLOUR);
                            sData.numControlPackets++;
                            routes.AddRange(node.SADSRDiscovery(destNode, env, rPacket, sData, delay)); // Recursive call
                        }
                    }
                }
            }
            foreach (Route r in routes)
            {
                if (r.GetNodeRoute().Contains(destNode))
                {
                    List<MobileNode> rList = r.GetNodeRoute();
                    for (int i = 0; i < rList.Count; i++)
                    {
                        if (rList[i] == this && i != 0)
                        {
                            controller.PrintToOutputPane("SADSR", string.Format("Sending RREP from Node {0} to Node {1}.", nodeID, rList[i - 1].GetNodeID()));
                            env.TransmitData(this, rList[i - 1], delay, env.RREP_COLOUR);
                            sData.numControlPackets++;
                        }
                    }
                }
            }
            return routes;
        }
        // Get the optimal route for SA-DSR
        public Route GetOptimalRouteSADSR(MobileNode node)
        {
            List<Route> routes = GetRoutesToNode(node);
            Route optRoute = new Route();
            double sdp = 0;
            if (routes == null) { return null; }
            foreach (Route r in routes)
            {
                if (sdp < r.CalcSDP())
                {
                    sdp = r.getSDP();
                    optRoute = r;
                }
            }
            return optRoute;
        }
        // Calculates the probability of a node dropping a packet based on their battery levels
        public bool PacketDrop()
        {
            Random random = new Random();
            // 50% chance of dropping packet if battery level is between 20 to 50 percent
            if (20 <= this.BatteryLevel && this.BatteryLevel < 50)
            {
                int r = random.Next(0, 1);
                if (r == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            // 10% chance of dropping packet if battery level is between 50 and 80 percent
            else if (50 <= this.BatteryLevel && this.BatteryLevel < 80)
            {
                int r = random.Next(0, 11);
                if (r == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            // 0% chance of dropping packet if battery level is between 80 to 100 percent
            else if (80 <= this.BatteryLevel && this.BatteryLevel <= 100)
            {
                return false;
            }
            else
            {
                // 100% chance of dropping packet if battery level is less than 20 percent 
                return true;
            }
        }
        // DSR implementation
        public List<Route> RouteDiscoveryDSR(MobileNode destNode, SimulationEnvironment env, SessionData sData, int delay)
        {
            controller.PrintToOutputPane("DSR", "Performing Route Discovery from Node " + nodeID + " to Node " + destNode.GetNodeID() + ".");
            Route rPacket = new Route();
            List<Route> routes = DSRDicovery(destNode, env, rPacket, sData, delay);
            if (knownRoutes.ContainsKey(destNode.GetNodeID()))
            {
                foreach (Route r in routes)
                {
                    bool exists = false;
                    foreach (Route r2 in knownRoutes[destNode.GetNodeID()])
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

        private List<Route> DSRDicovery(MobileNode destNode, SimulationEnvironment env, Route route, SessionData sData, int delay)
        {
            List<Route> routes = new List<Route>();

            if (knownRoutes.ContainsKey(destNode.GetNodeID()))
            {
                foreach (Route r in knownRoutes[destNode.GetNodeID()]) // TODO: destNode nullpointer exception jet48
                {
                    Route r2 = route.Copy();
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
                        Route rPacket = route.Copy();
                        rPacket.AddNodeToRoute(this); // Adding nodes to route
                        rPacket.AddNodeToRoute(node);
                        routes.Add(rPacket); // Adding all possible routes
                        controller.PrintToOutputPane("DSR", string.Format("Sending RREQ from Node {0} to Node {1}.", nodeID, node.GetNodeID()));
                        env.TransmitData(this, node, delay, env.RREQ_COLOUR);
                        sData.numControlPackets++;
                        controller.PrintToOutputPane("DSR", string.Format("Sending RREP from Node {0} to Node {1}.", node.GetNodeID(), nodeID));
                        env.TransmitData(node, this, delay, env.RREP_COLOUR);
                        sData.numControlPackets++;
                    }
                    else
                    {
                        Route rPacket = route.Copy();
                        rPacket.AddNodeToRoute(this);
                        controller.PrintToOutputPane("DSR", string.Format("Sending RREQ from Node {0} to Node {1}.", nodeID, node.GetNodeID()));
                        env.TransmitData(this, node, delay, env.RREQ_COLOUR);
                        sData.numControlPackets++;
                        routes.AddRange(node.DSRDicovery(destNode, env, rPacket, sData, delay)); // Recursive call
                    }
                }
            }
            foreach (Route r in routes)
            {
                if (r.GetNodeRoute().Contains(destNode))
                {
                    List<MobileNode> rList = r.GetNodeRoute();
                    for (int i = 0; i < rList.Count; i++)
                    {
                        if (rList[i] == this && i != 0)
                        {
                            controller.PrintToOutputPane("DSR", string.Format("Sending RREP from Node {0} to Node {1}.", nodeID, rList[i-1].GetNodeID()));
                            env.TransmitData(this, rList[i - 1], delay, env.RREP_COLOUR);
                            sData.numControlPackets++;
                        }
                    }
                    
                }
            }
            return routes;
        }

        public List<Route> GetRoutesToNode(MobileNode node)
        {
            // If there are no known routes for this destination, return null.
            if (!knownRoutes.ContainsKey(node.GetNodeID())) { return null; }
            // Otherwise, return the list of known routes.
            return knownRoutes[node.GetNodeID()];
        }

        public Route GetBestRouteDSR(MobileNode node)
        {
            List<Route> routes = GetRoutesToNode(node);
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

        public double GetTransmissionTimeToNode(MobileNode node)
        {
            if (IsWithinRangeOf(node))
            {
                return GetDistance(node) / (3.8 * Math.Pow(10,8));
            }
            return -1;
        }
    }
}