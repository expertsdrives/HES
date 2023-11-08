using Microsoft.AspNet.SignalR;
using Microsoft.Azure.Amqp.Framing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace HESMDMS.Hubs
{
    public class GridHub : Hub
    {
        public void SendData(string data)
        {
            // Broadcast data to all connected clients.
            Clients.All.updateData(data);
        }
        public override Task OnConnected()
        {
            // Perform actions when a client connects
            // For example, you can log the connection or update client lists
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            // Perform actions when a client disconnects
            // For example, you can update client lists or perform cleanup
            return base.OnDisconnected(stopCalled);
        }

        public void BroadcastDataUpdate(object updatedData)
        {
            // Broadcast the updated data to all clients
            Clients.All.updateGridData(updatedData);
        }
    }

}