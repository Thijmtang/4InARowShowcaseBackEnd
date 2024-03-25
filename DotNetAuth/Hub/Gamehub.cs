using System.Text.Json;
using System.Text.RegularExpressions;
using DotNetAuth.Models.DTO;
using DotNetAuth.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using NuGet.DependencyResolver;

namespace DotNetAuth.Hub
{
    public class Gamehub : Microsoft.AspNetCore.SignalR.Hub
    {
        private static readonly List<Group> GroupList = new();
        private static List<GameLobbyDTO> LobbyList = new();


        private static GameService _gameService;
        private static UserManager<IdentityUser> _userManager;

        public Gamehub(GameService gameService, UserManager<IdentityUser> userManager)
        {
            _gameService = gameService;
            _userManager = userManager;
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // get all lobbies which the current user is connected to
            var connectedLobbies = LobbyList.FindAll(l => l.GetPlayers().ContainsKey(Context.ConnectionId));
            foreach (GameLobbyDTO gameLobby in connectedLobbies)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameLobby.Code);
                var players = gameLobby.GetPlayers();
                // Delete the lobby since the disconnected user is the only one left
                if (players.Count == 1)
                {
                    LobbyList.Remove(gameLobby);
                    continue;
                }

                var user = getCurrentPlayer();

                gameLobby.RemovePlayer(Context.ConnectionId);

                await SendGroupAlert(gameLobby.Code, $"{user.Username} heeft de lobby verlaten", "error");
                await Clients.All.SendAsync("Endlobby");
                await SendAlert($"Je hebt de lobby {gameLobby.Code} verlaten", "error");
                await UpdateLobbyUsers(players, gameLobby.Code);
            }

            // Call base implementation to ensure disconnection is handled properly
            await base.OnDisconnectedAsync(exception);
        }

        public async Task CreateLobby(string groupName)
        {
            // Genereer unieke group naam;
            bool unique = FindLobby(groupName) == null;
            
            if (!unique)
            {
                int i = 1;

                string newName = $"{groupName}-{i++}";

                while (FindLobby(newName) != null)
                {
                    newName = $"{groupName}-{i++}";
                }

                groupName = newName;
            }

            var newLobby = new GameLobbyDTO()
            {
                Code = groupName,
            };

            // Add player leader
            var currentPlayer = getCurrentPlayer();
            currentPlayer.JoinDate = new DateTime();

            newLobby.AddPlayer(currentPlayer);

            // Save new lobby
            LobbyList.Add(newLobby);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await SendAlert($"Lobby {groupName} is succesvol aangemaakt!", "success");

            await Clients.Caller.SendAsync("RedirectLobby", JsonConvert.SerializeObject(newLobby, Formatting.Indented));
        }

        public async Task JoinLobby(string lobbyName)
        {
            var lobby = FindLobby(lobbyName);
            // Lobby does not exist
            if (lobby == null)
            {
                await SendAlert("Lobby kon niet gevonden worden!", "error");
                return;
            }

            var players = lobby.GetPlayers();

            if (players.ContainsKey(Context.ConnectionId))
            {
                await SendAlert("Je zit al in deze lobby!", "error");
                return;
            }

            // Lobby full
            if (players.Count >= 2)
            {
                //Geef vage fout melding eventueel
                await SendAlert("Lobby zit vol!", "error");
                return;
            }

            // Add user to lobby 
            var user = getCurrentPlayer();
            user.JoinDate = new DateTime();

            await SendGroupAlert(lobbyName, $"{user.Username} has joined the lobby", "success");

            lobby.AddPlayer(user);
            await Groups.AddToGroupAsync(Context.ConnectionId, lobbyName);

            await Clients.Caller.SendAsync("RedirectLobby", JsonConvert.SerializeObject(lobby));

            await UpdateLobbyUsers(players, lobbyName);

            if (players.Count() == 2)
            {
                // Unlock play button for leader
                await Clients.OthersInGroup(lobbyName).SendAsync("AllowGameStart");
            }
        }

        public async Task LeaveLobby(string roomName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        }

        public async Task StartGame(string roomName)
        {
            var lobby = FindLobby(roomName);

            var players = lobby.GetPlayers();
            
            if (players.Count != 2)
            {
                await SendGroupAlert(roomName, "Er is iets fout gegaan, lobby heeft geen 2 spelers", "error");
                return;
            }

            // Not the lobby leader
            if (players[Context.ConnectionId].PlayerType != PlayerType.Player1)
            {
                await SendAlert( "Jij bent niet de lobby leader", "error");
                return;
            }

            lobby.StartGame();

            await Clients.Groups(roomName).SendAsync("RenderField", JsonConvert.SerializeObject(lobby, Formatting.Indented));
            await Clients.Groups(roomName).SendAsync("StartGame");
        }


        private async Task SendAlert(string message, string type)
        {
            await Clients.Caller.SendAsync("FlashAlert", message, type);
        }


        private async Task SendGroupAlert(string groupName, string message, string type)
        {
            await Clients.Group(groupName).SendAsync("FlashAlert", message, type);
        }

        public async Task ClickCell(string roomName, int x)
        {
            var lobby = FindLobby(roomName);

            if (lobby.Status != STATUS.ONGOING)
            {
                return;
            }

            if (lobby.CurrentPlayerTurn != Context.ConnectionId)
            {
                return;
            }

            lobby.ClickCell(x);

            await Clients.Groups(roomName).SendAsync("RenderField", JsonConvert.SerializeObject(lobby));

            if (lobby.Status == STATUS.COMPLETED)
            {
                await Clients.Groups(roomName).SendAsync("ShowChoiceModal");
            }

        }

        /// <summary>
        /// Sends event which updates the list  of the players within the game lobby
        /// </summary>
        /// <param name="players"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        private async Task UpdateLobbyUsers(Dictionary<string, GamePlayerDto> players, string groupName)
        {
            await Clients.Groups(groupName).SendAsync("UpdatePlayerList", JsonConvert.SerializeObject(players, Formatting.Indented));
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

        private GameLobbyDTO FindLobby(string code)
        {
            return LobbyList.FirstOrDefault(l => l.Code == code);
        }



   
    }


}