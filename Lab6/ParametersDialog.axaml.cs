using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Lab6;

public partial class ParametersDialog : Window
{
    public class Parameters
    {
        public int NPoints { get; set; }
        public int XMin { get; set; }
        public int XMax { get; set; }
        public int YMin { get; set; }
        public int YMax { get; set; }
    }

    public Parameters? Result { get; private set; }

    public ParametersDialog()
    {
        InitializeComponent();
    }

    private void OkButton_Click(object? sender, RoutedEventArgs e)
    {
        // Валідація
        if (XMaxInput.Value <= XMinInput.Value)
        {
            ShowError("xMax повинен бути більшим за xMin");
            return;
        }

        if (YMaxInput.Value <= YMinInput.Value)
        {
            ShowError("yMax повинен бути більшим за yMin");
            return;
        }

        Result = new Parameters
        {
            NPoints = (int)NPointsInput.Value!,
            XMin = (int)XMinInput.Value!,
            XMax = (int)XMaxInput.Value!,
            YMin = (int)YMinInput.Value!,
            YMax = (int)YMaxInput.Value!
        };

        Close(Result);
    }

    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        Close(null);
    }

    private async void ShowError(string message)
    {
        var dialog = new Window
        {
            Title = "Помилка",
            Width = 300,
            Height = 150,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            Content = new StackPanel
            {
                Margin = new Avalonia.Thickness(20),
                Spacing = 15,
                Children =
                {
                    new TextBlock { Text = message, TextWrapping = Avalonia.Media.TextWrapping.Wrap },
                    new Button 
                    { 
                        Content = "OK", 
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                        Width = 100
                    }
                }
            }
        };

        if (dialog.Content is StackPanel panel && panel.Children[1] is Button btn)
        {
            btn.Click += (s, e) => dialog.Close();
        }

        await dialog.ShowDialog(this);
    }
}
