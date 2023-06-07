using BlazorChess.Data;
using BlazorChess.Game;
using BlazorChess.Pieces;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlazorChess.Pages
{
    public partial class Index : IDisposable
    {
        [Inject] private IDialogService? DialogService { get; set; }

        bool dragEnded = true;
        private Chessboard chessBoard = new Chessboard();
        IEnumerable<Piece> list = new List<Piece>();
        bool[,] availableMoves = new bool[8, 8];
        public bool whiteTurn = true;
        public bool lastTurn = true;
        public bool isStale = false;
        public string lastposition = "";
        protected override void OnInitialized()
        {
            list = chessBoard.board.Cast<Piece>().ToList();
            //todo connect
        }
        public bool CanDrop(Piece piece, string s)
        {
            if (whiteTurn == lastTurn && piece.Position != lastposition && !dragEnded)
            {
                availableMoves = new bool[8, 8];
                dragEnded = true;
            }
            if (whiteTurn && piece.PieceValue > PieceConstants.whiteKingValue)
            {
                return false;
            }
            if (!whiteTurn && piece.PieceValue < PieceConstants.blackPawnValue)
            {
                return false;
            }
            int i = s[0] - '0';
            int j = s[1] - '0';

            bool[,] staleArray = new bool[8, 8];
            foreach (Piece piece2 in chessBoard.board)
            {
                if (piece2.Color == Pieces.Color.White != whiteTurn)
                {
                    staleArray = piece2.checkForStale(chessBoard.board, staleArray);
                }
            }

            if (dragEnded)
            {
                if (piece is King)
                {
                    availableMoves = piece.calculatePossibleMoves(chessBoard.board, availableMoves, staleArray);
                }
                else
                {
                    availableMoves = piece.calculatePossibleMoves(chessBoard.board, availableMoves);
                }
                dragEnded = false;
                lastposition = piece.Position!;
                lastTurn = whiteTurn;
                RemoveMoveThatNotPossible(piece, i, j);
            }
            if (availableMoves[i, j] == true)
            {
                return true;
            }
            return false;
        }

        private async void ItemUpdated(MudItemDropInfo<Piece> piece)
        {
            int i = piece.Item.Position![0] - '0';
            int j = piece.Item.Position[1] - '0';
            i = piece.DropzoneIdentifier[0] - '0';
            j = piece.DropzoneIdentifier[1] - '0';
            bool isHittedPiece = list.Any(p => p.PieceValue != piece.Item.PieceValue && p.Position == piece.DropzoneIdentifier);
            if (isHittedPiece)
            {
                list.FirstOrDefault(p => p.Position == piece.DropzoneIdentifier)!.Position = null;
            }
            chessBoard.SetPiece(i, j, piece.Item);
            dragEnded = true;
            availableMoves = new bool[8, 8];
            bool checkMate = CheckMate.isCheckMate(chessBoard.board, whiteTurn);
            whiteTurn = !whiteTurn;
            isStale = Stale.staleChecker(chessBoard.board, whiteTurn);

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
        public void RemoveMoveThatNotPossible(Piece piece, int row, int col)
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

        public void Dispose()
        {
            //disconnect
        }
    }
}
