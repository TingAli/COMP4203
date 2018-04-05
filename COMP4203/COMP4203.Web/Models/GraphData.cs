using System;
using System.Collections.Generic;

namespace COMP4203.Web.Models
{
	public class GraphData
	{
		public Guid Id { get; private set; }
		public string YAxisTitle { get; set; }
		public List<double> YAxisValuesDsr { get; set; }
		public List<double> YAxisValuesSadsr { get; set; }
		public List<double> YAxisValuesMsadsr { get; set; }

		public GraphData()
		{
			Id = Guid.NewGuid();
			YAxisValuesDsr = new List<double>();
			YAxisValuesSadsr = new List<double>();
			YAxisValuesMsadsr = new List<double>();
		}
	}
}