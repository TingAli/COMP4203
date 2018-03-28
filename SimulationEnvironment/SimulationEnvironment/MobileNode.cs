﻿using System;
using System.Collections.Generic;

namespace SimulationProtocols
{
    class MobileNode
    {
        static int nodeCount = 0;
        static int range = 200;
        private int nodeID;
        private int batteryLevel;
        private int xPosition, yPosition;
        private double sent, received;
        private double numRPackets;
        private Dictionary<int, List<RoutingPacket>> knownRoutes;

        public MobileNode()
        {
            nodeID = nodeCount++;
            batteryLevel = 100;
            xPosition = yPosition = 0;
            sent = received = 0;
            numRPackets = 0;
            knownRoutes = new Dictionary<int, List<RoutingPacket>>();
        }

        public MobileNode(int x, int y, int bLevel)
        {
            nodeID = ++nodeCount;
            xPosition = x;
            yPosition = y;
            sent = received = 0;
            numRPackets = 0;
            batteryLevel = bLevel; knownRoutes = new Dictionary<int, List<RoutingPacket>>();
        }

        public double GetNumRPackets()
        {
            foreach (var k in knownRoutes.Keys)
            {
                numRPackets += knownRoutes[k].Count;
            }
            return numRPackets;
        }

        public int GetNodeID()
        {
            return nodeID;
        }

        public int GetBatteryLevel()
        {
            return batteryLevel;
        }

        public int GetXPosition()
        {
            return xPosition;
        }

        public int GetYPosition()
        {
            return yPosition;
        }

        public double getNumberOfSentPackets()
        {
            return sent;
        }

        public double getNumberOfReceivedPackets()
        {
            return received;
        }

        public void Print()
        {
            Console.WriteLine("Node " + nodeID + ":");
            Console.WriteLine("Battery: " + batteryLevel + "%");
            Console.WriteLine("Location: " + xPosition + ", " + yPosition);
        }

        public double GetDistance(MobileNode node)
        {
            return Math.Sqrt((Math.Pow((xPosition - node.xPosition), 2)) + (Math.Pow((yPosition - node.yPosition), 2)));
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
                if (IsWithinRangeOf(node) && !node.Equals(this))
                {
                    nodes.Add(node);
                }
            }
            return nodes;
        }
        // Get all the known routes
        public List<RoutingPacket> DSRRouteDiscovery(MobileNode destNode, SimulationEnvironment env)
        {
            RoutingPacket rPacket = new RoutingPacket();
            List<RoutingPacket> routes = DSRDicovery(destNode, env, rPacket);
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
        private List<RoutingPacket> DSRDicovery(MobileNode destNode, SimulationEnvironment env, RoutingPacket route)
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
                if (!route.IsInRouteAlready(node))
                {
                    if (node.Equals(destNode))
                    {
                        RoutingPacket rPacket = route.Copy();
                        rPacket.AddNodeToRoute(this);
                        rPacket.AddNodeToRoute(node);
                        routes.Add(rPacket);
                    }
                    else
                    {
                        RoutingPacket rPacket = route.Copy();
                        rPacket.AddNodeToRoute(this);
                        routes.AddRange(node.DSRDicovery(destNode, env, rPacket));
                    }
                }
            }
            return routes;
        }

        public bool DSRSendMessage(Message message, RoutingPacket route)
        {
            this.sent++;
            return DSRSMessage(message, route, 0);
        }

        public bool DSRSMessage(Message message, RoutingPacket route, int step)
        {
            if (message.GetDestinstationNode().Equals(this))
            {
                // Record the message received
                this.received++;
                message.calcMsgSpeed(message.GetSourceNode(), message.GetDestinstationNode());
                Console.WriteLine("Message {0} received at Node {1}.", message.GetMessageID(), nodeID);
                return true;
            }
            else
            {
                // Here randomly get a node to drop the message
                Random random = new Random();
                int r = random.Next(0, 2);
                if (r == 0) {
                    Console.WriteLine("Message {0} dropped by Node {1}.", message.GetMessageID(), nodeID);
                    return false;
                } 
                Console.WriteLine("Relaying Message {0} through Node {1}.", message.GetMessageID(), nodeID);
                return route.GetNodeRoute()[++step].DSRSMessage(message, route, ++step);
            }
        }
    }
}