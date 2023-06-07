using BlazorChess.Data;
using BlazorChess.Pieces;
using MudBlazor;
using MudBlazor.Extensions;
using Color = BlazorChess.Pieces.Color;

namespace BlazorChess.Game
{
    public class Chessboard
    {
        // Define the chessboard table
        public Piece[,] board = new Piece[8, 8];

        public Chessboard()
        {
            // Initialize the chessboard with the starting position
            Initialize();
        }

        // Initialize the chessboard with the starting position
        private void Initialize()
        {
            // Place the white pieces
            board[7, 0] = new Rook(Color.White, PieceConstants.whiteRookValue, @Icons.Custom.Uncategorized.ChessRook, "70");
            board[7, 1] = new Knight(Color.White, PieceConstants.whiteKnightValue, @Icons.Custom.Uncategorized.ChessKnight, "71");
            board[7, 2] = new Bishop(Color.White, PieceConstants.whiteBishopValue, @Icons.Custom.Uncategorized.ChessBishop, "72");
            board[7, 3] = new Queen(Color.White, PieceConstants.whiteQueenValue, @Icons.Custom.Uncategorized.ChessQueen, "73");
            board[7, 4] = new King(Color.White, PieceConstants.whiteKingValue, @Icons.Custom.Uncategorized.ChessKing, "74");
            board[7, 5] = new Bishop(Color.White, PieceConstants.whiteBishopValue, @Icons.Custom.Uncategorized.ChessBishop, "75");
            board[7, 6] = new Knight(Color.White, PieceConstants.whiteKnightValue, @Icons.Custom.Uncategorized.ChessKnight, "76");
            board[7, 7] = new Rook(Color.White, PieceConstants.whiteRookValue, @Icons.Custom.Uncategorized.ChessRook, "77");
            for (int i = 0; i < 8; i++)
            {
                board[6, i] = new Pawn(Color.White, PieceConstants.whitePawnValue, Icons.Custom.Uncategorized.ChessPawn, "6" + i.ToString());
            }

            // Place the black pieces
            board[0, 0] = new Rook(Color.Black, PieceConstants.blackRookValue, @Icons.Custom.Uncategorized.ChessRook, "00");
            board[0, 1] = new Knight(Color.Black, PieceConstants.blackKnightValue, @Icons.Custom.Uncategorized.ChessKnight, "01");
            board[0, 2] = new Bishop(Color.Black, PieceConstants.blackBishopValue, @Icons.Custom.Uncategorized.ChessBishop, "02");
            board[0, 3] = new Queen(Color.Black, PieceConstants.blackQueenValue, @Icons.Custom.Uncategorized.ChessQueen, "03");
            board[0, 4] = new King(Color.Black, PieceConstants.blackKingValue, @Icons.Custom.Uncategorized.ChessKing, "04");
            board[0, 5] = new Bishop(Color.Black, PieceConstants.blackBishopValue, @Icons.Custom.Uncategorized.ChessBishop, "05");
            board[0, 6] = new Knight(Color.Black, PieceConstants.blackKnightValue, @Icons.Custom.Uncategorized.ChessKnight, "06");
            board[0, 7] = new Rook(Color.Black, PieceConstants.blackRookValue, @Icons.Custom.Uncategorized.ChessRook, "07");
            for (int i = 0; i < 8; i++)
            {
                board[1, i] = new Pawn(Color.Black, PieceConstants.blackPawnValue, Icons.Custom.Uncategorized.ChessPawn, "1" + i.ToString());
            }

            // Fill the remaining cells with empty pieces
            for (int i = 2; i < 6; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    board[i, j] = new EmptyPiece();
                }
            }
        }

        // Set a piece at a specific cell on the chessboard
        public void SetPiece(int row, int col, Piece piece)
        {
            if (piece is King && piece.As<King>().AbleToCastling && new List<string> { "72", "76", "02", "06" }.Contains($"{row}{col}"))
            {
                Castling.castling(this, $"{row}{col}");
            }
            int oldRow = int.Parse(piece.Position![..1]);
            int oldCol = int.Parse(piece.Position!.Substring(1, 1));
            piece.setPosition($"{row}{col}");
            board[oldRow, oldCol] = new EmptyPiece();
            board[row, col] = piece;
        }
    }
}
