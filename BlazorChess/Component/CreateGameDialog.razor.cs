using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlazorChess.Component
{
    public partial class CreateGameDialog
    {
        [Inject]
        private NavigationManager navigationManager { get; set; }

        [CascadingParameter]
        private MudDialogInstance? mudDialog { get; set; }

        private string gameName = "";

        private void cancel()
        {
            mudDialog!.Cancel();
        }

        private void create()
        {
            navigationManager!.NavigateTo("game/" + gameName);
            mudDialog!.Close();
        }
    }
}
