using BlazorChess.Data;

namespace BlazorChess.Pieces
{
    public class Rook : Piece
    {
        public bool AbleToCastling { get; set; }
        public Rook(Color color, int pieceValue, string? icon, string? position) : base(color, pieceValue, icon, position)
        {
            AbleToCastling = true;
        }

        public override bool[,] calculatePossibleMoves(Piece[,] board, bool[,] availableMoves)
        {
            (int row, int col) = this.getPositionTuple();
            availableMoves = MoveGenerator.generateStraightMoves(row, col, this.Color, board, availableMoves);
            return base.calculatePossibleMoves(board, availableMoves);
        }

        public override bool[,] checkForStale(Piece[,] board, bool[,] staleArray)
        {
            staleArray = this.calculatePossibleMoves(board, staleArray);
            return base.checkForStale(board, staleArray);
        }

        public override void setPosition(string? position)
        {
            if (AbleToCastling)
            {
                AbleToCastling = false;
            }
            base.setPosition(position);
        }

        public override void setPosition(string? position, bool simulate)
        {
            base.setPosition(position);
        }
    }
}
