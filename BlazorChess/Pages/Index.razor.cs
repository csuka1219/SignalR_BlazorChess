using BlazorChess.Component;
using BlazorChess.Data;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlazorChess.Pages
{
    public partial class Index
    {
        [Inject]
        private NavigationManager navigationManager { get; set; } = default!;

        [Inject]
        private IDialogService dialogService { get; set; } = default!;

        [Inject]
        private IUserHandler userHandler { get; set; } = default!;

        private string searchString = "";
        private List<string> games = new List<string>();

        protected override void OnInitialized()
        {
            refreshLobby();
            base.OnInitialized();
        }

        private void refreshLobby()
        {
            games = userHandler.getConnectedPlayerKeys();
        }

        private async void createGame()
        {
            await dialogService.ShowAsync<CreateGameDialog>("Create");
        }

        private bool lobbyFilter(string element)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }

        private void rowClickEvent(TableRowClickEventArgs<string> tableRowClickEventArgs)
        {
            navigationManager.NavigateTo("game/" + tableRowClickEventArgs.Item);
        }
    }
}
