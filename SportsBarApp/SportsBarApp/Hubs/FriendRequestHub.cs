using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace SportsBarApp.Hubs
{
    public class FriendRequestHub : Hub
    {
        public void SendRequest(string name, string message)
        {
            Clients.All.notifyUser(name, message);
        }
    }
}