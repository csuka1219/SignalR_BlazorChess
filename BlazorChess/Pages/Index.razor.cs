using BlazorChess.Game;
using Microsoft.AspNetCore.Components;
using static MudBlazor.CategoryTypes;

namespace BlazorChess.Pages
{
    public partial class Index
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; }

        private string gameName = "";
        private string searchString = "";
        private List<string> games = new List<string>();

        protected override void OnInitialized()
        {
            games = UserHandler.connectedPlayers.Where(cp => cp.Value == 1).Select(cp => cp.Key).ToList();
        }

        private void connection(string gameName)
        {
            NavigationManager!.NavigateTo("game/" + gameName);
        }

        private bool lobbyFilter(string element)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }
    }
}
