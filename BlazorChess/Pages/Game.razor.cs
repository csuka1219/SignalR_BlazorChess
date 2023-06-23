using BlazorChess.Data;
using BlazorChess.Game;
using BlazorChess.Pieces;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System.Data;

namespace BlazorChess.Pages
{
    public partial class Game : IDisposable
    {
        [Inject]
        private ILocalStorageService localStorage { get; set; }

        [Inject] 
        private IDialogService? dialogService { get; set; }

		[Inject]
		private NavigationManager? navigationManager { get; set; }

        [Parameter]
        public string gameName { get; set; } = string.Empty;

        private Chessboard chessBoard = new Chessboard();
        private Player player = new Player();
        IEnumerable<Piece> list = new List<Piece>();
        private MudDropContainer<Piece> _container;
        private List<PieceChange> pieceChanges = new List<PieceChange>();

        bool[,] availableMoves = new bool[8, 8];
        public bool whiteTurn = true;
        public bool lastTurn = true;
        public bool isStale = false;
		bool dragEnded = true;
        public string lastposition = "";

        private HubConnection hubConnection;
        protected override async Task OnInitializedAsync()
        {
            // Convert the chessboard pieces to a list for UI component
            list = chessBoard.board.Cast<Piece>().ToList();

            // Create a new hub connection for the chess game
            hubConnection = new HubConnectionBuilder()
                .WithUrl(navigationManager!.ToAbsoluteUri("/chessHub"))
                .Build();

            // Register a callback for receiving moves from the hub
            hubConnection.On<int, int, int, int>("ReceiveMove", (fromX, fromY, toX, toY) =>
            {
                // Check if it's not the player's turn
                if (!player.IsMyTurn)
                {
                    // Create a MudItemDropInfo object representing the received move
                    MudItemDropInfo<Piece> piece = new MudItemDropInfo<Piece>(list.First(x => x.Position == $"{fromX}{fromY}"), $"{toX}{toY}", -1);

                    // Handle the received move
                    pieceUpdated(piece);

                    // Update the chess board UI
                    _container.Refresh();
                    StateHasChanged();
                }
            });

            // Start the hub connection
            await hubConnection.StartAsync();

            // Join the game
            await joinGame();
        }

        public bool canDrop(Piece selectedPiece, string s)
        {
            // Check if it's not the player's turn or the piece is not within the valid range for the current turn
            if (!player.IsMyTurn || (!whiteTurn && selectedPiece.PieceValue < PieceConstants.blackPawnValue) || (whiteTurn && selectedPiece.PieceValue > PieceConstants.whiteKingValue))
            {
                return false;
            }

            // Check if the turn and piece position match the previous turn and position, and the drag has not ended
            if (whiteTurn == lastTurn && selectedPiece.Position != lastposition && !dragEnded)
            {
                // Reset available moves and end the drag
                availableMoves = new bool[8, 8];
                dragEnded = true;
            }

            // Extract the row and column from the string representation of the dropzone identifier
            int row = s[0] - '0';
            int col = s[1] - '0';

            // Create a stale array to track stalemate positions
            bool[,] staleArray = new bool[8, 8];

            // Iterate over each piece on the chessboard
            foreach (Piece piece in chessBoard.board)
            {
                // Check if the piece's color matches the opposite of the current turn
                if (piece.Color == Pieces.Color.White != whiteTurn)
                {
                    // Update the stale array with stale positions for the opposite color
                    staleArray = piece.checkForStale(chessBoard.board, staleArray);
                }
            }

            // Check if the drag has ended
            if (dragEnded)
            {
                // Calculate possible moves for the selected piece based on the chessboard and stale positions
                if (selectedPiece is King)
                {
                    availableMoves = selectedPiece.calculatePossibleMoves(chessBoard.board, availableMoves, staleArray);
                }
                else
                {
                    availableMoves = selectedPiece.calculatePossibleMoves(chessBoard.board, availableMoves);
                }

                // Reset the drag flag, store the current position and turn, and remove invalid moves
                dragEnded = false;
                lastposition = selectedPiece.Position!;
                lastTurn = whiteTurn;
                removeInvalidMoves(selectedPiece, row, col);
            }

            // Check if the specified row and column are valid moves
            return availableMoves[row, col];
        }

        private async void pieceUpdated(MudItemDropInfo<Piece> piece)
        {
            // Extract the new row and column from the dropzone identifier
            int newRow = piece.DropzoneIdentifier[0] - '0';
            int newCol = piece.DropzoneIdentifier[1] - '0';

            // Check if the dropped piece captures another piece
            bool isHitPiece = list.Any(p => p.PieceValue != piece.Item!.PieceValue && p.Position == piece.DropzoneIdentifier);
            int hitpieceValue = 0;
            if (isHitPiece)
            {
                // Clear the position of the captured piece
                Piece hitPiece = list.First(p => p.Position == piece.DropzoneIdentifier);
                hitPiece.Position = null;
                hitpieceValue = hitPiece.PieceValue;
            }

            // Get the current position of the moved piece
            (int oldRow, int oldCol) = piece.Item!.getPositionTuple();

            //Store move
            pieceChanges.Add(new PieceChange((oldRow, oldCol), (newRow, newCol), piece.Item.PieceValue, hitpieceValue));

            // Set the piece on the chessboard to the new position
            chessBoard.SetPiece(newRow, newCol, piece.Item);

            // Set the dragEnded flag to true
            dragEnded = true;

            // Reset the available moves array
            availableMoves = new bool[8, 8];

            // Check if the game is in checkmate state
            bool checkMate = CheckMate.isCheckMate(chessBoard.board, whiteTurn);

            // Toggle the turn to the opposite player
            whiteTurn = !whiteTurn;

            // Check if it's the player's turn to handle the move
            if (player.IsMyTurn)
            {
                // Handle the move asynchronously
                await HandleMove(oldRow, oldCol, newRow, newCol);
            }

            // Toggle the turn for the player
            player.IsMyTurn = !player.IsMyTurn;

            // Check if the game is in a stalemate state
            isStale = checkMate || Stale.staleChecker(chessBoard.board, whiteTurn);

            // Check if the game is in checkmate state
            if (checkMate)
            {
                // Show a message box with options to exit or play again
                bool? result = await dialogService!.ShowMessageBox(
                    "Sakkmatt",
                    "later",
                    yesText: "Exit!", cancelText: "Again");

                if (result.HasValue && result.Value)
                {
                    // Perform necessary actions for exiting or starting a new game
                    // InitGame();
                    StateHasChanged();
                }
            }
        }

        public void removeInvalidMoves(Piece piece, int row, int col)
        {
            // Iterate over each cell on the chessboard
            for (int newRow = 0; newRow < 8; newRow++)
            {
                for (int newCol = 0; newCol < 8; newCol++)
                {
                    // Check if the piece can move into the current cell
                    if (availableMoves[newRow, newCol])
                    {
                        // Store the piece that might be captured in the destination cell
                        Piece lastHitPiece = chessBoard.board[newRow, newCol];

                        // Move the piece to the new cell and update its position
                        chessBoard.board[newRow, newCol] = piece;
                        chessBoard.board[newRow, newCol].setPosition($"{row}{col}", true);
                        chessBoard.board[row, col].setPosition($"{newRow}{newCol}", true);

                        // Replace the original cell with an empty piece
                        chessBoard.board[row, col] = new EmptyPiece();

                        // Check if the move leads to a stale for the player
                        if (whiteTurn && Stale.staleChecker(chessBoard.board, whiteTurn))
                        {
                            // Mark the move as invalid
                            availableMoves[newRow, newCol] = false;
                        }
                        if (!whiteTurn && Stale.staleChecker(chessBoard.board, whiteTurn))
                        {
                            // Mark the move as invalid
                            availableMoves[newRow, newCol] = false;
                        }

                        // Restore the original positions and pieces on the chessboard
                        chessBoard.board[newRow, newCol] = lastHitPiece;
                        chessBoard.board[newRow, newCol].setPosition($"{newRow}{newCol}", true);
                        chessBoard.board[row, col] = piece;
                        chessBoard.board[row, col].setPosition($"{row}{col}", true);
                    }
                }
            }
        }

        private string convertMoveToString(PieceChange pieceChange)
        {
            Dictionary<int, string> pieceMap = new Dictionary<int, string>
            {
                { 1, "" }, { 2, "r" }, { 3, "n" }, { 4, "b" },
                { 5, "q" }, { 6, "k" }, { 11, "" }, { 12, "r" },
                { 13, "n" }, { 14, "b" }, { 15, "q" }, { 16, "k" }
            };

            Dictionary<int, string> fileMap = new Dictionary<int, string>
            {
                { 0, "a" }, { 1, "b" }, { 2, "c" }, { 3, "d" },
                { 4, "e" }, { 5, "f" }, { 6, "g" }, { 7, "h" }
            };

            Dictionary<int, string> rankMap = new Dictionary<int, string>
            {
                { 0, "1" }, { 1, "2" }, { 2, "3" }, { 3, "4" },
                { 4, "5" }, { 5, "6" }, { 6, "7" }, { 7, "8" }
            };

            int rowIndex = 7 - pieceChange.toMove.row;
            int colIndex = pieceChange.toMove.col;

            string isHit = pieceChange.hitPiece == 0 ? "" : "x";
            string piece = pieceMap[pieceChange.movedPieceValue];
            piece = pieceChange.hitPiece != 0 && string.IsNullOrEmpty(piece) ? fileMap[pieceChange.fromMove.col] : piece;
            string row = rankMap[rowIndex];
            string col = fileMap[colIndex];

            return piece + isHit + col + row;
        }

        private async Task joinGame()
        {
            // Retrieve the unique GUID from local storage
            string uniqueGuid = await localStorage.GetItemAsync<string>("uniqueGuid");

            // Join the new game
            await hubConnection.SendAsync("JoinGame", gameName);

            // Check if there are already connected players for the game
            if (UserHandler.connectedPlayers.ContainsKey(gameName))
            {
                if (UserHandler.connectedPlayers[gameName].Count > 1 && !UserHandler.connectedPlayers[gameName].Contains(uniqueGuid))
                {
                    navigationManager!.NavigateTo("/");
                    return;
                }
                // Check if the current player is already connected to the game
                if (UserHandler.connectedPlayers[gameName].Contains(uniqueGuid))
                {
                    // The player is refreshing or renavigating
                    // Update the chessboard, pieces list, and player turn
                    chessBoard.board = UserHandler.getMatchInfoBoard(gameName);
                    pieceChanges = UserHandler.getMatchInfoMoves(gameName);
                    list = chessBoard.board.Cast<Piece>().ToList();
                    bool isWhitePlayer = UserHandler.connectedPlayers[gameName].First() == uniqueGuid;
                    player.IsMyTurn = isWhitePlayer == UserHandler.matchInfos[gameName].isWhiteTurn;
                    whiteTurn = UserHandler.matchInfos[gameName].isWhiteTurn;
                    StateHasChanged();
                    _container.Refresh();
                }
                else
                {
                    // The second player has connected
                    // Add the current player to the connected players list and set the turn to false
                    UserHandler.connectedPlayers[gameName].Add(uniqueGuid);
                    player.IsMyTurn = false;
                }
            }
            else
            {
                // First player to connect to the game
                // Create new entries for connected players and match information
                UserHandler.connectedPlayers.Add(gameName, new List<string>() { uniqueGuid });
                UserHandler.matchInfos.Add(gameName, new MatchInfo());
                player.IsMyTurn = true;
            }
        }

        private async Task HandleMove(int fromRow, int fromCol, int toRow, int toCol)
        {
            // Send a move piece request to the hub with the game name and coordinates of the move
            await hubConnection.SendAsync("MovePiece", gameName, fromRow, fromCol, toRow, toCol);
        }

        // Implementation of the IDisposable interface to perform cleanup when the object is disposed
        public void Dispose()
        {
            // Update the match information with the current state of the chessboard
            UserHandler.setMatchInfoBoard(gameName, chessBoard.board);
            UserHandler.setMatchInfoMoves(gameName, pieceChanges, whiteTurn);
        }

    }
}
