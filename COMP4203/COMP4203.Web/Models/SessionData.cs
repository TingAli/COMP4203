using COMP4203.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace COMP4203.Web.Models
{
    public class SessionData
    {
        private double numberOfControlPackets = 0;
        private double numberOfAttemptedTransmissions = 0;
        private double numberOfSuccessfulTransmissions = 0;
        public List<double> endToEndDelays;
        public List<double> startingBatteryLevels;
        public List<double> endingBatteryLevels;

        private ComponentController controller;

        public SessionData()
        {
            endToEndDelays = new List<double>();
            startingBatteryLevels = new List<double>();
            endingBatteryLevels = new List<double>();
            controller = new ComponentController();
        }

        public double GetNumberOfAttemptedTransmissions() { return numberOfAttemptedTransmissions; }
        public void SetNumberOfAttemptedTransmissions(double n) { numberOfAttemptedTransmissions = n; }
        public double GetNumberOfSuccessfulTranmissions() { return numberOfSuccessfulTransmissions; }
        public void SetNumberOfSuccessfulTransmissions(double n) { numberOfSuccessfulTransmissions = n; }
        public double GetNumberOfControlPackets() { return numberOfControlPackets; }
        public void SetNumberOfControlPackets(double n) { numberOfControlPackets = n; }

        public void IncrementNumberOfSuccessfulTransmissions() { numberOfSuccessfulTransmissions++; }
        public void IncrementNumberOfControlPackets() { numberOfControlPackets++; }

        public double CalculatePacketDeliveryRatio()
        {
            if (numberOfAttemptedTransmissions == 0) { return 0; }
            return numberOfSuccessfulTransmissions / numberOfAttemptedTransmissions;

        }

        public double CalculateAverageEndToEndDelay()
        {
            if (endToEndDelays.Count == 0) { return -1; }
            double sum = 0;
            foreach (double x in endToEndDelays)
            {
                sum += x;
            }
            return sum / (double)endToEndDelays.Count;
        }

        public double CalculateNormalizedRoutingOverhead()
        {
            if (numberOfSuccessfulTransmissions == 0) { return -1; }
            return numberOfControlPackets / numberOfSuccessfulTransmissions;
        }

        public double CalculateBatteryDepletionDeviation()
        {
            List<double> values = GetDeviationValues();
            if (values.Count == 0) { return -1; }
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

        public void PrintResults()
        {
            controller.PrintToOutputPane(OutputTag.TAG_RESULTS, "PDR: " + CalculatePacketDeliveryRatio());
            controller.PrintToOutputPane(OutputTag.TAG_RESULTS, "AEED: " + CalculateAverageEndToEndDelay());
            controller.PrintToOutputPane(OutputTag.TAG_RESULTS, "NRO: " + CalculateNormalizedRoutingOverhead());
            controller.PrintToOutputPane(OutputTag.TAG_RESULTS, "BDD: " + CalculateBatteryDepletionDeviation());
        }
    }
}
