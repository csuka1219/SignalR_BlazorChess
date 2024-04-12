﻿namespace BlazorChess.Pages
{
    // Game.UI.cs - Part of the partial Game class focusing on UI
    public partial class Game
    {
        private string getCellCss(int index)
        {
            string backgroundColor = index % 2 == 0 ? "#eeeed2" : "#769656";
            return "height:10vw; width:10vh; max-height: 64px; max-width: 64px; background-color:" + backgroundColor;
        }

        private string getPlayerTableView()
        {
            Dictionary<string, List<string>> connectedPlayers = userHandler.getConnectedPlayers();

            if (connectedPlayers.ContainsKey(gameName) && connectedPlayers[gameName].Count == 1)
            {
                return "";
            }
            if (connectedPlayers.ContainsKey(gameName) && connectedPlayers[gameName].Count == 2)
            {
                return ChessGameService.player.isWhitePlayer ? "" : "transform: rotate(180deg);";
            }
            return "transform: rotate(180deg);";
        }

        private string getPlayerPieceView()
        {
            Dictionary<string, List<string>> connectedPlayers = userHandler.getConnectedPlayers();

            if (connectedPlayers.ContainsKey(gameName) && connectedPlayers[gameName].Count == 1)
            {
                return "transform: rotate(180deg);";
            }
            else if (connectedPlayers.ContainsKey(gameName) && connectedPlayers[gameName].Count == 2)
            {
                return ChessGameService.player.isWhitePlayer ? "" : "transform: rotate(180deg);";
            }
            return "";
        }
    }

}
