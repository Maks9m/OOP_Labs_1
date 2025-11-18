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
    }

    private void InitializeMessageServer()
    {
        _messageServer = new Communication.MessageServer("Object2_Pipe");
        _messageServer.MessageReceived += OnMessageReceived;
        _messageServer.Start();
        // Повідомляємо менеджера про готовність до прийому параметрів
        _ = Communication.MessageClient.SendMessageAsync("Lab6_Main", "OBJECT2_READY");
    }

    private async void OnMessageReceived(string message)
    {
        await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(async () =>
        {
            if (message == "START")
            {
                // START without parameters — do nothing. Generation requires PARAMS from Lab6.
                StatusTextBlock.Text = "Отримано START без параметрів — очікування PARAMS";
            }
            else if (message.StartsWith("PARAMS:", StringComparison.OrdinalIgnoreCase))
            {
                // PARAMS:n;xMin;xMax;yMin;yMax
                var payload = message.Substring("PARAMS:".Length).Trim();
                if (TryParseParams(payload, out var p))
                {
                    await GenerateData(p.n, p.xMin, p.xMax, p.yMin, p.yMax);
                }
            }
            else if (string.Equals(message, "EXIT", StringComparison.OrdinalIgnoreCase))
            {
                _messageServer?.Dispose();
                Close();
            }
        });
    }

    // Manual generation via UI removed; Object2 is passive and waits for PARAMS from Lab6.

    private async Task GenerateData()
    {
        try
        {
            // Читаємо параметри з файлу
            string paramFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "parameters.txt");
            
            if (!File.Exists(paramFile))
            {
                // У типовому потоці керування параметри надходять через Lab6 (PARAMS)
                // Не вважаємо це помилкою: просто повідомляємо про очікування
                StatusTextBlock.Text = "Очікування параметрів від менеджера (Lab6)";
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

    private async Task GenerateData(int nPoints, int xMin, int xMax, int yMin, int yMax)
    {
        try
        {
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

            _generatedData = _generatedData.OrderBy(p => p.x).ToList();

            DisplayData();
            await CopyToClipboard();

            StatusTextBlock.Text = $"Згенеровано {nPoints} точок та скопійовано у Clipboard";

            await Communication.MessageClient.SendMessageAsync("Lab6_Main", "OBJECT2_COMPLETED");
        }
        catch (Exception ex)
        {
            StatusTextBlock.Text = $"Помилка: {ex.Message}";
        }
    }

    private static bool TryParseParams(string payload, out (int n, int xMin, int xMax, int yMin, int yMax) p)
    {
        p = default;
        var parts = payload.Split(';');
        if (parts.Length != 5) return false;

        bool okN = int.TryParse(parts[0], out int n);
        bool okXMin = int.TryParse(parts[1], out int xMin);
        bool okXMax = int.TryParse(parts[2], out int xMax);
        bool okYMin = int.TryParse(parts[3], out int yMin);
        bool okYMax = int.TryParse(parts[4], out int yMax);

        if (!(okN && okXMin && okXMax && okYMin && okYMax)) return false;
        if (n <= 0 || xMin > xMax || yMin > yMax) return false;

        p = (n, xMin, xMax, yMin, yMax);
        return true;
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
