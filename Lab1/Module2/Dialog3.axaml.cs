using Avalonia.Controls;

namespace Module2;

public partial class Dialog3 : Window
{
    public Dialog3()
    {
        InitializeComponent();

        PreviousButton.Click += (_, __) =>
        {
            var dialog2 = new Dialog2();
            dialog2.Show();
            Close();
        };
        CancelButton.Click += (_, __) =>
        {
            var dialog2 = new Dialog2();
            dialog2.Show();
            Close();
        };
    }
}
