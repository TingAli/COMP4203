using System;

namespace COMP4203.Web.Models
{
	public class GraphData
	{
		public Guid Id { get; set; }

		public GraphData()
		{
			Id = Guid.NewGuid();
		}
	}
}