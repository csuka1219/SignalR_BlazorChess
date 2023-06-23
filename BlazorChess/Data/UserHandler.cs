using BlazorChess.Data;
using BlazorChess.Pieces;
using MudBlazor;
using MudBlazor.Extensions;
using System.Text;

namespace BlazorChess.Data
{
    public static class UserHandler
    {
        public static Dictionary<string, List<string>> connectedPlayers = new Dictionary<string, List<string>>();
        public static Dictionary<string, MatchInfo> matchInfos = new Dictionary<string, MatchInfo>();

        public static List<string> getConnectedPlayerKeys()
        {
            return connectedPlayers.Where(cp => cp.Value.Count == 1).Select(cp => cp.Key).ToList();
        }

        public static void setMatchInfoMoves(string gameName, List<PieceChange> pieceChanges, bool isWhiteTurn)
        {
            matchInfos[gameName].pieceChanges = new List<PieceChange>(pieceChanges);
            matchInfos[gameName].isWhiteTurn = isWhiteTurn;
        }

        public static void setMatchInfoBoard(string gameName, Piece[,] board)
        {
            string boardString = convertBoardToString(board);
            matchInfos[gameName].boardInfo = boardString;
        }

        public static Piece[,] getMatchInfoBoard(string gameName)
        {
            return convertStringToBoard(matchInfos[gameName].boardInfo);
        }

        public static List<PieceChange> getMatchInfoMoves(string gameName)
        {
            return matchInfos[gameName].pieceChanges;
        }

        private static string convertBoardToString(Piece[,] board)
        {
            StringBuilder sb = new StringBuilder();

            for (int rank = 0; rank < 8; rank++)
            {
                int emptySquareCount = 0;

                for (int file = 0; file < 8; file++)
                {
                    Piece piece = board[rank, file];

                    if (piece == null)
                    {
                        emptySquareCount++;
                    }
                    else
                    {
                        if (emptySquareCount > 0)
                        {
                            sb.Append(emptySquareCount);
                            emptySquareCount = 0;
                        }

                        sb.Append(piece.getFENRepresentation());
                    }
                }

                if (emptySquareCount > 0)
                {
                    sb.Append(emptySquareCount);
                }

                if (rank < 8)
                {
                    sb.Append('/');
                }
            }

            // Add castling availability
            sb.Append(' ');
            sb.Append(Castling.getCastlingAvailability(board));

            return sb.ToString();
        }

        private static Piece[,] convertStringToBoard(string boardString)
        {
            Piece[,] board = new Piece[8, 8];

            string[] fenParts = boardString.Split(' ');

            string[] ranks = boardString.Split('/');

            for (int rank = 7; rank >= 0; rank--)
            {
                for (int file = 7; file >= 0; file--)
                {
                    char c = ranks[rank][file];
                   
                    Piece piece = createPieceFromFEN(c,rank,file);
                    if (piece != null)
                    {
                        board[rank, file] = piece;
                    }
                }
            }

            Castling.setCastlingAvailability(board, fenParts[1]);

            return board;
        }
   
        private static Piece createPieceFromFEN(char fen,int row,int col)
        {
            Pieces.Color color = char.IsUpper(fen) ? Pieces.Color.White : Pieces.Color.Black;
            bool isWhite = color == Pieces.Color.White;

            switch (char.ToLower(fen))
            {
                case 'r':
                    return new Rook(color, isWhite ? PieceConstants.whiteRookValue: PieceConstants.blackRookValue, Icons.Custom.Uncategorized.ChessRook, $"{row}{col}", ableToCastling: false);
                case 'n':
                    return new Knight(color, isWhite ? PieceConstants.whiteKnightValue : PieceConstants.blackKnightValue, Icons.Custom.Uncategorized.ChessKnight, $"{row}{col}");
                case 'b':
                    return new Bishop(color, isWhite ? PieceConstants.whiteBishopValue : PieceConstants.blackBishopValue, Icons.Custom.Uncategorized.ChessBishop, $"{row}{col}");
                case 'q':
                    return new Queen(color, isWhite ? PieceConstants.whiteQueenValue : PieceConstants.blackQueenValue, Icons.Custom.Uncategorized.ChessQueen, $"{row}{col}");
                case 'k':
                    return new King(color, isWhite ? PieceConstants.whiteKingValue : PieceConstants.blackKingValue, Icons.Custom.Uncategorized.ChessKing, $"{row}{col}", ableToCastling: false);
                case 'p':
                    return new Pawn(color, isWhite ? PieceConstants.whitePawnValue : PieceConstants.blackPawnValue, Icons.Custom.Uncategorized.ChessPawn, $"{row}{col}");
                default:
                    return new EmptyPiece();
            }
        }
    }
}

public class MatchInfo
{
    public List<PieceChange> pieceChanges { get; set; }
    public string boardInfo { get; set; }
    public bool isWhiteTurn { get; set; }
        
    public MatchInfo() 
    {
        pieceChanges = new List<PieceChange>();
        boardInfo = string.Empty;
        isWhiteTurn = true;
    }
}
