﻿using BlazorChess.Data;

namespace BlazorChess.Pieces
{
    public class Queen : Piece
    {
        public Queen(Color color, int pieceValue, string? icon, string? position) : base(color, pieceValue, icon, position)
        {
        }
        public override bool[,] calculatePossibleMoves(Piece[,] board, bool[,] availableMoves)
        {
            (int row, int col) = this.getPositionTuple();
            availableMoves = MoveGenerator.generateDiagonalMoves(row, col, this.Color, board, availableMoves);
            availableMoves = MoveGenerator.generateStraightMoves(row, col, this.Color, board, availableMoves);
            return base.calculatePossibleMoves(board, availableMoves);
        }
        public override bool[,] checkForStale(Piece[,] board, bool[,] staleArray)
        {
            staleArray = this.calculatePossibleMoves(board, staleArray);
            return base.checkForStale(board, staleArray);
        }
    }
}
