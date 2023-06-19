using BlazorChess.Data;
using BlazorChess.Game;
using BlazorChess.Pieces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System.Data;

namespace BlazorChess.Pages
{
    public partial class Game : IDisposable
    {
        [Inject] 
        private IDialogService? DialogService { get; set; }

		[Inject]
		private NavigationManager? NavigationManager { get; set; }

        [Parameter]
        public string gameName { get; set; }

        private Chessboard chessBoard = new Chessboard();
        private Player player = new Player();
        IEnumerable<Piece> list = new List<Piece>();
        private MudDropContainer<Piece> _container;

        bool[,] availableMoves = new bool[8, 8];
        public bool whiteTurn = true;
        public bool lastTurn = true;
        public bool isStale = false;
		bool dragEnded = true;
        public string lastposition = "";

        private HubConnection hubConnection;
		protected override async Task OnInitializedAsync()
        {
            list = chessBoard.board.Cast<Piece>().ToList();
            //todo connect

            hubConnection = new HubConnectionBuilder()
			.WithUrl(NavigationManager!.ToAbsoluteUri("/chessHub"))
			.Build();
			hubConnection.On<int, int, int, int>("ReceiveMove", (fromX, fromY, toX, toY) =>
			{
                if (!player.IsMyTurn)
                {
                    // Handle received move
                    // Update the chess board UI
                    MudItemDropInfo<Piece> piece = new MudItemDropInfo<Piece>(list.First(x => x.Position == $"{fromX}{fromY}"), $"{toX}{toY}", -1);
					pieceUpdated(piece);
                    _container.Refresh();
                }
			});

			await hubConnection.StartAsync();
            await joinGame(gameName);
		}
        public bool canDrop(Piece selectedPiece, string s)
        {
            if (!player.IsMyTurn)
            {
                return false;
            }

			if (whiteTurn == lastTurn && selectedPiece.Position != lastposition && !dragEnded)
			{
				availableMoves = new bool[8, 8];
				dragEnded = true;
			}

			if ((whiteTurn && selectedPiece.PieceValue > PieceConstants.whiteKingValue) || (!whiteTurn && selectedPiece.PieceValue < PieceConstants.blackPawnValue))
			{
				return false;
			}

			int row = s[0] - '0';
            int col = s[1] - '0';

            bool[,] staleArray = new bool[8, 8];
            foreach (Piece piece in chessBoard.board)
            {
                if (piece.Color == Pieces.Color.White != whiteTurn)
                {
                    staleArray = piece.checkForStale(chessBoard.board, staleArray);
                }
            }

            if (dragEnded)
            {
                if (selectedPiece is King)
                {
                    availableMoves = selectedPiece.calculatePossibleMoves(chessBoard.board, availableMoves, staleArray);
                }
                else
                {
                    availableMoves = selectedPiece.calculatePossibleMoves(chessBoard.board, availableMoves);
                }
                dragEnded = false;
                lastposition = selectedPiece.Position!;
                lastTurn = whiteTurn;
                removeInvalidMoves(selectedPiece, row, col);
            }

            return availableMoves[row, col];
        }
        private async void pieceUpdated(MudItemDropInfo<Piece> piece)
        {
            int newRow = piece.DropzoneIdentifier[0] - '0';
            int newCol = piece.DropzoneIdentifier[1] - '0';
            bool isHittedPiece = list.Any(p => p.PieceValue != piece.Item.PieceValue && p.Position == piece.DropzoneIdentifier);
            if (isHittedPiece)
            {
                list.FirstOrDefault(p => p.Position == piece.DropzoneIdentifier)!.Position = null;
            }
            (int a, int b) = piece.Item.getPositionTuple();


            chessBoard.SetPiece(newRow, newCol, piece.Item);
            dragEnded = true;
            availableMoves = new bool[8, 8];
            bool checkMate = CheckMate.isCheckMate(chessBoard.board, whiteTurn);
            whiteTurn = !whiteTurn;

            if (player.IsMyTurn)
            {
                await HandleMove(a, b, newRow, newCol);
            }

            player.IsMyTurn = !player.IsMyTurn;
            isStale = checkMate || Stale.staleChecker(chessBoard.board, whiteTurn);

            if (checkMate)
            {
                bool? result = await DialogService!.ShowMessageBox(
                            "Sakkmatt",
                            "later",
                            yesText: "Exit!", cancelText: "Again");
                if (result.HasValue && result.Value)
                {
                    //InitGame();
                    StateHasChanged();
                }
            }

        }
        public void removeInvalidMoves(Piece piece, int row, int col)
        {
            for (int newRow = 0; newRow < 8; newRow++)
            {
                for (int newCol = 0; newCol < 8; newCol++)
                {
                    if (availableMoves[newRow, newCol])
                    {
                        Piece lastHitPiece = chessBoard.board[newRow, newCol];

                        chessBoard.board[newRow, newCol] = piece;
                        chessBoard.board[newRow, newCol].setPosition($"{row}{col}", true);
                        chessBoard.board[row, col].setPosition($"{newRow}{newCol}", true);
                        chessBoard.board[row, col] = new EmptyPiece();

                        if (whiteTurn && Stale.staleChecker(chessBoard.board, whiteTurn))
                        {
                            availableMoves[newRow, newCol] = false;
                        }
                        if (!whiteTurn && Stale.staleChecker(chessBoard.board, whiteTurn))
                        {
                            availableMoves[newRow, newCol] = false;
                        }

                        chessBoard.board[newRow, newCol] = lastHitPiece;
                        chessBoard.board[newRow, newCol].setPosition($"{newRow}{newCol}", true);
                        chessBoard.board[row, col] = piece;
                        chessBoard.board[row, col].setPosition($"{row}{col}", true);
                    }
                }
            }
        }

		private async Task joinGame(string groupName)
		{
			if (!string.IsNullOrEmpty(gameName))
			{
				await hubConnection.SendAsync("LeaveGame", gameName);
			}

			gameName = groupName;
			await hubConnection.SendAsync("JoinGame", gameName);
            if (UserHandler.connectedPlayers.ContainsKey(groupName))
            {
                UserHandler.connectedPlayers[groupName] = 2;
            }
            else
            {
                UserHandler.connectedPlayers.Add(groupName, 1);
            }

            player.IsMyTurn = UserHandler.connectedPlayers[gameName] == 1;
        }

		private async Task HandleMove(int fromX, int fromY, int toX, int toY)
		{
			await hubConnection.SendAsync("MovePiece", gameName, fromX, fromY, toX, toY);
		}

		public void Dispose()
        {
            UserHandler.connectedPlayers.Remove(gameName);
        }
    }
}
