﻿using COMP4203.Web.Controllers;
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