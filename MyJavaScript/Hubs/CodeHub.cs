using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.Identity;
using System.Data.Entity;

namespace MyJavaScript.Hubs
{
	public class CodeHub : Hub
	{
		static HashSet<string> CurrentConnections = new HashSet<string>();

		public override System.Threading.Tasks.Task OnConnected()
		{
			var id = HttpContext.Current.User.Identity.Name;
			CurrentConnections.Add(id);

			return base.OnConnected();
		}

		public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
		{
			if(stopCalled)
			{
				var name = "";
				try
				{
					 name = HttpContext.Current.User.Identity.Name;
				}
				catch
				{
					new NullReferenceException();
				}
				var connection = "";
				if (name != null)
				{
					 connection = CurrentConnections.FirstOrDefault(x => x == name);
				}
				

				if (connection != null || connection != "")
				{
					CurrentConnections.Remove(connection);
				}
			}

			return base.OnDisconnected(stopCalled);
		}


		//return list of all active connections
		public List<string> GetAllActiveConnections()
		{
			return CurrentConnections.ToList();
		}
		public void JoinDocument(int documentID)
		{
			Groups.Add(Context.ConnectionId, Convert.ToString(documentID));
		}
		public void OnChange(object changeData, int documentID)
		{
			Clients.Group(Convert.ToString(documentID), Context.ConnectionId).onChange(changeData);
			//Clients.All.OnChange(changeData);
		}
	}
}