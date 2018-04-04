using COMP4203.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace COMP4203.Web.Models
{
    public class SessionData
    {
        public double numControlPackets = 0;
        public double numDataPacketsSent = 0;
        public double numDataPacketsReceived = 0;
        public List<int> endToEndDelays;
        public List<double> startingBatteryLevels;
        public List<double> endingBatteryLevels;

        public SessionData()
        {
            endToEndDelays = new List<int>();
            startingBatteryLevels = new List<double>();
            endingBatteryLevels = new List<double>();
        }

        public double CalculatePacketDeliveryRatio()
        {
            return numDataPacketsReceived / numDataPacketsSent;

        }

        public double CalculateAverageEndToEndDelay()
        {
            if (endToEndDelays.Count == 0) { return -1; }
            int sum = 0;
            foreach (int x in endToEndDelays)
            {
                sum += x;
            }
            return sum / endToEndDelays.Count;
        }

        public double CalculateNormalizedRoutingOverhead()
        {
            return numControlPackets / numDataPacketsReceived;
        }

        public double CalculateBatteryDepletionDeviation()
        {
            List<double> values = GetDeviationValues();
            double mean = values.Average();
            double total = 0;
            foreach (int value in values)
            {
                total += Math.Pow((value - mean), 2);
            }
            total = total / (values.Count - 1);
            return Math.Sqrt(total);
        }

        public List<double> GetDeviationValues()
        {
            List<double> values = new List<double>();
            for (int i = 0; i < startingBatteryLevels.Count; i++)
            {
                values.Add(startingBatteryLevels[i] - endingBatteryLevels[i]);
            }
            return values;
        }

        public void SetStartingBatteryLevels(List<MobileNode> nodes)
        {
            foreach (MobileNode node in nodes)
            {
                startingBatteryLevels.Add(node.GetBatteryLevel());
            }
        }

        public void SetEndingBatteryLevels(List<MobileNode> nodes)
        {
            foreach (MobileNode node in nodes)
            {
                endingBatteryLevels.Add(node.GetBatteryLevel());
            }
        }
    }
}
