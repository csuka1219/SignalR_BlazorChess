﻿using MudBlazor.Extensions;

namespace BlazorChess.Pieces
{
    public class King : Piece
    {
        public bool AbleToCastling { get; set; }

        public King(Color color, int pieceValue, string? icon, string? position) : base(color, pieceValue, icon, position)
        {
            AbleToCastling = true;
        }

        public override bool[,] calculatePossibleMoves(Piece[,] board, bool[,] availableMoves, bool[,] staleArray)
        {
            (int row, int col) = this.getPositionTuple();
            int size = 8;

            // Check for valid moves in all directions
            checkMove(row, col - 1);
            checkMove(row, col + 1);
            checkMove(row - 1, col);
            checkMove(row + 1, col);
            checkMove(row - 1, col - 1);
            checkMove(row - 1, col + 1);
            checkMove(row + 1, col - 1);
            checkMove(row + 1, col + 1);

            // Check for additional moves if not in 'all' mode and castling conditions are met
            if (AbleToCastling)
            {
                checkCastling();
            }

            void checkMove(int r, int c)
            {
                if (IsValidMove(r, c, board))
                {
                    availableMoves[r, c] = true;
                }
            }

            void checkCastling()
            {
                if (Color == Color.White)
                {
                    CheckCastlingForRook(7, 0, size - 1, 2, 3);
                    CheckCastlingForRook(7, 7, size - 1, 6, 5);
                }
                else
                {
                    CheckCastlingForRook(0, 0, 0, 2, 3);
                    CheckCastlingForRook(0, 7, 0, 6, 5);
                }
            }

            void CheckCastlingForRook(int rookRow, int rookCol, int row, int targetCol1, int targetCol2)
            {
                var rook = board[rookRow, rookCol];
                if (rook.GetType() == typeof(Rook) && rook.As<Rook>().AbleToCastling && board[row, targetCol1].PieceValue == 0 && board[row, targetCol2].PieceValue == 0 && !staleArray[row, targetCol1] && !staleArray[row, targetCol2])
                {
                    availableMoves[row, targetCol1] = true;
                }
            }

            return base.calculatePossibleMoves(board, availableMoves);
        }

        private bool IsValidMove(int r, int c, Piece[,] board)
        {
            if (r >= 0 && r < 8 && c >= 0 && c < 8)
            {
                if (Color == Color.White)
                {
                    return board[r, c].PieceValue > 10 || board[r, c].PieceValue == 0;
                }
                else
                {
                    return board[r, c].PieceValue == 0 || board[r, c].PieceValue < 10;
                }
            }
            return false;
        }

        public override bool[,] checkForStale(Piece[,] board, bool[,] staleArray)
        {
            staleArray = this.calculatePossibleMoves(board, staleArray, new bool[8,8]);
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
