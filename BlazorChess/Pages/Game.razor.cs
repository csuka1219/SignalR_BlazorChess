using BlazorChess.Data;
using BlazorChess.Game;
using BlazorChess.Pieces;
using BlazorChess.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System.Data;
using System.Xml;

namespace BlazorChess.Pages
{
    public partial class Game : IDisposable
    {
        [Inject]
        private ILocalStorageService localStorage { get; set; } = default!;

        [Inject] 
        private IDialogService dialogService { get; set; } = default!;

        [Inject]
		private NavigationManager navigationManager { get; set; } = default!;

        [Inject]
        private IUserHandler userHandler { get; set; } = default!;

        private GameHubService gameHubService = default!;

        private ChessGameService chessGameService = default!;

        [Parameter]
        public string gameName { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            gameHubService = new GameHubService(navigationManager);
            chessGameService = new ChessGameService();
            // Create a new hub connection for the chess game
            gameHubService.OnMoveReceived += handleMoveReceived;
            gameHubService.OnJoined += handleJoined;
            await gameHubService.startAsync();
            await joinGame();
        }


        private async void pieceUpdated(MudItemDropInfo<Piece> piece)
        {
            chessGameService.movePiece(piece, gameHubService, gameName);
            if (chessGameService.checkForCheckmate())
            {
                bool? result = await dialogService.ShowMessageBox(
                    "Sakkmatt",
                    "later",
                    yesText: "Exit!", cancelText: "Again");

                    if (result.HasValue && result.Value)
                    {
                        // Perform necessary actions for exiting or starting a new game
                        // InitGame();
                        //StateHasChanged();
                    }
            }
            
        }

        private async Task joinGame()
        {
            // Retrieve the unique GUID from local storage
            string uniqueGuid = await localStorage.GetItemAsync<string>("uniqueGuid");

            // Join the new game
            await gameHubService.joinGameAsync(gameName);

            Dictionary<string, List<string>> connectedPlayers = userHandler.getConnectedPlayers();
            Dictionary<string, MatchInfo> matchInfos = userHandler.getMatchInfos();

            // Check if there are already connected players for the game
            if (connectedPlayers.ContainsKey(gameName))
            {
                handleExistingPlayer(connectedPlayers, matchInfos, uniqueGuid);
            }
            else
            {
                handleNewPlayer(connectedPlayers, matchInfos, uniqueGuid);
            }
        }
        private async void handleExistingPlayer(Dictionary<string, List<string>> connectedPlayers, Dictionary<string, MatchInfo> matchInfos, string uniqueGuid)
        {
            if (connectedPlayers[gameName].Count > 1 && !connectedPlayers[gameName].Contains(uniqueGuid))
            {
                navigationManager.NavigateTo("/");
                return;
            }
            // Check if the current player is already connected to the game
            if (connectedPlayers[gameName].Contains(uniqueGuid))
            {
                // The player is refreshing or renavigating
                // Update the chessboard, pieces list, and player turn
                chessGameService.chessBoard.board = userHandler.getMatchInfoBoard(gameName);
                chessGameService.pieceChanges = userHandler.getMatchInfoMoves(gameName);
                chessGameService.piecesOnBoard = chessGameService.chessBoard.board.Cast<Piece>().ToList();
                bool isWhitePlayer = connectedPlayers[gameName].First() == uniqueGuid;

                if (connectedPlayers[gameName].Count == 2)
                {
                    chessGameService.ableToMove = true;
                }

                chessGameService.player.IsMyTurn = isWhitePlayer == matchInfos[gameName].isWhiteTurn;
                chessGameService.player.isWhitePlayer = isWhitePlayer;
                chessGameService.whiteTurn = matchInfos[gameName].isWhiteTurn;
                StateHasChanged();
                chessGameService._container.Refresh();
            }
            else
            {
                // The second player has connected
                // Add the current player to the connected players list and set the turn to false
                connectedPlayers[gameName].Add(uniqueGuid);
                chessGameService.player.IsMyTurn = false;
                chessGameService.player.isWhitePlayer = false;
                chessGameService.ableToMove = true;
                await gameHubService.startGameAsync(gameName);
            }
        }
        
        private void handleNewPlayer(Dictionary<string, List<string>> connectedPlayers, Dictionary<string, MatchInfo> matchInfos, string uniqueGuid)
        {
            // First player to connect to the game
            // Create new entries for connected players and match information
            connectedPlayers.Add(gameName, new List<string>() { uniqueGuid });
            matchInfos.Add(gameName, new MatchInfo());
            chessGameService.player.IsMyTurn = true;
            chessGameService.player.isWhitePlayer = true;
        }
        private void handleMoveReceived(int fromX, int fromY, int toX, int toY)
        {
            if (!chessGameService.player.IsMyTurn)
            {
                // Create a MudItemDropInfo object representing the received move
                MudItemDropInfo<Piece> piece = new MudItemDropInfo<Piece>(chessGameService.piecesOnBoard.First(x => x.Position == $"{fromX}{fromY}"), $"{toX}{toY}", -1);

                // Handle the received move
                pieceUpdated(piece);

                // Update the chess board UI
                chessGameService._container.Refresh();
                StateHasChanged();
            }
        }

        private void handleJoined()
        {
            chessGameService.ableToMove = true;
        }

        // Implementation of the IDisposable interface to perform cleanup when the object is disposed
        public async void Dispose()
        {
            // Update the match information with the current state of the chessboard
            userHandler.setMatchInfoBoard(gameName, chessGameService.chessBoard.board);
            userHandler.setMatchInfoMoves(gameName, chessGameService.pieceChanges, chessGameService.whiteTurn);
            gameHubService.OnMoveReceived -= handleMoveReceived;
            gameHubService.OnJoined -= handleJoined;
            await gameHubService.leaveGameAsync(gameName);
            await gameHubService.stopAsync();
        }
    }
}