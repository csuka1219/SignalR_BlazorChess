//using MudBlazor;
//using static BlazorChess.Pages.Index;

//namespace BlazorChess.Data
//{
//    public static class PlayTable
//    {
//        public static int[,] board = new int[8, 8]
//        {
//                { 02, 03, 04, 05, 06, 09, 08, 07},
//                { 01, 01, 01, 01, 01, 01, 01, 01},
//                { 00, 00, 00, 00, 00, 00, 00, 00},
//                { 00, 00, 00, 00, 00, 00, 00 ,00},
//                { 00, 00, 00, 00, 00, 00, 00 ,00},
//                { 00, 00, 00, 00, 00, 00, 00 ,00},
//                { 11, 11, 11, 11, 11, 11, 11, 11},
//                { 12, 13, 14, 15, 16, 19, 18, 17},
//        };
//        public static bool[,] availableMovesTable = new bool[8, 8];

//        public static bool whiteTurn = true;
//        public static bool lastTurn = true;
//        public static string lastposition = "";

//        public static void CheckDraggedItemMoves(Piece draggedPiece, int i, int j, bool later)
//        {
//            if (whiteTurn)
//            {
//                switch (draggedPiece.Id)
//                {
//                    case PiecesId.whitePawn:
//                        WhitePawn.GetPossibleMoves(i, j, whiteTurn, false); return;
//                    case PiecesId.whiteKnight1:
//                        WhiteKnight.GetPossibleMoves(i, j, whiteTurn, false); return;
//                    case PiecesId.whiteKnight2:
//                        WhiteKnight2.GetPossibleMoves(i, j, whiteTurn, false); return;
//                    case PiecesId.whiteBishop1:
//                        WhiteBishop.GetPossibleMoves(i, j, whiteTurn, false); return;
//                    case PiecesId.whiteBishop2:
//                        WhiteBishop2.GetPossibleMoves(i, j, whiteTurn, false); return;
//                    case PiecesId.whiteQueen:
//                        WhiteQueen.GetPossibleMoves(i, j, whiteTurn, false); return;
//                    case PiecesId.whiteKing:
//                        WhiteKing.GetPossibleMoves(i, j, whiteTurn, false); return;
//                    case PiecesId.whiteRook1:
//                        WhiteRook1.GetPossibleMoves(i, j, whiteTurn, false); return;
//                    case PiecesId.whiteRook2:
//                        WhiteRook2.GetPossibleMoves(i, j, whiteTurn, false); return;
//                }
//            }
//            else
//            {
//                switch (draggedPiece.Id)
//                {
//                    case PiecesId.blackPawn:
//                        BlackPawn.GetPossibleMoves(i, j, whiteTurn, false); return;
//                    case PiecesId.blackKnight1:
//                        BlackKnight.GetPossibleMoves(i, j, whiteTurn, false); return;
//                    case PiecesId.blackKnight2:
//                        BlackKnight2.GetPossibleMoves(i, j, whiteTurn, false); return;
//                    case PiecesId.blackBishop1:
//                        BlackBishop.GetPossibleMoves(i, j, whiteTurn, false); return;
//                    case PiecesId.blackBishop2:
//                        BlackBishop2.GetPossibleMoves(i, j, whiteTurn, false); return;
//                    case PiecesId.blackQueen:
//                        BlackQueen.GetPossibleMoves(i, j, whiteTurn, false); return;
//                    case PiecesId.blackKing:
//                        BlackKing.GetPossibleMoves(i, j, whiteTurn, false); return;
//                    case PiecesId.blackRook1:
//                        BlackRook1.GetPossibleMoves(i, j, whiteTurn, false); return;
//                    case PiecesId.blackRook2:
//                        BlackRook2.GetPossibleMoves(i, j, whiteTurn, false); return;
//                }
//            }
//        }

//        public static List<Piece> _pieces = new()
//        {
//            new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessRook, Color = Color.Primary, Position = "00",Id=PiecesId.blackRook1},
//            new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessKnight, Color = Color.Primary, Position = "01" ,Id=PiecesId.blackKnight1},
//            new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessBishop, Color = Color.Primary, Position = "02" ,Id=PiecesId.blackBishop1},
//            new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessQueen, Color = Color.Primary, Position = "03" ,Id=PiecesId.blackQueen},
//            new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessKing, Color = Color.Primary, Position = "04" ,Id=PiecesId.blackKing},
//            new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessBishop, Color = Color.Primary, Position = "05" ,Id=PiecesId.blackBishop2},
//            new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessKnight, Color = Color.Primary, Position = "06" ,Id=PiecesId.blackKnight2},
//            new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessRook, Color = Color.Primary, Position = "07" ,Id=PiecesId.blackRook2},
//            new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessPawn, Color = Color.Primary, Position = "10" ,Id=PiecesId.blackPawn},
//            new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessPawn, Color = Color.Primary, Position = "11" ,Id=PiecesId.blackPawn},
//            new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessPawn, Color = Color.Primary, Position = "12" ,Id=PiecesId.blackPawn},
//            new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessPawn, Color = Color.Primary, Position = "13" ,Id=PiecesId.blackPawn},
//            new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessPawn, Color = Color.Primary, Position = "14" ,Id=PiecesId.blackPawn},
//            new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessPawn, Color = Color.Primary, Position = "15" ,Id=PiecesId.blackPawn},
//            new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessPawn, Color = Color.Primary, Position = "16" ,Id=PiecesId.blackPawn},
//            new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessPawn, Color = Color.Primary, Position = "17" ,Id=PiecesId.blackPawn},

//            new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessPawn, Color = Color.Secondary, Position = "60" ,Id=PiecesId.whitePawn},
//            new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessPawn, Color = Color.Secondary, Position = "61" ,Id=PiecesId.whitePawn},
//            new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessPawn, Color = Color.Secondary, Position = "62" ,Id=PiecesId.whitePawn},
//            new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessPawn, Color = Color.Secondary, Position = "63" ,Id=PiecesId.whitePawn},
//            new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessPawn, Color = Color.Secondary, Position = "64" ,Id=PiecesId.whitePawn},
//            new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessPawn, Color = Color.Secondary, Position = "65" ,Id=PiecesId.whitePawn},
//            new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessPawn, Color = Color.Secondary, Position = "66" ,Id=PiecesId.whitePawn},
//            new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessPawn, Color = Color.Secondary, Position = "67" ,Id=PiecesId.whitePawn},
//            new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessRook, Color = Color.Secondary, Position = "70" ,Id=PiecesId.whiteRook1},
//            new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessKnight, Color = Color.Secondary, Position = "71" ,Id=PiecesId.whiteKnight1},
//            new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessBishop, Color = Color.Secondary, Position = "72" ,Id=PiecesId.whiteBishop1},
//            new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessQueen, Color = Color.Secondary, Position = "73" ,Id=PiecesId.whiteQueen},
//            new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessKing, Color = Color.Secondary, Position = "74" ,Id=PiecesId.whiteKing},
//            new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessBishop, Color = Color.Secondary, Position = "75" ,Id=PiecesId.whiteBishop2},
//            new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessKnight, Color = Color.Secondary, Position = "76" ,Id=PiecesId.whiteKnight2},
//            new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessRook, Color = Color.Secondary, Position = "77" ,Id=PiecesId.whiteRook2},
//        };

//        public static void InitGame()
//        {
//            board = new int[8, 8]
//            {
//                { 02, 03, 04, 05, 06, 09, 08, 07},
//                { 01, 01, 01, 01, 01, 01, 01, 01},
//                { 00, 00, 00, 00, 00, 00, 00, 00},
//                { 00, 00, 00, 00, 00, 00, 00 ,00},
//                { 00, 00, 00, 00, 00, 00, 00 ,00},
//                { 00, 00, 00, 00, 00, 00, 00 ,00},
//                { 11, 11, 11, 11, 11, 11, 11, 11},
//                { 12, 13, 14, 15, 16, 19, 18, 17},
//            };
//            whiteTurn = true;
//            lastTurn = true;
//            lastposition = "";
//            _pieces = new()
//            {
//                new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessRook, Color = Color.Primary, Position = "00",Id=PiecesId.blackRook1},
//                new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessKnight, Color = Color.Primary, Position = "01" ,Id=PiecesId.blackKnight1},
//                new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessBishop, Color = Color.Primary, Position = "02" ,Id=PiecesId.blackBishop1},
//                new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessQueen, Color = Color.Primary, Position = "03" ,Id=PiecesId.blackQueen},
//                new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessKing, Color = Color.Primary, Position = "04" ,Id=PiecesId.blackKing},
//                new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessBishop, Color = Color.Primary, Position = "05" ,Id=PiecesId.blackBishop2},
//                new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessKnight, Color = Color.Primary, Position = "06" ,Id=PiecesId.blackKnight2},
//                new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessRook, Color = Color.Primary, Position = "07" ,Id=PiecesId.blackRook2},
//                new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessPawn, Color = Color.Primary, Position = "10" ,Id=PiecesId.blackPawn},
//                new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessPawn, Color = Color.Primary, Position = "11" ,Id=PiecesId.blackPawn},
//                new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessPawn, Color = Color.Primary, Position = "12" ,Id=PiecesId.blackPawn},
//                new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessPawn, Color = Color.Primary, Position = "13" ,Id=PiecesId.blackPawn},
//                new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessPawn, Color = Color.Primary, Position = "14" ,Id=PiecesId.blackPawn},
//                new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessPawn, Color = Color.Primary, Position = "15" ,Id=PiecesId.blackPawn},
//                new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessPawn, Color = Color.Primary, Position = "16" ,Id=PiecesId.blackPawn},
//                new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessPawn, Color = Color.Primary, Position = "17" ,Id=PiecesId.blackPawn},

//                new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessPawn, Color = Color.Secondary, Position = "60" ,Id=PiecesId.whitePawn},
//                new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessPawn, Color = Color.Secondary, Position = "61" ,Id=PiecesId.whitePawn},
//                new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessPawn, Color = Color.Secondary, Position = "62" ,Id=PiecesId.whitePawn},
//                new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessPawn, Color = Color.Secondary, Position = "63" ,Id=PiecesId.whitePawn},
//                new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessPawn, Color = Color.Secondary, Position = "64" ,Id=PiecesId.whitePawn},
//                new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessPawn, Color = Color.Secondary, Position = "65" ,Id=PiecesId.whitePawn},
//                new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessPawn, Color = Color.Secondary, Position = "66" ,Id=PiecesId.whitePawn},
//                new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessPawn, Color = Color.Secondary, Position = "67" ,Id=PiecesId.whitePawn},
//                new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessRook, Color = Color.Secondary, Position = "70" ,Id=PiecesId.whiteRook1},
//                new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessKnight, Color = Color.Secondary, Position = "71" ,Id=PiecesId.whiteKnight1},
//                new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessBishop, Color = Color.Secondary, Position = "72" ,Id=PiecesId.whiteBishop1},
//                new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessQueen, Color = Color.Secondary, Position = "73" ,Id=PiecesId.whiteQueen},
//                new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessKing, Color = Color.Secondary, Position = "74" ,Id=PiecesId.whiteKing},
//                new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessBishop, Color = Color.Secondary, Position = "75" ,Id=PiecesId.whiteBishop2},
//                new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessKnight, Color = Color.Secondary, Position = "76" ,Id=PiecesId.whiteKnight2},
//                new Piece(){ Icon = @Icons.Custom.Uncategorized.ChessRook, Color = Color.Secondary, Position = "77" ,Id=PiecesId.whiteRook2},
//            };
//            Castling.ClearCastling();
//        }
//        public class Piece
//        {
//            public PiecesId Id { get; set; }
//            public string? Icon { get; init; }
//            public Color Color { get; set; }
//            public string? Position { get; set; }
//        }

//    }
//}
