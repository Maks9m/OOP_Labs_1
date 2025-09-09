using Avalonia.Controls;

namespace Module2;

public partial class Dialog2 : Window
{
    public Dialog2()
    {
        InitializeComponent();

        NextButton.Click += (_, __) =>
        {
            var dialog3 = new Dialog3();
            dialog3.Show();
            Close();
        };
        CancelButton.Click += (_, __) => Close();
    }
}
