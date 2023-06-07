using BlazorChess.Data;

namespace BlazorChess.Pieces
{
    public class Pawn : Piece
    {
        public Pawn(Color color, int pieceValue, string? icon, string? position) : base(color, pieceValue, icon, position)
        {
        }

        public Pawn(Piece piece) : base(piece)
        {
        }

        public override bool[,] calculatePossibleMoves(Piece[,] board, bool[,] availableMoves)
        {
            (int row, int col) = this.getPositionTuple();
            if (row == 7 || row == 0) return availableMoves;

            bool isWhiteTurn = this.Color == Color.White;
            int forwardDirection = isWhiteTurn ? -1 : 1;
            int startingRow = isWhiteTurn ? 6 : 1;

            int leftDiagonalCol = col - 1;
            int rightDiagonalCol = col + 1;

            if (isValidMove(board, row + forwardDirection, col, isWhiteTurn))
            {
                availableMoves[row + forwardDirection, col] = true;
            }

            if (isValidCapture(board, row + forwardDirection, leftDiagonalCol, isWhiteTurn))
            {
                availableMoves[row + forwardDirection, leftDiagonalCol] = true;
            }

            if (isValidCapture(board, row + forwardDirection, rightDiagonalCol, isWhiteTurn))
            {
                availableMoves[row + forwardDirection, rightDiagonalCol] = true;
            }

            if (row == startingRow)
            {
                int doubleMoveRow = row + 2 * forwardDirection;
                if (isValidMove(board, doubleMoveRow, col, isWhiteTurn) && board[row + forwardDirection, col].PieceValue == 0)
                {
                    availableMoves[doubleMoveRow, col] = true;
                }
            }

            return base.calculatePossibleMoves(board, availableMoves);
        }

        private bool isValidMove(Piece[,] board, int row, int col, bool isWhiteTurn)
        {
            if (row >= 0 && row < 8 && col >= 0 && col < 8)
            {
                if (board[row, col].PieceValue == 0)
                {
                    return true;
                }
            }
            return false;
        }

        private bool isValidCapture(Piece[,] board, int row, int col, bool isWhiteTurn)
        {
            if (row >= 0 && row < 8 && col >= 0 && col < 8)
            {
                int pieceValue = board[row, col].PieceValue;

                if (isWhiteTurn && pieceValue > 10 && pieceValue != 0)
                {
                    return true;
                }
                else if (!isWhiteTurn && pieceValue < 10 && pieceValue != 0)
                {
                    return true;
                }
            }
            return false;
        }


        public override bool[,] checkForStale(Piece[,] board, bool[,] staleArray)
        {
            bool isWhite = this.Color == Color.White;
            int direction = isWhite ? 1 : -1;

            (int i, int j) = this.getPositionTuple();

            int leftDiagonalCol = j - 1;
            int rightDiagonalCol = j + 1;
            if (leftDiagonalCol >= 0)
            {
                staleArray[i - direction, leftDiagonalCol] = true;
            }

            if (rightDiagonalCol < 8)
            {
                staleArray[i - direction, rightDiagonalCol] = true;
            }

            return base.checkForStale(board, staleArray);
        }
    }
}
