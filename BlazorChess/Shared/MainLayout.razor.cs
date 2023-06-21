using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlazorChess.Shared
{
    public partial class MainLayout
    {
        private bool isDrawerOpen = true;

        private MudTheme myCustomTheme = new MudTheme()
        {
            Palette = new PaletteLight()
            {
                Primary = Colors.Red.Default,
                Secondary = Colors.Green.Accent4,
                AppbarBackground = Colors.Red.Default,
            },
            PaletteDark = new PaletteDark()
            {
                Primary = "#00e676ff",
                Secondary = "#FFFFFF",
                Tertiary = "#00C853",
                Background = "#37474fff",
                Dark = "#263238ff",
                DrawerBackground = "#263238ff",
                AppbarBackground = "#263238ff",
                DrawerText = "#FFFFFF",
                DrawerIcon = "#FFFFFF",
                Surface = "#263238ff",
                TableStriped = "#37474fff",
                TableLines = "#424242ff",
            },
        };

        private void ToggleDrawer()
        {
            isDrawerOpen = !isDrawerOpen;
        }

        private string GetAvatarSize()
        {
            return isDrawerOpen ? "width:56px;height:56px;" : "width:40px;height:40px;";
        }
    }
}
