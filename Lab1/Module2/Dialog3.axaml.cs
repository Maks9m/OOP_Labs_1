using Avalonia.Controls;

namespace Module2;

public partial class Dialog3 : Window
{
    public Dialog3()
    {
        InitializeComponent();

        OkButton.Click += (_, __) => Close();
        PreviousButton.Click += (_, __) => ShowDialog2();
        CancelButton.Click += (_, __) => ShowDialog2();
    }

    private void ShowDialog2()
    {
        var dialog2 = new Dialog2();
        dialog2.Show();
        Close();
    }
}
