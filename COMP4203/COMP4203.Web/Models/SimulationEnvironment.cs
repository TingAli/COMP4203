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

        public static int PROTOCOL_INDEX_DSR = 0;
        public static int PROTOCOL_INDEX_SADSR = 1;
        public static int PROTOCOL_INDEX_MSADSR = 2;

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

        public SessionData RunSimulation(int delay, int tabIndex)
        {
            SessionData sessionData = new SessionData(tabIndex);
            sessionData.SetStartingBatteryLevels(mobileNodes);              // Save starting battery levels
            sessionData.SetNumberOfAttemptedTransmissions(messages.Count);  // Save number of attempted transmissions

            string tag = "";
            if (tabIndex == PROTOCOL_INDEX_DSR) { tag = OutputTag.TAG_DSR; }
            else if (tabIndex == PROTOCOL_INDEX_SADSR) { tag = OutputTag.TAG_SADSR; }
            else if (tabIndex == PROTOCOL_INDEX_MSADSR) { tag = OutputTag.TAG_MSADSR; }

            /* Send all messages */
            foreach (Message message in messages)
            {
                controller.PrintToOutputPane(OutputTag.TAG_NOTE, "Message: " + message.GetMessageID());
                if (tabIndex == PROTOCOL_INDEX_DSR)
                {
                    controller.PrintToOutputPane(OutputTag.TAG_DSR, "Running DSR Simulation.");
                    SendMessageDSR(message, sessionData, delay);
                } else if (tabIndex == PROTOCOL_INDEX_SADSR)
                {
                    controller.PrintToOutputPane(OutputTag.TAG_SADSR, "Running SA-DSR Simulation.");
                    SendMessageSADSR(message, sessionData, delay);
                } else if (tabIndex == PROTOCOL_INDEX_MSADSR)
                {
                    controller.PrintToOutputPane(OutputTag.TAG_MSADSR, "Running MSA-DSR Simulation.");
                    SendMessageMSADSR(message, sessionData, delay);
                }
            }

            sessionData.SetEndingBatteryLevels(mobileNodes);    // Save ending battery levels
            sessionData.PrintResults();     // Print collected metrics
            sessions.Add(sessionData);      // Add collected data to recorded sessions
            controller.PrintToOutputPane(tag, sessionData.GetNumberOfAttemptedTransmissions() + " attempted transmissions.");
            controller.PrintToOutputPane(tag, sessionData.GetNumberOfSuccessfulTranmissions() + " successful transmissions.");
            controller.PrintToOutputPane(tag, "Finished Transmitting Messages.");
            controller.MarkTransferAsComplete(tabIndex);

	        return sessionData;
        }

        public void AddNode(MobileNode node)
        {
            mobileNodes.Add(node);
        }

        public void AddMessage(Message message)
        {
            messages.Add(message);
        }

        public void GenerateRandomNodes(int numNodes, int range, int numPureSelfish, int numPartialSelfish)
        {
            mobileNodes.Clear();
            Random random = new Random();
            for (int i = 0; i < numNodes; i++)
            {
                int x = random.Next(width);
                int y = random.Next(height);
                int bLevel = random.Next(100);
                mobileNodes.Add(new MobileNode(x, y, bLevel, range));
            }
            List<int> selfishIndexes = new List<int>();
            for (int i = 0; i < numPureSelfish; i++)
            {
                int choice = random.Next(numNodes);
                while (selfishIndexes.Contains(choice))
                {
                    choice = random.Next(numNodes);
                }
                mobileNodes[choice].SetPureSelfish();
                selfishIndexes.Add(choice);
            }
            for (int i = 0; i < numPartialSelfish; i++)
            {
                int choice = random.Next(numNodes);
                while (selfishIndexes.Contains(choice))
                {
                    choice = random.Next(numNodes);
                }
                mobileNodes[choice].SetPartialSelfish();
                selfishIndexes.Add(choice);
            }
        }

        public void GenerateRandomMessages(int numMessages)
        {
            messages.Clear();
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
            Route route = sourceNode.GetBestRouteDSR(destinationNode);

            /* If no known route, attempt to find one */
            if (route == null)
            {
                controller.PrintToOutputPane(OutputTag.TAG_DSR, "No Known Route to Destination.");
                /* Perform Route Discovery */
                sourceNode.DSRRouteDiscovery(destinationNode, this, sData, delay);
                route = sourceNode.GetBestRouteDSR(destinationNode);
                /* If no route found, abort transfer */
                if (route == null)
                {
                    controller.PrintToOutputPane(OutputTag.TAG_DSR, "No Route to Destination.");
                    return false;
                }
            }

            controller.PrintToOutputPane(OutputTag.TAG_DSR, string.Format("Sending Message #{0}.", message.GetMessageID()));
            controller.PrintToOutputPane(OutputTag.TAG_DSR, "Source Node: " + sourceNode.GetNodeID());
            controller.PrintToOutputPane(OutputTag.TAG_DSR, "Destination Node: " + destinationNode.GetNodeID());
            controller.PrintToOutputPane(OutputTag.TAG_DSR, "Route Chosen: " + route.GetRouteAsString());

            List<MobileNode> nodes = route.GetNodeRoute();
            /* Send DATA Packet */
            for (int i = 1; i < nodes.Count; i++) {
                if (!nodes[i - 1].SendDataPacket(nodes[i], delay, OutputTag.TAG_DSR))
                {
                    return false;
                }
            }

            /* Send ACK Packet */
            for (int i = nodes.Count - 2; i >= 0; i--)
            {
                nodes[i + 1].SendAckPacket(nodes[i], delay, OutputTag.TAG_DSR);
                sData.IncrementNumberOfControlPackets();
            }

            /* Calculate End-To-End Delay */
            sData.IncrementNumberOfSuccessfulTransmissions();
            sData.endToEndDelays.Add((route.GetTransmissionTime())*4);
            return true;
        }

        public bool SendMessageSADSR(Message message, SessionData sData, int delay)
        {
            MobileNode sourceNode = message.GetSourceNode();
            MobileNode destinationNode = message.GetDestinstationNode();
            Route route = sourceNode.GetBestRouteSADSR(destinationNode);

            /* If no route known, perform Route Discovery to find one */
            List<Route> routes = null;
            if (route == null)
            {
                controller.PrintToOutputPane(OutputTag.TAG_SADSR, "No Known Route to Destination.");
                routes = sourceNode.SADSRRouteDiscovery(destinationNode, this, sData, delay, false); // Route Discovery
                route = sourceNode.GetBestRouteSADSR(destinationNode);  // Get best known route
                // If still no route found, abort.
                if (route == null)
                {
                    controller.PrintToOutputPane(OutputTag.TAG_SADSR, "No Route to Destination.");
                    return false;
                }
            }

            int validCounter = 0;
            if (routes != null)
            {
                controller.PrintToOutputPane(OutputTag.TAG_NOTE, routes.Count + " routes found.");

                foreach (Route r in routes)
                {
                    controller.PrintToOutputPane(OutputTag.TAG_NOTE, "SDP: " + r.CalcSDP());
                    controller.PrintToOutputPane(OutputTag.TAG_NOTE, "Delay: " + r.GetTransmissionTime());
                    if (route.GetTransmissionTime() < MobileNode.SA_TIMEOUT) validCounter++;
                }
                controller.PrintToOutputPane(OutputTag.TAG_NOTE, validCounter + " routes within timeframe");
            }

            controller.PrintToOutputPane(OutputTag.TAG_SADSR, "Route Selected: " + route.GetRouteAsString());
            controller.PrintToOutputPane(OutputTag.TAG_SADSR, "Selected Route's SDP: " + route.CalcSDP());
            controller.PrintToOutputPane(OutputTag.TAG_SADSR, "Selected Route's Transmission Time: " + route.GetTransmissionTime());

            List<MobileNode> nodes = route.GetNodeRoute();
            /* Send DATA Packet */
            for (int i = 1; i < nodes.Count; i++)
            {
                if (!nodes[i - 1].SendDataPacket(nodes[i], delay, OutputTag.TAG_SADSR))
                {
                    return false;
                }
            }

            /* Send ACK Packet */
            for (int i = nodes.Count - 2; i >= 0; i--)
            {
                nodes[i + 1].SendAckPacket(nodes[i], delay, OutputTag.TAG_SADSR);
                sData.IncrementNumberOfControlPackets();
            }

            /* Calculate End-To-End Delay */
            sData.IncrementNumberOfSuccessfulTransmissions();
            sData.endToEndDelays.Add((route.GetTransmissionTime()) * 4); // this is good for normal SA-DSR, not for MSA-DSR
            return true;
        }

        public bool SendMessageMSADSR(Message message, SessionData sData, int delay)
        {
            MobileNode sourceNode = message.GetSourceNode();
            MobileNode destinationNode = message.GetDestinstationNode();
            Route route = sourceNode.GetBestRouteMSADSR(destinationNode);

            /* If no route known, perform Route Discovery to find one */
            List<Route> routes = null;
            if (route == null) {
                controller.PrintToOutputPane(OutputTag.TAG_MSADSR, "No Known Route to Destination.");
                routes = sourceNode.SADSRRouteDiscovery(destinationNode, this, sData, delay, true); // Route Discovery
                route = sourceNode.GetBestRouteMSADSR(destinationNode);  // Get best known route
                // If still no route found, abort.
                if (route == null) {
                    controller.PrintToOutputPane(OutputTag.TAG_MSADSR, "No Route to Destination.");
                    return false;
                }
            }

            int validCounter = 0;
            if (routes != null)
            {
                controller.PrintToOutputPane(OutputTag.TAG_NOTE, routes.Count + " routes found.");
            
                foreach (Route r in routes)
                {
                    controller.PrintToOutputPane(OutputTag.TAG_NOTE, "SDP: " + r.CalcSDP());
                    controller.PrintToOutputPane(OutputTag.TAG_NOTE, "Delay: " + r.GetTransmissionTime());
                    if (route.GetTransmissionTime() < MobileNode.SA_TIMEOUT) validCounter++;
                }
                controller.PrintToOutputPane(OutputTag.TAG_NOTE, validCounter + " routes within timeframe");
            }

            controller.PrintToOutputPane(OutputTag.TAG_MSADSR , "Route Selected: " + route.GetRouteAsString());
            controller.PrintToOutputPane(OutputTag.TAG_MSADSR, "Selected Route's SDP: " + route.CalcSDP());
            controller.PrintToOutputPane(OutputTag.TAG_MSADSR, "Selected Route's Transmission Time: " + route.GetTransmissionTime());

            List<MobileNode> nodes = route.GetNodeRoute();
            /* Send DATA Packet */
            for (int i = 1; i < nodes.Count; i++) {
                if (!nodes[i - 1].SendDataPacket(nodes[i], delay, OutputTag.TAG_MSADSR)) {
                    return false;
                }
                if (i > 2)
                {
                    nodes[i].SendAckPacket(nodes[i - 1], delay, OutputTag.TAG_MSADSR);
                }
            }

            /* Send ACK Packet */
            for (int i = nodes.Count - 2; i >= 0; i--) {
                nodes[i + 1].SendAckPacket(nodes[i], delay, OutputTag.TAG_MSADSR);
                sData.IncrementNumberOfControlPackets();
            }

            /* Calculate End-To-End Delay */
            sData.IncrementNumberOfSuccessfulTransmissions();
            sData.endToEndDelays.Add((route.GetTransmissionTime()) * 4); // this is good for normal SA-DSR, not for MSA-DSR
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
