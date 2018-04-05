using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace COMP4203.Web.Models
{
    public class DataExporter
    {
        public void export(SessionData sData, string file)
        {
            // Append metrics to the end of the csv file 
            StringBuilder data = new StringBuilder();
            string path = "D:\\" + file + ".csv";
            data.AppendFormat("{0}, {1}, {2}, {3}", 
                sData.CalculateAverageEndToEndDelay(), sData.CalculatePacketDeliveryRatio(), sData.CalculateNormalizedRoutingOverhead(), sData.CalculateBatteryDepletionDeviation());
            data.AppendLine();
            File.AppendAllText(path, data.ToString());
        }
    }
}