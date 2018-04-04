using COMP4203.Web.Controllers;
using COMP4203.Web.Controllers.Hubs;
using System;
using System.Collections.Generic;
using System.Threading;

namespace COMP4203.Web.Models
{
	public class MobileNode
    {
        static double TRANSMIT_COST = 0.02;
        static double RECEIVE_PROCESS_COST = 0.01;

        public string RREQ_COLOUR = "#9bc146"; // Green
        public string RREP_COLOUR = "#ffe338"; // Yellow
        public string DATA_COLOUR = "#52a0d0"; // Blue
        public string ACK_COLOUR = "#df1313"; // Red

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

        public void SendDataPacket(MobileNode node, int wait)
        {
            controller.PrintToOutputPane(OutputTag.TAG_DSR, "Sending DATA from " + nodeID + " to " + node.GetNodeID() + ".");
            TransmitData(this, node, wait, DATA_COLOUR);
        }

        public void SendAckPacket(MobileNode node, int wait)
        {
            controller.PrintToOutputPane(OutputTag.TAG_DSR, "Sending ACK from " + nodeID + " to " + node.GetNodeID());
            TransmitData(this, node, wait, ACK_COLOUR);

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

        public List<Route> RouteDiscoveryDSR(MobileNode destNode, SimulationEnvironment env, SessionData sData, int delay)
        {
            controller.PrintToOutputPane(OutputTag.TAG_DSR, "Performing Route Discovery from Node " + nodeID + " to Node " + destNode.GetNodeID() + ".");
            /* Perform Recursive Route Discovery to Collect all Valid Routes to the Destination */
            List<Route> routes = DSRDicovery(destNode, env, new Route(), sData, delay);
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
                        controller.PrintToOutputPane(OutputTag.TAG_DSR, string.Format("Sending RREQ from Node {0} to Node {1}.", nodeID, node.GetNodeID()));
                        env.TransmitData(this, node, delay, env.RREQ_COLOUR);
                        sData.IncrementNumberOfControlPackets();
                        controller.PrintToOutputPane(OutputTag.TAG_DSR, string.Format("Sending RREP from Node {0} to Node {1}.", node.GetNodeID(), nodeID));
                        env.TransmitData(node, this, delay, env.RREP_COLOUR);
                        sData.IncrementNumberOfControlPackets();
                    }
                    else
                    {
                        Route rPacket = route.Copy();
                        rPacket.AddNodeToRoute(this);
                        controller.PrintToOutputPane(OutputTag.TAG_DSR, string.Format("Sending RREQ from Node {0} to Node {1}.", nodeID, node.GetNodeID()));
                        env.TransmitData(this, node, delay, env.RREQ_COLOUR);
                        sData.IncrementNumberOfControlPackets();
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
                            controller.PrintToOutputPane(OutputTag.TAG_DSR, string.Format("Sending RREP from Node {0} to Node {1}.", nodeID, rList[i-1].GetNodeID()));
                            env.TransmitData(this, rList[i - 1], delay, env.RREP_COLOUR);
                            sData.IncrementNumberOfControlPackets();
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