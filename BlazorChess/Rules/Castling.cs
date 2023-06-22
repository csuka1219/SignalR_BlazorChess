using BlazorChess.Game;
using BlazorChess.Pieces;
using MudBlazor.Extensions;
using System.Text;

namespace BlazorChess.Data
{
    public static class Castling
    {
        public static void castling(Chessboard chessBoard, string position)
        {
            switch (position)
            {
                case "72":
                    chessBoard.board[7, 0].setPosition("73");
                    Piece rook = chessBoard.board[7, 0];
                    chessBoard.board[7, 0] = new EmptyPiece();
                    chessBoard.board[7, 3] = rook;
                    break;
                case "76":
                    chessBoard.board[7, 7].setPosition("75");
                    rook = chessBoard.board[7, 7];
                    chessBoard.board[7, 7] = new EmptyPiece();
                    chessBoard.board[7, 5] = rook;
                    break;
                case "02":
                    chessBoard.board[0, 0].setPosition("03");
                    rook = chessBoard.board[0, 0];
                    chessBoard.board[0, 0] = new EmptyPiece();
                    chessBoard.board[0, 3] = rook;
                    break;
                case "06":
                    chessBoard.board[0, 7].setPosition("05");
                    rook = chessBoard.board[0, 7];
                    chessBoard.board[0, 7] = new EmptyPiece();
                    chessBoard.board[0, 5] = rook;
                    break;
            }
        }

        public static string getCastlingAvailability(Piece[,] board)
        {
            StringBuilder sb = new StringBuilder();

            if (canCastleKingSide(Color.White, board))
            {
                sb.Append('K');
            }
            if (canCastleQueenSide(Color.White, board))
            {
                sb.Append('Q');
            }
            if (canCastleKingSide(Color.Black, board))
            {
                sb.Append('k');
            }
            if (canCastleQueenSide(Color.Black, board))
            {
                sb.Append('q');
            }

            return sb.Length > 0 ? sb.ToString() : "-";
        }

        private static bool canCastleKingSide(Pieces.Color color, Piece[,] board)
        {
            int rank = (color == Color.White) ? 7 : 0;

            // Check if the king and rook are in their initial positions
            Piece king = board[rank, 4];
            Piece rook = board[rank, 7];

            if (king is King && rook is Rook && king.Color == color && rook.Color == color)
            {
                return king.As<King>().ableToCastling && rook.As<Rook>().ableToCastling;
            }

            return false;
        }
        private static bool canCastleQueenSide(Color color, Piece[,] board)
        {
            int rank = (color == Color.White) ? 7 : 0;

            // Check if the king and rook are in their initial positions
            Piece king = board[rank, 4];
            Piece rook = board[rank, 0];

            if (king is King && rook is Rook && king.Color == color && rook.Color == color)
            {
                return king.As<King>().ableToCastling && rook.As<Rook>().ableToCastling;
            }

            return false;
        }

        //-----------------------------------------

        public static void setCastlingAvailability(Piece[,] board, string castlingAvailability)
        {
            if (castlingAvailability != "-")
            {
                string[] availableCastling = castlingAvailability.Select(c => c.ToString()).ToArray();

                foreach (string castling in availableCastling)
                {
                    Color color = char.IsUpper(castling[0]) ? Color.White : Color.Black;

                    switch (char.ToLower(castling[0]))
                    {
                        case 'k':
                            setKingCastlingAvailability(board, color, true);
                            break;
                        case 'q':
                            setQueenCastlingAvailability(board, color, true);
                            break;
                        default:
                            setRookCastlingAvailability(board, color, castling[0], true);
                            break;
                    }
                }
            }
        }

        private static void setRookCastlingAvailability(Piece[,] board, Color color, char rookFEN, bool ableToCastle)
        {
            int rank = (color == Color.White) ? 7 : 0;

            int file = Char.ToLower(rookFEN) - 'a';

            if (board[rank, file] is Rook && board[rank, file].Color == color)
            {
                board[rank, file].As<Rook>().ableToCastling = ableToCastle;
            }
        }

        private static void setQueenCastlingAvailability(Piece[,] board, Color color, bool ableToCastle)
        {
            int rank = (color == Color.White) ? 7 : 0;

            int file = 7;

            if (board[rank, file] is Rook && board[rank, file].Color == color)
            {
                board[rank, file].As<Rook>().ableToCastling = ableToCastle;
            }
        }

        private static void setKingCastlingAvailability(Piece[,] board, Color color, bool ableToCastle)
        {
            int rank = (color == Color.White) ? 7 : 0;

            int file = 4;

            if (board[rank, file] is King && board[rank, file].Color == color)
            {
                board[rank, file].As<King>().ableToCastling = ableToCastle;
            }
        }
    }
}
