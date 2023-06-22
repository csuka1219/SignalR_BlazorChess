using Microsoft.AspNetCore.SignalR;

namespace BlazorChess.Game
{
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
