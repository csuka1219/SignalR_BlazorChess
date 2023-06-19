using Microsoft.AspNetCore.Components;

namespace BlazorChess.Pages
{
    public partial class Index
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; }

        private string gameName = "";

        private void connection()
        {
            NavigationManager!.NavigateTo("game/" + gameName);
        }
    }
}
