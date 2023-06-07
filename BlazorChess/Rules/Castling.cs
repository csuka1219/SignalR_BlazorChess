using BlazorChess.Game;
using BlazorChess.Pieces;

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
    }
}
