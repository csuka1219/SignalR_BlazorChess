using BlazorChess.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using MudBlazor;

namespace BlazorChess.Game
{
    public class ChessHub : Hub
	{
        public async Task movePiece(string groupName, int fromRow, int fromCol, int toRow, int toCol)
		{
			await Clients.GroupExcept(groupName, Context.ConnectionId).SendAsync("ReceiveMove", fromRow, fromCol, toRow, toCol);
		}

		public async Task joinGame(string groupName)
		{
			await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

		public async Task startGame(string groupName)
		{
            await Clients.GroupExcept(groupName, Context.ConnectionId).SendAsync("Joined");
        }

        public async Task leaveGame(string groupName)
		{
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
		}
	}
}
