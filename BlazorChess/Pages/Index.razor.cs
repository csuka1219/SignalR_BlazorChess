using BlazorChess.Data;
using BlazorChess.Game;
using BlazorChess.Pieces;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlazorChess.Pages
{
    public partial class Index : IDisposable
    {
        [Inject] 
        private IDialogService? DialogService { get; set; }

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
        public bool canDrop(Piece selectedPiece, string s)
        {
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

            chessBoard.SetPiece(newRow, newCol, piece.Item);
            dragEnded = true;
            availableMoves = new bool[8, 8];
            bool checkMate = CheckMate.isCheckMate(chessBoard.board, whiteTurn);
            whiteTurn = !whiteTurn;
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

        public void Dispose()
        {
            //disconnect
        }
    }
}
