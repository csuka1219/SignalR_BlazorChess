using Microsoft.AspNetCore.SignalR;

namespace BlazorChess.Game
{
    public static class UserHandler
    {
		public static Dictionary<string, int> connectedPlayers = new Dictionary<string, int>();

		public static List<string> getConnectedPlayerKeys() 
		{ 
			return connectedPlayers.Where(cp => cp.Value == 1).Select(cp => cp.Key).ToList(); 
		}
    }
    public class ChessHub : Hub
	{
		public async Task MovePiece(string groupName, int fromX, int fromY, int toX, int toY)
		{
			await Clients.GroupExcept(groupName, Context.ConnectionId).SendAsync("ReceiveMove", fromX, fromY, toX, toY);
		}

		public async Task JoinGame(string groupName)
		{
			await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task LeaveGame(string groupName)
		{
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
		}
	}
}
