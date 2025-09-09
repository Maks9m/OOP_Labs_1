using Avalonia.Controls;

namespace Module2;

public static class Work2
{
    public static int OnClick(Window parent)
    {
        var dialog = new Dialog2();
        dialog.Show();
        return 1; 
    }
}
