﻿using COMP4203.Web.Controllers;
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

        public void RunSimulation(int delay, int tabIndex)
        {
            SessionData sessionData = new SessionData();
            sessionData.SetStartingBatteryLevels(mobileNodes);              // Save starting battery levels
            sessionData.SetNumberOfAttemptedTransmissions(messages.Count);  // Save number of attempted transmissions

            /* Send all messages */
            foreach (Message message in messages)
            {
                SendMessageDSR(message, sessionData, delay);
            }

            sessionData.SetEndingBatteryLevels(mobileNodes);    // Save ending battery levels
            sessionData.PrintResults();     // Print collected metrics
            sessions.Add(sessionData);      // Add collected data to recorded sessions
            controller.PrintToOutputPane(OutputTag.TAG_DSR, sessionData.GetNumberOfAttemptedTransmissions() + " attempted transmissions.");
            controller.PrintToOutputPane(OutputTag.TAG_DSR, sessionData.GetNumberOfSuccessfulTranmissions() + " successful transmissions.");
            controller.PrintToOutputPane(OutputTag.TAG_DSR, "Finished Transmitting Messages.");
            controller.MarkTransferAsComplete(tabIndex);
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

        public bool SendMessageSADSR(Message message, SessionData sData, int delay)
        {
            MobileNode sourceNode = message.GetSourceNode();
            MobileNode destinationNode = message.GetDestinstationNode();
            Route route = sourceNode.GetOptimalRouteSADSR(destinationNode);

            /* If no known route, attempt to find one */
            if (route == null)
            {
                controller.PrintToOutputPane("SADSR", "No Known Route to Destination");
                sourceNode.RouteDiscoverySADSR(destinationNode, this, sData, delay);
                route = sourceNode.GetOptimalRouteSADSR(destinationNode);
                if (route == null)
                {
                    controller.PrintToOutputPane("SADSR", "No Route to Destination.");
                    return false;
                }
            }
            controller.PrintToOutputPane("SADSR", string.Format("Sending Message #{0}:", message.GetMessageID()));
            controller.PrintToOutputPane("SADSR", "Source Node: " + sourceNode.GetNodeID());
            controller.PrintToOutputPane("SADSR", "Destination Node: " + destinationNode.GetNodeID());
            controller.PrintToOutputPane("SADSR", "Route Chosen: " + route.GetRouteAsString());

            List<MobileNode> nodes = route.GetNodeRoute();

            /* Send DATA Packet */
            controller.PrintToOutputPane("SADSR", "Beginning Message Transmission from Source Node " + sourceNode.GetNodeID());
            for (int i = 1; i < nodes.Count; i++)
            {
                controller.PrintToOutputPane("SADSR", "Sending Message from " + nodes[i - 1].GetNodeID() + " to " + nodes[i].GetNodeID() + ".");
                TransmitData(nodes[i - 1], nodes[i], delay * 4, DATA_COLOUR);
            }
            controller.PrintToOutputPane("SADSR", "Received Message at Destination Node " + destinationNode.GetNodeID());
            sData.IncrementNumberOfSuccessfulTransmissions();

            /* Send ACK Packet */
            controller.PrintToOutputPane("SADSR", "Beginning ACK Transmission from Destination Node " + destinationNode.GetNodeID());
            for (int i = nodes.Count - 2; i >= 0; i--)
            {
                controller.PrintToOutputPane("SADSR", "Sending ACK from " + nodes[i + 1].GetNodeID() + " to " + nodes[i].GetNodeID());
                TransmitData(nodes[i + 1], nodes[i], delay * 4, ACK_COLOUR);
                sData.IncrementNumberOfControlPackets();
            }
            controller.PrintToOutputPane("SADSR", "Received ACK at Source Node " + sourceNode.GetNodeID());

            /* Calculate End-To-End Delay */
            sData.endToEndDelays.Add((route.GetNodeRoute().Count - 1) * 4);
            return true;
        }

        public bool SendMessageDSR(Message message, SessionData sData, int delay)
        {
            MobileNode sourceNode = message.GetSourceNode();
            MobileNode destinationNode = message.GetDestinstationNode();
            Route route = sourceNode.GetBestRouteDSR(destinationNode);

            /* If no known route, attempt to find one */
            if (route == null)
            {
                controller.PrintToOutputPane(OutputTag.TAG_DSR, "No Known Route to Destination.");
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

            controller.PrintToOutputPane(OutputTag.TAG_DSR, string.Format("Sending Message #{0}.", message.GetMessageID()));
            controller.PrintToOutputPane(OutputTag.TAG_DSR, "Source Node: " + sourceNode.GetNodeID());
            controller.PrintToOutputPane(OutputTag.TAG_DSR, "Destination Node: " + destinationNode.GetNodeID());
            controller.PrintToOutputPane(OutputTag.TAG_DSR, "Route Chosen: " + route.GetRouteAsString());

            List<MobileNode> nodes = route.GetNodeRoute();

            /* Send DATA Packet */
            for (int i = 1; i < nodes.Count; i++) { nodes[i - 1].SendDataPacket(nodes[i], delay); }

            /* Send ACK Packet */
            for (int i = nodes.Count-2; i >=0; i--)
            {
                nodes[i + 1].SendAckPacket(nodes[i], delay);
                sData.IncrementNumberOfControlPackets();
            }

            /* Calculate End-To-End Delay */
            sData.IncrementNumberOfSuccessfulTransmissions();
            sData.endToEndDelays.Add((route.GetTransmissionTime())*4);
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
