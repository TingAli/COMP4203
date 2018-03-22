using Microsoft.AspNet.SignalR;

namespace COMP4203.Web.Controllers.Hubs
{
	public class TestHub : Hub
	{
		public void SendMessage(string name, string message)
		{
			Clients.All.broadcastMessage(name, message);
		}
	}
}