﻿using BlazorChess.Data;
using BlazorChess.Game;
using BlazorChess.Pieces;
using MudBlazor;

namespace BlazorChess.Services
{

    public class ChessGameService
    {
        public Chessboard chessBoard = new Chessboard();
        public Player player = new Player();
        public IEnumerable<Piece> piecesOnBoard = new List<Piece>();
        public MudDropContainer<Piece> _container = default!;
        public List<PieceChange> pieceChanges = new List<PieceChange>();

        public bool[,] availableMoves = new bool[8, 8];
        public bool whiteTurn = true;
        private bool lastTurn = true;
        public bool ableToMove = false;
        private bool dragEnded = true;
        private string lastposition = "";
        public ChessGameService()
        {
            piecesOnBoard = chessBoard.board.Cast<Piece>().ToList();
        }

        public bool canMovePiece(Piece selectedPiece, string s)
        {
            // Check if it's not the player's turn or the piece is not within the valid range for the current turn
            if (!ableToMove || !player.IsMyTurn || (!whiteTurn && selectedPiece.PieceValue < PieceConstants.blackPawnValue) || (whiteTurn && selectedPiece.PieceValue > PieceConstants.whiteKingValue))
            {
                return false;
            }

            // Check if the turn and piece position match the previous turn and position, and the drag has not ended
            if (whiteTurn == lastTurn && selectedPiece.Position != lastposition && !dragEnded)
            {
                // Reset available moves and end the drag
                availableMoves = new bool[8, 8];
                dragEnded = true;
            }

            // Extract the row and column from the string representation of the dropzone identifier
            int row = s[0] - '0';
            int col = s[1] - '0';

            // Create a stale array to track stale positions
            bool[,] staleArray = new bool[8, 8];

            // Iterate over each piece on the chessboard
            foreach (Piece piece in chessBoard.board)
            {
                // Check if the piece's color matches the opposite of the current turn
                if (piece.Color == Pieces.Color.White != whiteTurn)
                {
                    // Update the stale array with stale positions for the opposite color
                    staleArray = piece.checkForStale(chessBoard.board, staleArray);
                }
            }

            // Check if the drag has ended
            if (dragEnded)
            {
                // Calculate possible moves for the selected piece based on the chessboard and stale positions
                if (selectedPiece is King)
                {
                    availableMoves = selectedPiece.calculatePossibleMoves(chessBoard.board, availableMoves, staleArray);
                }
                else
                {
                    availableMoves = selectedPiece.calculatePossibleMoves(chessBoard.board, availableMoves);
                }

                // Reset the drag flag, store the current position and turn, and remove invalid moves
                dragEnded = false;
                lastposition = selectedPiece.Position!;
                lastTurn = whiteTurn;
                removeInvalidMoves(selectedPiece);
            }

            // Check if the specified row and column are valid moves
            return availableMoves[row, col];
        }

        public async void movePiece(MudItemDropInfo<Piece> piece, GameHubService gameHubService, string gameName)
        {
            // Extract the new row and column from the dropzone identifier
            int newRow = piece.DropzoneIdentifier[0] - '0';
            int newCol = piece.DropzoneIdentifier[1] - '0';

            // Check if the dropped piece captures another piece
            bool isHitPiece = piecesOnBoard.Any(p => p.PieceValue != piece.Item!.PieceValue && p.Position == piece.DropzoneIdentifier);
            int hitpieceValue = 0;
            if (isHitPiece)
            {
                // Clear the position of the captured piece
                Piece hitPiece = piecesOnBoard.First(p => p.Position == piece.DropzoneIdentifier);
                hitPiece.Position = null;
                hitpieceValue = hitPiece.PieceValue;
            }

            // Get the current position of the moved piece
            (int oldRow, int oldCol) = piece.Item!.getPositionTuple();

            //Store move
            pieceChanges.Add(new PieceChange((oldRow, oldCol), (newRow, newCol), piece.Item.PieceValue, hitpieceValue));

            // Set the piece on the chessboard to the new position
            chessBoard.setPiece(newRow, newCol, piece.Item, piecesOnBoard);

            // Set the dragEnded flag to true
            dragEnded = true;

            // Reset the available moves array
            availableMoves = new bool[8, 8];


            // Toggle the turn to the opposite player
            whiteTurn = !whiteTurn;

            // Check if it's the player's turn to handle the move
            if (player.IsMyTurn)
            {
                // Handle the move asynchronously
                await gameHubService.sendMoveAsync(gameName, oldRow, oldCol, newRow, newCol);
            }

            // Toggle the turn for the player
            player.IsMyTurn = !player.IsMyTurn;
        }
        
        public bool checkForCheckmate()
        {
            // Check if the game is in checkmate state
            bool checkMate = CheckMate.isCheckMate(chessBoard.board, !whiteTurn);
            ableToMove = checkMate == false;
            return checkMate;
        }

        public void removeInvalidMoves(Piece piece)
        {
            (int row, int col) = piece.getPositionTuple();
            // Iterate over each cell on the chessboard
            for (int newRow = 0; newRow < 8; newRow++)
            {
                for (int newCol = 0; newCol < 8; newCol++)
                {
                    // Check if the piece can move into the current cell
                    if (availableMoves[newRow, newCol])
                    {
                        // Store the piece that might be captured in the destination cell
                        Piece lastHitPiece = chessBoard.board[newRow, newCol];
                        // Move the piece to the new cell and update its position
                        chessBoard.board[newRow, newCol] = piece;
                        chessBoard.board[newRow, newCol].setPosition($"{row}{col}", true);
                        chessBoard.board[row, col].setPosition($"{newRow}{newCol}", true);

                        // Replace the original cell with an empty piece
                        chessBoard.board[row, col] = new EmptyPiece();

                        // Check if the move leads to a stale for the player
                        if (whiteTurn && Check.checkChecker(chessBoard.board, whiteTurn))
                        {
                            // Mark the move as invalid
                            availableMoves[newRow, newCol] = false;
                        }
                        if (!whiteTurn && Check.checkChecker(chessBoard.board, whiteTurn))
                        {
                            // Mark the move as invalid
                            availableMoves[newRow, newCol] = false;
                        }

                        // Restore the original positions and pieces on the chessboard
                        chessBoard.board[newRow, newCol] = lastHitPiece;
                        chessBoard.board[newRow, newCol].setPosition($"{newRow}{newCol}", true);
                        chessBoard.board[row, col] = piece;
                        chessBoard.board[row, col].setPosition($"{row}{col}", true);
                    }
                }
            }
        }
    }

}
