using System.Text.RegularExpressions;
using DotNetAuth.Models.DTO;
using DotNetAuth.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace DotNetAuth.Hub
{
    public class Gamehub : Microsoft.AspNetCore.SignalR.Hub
    {
        private static readonly List<Group> GroupList = new();
        private static Dictionary<string, GameLobbyDTO> LobbyList = new();


        private static GameService _gameService;
        private static UserManager<IdentityUser> _userManager;

        public Gamehub(GameService gameService, UserManager<IdentityUser> userManager)
        {
            _gameService = gameService;
            _userManager = userManager;
        }

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
            bool unique = !LobbyList.ContainsKey(groupName);
            
            if (!unique)
            {
            
                int i = 1;

                string newName = $"{groupName}-{i++}";


                while (LobbyList.ContainsKey(newName))
                {
                    newName = $"{groupName}-{i++}";
                }


                groupName = newName;
            }


            var newLobby = new GameLobbyDTO()
            {
                GameField = _gameService.GenerateField(),
            };

            // Add player leader
            // var currentPlayer = getCurrentPlayer();
            newLobby.AddPlayer(getCurrentPlayer());

            // Save new lobby
            LobbyList.Add(groupName, newLobby);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await SendAlert($"Lobby {groupName} is succesvol aangemaakt!", "success");
        }

        public async Task JoinLobby(string lobbyName)
        {

            // Lobby does not exist
            if (!LobbyList.ContainsKey(lobbyName))
            {
                await SendAlert("Lobby kon niet gevonden worden!", "error");
                return;
            }


            var lobby = LobbyList[lobbyName];

            if (lobby.getPlayers().ContainsKey(Context.ConnectionId))
            {
                await SendAlert("Je zit al in deze lobby!", "error");

            }
            // Lobby full
            if (lobby.getPlayers().Count >= 2)
            {
                //Geef vage fout melding eventueel
                await SendAlert("Lobby zit vol!", "error");
                return;
            }

            var user = getCurrentPlayer();
            lobby.AddPlayer(user);

            // await Clients.Caller.SendAsync("ShowMessage", "It is not your turn", "warning");
            await Groups.AddToGroupAsync(Context.ConnectionId, lobbyName);

            await SendGroupAlert(lobbyName, $"{user.Username} has joined the lobby", "success");
        }

        public async Task LeaveLobby(string roomName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        }


        private async Task SendAlert(string message, string type)
        {
            await Clients.Caller.SendAsync("FlashAlert", message, type);
        }


        private async Task SendGroupAlert(string groupName, string message, string type)
        {
            await Clients.Group(groupName).SendAsync("FlashAlert", message, type);

        }



        private GamePlayerDto? getCurrentPlayer()
        {
            string username = _userManager.GetUserName(Context.User);
            

            var player = new GamePlayerDto()
            {
                ConnectionId = Context.ConnectionId,
                Username = username,
            };

            return player;
        }

       
    }


}