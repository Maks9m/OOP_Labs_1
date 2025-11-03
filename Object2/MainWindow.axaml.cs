using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextCopy;

namespace Object2;

public partial class MainWindow : Window
{
    private Communication.MessageServer? _messageServer;
    private List<(int x, int y)> _generatedData = new();

    public MainWindow()
    {
        InitializeComponent();
        InitializeMessageServer();
        
        // Автоматично генеруємо дані при запуску
        _ = Task.Run(async () =>
        {
            await Task.Delay(500); // Невелика затримка для ініціалізації
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(async () =>
            {
                await GenerateData();
            });
        });
    }

    private void InitializeMessageServer()
    {
        _messageServer = new Communication.MessageServer("Object2_Pipe");
        _messageServer.MessageReceived += OnMessageReceived;
        _messageServer.Start();
    }

    private async void OnMessageReceived(string message)
    {
        await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(async () =>
        {
            if (message == "START")
            {
                await GenerateData();
            }
        });
    }

    private async void Generate_Click(object? sender, RoutedEventArgs e)
    {
        await GenerateData();
    }

    private async Task GenerateData()
    {
        try
        {
            // Читаємо параметри з файлу
            string paramFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "parameters.txt");
            
            if (!File.Exists(paramFile))
            {
                StatusTextBlock.Text = "Помилка: файл параметрів не знайдено";
                return;
            }

            var lines = await File.ReadAllLinesAsync(paramFile);
            if (lines.Length < 5)
            {
                StatusTextBlock.Text = "Помилка: невірний формат файлу параметрів";
                return;
            }

            int nPoints = int.Parse(lines[0]);
            int xMin = int.Parse(lines[1]);
            int xMax = int.Parse(lines[2]);
            int yMin = int.Parse(lines[3]);
            int yMax = int.Parse(lines[4]);

            StatusTextBlock.Text = $"Генерація {nPoints} точок...";

            // Генеруємо випадкові дані
            var random = new Random();
            _generatedData.Clear();

            for (int i = 0; i < nPoints; i++)
            {
                int x = random.Next(xMin, xMax + 1);
                int y = random.Next(yMin, yMax + 1);
                _generatedData.Add((x, y));
            }

            // Сортуємо по X для правильного відображення графіка
            _generatedData = _generatedData.OrderBy(p => p.x).ToList();

            // Відображаємо дані
            DisplayData();

            // Записуємо у Clipboard
            await CopyToClipboard();

            StatusTextBlock.Text = $"Згенеровано {nPoints} точок та скопійовано у Clipboard";

            // Повідомляємо Lab6 про завершення
            await Communication.MessageClient.SendMessageAsync("Lab6_Main", "OBJECT2_COMPLETED");
        }
        catch (Exception ex)
        {
            StatusTextBlock.Text = $"Помилка: {ex.Message}";
        }
    }

    private void DisplayData()
    {
        var sb = new StringBuilder();
        sb.AppendLine("x\ty");
        
        foreach (var point in _generatedData)
        {
            sb.AppendLine($"{point.x}\t{point.y}");
        }

        DataTextBlock.Text = sb.ToString();
    }

    private async Task CopyToClipboard()
    {
        var sb = new StringBuilder();
        sb.AppendLine("x\ty");
        
        foreach (var point in _generatedData)
        {
            sb.AppendLine($"{point.x}\t{point.y}");
        }

        await ClipboardService.SetTextAsync(sb.ToString());
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
