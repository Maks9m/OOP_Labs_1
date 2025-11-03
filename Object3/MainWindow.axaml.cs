using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextCopy;

namespace Object3;

public partial class MainWindow : Window
{
    private Communication.MessageServer? _messageServer;

    public MainWindow()
    {
        InitializeComponent();
        InitializeMessageServer();
        
        // Автоматично завантажуємо дані при запуску
        _ = Task.Run(async () =>
        {
            await Task.Delay(500);
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(async () =>
            {
                await LoadDataFromClipboard();
            });
        });
    }

    private void InitializeMessageServer()
    {
        _messageServer = new Communication.MessageServer("Object3_Pipe");
        _messageServer.MessageReceived += OnMessageReceived;
        _messageServer.Start();
    }

    private async void OnMessageReceived(string message)
    {
        await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(async () =>
        {
            if (message == "UPDATE")
            {
                await LoadDataFromClipboard();
            }
        });
    }

    private async void Update_Click(object? sender, RoutedEventArgs e)
    {
        await LoadDataFromClipboard();
    }

    private async Task LoadDataFromClipboard()
    {
        try
        {
            StatusTextBlock.Text = "Читання даних з Clipboard...";

            string? clipboardText = await ClipboardService.GetTextAsync();

            if (string.IsNullOrWhiteSpace(clipboardText))
            {
                StatusTextBlock.Text = "Помилка: Clipboard порожній";
                return;
            }

            var data = ParseClipboardData(clipboardText);

            if (data.Count == 0)
            {
                StatusTextBlock.Text = "Помилка: Не вдалося розпізнати дані";
                return;
            }

            ChartCanvas.SetData(data);
            StatusTextBlock.Text = $"Завантажено {data.Count} точок";

            // Повідомляємо Lab6 про завершення
            await Communication.MessageClient.SendMessageAsync("Lab6_Main", "OBJECT3_COMPLETED");
        }
        catch (Exception ex)
        {
            StatusTextBlock.Text = $"Помилка: {ex.Message}";
        }
    }

    private List<(int x, int y)> ParseClipboardData(string text)
    {
        var result = new List<(int x, int y)>();
        var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines.Skip(1)) // Пропускаємо заголовок
        {
            var parts = line.Split('\t');
            if (parts.Length >= 2)
            {
                if (int.TryParse(parts[0].Trim(), out int x) && 
                    int.TryParse(parts[1].Trim(), out int y))
                {
                    result.Add((x, y));
                }
            }
        }

        return result;
    }

    private void Exit_Click(object? sender, RoutedEventArgs e)
    {
        _messageServer?.Dispose();
        Close();
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        _messageServer?.Dispose();
        base.OnClosing(e);
    }
}
