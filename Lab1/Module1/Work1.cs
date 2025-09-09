using System.Threading.Tasks;
using Avalonia.Controls;

namespace Module1;

public static class Work1
{
public async static Task<int> OnClick(Window parent)
{
    var dialog = new Dialog1();
    var result = await dialog.ShowDialog<bool>(parent);
    if (result)
        {
            return dialog.SelectedValue;
        }
    return 0;
}
}
