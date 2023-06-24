using BlazorChess.Data;
using Microsoft.AspNetCore.SignalR;
using MudBlazor;

namespace BlazorChess.Game
{
    public class ChessHub : Hub
	{
		public async Task MovePiece(string groupName, int fromRow, int fromCol, int toRow, int toCol)
		{
			await Clients.GroupExcept(groupName, Context.ConnectionId).SendAsync("ReceiveMove", fromRow, fromCol, toRow, toCol);
		}

		public async Task JoinGame(string groupName)
		{
			await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
			if (UserHandler.connectedPlayers[groupName].Count == 2)
			{
				await Clients.GroupExcept(groupName, Context.ConnectionId).SendAsync("Joined");
			}
        }

        public async Task LeaveGame(string groupName)
		{
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
		}
	}
}
