using System.Text.RegularExpressions;
using Microsoft.AspNetCore.SignalR;

namespace DotNetAuth.Hub
{
    public class Gamehub : Microsoft.AspNetCore.SignalR.Hub
    {
        private static readonly List<Group> GroupList = new();
        private static Dictionary<string, List<string>> LobbyList = new();

        // public async Task JoinLobby(string lobbyCode)
        // {

        // }   


        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }


        public async Task CreateLobby(string groupName)
        {
            // Genereer unieke group naam;
            bool unique = LobbyList.ContainsKey(groupName);

            if (!unique)
            {
                string newName = groupName;

                int i = 1;

                while (LobbyList.ContainsKey(newName))
                {
                    newName = $"{groupName}-{i++}";
                }

                groupName = newName;
            }

            LobbyList.Add(groupName, new List<string>());
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);


            await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has joined the group {groupName}.", "failure");
        }

        public async Task JoinLobby(string lobbyName)
        {

            // Lobby does not exist
            if (!LobbyList.ContainsKey(lobbyName))
            {
                await SendAlert("Lobby kon niet gevonden worden!", "error");
                return;
            }

            // Lobby full


            // await Clients.Caller.SendAsync("ShowMessage", "It is not your turn", "warning");
            await Groups.AddToGroupAsync(Context.ConnectionId, lobbyName);

            await Clients.Group(lobbyName).SendAsync("Send", $"{Context.ConnectionId} has joined the group {lobbyName}.", "failure");

            return;
        }

        public async Task LeaveLobby(string roomName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        }


        private async Task SendAlert(string message, string type)
        {
            await Clients.Caller.SendAsync("FlashAlert", message, type);
        }


       
    }


}