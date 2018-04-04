using COMP4203.Web.Controllers;
using COMP4203.Web.Controllers.Hubs;
using System;
using System.Collections.Generic;
using System.Threading;

namespace COMP4203.Web.Models
{
    public class SimulationEnvironment
    {
        public string RREQ_COLOUR = "#9bc146"; // Green
        public string RREP_COLOUR = "#ffe338"; // Yellow
        public string DATA_COLOUR = "#52a0d0"; // Blue
        public string ACK_COLOUR = "#df1313"; // Red

        private int height, width;
        private List<MobileNode> mobileNodes;
        private List<Message> messages;
        private List<SessionData> sessions;

        private ComponentController controller;

        public SimulationEnvironment()
        {
            height = 500;
            width = 500;
            mobileNodes = new List<MobileNode>();
            messages = new List<Message>();
            sessions = new List<SessionData>();
            controller = new ComponentController();
        }

        public int GetHeight() { return height; }
        public int GetWidth() { return width; }
        public List<MobileNode> GetNodes() { return mobileNodes; }
        public List<Message> GetMessages() { return messages; }

        public void RunSimulation(int delay)
        {
            SessionData sessionData = new SessionData();
            sessionData.SetStartingBatteryLevels(mobileNodes);
            sessionData.numDataPacketsSent = messages.Count;
            foreach (Message message in messages)
            {
                SendMessageDSR(message, sessionData, delay);
            }
            sessionData.SetEndingBatteryLevels(mobileNodes);
            controller.PrintToOutputPane("DSR", "Finished Transmitting Messages.");
            controller.PrintToOutputPane("DSR", messages.Count + " messages transmitted.");
            controller.PrintToOutputPane("DSR_Results", "# of control packets: " + sessionData.numControlPackets);
            controller.PrintToOutputPane("DSR_Results", "# of data packets sent: " + sessionData.numDataPacketsSent);
            controller.PrintToOutputPane("DSR_Results", "# of data packets received: " + sessionData.numDataPacketsReceived);
            controller.PrintToOutputPane("DSR_Results", "PDR: " + sessionData.CalculatePacketDeliveryRatio());
            controller.PrintToOutputPane("DSR_Results", "AEED: " + sessionData.CalculateAverageEndToEndDelay());
            controller.PrintToOutputPane("DSR_Results", "NRO: " + sessionData.CalculateNormalizedRoutingOverhead());
            controller.PrintToOutputPane("DSR_Results", "BDD: " + sessionData.CalculateBatteryDepletionDeviation());
            sessions.Add(sessionData);
        }

        public void AddNode(MobileNode node)
        {
            mobileNodes.Add(node);
        }

        public void AddMessage(Message message)
        {
            messages.Add(message);
        }

        public void GenerateRandomNodes(int numNodes, int range)
        {
            Random random = new Random();
            for (int i = 0; i < numNodes; i++)
            {
                int x = random.Next(width);
                int y = random.Next(height);
                int bLevel = random.Next(100);
                mobileNodes.Add(new MobileNode(x, y, bLevel, range));
            }
        }

        public void GenerateRandomMessages(int numMessages)
        {
            Random random = new Random();
            for (int i = 0; i < numMessages; i++)
            {
                int srcIndex = random.Next(mobileNodes.Count);
                int dstIndex = srcIndex;
                while (dstIndex == srcIndex)
                {
                    dstIndex = random.Next(mobileNodes.Count);
                }
                messages.Add(new Message(mobileNodes[srcIndex], mobileNodes[dstIndex]));
            }
        }

        public bool SendMessageDSR(Message message, SessionData sData, int delay)
        {
            MobileNode sourceNode = message.GetSourceNode();
            MobileNode destinationNode = message.GetDestinstationNode();
            RoutingPacket route = sourceNode.GetBestRouteDSR(destinationNode);

            /* If no known route, attempt to find one */
            if (route == null)
            {
                controller.PrintToOutputPane("DSR", "No Known Route to Destination.");
                /* Perform Route Discovery */
                sourceNode.RouteDiscoveryDSR(destinationNode, this, sData, delay);
                route = sourceNode.GetBestRouteDSR(destinationNode);
                /* If no route found, abort transfer */
                if (route == null)
                {
                    controller.PrintToOutputPane("DSR", "No Route to Destination.");
                    return false;
                }
            }

            controller.PrintToOutputPane("DSR", string.Format("Sending Message #{0}:", message.GetMessageID()));
            controller.PrintToOutputPane("DSR", "Source Node: " + sourceNode.GetNodeID());
            controller.PrintToOutputPane("DSR", "Destination Node: " + destinationNode.GetNodeID());
            controller.PrintToOutputPane("DSR", "Route Chosen: " + route.GetRouteAsString());

            List<MobileNode> nodes = route.GetNodeRoute();

            /* Send DATA Packet */
            controller.PrintToOutputPane("DSR", "Beginning Message Transmission from Source Node " + sourceNode.GetNodeID());
            for (int i = 1; i < nodes.Count; i++)
            {
                controller.PrintToOutputPane("DSR", "Sending Message from " + nodes[i - 1].GetNodeID() + " to " + nodes[i].GetNodeID() + ".");
                TransmitData(nodes[i - 1], nodes[i], delay*4, DATA_COLOUR);
            
            }
            controller.PrintToOutputPane("DSR", "Received Message at Destination Node " + destinationNode.GetNodeID());
            sData.numDataPacketsReceived += 1;

            /* Send ACK Packet */
            controller.PrintToOutputPane("DSR", "Beginning ACK Transmission from Destination Node " + destinationNode.GetNodeID());
            for (int i = nodes.Count-2; i >=0; i--)
            {
                controller.PrintToOutputPane("DSR", "Sending ACK from " + nodes[i + 1].GetNodeID() + " to " + nodes[i].GetNodeID());
                TransmitData(nodes[i + 1], nodes[i], delay*4, ACK_COLOUR);
                sData.numControlPackets += 1;
            }
            controller.PrintToOutputPane("DSR", "Received ACK at Source Node " + sourceNode.GetNodeID());

            /* Calculate End-To-End Delay */
            sData.endToEndDelays.Add((route.GetNodeRoute().Count-1)*4);
            return true;
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
    }
}
