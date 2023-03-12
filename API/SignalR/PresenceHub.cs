using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    [Authorize]
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker _tracker;
        public PresenceHub(PresenceTracker tracker)
        {
            _tracker = tracker;
        }
        public override async Task OnConnectedAsync()
        {
            var isOnline = await _tracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);

            if (isOnline)
            {
                await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());
            }

            var currentUsers = await _tracker.GetOnlineUsers();
            await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var isOffline = await _tracker.UserDisconnected(Context.User.GetUsername(), Context.ConnectionId);

            if (isOffline)
            {
                await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}

/*
Este código define una clase llamada "PresenceHub" que hereda de la clase "Hub" de SignalR, una biblioteca de tiempo real para .NET. La clase "PresenceHub" se utiliza para implementar la funcionalidad de seguimiento de presencia para usuarios conectados a una aplicación SignalR.

El constructor de la clase "PresenceHub" toma un objeto "PresenceTracker" como argumento. El objeto "PresenceTracker" es una clase que se utiliza para realizar el seguimiento de los usuarios conectados y desconectados.

El método "OnConnectedAsync()" se ejecuta cuando un nuevo usuario se conecta a la aplicación SignalR. En este método, se llama al método "UserConnected()" del objeto "PresenceTracker" para registrar al usuario como conectado y se obtiene un valor booleano que indica si el usuario estaba en línea antes de conectarse. Si el usuario estaba en línea antes de conectarse, se envía un mensaje a otros usuarios de la aplicación SignalR para notificarles que el usuario ha vuelto en línea. También se llama al método "GetOnlineUsers()" del objeto "PresenceTracker" para obtener una lista de los usuarios conectados y se envía a través de SignalR al usuario recién conectado utilizando el método "SendAsync()" de la clase "Clients".

El método "OnDisconnectedAsync()" se ejecuta cuando un usuario se desconecta de la aplicación SignalR. En este método, se llama al método "UserDisconnected()" del objeto "PresenceTracker" para registrar al usuario como desconectado y se obtiene un valor booleano que indica si el usuario estaba en línea antes de desconectarse. Si el usuario estaba en línea antes de desconectarse, se envía un mensaje a otros usuarios de la aplicación SignalR para notificarles que el usuario ha abandonado la aplicación. Se llama al método "OnDisconnectedAsync()" de la clase "Hub" para permitir que SignalR realice las tareas de limpieza necesarias después de que se desconecte el usuario.

En resumen, este código implementa una funcionalidad de seguimiento de presencia para usuarios conectados a una aplicación SignalR, utilizando un objeto "PresenceTracker" para realizar el seguimiento de los usuarios conectados y desconectados. Cuando un usuario se conecta o desconecta, se notifica a los otros usuarios de la aplicación SignalR para que puedan ver quiénes están conectados en tiempo real.
*/