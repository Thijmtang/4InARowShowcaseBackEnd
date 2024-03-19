using System.Text.RegularExpressions;
using Microsoft.AspNetCore.SignalR;

namespace DotNetAuth.Hub
{
    public class Gamehub: Microsoft.AspNetCore.SignalR.Hub
    {
        private static readonly List<Group> GroupList = new();

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


            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has joined the group {groupName}.");
        }
        //
        // private string generateUniqueGroupName(string groupName)
        // {
        //     IGroupManager groupManager = GroupList.GetGroupManager();
        //     string uniqueName = groupName;
        //
        //     while (Groups.)
        //     {
        //         
        //     }
        //
        //
        //
        //
        //
        //
        //     return groupName;
        // }

    }
}
