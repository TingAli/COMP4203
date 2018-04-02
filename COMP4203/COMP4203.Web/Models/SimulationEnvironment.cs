using COMP4203.Web.Controllers;
using COMP4203.Web.Controllers.Hubs;
using System;
using System.Collections.Generic;
using System.Threading;

namespace COMP4203.Web.Models
{
    public class SimulationEnvironment
    {
        private int height, width;
        private List<MobileNode> mobileNodes;
        private List<Message> messages;
        private List<SessionData> sessions;

        public string RREQ_COLOUR = "#9bc146"; // Green
        public string RREP_COLOUR = "#ffe338"; // Yellow
        public string DATA_COLOUR = "#52a0d0"; // Blue
        public string ACK_COLOUR = "#df1313"; // Red

        public SimulationEnvironment()
        {
            height = 500;
            width = 500;
            mobileNodes = new List<MobileNode>();
            messages = new List<Message>();
            sessions = new List<SessionData>();
        }

        public int GetHeight() { return height; }
        public int GetWidth() { return width; }
        public List<MobileNode> GetNodes() { return mobileNodes; }
        public List<Message> GetMessages() { return messages; }

        public void RunSimulation()
        {
            SessionData sessionData = new SessionData();
            sessionData.SetStartingBatteryLevels(mobileNodes);
            foreach (Message message in messages)
            {
                SendMessageDSR(message, sessionData);
            }
            sessionData.SetEndingBatteryLevels(mobileNodes);
            new OutputPaneController().PrintToOutputPane("DSR", "Finished Transmitting Messages.");
            new OutputPaneController().PrintToOutputPane("DSR", messages.Count + " messages transmitted.");
            new OutputPaneController().PrintToOutputPane("DSR_Results", "# of control packets: " + sessionData.numControlPackets);
            new OutputPaneController().PrintToOutputPane("DSR_Results", "# of data packets sent: " + sessionData.numDataPacketsSent);
            new OutputPaneController().PrintToOutputPane("DSR_Results", "# of data packets received: " + sessionData.numDataPacketsReceived);
            new OutputPaneController().PrintToOutputPane("DSR_Results", "# of required transmission packets: " + sessionData.totalRequiredPacket);
            new OutputPaneController().PrintToOutputPane("DSR_Results", "PDR: " + sessionData.CalculatePacketDeliveryRatio());
            //new OutputPaneController().PrintToOutputPane("DSR_Results", "AEED: " + sessionData.CalculateAverageEndToEndDelay());
            new OutputPaneController().PrintToOutputPane("DSR_Results", "NRO: " + sessionData.CalculateNormalizedRoutingOverhead());
            new OutputPaneController().PrintToOutputPane("DSR_Results", "BDD: " + sessionData.CalculateBatteryDepletionDeviation());
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

        public void GenerateRandomNodes(int numNodes)
        {
            Random random = new Random();
            for (int i = 0; i < numNodes; i++)
            {
                int x = random.Next(width);
                int y = random.Next(height);
                int bLevel = random.Next(100);
                mobileNodes.Add(new MobileNode(x, y, bLevel));
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

        public bool SendMessageDSR(Message message, SessionData sData)
        {
            MobileNode sourceNode = message.GetSourceNode();
            MobileNode destinationNode = message.GetDestinstationNode();
            RoutingPacket route = sourceNode.GetBestRouteDSR(destinationNode);

            // If no known route found
            if (route == null)
            {
                new OutputPaneController().PrintToOutputPane("DSR", "No Known Route to Destination.");
                sourceNode.RouteDiscoveryDSR(destinationNode, this, sData); // Perform Route Discovery
                route = sourceNode.GetBestRouteDSR(destinationNode); // Attempt to assign newly found best route
                if (route == null)
                {
                    new OutputPaneController().PrintToOutputPane("DSR", "No Route to Destination.");
                    return false;
                }
            }

            new OutputPaneController().PrintToOutputPane("DSR", "Sending Message:");
            new OutputPaneController().PrintToOutputPane("DSR", "Source Node: " + sourceNode.GetNodeID());
            new OutputPaneController().PrintToOutputPane("DSR", "Destination Node: " + destinationNode.GetNodeID());
            new OutputPaneController().PrintToOutputPane("DSR", "Route Chosen: " + route.GetRouteAsString());

            List<MobileNode> nodes = route.GetNodeRoute();
            new OutputPaneController().PrintToOutputPane("DSR", "Beginning Message Transmission from Source Node " + sourceNode.GetNodeID());
            sData.numDataPacketsSent += 1;
            sData.totalRequiredPacket += 1;
            for (int i = 1; i < nodes.Count; i++)
            {
                new OutputPaneController().PrintToOutputPane("DSR", "Sending Message from " + nodes[i - 1].GetNodeID() + " to " + nodes[i].GetNodeID() + ".");
                TransmitData(nodes[i - 1], nodes[i], 2000, DATA_COLOUR);
            
            }
            new OutputPaneController().PrintToOutputPane("DSR", "Received Message at Destination Node " + destinationNode.GetNodeID());
            sData.numDataPacketsReceived += 1;
            sData.totalRequiredPacket += 1;

            new OutputPaneController().PrintToOutputPane("DSR", "Beginning ACK Transmission from Destination Node " + destinationNode.GetNodeID());
            for (int i = nodes.Count-2; i >=0; i--)
            {
                new OutputPaneController().PrintToOutputPane("DSR", "Sending ACK from " + nodes[i + 1].GetNodeID() + " to " + nodes[i].GetNodeID());
                TransmitData(nodes[i + 1], nodes[i], 500, ACK_COLOUR);
                sData.numControlPackets += 1;
                sData.totalRequiredPacket += 1;
            }
            new OutputPaneController().PrintToOutputPane("DSR", "Received ACK at Source Node " + sourceNode.GetNodeID());
            return true;
        }

        public void TransmitData(MobileNode srcNode, MobileNode dstNode, int wait, string colour)
        {
            new OutputPaneController().PrintArrow(srcNode, dstNode, colour);
            Thread.Sleep(wait);
            srcNode.TransmitPacket();
            dstNode.ReceiveProcessPacket();
            new OutputPaneController().UpdateBatteryLevel(srcNode);
            new OutputPaneController().UpdateBatteryLevel(dstNode);
        }
    }
}
