using Avalonia.Controls;
using Module1;
using Module2;

namespace Lab1;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void Work1_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        int selectedNumber = await Work1.OnClick(this);
        if (selectedNumber == -1) return;
        SelectedNumberTextBlock.Text = $"Вибране число: {selectedNumber}";
    }

    private void Work2_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Work2.OnClick(this);
    }
}
