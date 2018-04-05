using COMP4203.Web.Controllers;
using COMP4203.Web.Controllers.Hubs;
using System;
using System.Collections.Generic;
using System.Threading;

namespace COMP4203.Web.Models
{
	public class MobileNode
    {
        public enum SelfishType { NOT_SELFISH, PURE_SELFISH, PARTIAL_SELFISH};

        static double TRANSMIT_COST = 0.02;
        static double RECEIVE_PROCESS_COST = 0.01;

        public string RREQ_COLOUR = "#9bc146"; // Green
        public string RREP_COLOUR = "#ffe338"; // Yellow
        public string DATA_COLOUR = "#52a0d0"; // Blue
        public string ACK_COLOUR = "#df1313"; // Red
        public string TWOACK_COLOR = "#000000"; // Black

        public static string NODE_COLOUR_NOT_SELFISH = "#f2bd2b"; // yellow
        public static string NODE_COLOUR_PURE_SELFISH = "#c9052e"; // red
        public static string NODE_COLOUR_PARTIAL_SELFISH = "#66a548"; // green

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
        private int AltruismCoefficient;
        private bool dropStatus = false;
        private bool selfish = false;
        private SelfishType selfishType = SelfishType.NOT_SELFISH;

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
            AltruismCoefficient = 50;
            FillColour = NODE_COLOUR_NOT_SELFISH;
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
            AltruismCoefficient = 50;
            FillColour = NODE_COLOUR_NOT_SELFISH;
        }

        public void SetNotSelfish()
        {
            selfishType = SelfishType.NOT_SELFISH;
            FillColour = NODE_COLOUR_NOT_SELFISH;
        }

        public void SetPureSelfish()
        {
            selfishType = SelfishType.PURE_SELFISH;
            FillColour = NODE_COLOUR_PURE_SELFISH;
        }

        public void SetPartialSelfish()
        {
            selfishType = SelfishType.PURE_SELFISH;
            FillColour = NODE_COLOUR_PARTIAL_SELFISH;
        }

        public Boolean IsSelfish()
        {
            return selfishType == SelfishType.NOT_SELFISH;
        }

        public Boolean IsPartialSelfish()
        {
            return selfishType == SelfishType.PARTIAL_SELFISH;
        }

        public Boolean IsPureSelfish()
        {
            return selfishType == SelfishType.PURE_SELFISH;
        }

        public int GetAltruismCoefficient()
        {
            return AltruismCoefficient;
        }

        public bool GetDropStatus()
        {
            return dropStatus;
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
                        controller.PrintToOutputPane(OutputTag.TAG_NOTE, "Node " + n.GetNodeID() + " is within range. Distance: " + GetDistance(n));
                    }
                    else
                    {
                        controller.PrintToOutputPane(OutputTag.TAG_NOTE, "Node " + n.GetNodeID() + " is not within range. Distance: " + GetDistance(n));
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

        public void SendDataPacket(MobileNode node, int wait, string tag)
        {
            controller.PrintToOutputPane(tag, "Sending DATA from " + nodeID + " to " + node.GetNodeID() + ".");
            TransmitData(this, node, wait, DATA_COLOUR);
        }

        public void SendAckPacket(MobileNode node, int wait, string tag)
        {
            controller.PrintToOutputPane(tag, "Sending ACK from " + nodeID + " to " + node.GetNodeID());
            TransmitData(this, node, wait, ACK_COLOUR);
        }

        public void SendRREQPacket(MobileNode node, int wait, SessionData sessionData, string tag)
        {
            controller.PrintToOutputPane(tag, string.Format("Sending RREQ from Node {0} to Node {1}.", nodeID, node.GetNodeID()));
            TransmitData(this, node, wait, RREQ_COLOUR);
            sessionData.IncrementNumberOfControlPackets();
        }

        public void SendRREPPacket(MobileNode node, int wait, SessionData session, string tag)
        {
            controller.PrintToOutputPane(tag, string.Format("Sending RREP from Node {0} to Node {1}.", node.GetNodeID(), nodeID));
            TransmitData(node, this, wait, RREP_COLOUR);
            session.IncrementNumberOfControlPackets();
        }

        public void SendTWOACKPacket(MobileNode node, int wait, string tag)
        {
            controller.PrintToOutputPane(tag, string.Format("Sending TWOACK from Node {0} to Node {1}.", nodeID, node.GetNodeID()));
            TransmitData(this, node, wait, TWOACK_COLOR);
        }

        public void TransmitData(MobileNode srcNode, MobileNode dstNode, int wait, string colour)
        {
            controller.PrintArrow(srcNode, dstNode, colour);
            Thread.Sleep(wait);
            srcNode.TransmitPacket();
            dstNode.ReceiveProcessPacket();
            controller.UpdateBatteryLevel(srcNode);
            controller.UpdateBatteryLevel(dstNode);
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

        public void TWOACK(List<MobileNode> nodesWithinRange, int delay, int i)
        {
            SendTWOACKPacket(nodesWithinRange[i - 1], delay, "MSADSR");
            if (nodesWithinRange[i - 1].dropStatus == true)
            {
                selfish = true;
            }
            else
            {
                SendTWOACKPacket(nodesWithinRange[i - 2], delay, "MSADSR");
            }
        }
        public List<Route> RouteDiscoverySADSR(MobileNode destNode, SimulationEnvironment env, SessionData sData, int delay)
        {
            Route rPacket = new Route();
            List<Route> routes = SADSRDiscovery(destNode, env, rPacket, sData, delay);
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

                        Route rPacket = route.Copy();
                        rPacket.AddNodeToRoute(this); 
                        rPacket.AddNodeToRoute(node);
                        if (this.PacketDrop() == true)
                        {
                            route.GetNodeRoute()[0].AltruismCoefficient -= 10; // Decreasing altruism coefficient
                            controller.PrintToOutputPane("SADSR", string.Format("RREQ dropped by node {0}", nodeID));
                            dropStatus = true;
                        }
                        if (this.PacketDrop() == false)
                        {
                            route.GetNodeRoute()[0].AltruismCoefficient += 10; // Increase altruism coefficient
                            routes.Add(rPacket); 
                            SendRREQPacket(node, delay, sData, "SADSR");
                            SendRREPPacket(node, delay, sData, "SADSR");
                            dropStatus = false;
                        }
                    }
                    else
                    {
                        Route rPacket = route.Copy();
                        rPacket.AddNodeToRoute(this);
                        if (this.PacketDrop() == true)
                        {
                            controller.PrintToOutputPane("SADSR", string.Format("RREQ dropped by node {0}", nodeID));
                            dropStatus = true;
                        }
                        if (this.PacketDrop() == false)
                        {
                            SendRREQPacket(node, delay, sData, "SADSR");
                            dropStatus = false;
                        }
                        routes.AddRange(node.SADSRDiscovery(destNode, env, rPacket, sData, delay)); 
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
                        if (rList[i] == this && rList[i].dropStatus == true)
                        {
                            controller.PrintToOutputPane("SADSR", string.Format("Node {0} has dropped a packet", nodeID));
                            break;
                        }
                        if (rList[i] == this && i != 0)
                        {
                            controller.PrintToOutputPane("SADSR", string.Format("Sending RREP from Node {0} to Node {1}.", nodeID, rList[i - 1].GetNodeID()));
                            env.TransmitData(this, rList[i - 1], delay, env.RREP_COLOUR);
                            sData.IncrementNumberOfControlPackets();
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
                controller.PrintToOutputPane("SADSR", string.Format("SDP is {0}", r.CalcSDP()));
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
            controller.PrintToOutputPane(OutputTag.TAG_DSR, "Performing Route Discovery from Node " + nodeID + " to Node " + destNode.GetNodeID() + ".");
            /* Perform Recursive Route Discovery to Collect all Valid Routes to the Destination */
            List<Route> routes = DSRDicovery(destNode, env, new Route(), sData, delay);
            /* If there are already known routes, add if unique */
            if (knownRoutes.ContainsKey(destNode.GetNodeID()))
            {
                foreach (Route r in routes)
                {
                    bool exists = false;
                    foreach (Route r2 in knownRoutes[destNode.GetNodeID()]) // for each route in the routing table corresponding to the destination node
                    {
                        if (r2.RouteCompare(r)) // if they are equivalent routes, don't bother adding to routing table
                        {
                            exists = true;
                            break;
                        }
                    }
                    if (!exists) // if the route isn't in the routing table, add it
                    {
                        knownRoutes[destNode.GetNodeID()].Add(r);
                    }
                }
            }
            /* Otherwise, add all routes to routing table */
            else
            {
                knownRoutes.Add(destNode.GetNodeID(), routes);
            }
            return routes;
        }

        private List<Route> DSRDicovery(MobileNode destNode, SimulationEnvironment env, Route route, SessionData sData, int delay)
        {
            List<Route> routes = new List<Route>();     // List to hold routes from this node to the destination
            
            /* Collect all known routes from here */
            /* If there are already known routes to the destination node */
            if (knownRoutes.ContainsKey(destNode.GetNodeID()) && knownRoutes[destNode.GetNodeID()] != null)
            {   // for each known route to the destination...
                foreach (Route r in knownRoutes[destNode.GetNodeID()])
                {   // create copy of route, add current node, and add to routes list
                    Route r2 = route.Copy();
                    r2.AddNodesToRoute(r.GetNodeRoute());
                    routes.Add(r2);
                }
                return routes;
            }

            List<MobileNode> nodesWithinRange = GetNodesWithinRange(env);
            if (nodesWithinRange.Count == 0 && !destNode.Equals(this)) { return null; }
            /* For all nodes within range... */
            foreach (MobileNode node in nodesWithinRange)
            {
                // If node isn't in route yet...
                if (!route.IsInRouteAlready(node))
                {
                    // If node is the destination node...
                    if (node.Equals(destNode))
                    {
                        // Add current node and dest node to route
                        Route rPacket = route.Copy();
                        rPacket.AddNodeToRoute(this);
                        rPacket.AddNodeToRoute(node);
                        // Add new route to routes collection
                        routes.Add(rPacket);
                        /* Send RREQ from current node to the destination node */
                        SendRREQPacket(node, delay, sData, OutputTag.TAG_DSR);
                        /* Send RREQ from destination node to the current node */
                        node.SendRREPPacket(this, delay, sData, OutputTag.TAG_DSR);
                    }
                    // If node is not the destination node...
                    else
                    {
                        // Add current node to the route
                        Route rPacket = route.Copy();
                        rPacket.AddNodeToRoute(this);
                        /* Send RREQ from this node to node */
                        SendRREQPacket(node, delay, sData, OutputTag.TAG_DSR);
                        /* Recursively perform discovery from this node, and collect all returned valid routes */
                        routes.AddRange(node.DSRDicovery(destNode, env, rPacket, sData, delay));
                    }
                }
            }

            /* Iterate through valid found routes, performing RREP returns */
            foreach (Route r in routes)
            {
                if (r.GetNodeRoute().Contains(destNode)) // redundancy check
                {
                    List<MobileNode> rList = r.GetNodeRoute();
                    for (int i = 0; i < rList.Count; i++)
                    {
                        if (rList[i] == this && i != 0)
                        {
                            SendRREPPacket(rList[i-1], delay, sData, OutputTag.TAG_DSR);
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