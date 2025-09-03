using NUnit.Framework;
using DotNetAuth.Models.DTO;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using DotNetAuth.Models;

namespace DotNetAuth.Tests
{
    [TestFixture]
    public class GameLobbyDTOTests
    {
        [Test]
        public void TestAddPlayer()
        {
            // Arrange
            var lobby = new GameLobby();
            var player1 = new GamePlayerDto { ConnectionId = "1" };

            // Act
            lobby.AddPlayer(player1);

            // Assert
            Assert.AreEqual(1, lobby.Players.Count);
            Assert.IsTrue(lobby.Players.ContainsKey("1"));
            Assert.AreEqual(PlayerType.Player1, lobby.Players["1"].PlayerType);
        }

        [Test]
        public void TestAddPlayer_WhenPlayerAlreadyExists_ThrowsArgumentException()
        {
            // Arrange
            var lobby = new GameLobby();
            var player1 = new GamePlayerDto { ConnectionId = "1" };
            lobby.AddPlayer(player1);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => lobby.AddPlayer(player1));
        }

        [Test]
        public void TestAddPlayer_WhenMaxPlayersReached_ThrowsException()
        {
            // Arrange
            var lobby = new GameLobby();
            var player1 = new GamePlayerDto { ConnectionId = "1" };
            var player2 = new GamePlayerDto { ConnectionId = "2" };
            lobby.AddPlayer(player1);
            lobby.AddPlayer(player2);

            // Act & Assert
            Assert.Throws<Exception>(() => lobby.AddPlayer(new GamePlayerDto { ConnectionId = "3" }));
        }

        [Test]
        public void TestStartGame_WhenNotEnoughPlayers_ThrowsException()
        {
            // Arrange
            var lobby = new GameLobby();
            var player1 = new GamePlayerDto { ConnectionId = "1" };
            lobby.AddPlayer(player1);

            // Act & Assert
            Assert.Throws<Exception>(() => lobby.StartGame());
        }

        [Test]
        public void TestStartGame()
        {
            // Arrange
      
            var lobby = new GameLobby();
            var player1 = new GamePlayerDto { ConnectionId = "1" };
            var player2 = new GamePlayerDto { ConnectionId = "2" };

            lobby.AddPlayer(player1);
            lobby.AddPlayer(player2);


            // Act
            lobby.StartGame();

            // Assert
            Assert.AreEqual(Status.Ongoing, lobby.Status);
            Assert.IsNotNull(lobby.GameField);
            Assert.IsNotEmpty(lobby.CurrentPlayerTurn);
        }

        [Test]
        public void TestClickCell_SwapUserTurn()
        {
            // Arrange
            var lobby = new GameLobby();
            var player1 = new GamePlayerDto { ConnectionId = "1", PlayerType = PlayerType.Player1 };
            var player2 = new GamePlayerDto { ConnectionId = "2", PlayerType = PlayerType.Player2 };
            lobby.AddPlayer(player1);
            lobby.AddPlayer(player2);
            lobby.StartGame();


            lobby.ClickCell(1);
            lobby.ClickCell(1);

            // Act & Assert
            Assert.AreEqual(player1.ConnectionId, lobby.CurrentPlayerTurn);
        }

        [Test]
        public void CheckWin_ShouldReturnTrue_WhenWinningConditionMet()
        {
            // Arrange
            var gameLobbyDTO = new GameLobby();
            var placedCell = new GameCell { X = 0, Y = 0, Value = 1 }; 

            // Simulate the game field
            var gameField = new List<GameCell>
            {
                placedCell,
                new GameCell { X = 1, Y = 1, Value = 1 },
                new GameCell { X = 2, Y = 2, Value = 1 },
                new GameCell { X = 3, Y = 3, Value = 1 }
            };

            gameLobbyDTO.GameField = gameField;

            // Act
            bool result = gameLobbyDTO.CheckWin(placedCell);

            // Assert
            Assert.IsTrue(result, "The game should detect a win with four cells in a row.");
        }

        public void CheckWin_ShouldReturnTrue_ForHorizontalWin()
        {
            // Arrange
            var gameLobbyDTO = new GameLobby();
            var placedCell = new GameCell { X = 0, Y = 0, Value = 1 };

            // Simulate the game field for horizontal win
            var gameField = new List<GameCell>
            {
                placedCell,
                new GameCell { X = 1, Y = 0, Value = 1 },
                new GameCell { X = 2, Y = 0, Value = 1 },
                new GameCell { X = 3, Y = 0, Value = 1 }
            };

            gameLobbyDTO.GameField = gameField;

            // Act
            bool result = gameLobbyDTO.CheckWin(placedCell);

            // Assert
            Assert.IsTrue(result, "The game should detect a win with four cells in a row horizontally.");
        }

        [Test]
        public void CheckWin_ShouldReturnTrue_ForVerticalWin()
        {
            // Arrange
            var gameLobbyDTO = new GameLobby();
            var placedCell = new GameCell { X = 0, Y = 0, Value = 1 }; 

            // Simulate the game field for vertical win
            var gameField = new List<GameCell>
            {
                placedCell,
                new GameCell { X = 0, Y = 1, Value = 1 },
                new GameCell { X = 0, Y = 2, Value = 1 },
                new GameCell { X = 0, Y = 3, Value = 1 }
            };

            gameLobbyDTO.GameField = gameField;

            // Act
            bool result = gameLobbyDTO.CheckWin(placedCell);

            // Assert
            Assert.IsTrue(result, "The game should detect a win with four cells in a column vertically.");
        }

        [Test]
        public void CheckWin_ShouldReturnTrue_ForLeftToRightDiagonalWin()
        {
            // Arrange
            var gameLobbyDTO = new GameLobby();
            var placedCell = new GameCell { X = 0, Y = 0, Value = 1 }; 

            // Simulate the game field for left-to-right diagonal win
            var gameField = new List<GameCell>
            {
                placedCell,
                new GameCell { X = 1, Y = 1, Value = 1 },
                new GameCell { X = 2, Y = 2, Value = 1 },
                new GameCell { X = 3, Y = 3, Value = 1 }
            };

            gameLobbyDTO.GameField = gameField;

            // Act
            bool result = gameLobbyDTO.CheckWin(placedCell);

            // Assert
            Assert.IsTrue(result, "The game should detect a win with four cells in a left-to-right diagonal.");
        }

        [Test]
        public void CheckWin_ShouldReturnTrue_ForRightToLeftDiagonalWin()
        {
            // Arrange
            var gameLobbyDTO = new GameLobby();
            var placedCell = new GameCell { X = 3, Y = 0, Value = 1 }; 

            // Simulate the game field for right-to-left diagonal win
            var gameField = new List<GameCell>
            {
                new GameCell{ X = 3, Y = 0, Value = 1 },
                new GameCell { X = 2, Y = 1, Value = 1 },
                new GameCell { X = 1, Y = 2, Value = 1 },
                new GameCell { X = 0, Y = 3, Value = 1 }
            };

            gameLobbyDTO.GameField = gameField;

            // Act
            bool result = gameLobbyDTO.CheckWin(placedCell);

            // Assert
            Assert.IsTrue(result, "The game should detect a win with four cells in a right-to-left diagonal.");
        }

    }
}