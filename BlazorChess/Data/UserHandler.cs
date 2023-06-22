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

        public static void setMatchInfoMoves(string gameName, (int,int) fromMove,(int,int) toMove)
        {
            matchInfos[gameName].pieceChanges.Add(new PieceChanges(fromMove, toMove));
            matchInfos[gameName].isWhiteTurn = !matchInfos[gameName].isWhiteTurn;
        }

        public static void setMatchInfoBoard(string gameName, Piece[,] board)
        {
            string boardString = ConvertBoardToString(board);
            matchInfos[gameName].boardInfo = boardString;
        }

        public static Piece[,] getMatchInfoBoard(string gameName)
        {
            return ConvertStringToBoard(matchInfos[gameName].boardInfo);
        }

        private static string ConvertBoardToString(Piece[,] board)
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

        private static Piece[,] ConvertStringToBoard(string boardString)
        {
            Piece[,] board = new Piece[8, 8];

            string[] fenParts = boardString.Split(' ');

            string[] ranks = boardString.Split('/');

            for (int rank = 7; rank >= 0; rank--)
            {
                for (int file = 7; file >= 0; file--)
                {
                    char c = ranks[rank][file];
                   
                    Piece piece = CreatePieceFromFEN(c,rank,file);
                    if (piece != null)
                    {
                        board[rank, file] = piece;
                    }
                }
            }

            Castling.setCastlingAvailability(board, fenParts[1]);

            return board;
        }

        
        private static Piece CreatePieceFromFEN(char fen,int row,int col)
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
    public List<PieceChanges> pieceChanges { get; set; }
    public string boardInfo { get; set; }
    public bool isWhiteTurn { get; set; }
        
    public MatchInfo() 
    {
        pieceChanges = new List<PieceChanges>();
        boardInfo = string.Empty;
        isWhiteTurn = true;
    }
}

public class PieceChanges
{
    public (int, int) fromMove { get; set; }
    public (int, int) toMove { get; set; }

    public PieceChanges((int,int)fromMove, (int, int) toMove)
    {
        this.fromMove = fromMove;
        this.toMove = toMove;
    }
}
