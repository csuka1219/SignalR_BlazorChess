﻿using BlazorChess.Pieces;

namespace BlazorChess.Data
{
    public static class CheckMate
    {
        public static bool isCheckMate(Piece[,] board, bool whiteTurn)
        {
            whiteTurn = !whiteTurn;
            foreach (Piece piece in board)
            {
                if (piece is EmptyPiece)
                {
                    continue;
                }

                bool[,] availableMoves = new bool[8, 8];
                (int row, int col) = piece.getPositionTuple();

                if (piece.Color == Color.White == whiteTurn)
                {
                    availableMoves = piece.calculatePossibleMoves(board, availableMoves);
                    if (checkAvailableMoves(board, piece, availableMoves, row, col, whiteTurn))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool checkAvailableMoves(Piece[,] board, Piece piece, bool[,] availableMoves, int row, int col, bool isWhiteTurn)
        {
            for (int newRow = 0; newRow < 8; newRow++)
            {
                for (int newCol = 0; newCol < 8; newCol++)
                {
                    if (availableMoves[newRow, newCol])
                    {
                        Piece lastHitPiece = board[newRow, newCol];
                        MovePiece(board, piece, newRow, newCol, row, col);

                        if (isWhiteTurn && Check.checkChecker(board, isWhiteTurn))
                        {
                            availableMoves[newRow, newCol] = false;
                        }
                        else if (isWhiteTurn)
                        {
                            UndoMove(board, lastHitPiece, newRow, newCol, row, col);
                            return true;
                        }
                        if (!isWhiteTurn && Check.checkChecker(board, isWhiteTurn))
                        {
                            availableMoves[newRow, newCol] = false;
                        }
                        else if (!isWhiteTurn)
                        {
                            UndoMove(board, lastHitPiece, newRow, newCol, row, col);
                            return true;
                        }

                        UndoMove(board, lastHitPiece, newRow, newCol, row, col);
                    }
                }
            }
            return false;
        }

        private static void MovePiece(Piece[,] board, Piece piece, int newRow, int newCol, int oldRow, int oldCol)
        {
            board[newRow, newCol] = piece;
            board[newRow, newCol].setPosition($"{oldRow}{oldCol}", true);
            board[oldRow, oldCol].setPosition($"{newRow}{newCol}", true);
        }

        private static void UndoMove(Piece[,] board, Piece lastHitPiece, int newRow, int newCol, int oldRow, int oldCol)
        {
            board[newRow, newCol] = lastHitPiece;
            board[newRow, newCol].setPosition($"{newRow}{newCol}", true);
            board[oldRow, oldCol].setPosition($"{oldRow}{oldCol}", true);
        }


    }
}