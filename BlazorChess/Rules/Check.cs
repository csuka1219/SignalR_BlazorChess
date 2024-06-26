﻿using BlazorChess.Pieces;
using MudBlazor.Extensions;

namespace BlazorChess.Data
{
    public static class Check
    {
        public static bool checkChecker(Piece[,] board, bool whiteTurn)
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

                    // after the current player moved anything, every en passant opportunity dissapear from the table
                    if (piece is Pawn)
                    {
                        piece.As<Pawn>().ableToEnPassant = false;
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
