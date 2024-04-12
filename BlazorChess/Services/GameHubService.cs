using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace BlazorChess.Services
{
    public class GameHubService
    {
        private HubConnection _hubConnection;
        public bool IsConnected => _hubConnection.State == HubConnectionState.Connected;

        // Define events that components can subscribe to
        public event Action<int, int, int, int> OnMoveReceived = default!;
        public event Action OnJoined = default!;

        public GameHubService(NavigationManager navigationManager)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(navigationManager.ToAbsoluteUri("/chessHub"))
                .WithAutomaticReconnect()
                .Build();

            // Register event handlers for receiving messages
            _hubConnection.On<int, int, int, int>("ReceiveMove", (fromX, fromY, toX, toY) =>
            {
                OnMoveReceived?.Invoke(fromX, fromY, toX, toY);
            });

            _hubConnection.On("Joined", () =>
            {
                OnJoined?.Invoke();
            });
        }

        public async Task startAsync()
        {
            await _hubConnection.StartAsync();
        }

        public async Task stopAsync()
        {
            await _hubConnection.StopAsync();
        }

        public async Task sendMoveAsync(string gameName, int fromX, int fromY, int toX, int toY)
        {
            if (IsConnected)
            {
                await _hubConnection.SendAsync("sendMove", gameName, fromX, fromY, toX, toY);
            }
        }

        public async Task joinGameAsync(string gameName)
        {
            if (IsConnected)
            {
                await _hubConnection.SendAsync("joinGame", gameName);
            }
        }
        public async Task startGameAsync(string gameName)
        {
            if (IsConnected)
            {
                await _hubConnection.SendAsync("startGame", gameName);
            }
        }
        public async Task leaveGameAsync(string gameName)
        {
            if (IsConnected)
            {
                await _hubConnection.SendAsync("leaveGame", gameName);
            }
        }


    }
}
