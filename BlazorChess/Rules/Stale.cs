using BlazorChess.Pieces;

namespace BlazorChess.Data
{
    public static class Stale
    {
        public static bool staleChecker(Piece[,] board, bool whiteTurn)
        {
            bool[,] staleArray = new bool[8, 8];
            int kingRow = -1, kingCol = -1;
            foreach (Piece piece in board)
            {
                if (piece.Color == Color.White != whiteTurn)
                {
                    staleArray = piece.checkForStale(board, staleArray);
                }
                else
                {
                    if (piece is King)
                    {
                        (kingRow, kingCol) = piece.getPositionTuple();
                    }
                }
            }

            if (kingRow == -1)
            {
                return true;
            }

            return staleArray[kingRow, kingCol];
        }
    }
}
