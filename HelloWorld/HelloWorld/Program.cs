using System;
using System.Collections.Generic;

namespace SimulationProtocols
{
    class Program
    {
        static void Main(string[] args)
        {
            SimulationEnvironment sTest = new SimulationEnvironment();
            // Add Test Nodes
            sTest.AddNode(new MobileNode(0, 0, 100));
            sTest.AddNode(new MobileNode(100, 110, 100));
            sTest.AddNode(new MobileNode(110, 100, 100));
            sTest.AddNode(new MobileNode(198, 198, 100));
            // Add Test Message
            sTest.AddMessage(new Message(sTest.GetNodes()[0], sTest.GetNodes()[3]));

            foreach (MobileNode node in sTest.GetNodes())
            {
                node.Print();
                node.PrintNodesWithinRange(sTest);
                Console.WriteLine();
            }

            // Send 1st message
            sTest.SendMessageDSR(sTest.GetMessages()[0]);

            // Print Nodes After Message
            foreach (MobileNode node in sTest.GetNodes()) { node.Print(); }

            Console.ReadKey();

            //SimulationEnvironment sim = new SimulationEnvironment();
            //sim.GenerateRandomNodes(15);
            //sim.GenerateRandomMessages(50);

            //Console.WriteLine("==============================================");
            //Console.WriteLine("Simulation Nodes");
            //Console.WriteLine("==============================================");

            //foreach (MobileNode node in sim.GetNodes())
            //{
            //    node.Print();
            //    foreach (MobileNode n in sim.GetNodes())
            //    {
            //        if (!node.Equals(n))
            //        {
            //            if (node.IsWithinRangeOf(n))
            //            {
            //                Console.WriteLine("Node {0} is within range. Distance: {1}", n.GetNodeID(), node.GetDistance(n));
            //            } else
            //            {
            //                Console.WriteLine("Node {0} is not within range. Distance: {1}", n.GetNodeID(), node.GetDistance(n));
            //            }
            //        }
            //    }
            //    Console.WriteLine();
            //}
            //Console.WriteLine();
            //Console.WriteLine("==============================================");
            //Console.WriteLine("Simulation Messages");
            //Console.WriteLine("==============================================");
            //foreach (Message message in sim.GetMessages())
            //{
            //    message.Print();
            //}

            //Console.ReadKey();
        }
    }
}
