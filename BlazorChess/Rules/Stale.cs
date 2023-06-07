using BlazorChess.Pieces;

namespace BlazorChess.Data
{
    public static class Stale
    {
        public static bool staleChecker(Piece[,] board, bool whiteTurn)
        {
            bool[,] staleArray = new bool[8, 8];
            string kingPosition = "";
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
                        kingPosition = piece.Position!;
                    }
                }
            }

            if (kingPosition == "")
            {
                return true;
            }

            int kingRow = int.Parse(kingPosition.Substring(0, 1));
            int kingCol = int.Parse(kingPosition.Substring(1, 1));
            return staleArray[kingRow, kingCol];
        }
    }
}
