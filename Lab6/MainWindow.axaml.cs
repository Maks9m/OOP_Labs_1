using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Lab6.Communication;

namespace Lab6;

public partial class MainWindow : Window
{
    private MessageServer? _messageServer;
    private Process? _object2Process;
    private Process? _object3Process;
    private string _logText = "";

    public MainWindow()
    {
        InitializeComponent();
        InitializeMessageServer();
    }

    private void InitializeMessageServer()
    {
        _messageServer = new MessageServer("Lab6_Main");
        _messageServer.MessageReceived += OnMessageReceived;
        _messageServer.Start();
        AddLog("Сервер повідомлень запущено");
    }

    private async void OnMessageReceived(string message)
    {
        await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
        {
            AddLog($"Отримано повідомлення: {message}");
            
            if (message == "OBJECT2_READY")
            {
                StatusTextBlock.Text = "Object2 готовий до роботи";
            }
            else if (message == "OBJECT2_COMPLETED")
            {
                StatusTextBlock.Text = "Object2 завершив роботу, запускаємо Object3";
                _ = StartObject3();
            }
            else if (message == "OBJECT3_READY")
            {
                StatusTextBlock.Text = "Object3 готовий до роботи";
            }
            else if (message == "OBJECT3_COMPLETED")
            {
                StatusTextBlock.Text = "Процес завершено успішно!";
                AddLog("Всі операції виконано успішно!");
            }
        });
    }

    private async void StartProcess_Click(object? sender, RoutedEventArgs e)
    {
        var dialog = new ParametersDialog();
        var result = await dialog.ShowDialog<ParametersDialog.Parameters?>(this);
        
        if (result == null)
        {
            AddLog("Операцію скасовано користувачем");
            return;
        }

        AddLog($"Параметри: nPoints={result.NPoints}, xMin={result.XMin}, xMax={result.XMax}, yMin={result.YMin}, yMax={result.YMax}");
        
        // Зберігаємо параметри у файл для Object2
        await SaveParametersToFile(result);
        
        // Запускаємо Object2
        await StartObject2();
    }

    private async Task SaveParametersToFile(ParametersDialog.Parameters parameters)
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "parameters.txt");
        string content = $"{parameters.NPoints}\n{parameters.XMin}\n{parameters.XMax}\n{parameters.YMin}\n{parameters.YMax}";
        await File.WriteAllTextAsync(path, content);
        AddLog($"Параметри збережено у файл: {path}");
    }

    private async Task StartObject2()
    {
        try
        {
            string exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Object2.exe");
            
            // Перевіряємо чи Object2 вже запущений
            if (_object2Process != null && !_object2Process.HasExited)
            {
                AddLog("Object2 вже запущений, надсилаємо сигнал для повторної обробки");
                await MessageClient.SendMessageAsync("Object2_Pipe", "START");
                return;
            }

            if (!File.Exists(exePath))
            {
                AddLog($"ПОМИЛКА: Не знайдено файл {exePath}");
                StatusTextBlock.Text = "Помилка: Object2.exe не знайдено";
                return;
            }

            _object2Process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    UseShellExecute = true,
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
                }
            };

            _object2Process.Start();
            AddLog("Object2 запущено");
            StatusTextBlock.Text = "Object2 запущено, очікування готовності...";
        }
        catch (Exception ex)
        {
            AddLog($"Помилка запуску Object2: {ex.Message}");
            StatusTextBlock.Text = $"Помилка: {ex.Message}";
        }
    }

    private async Task StartObject3()
    {
        try
        {
            await Task.Delay(1000); // Невелика затримка для завершення запису в clipboard
            
            string exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Object3.exe");
            
            // Перевіряємо чи Object3 вже запущений
            if (_object3Process != null && !_object3Process.HasExited)
            {
                AddLog("Object3 вже запущений, надсилаємо сигнал для оновлення");
                await MessageClient.SendMessageAsync("Object3_Pipe", "UPDATE");
                return;
            }

            if (!File.Exists(exePath))
            {
                AddLog($"ПОМИЛКА: Не знайдено файл {exePath}");
                StatusTextBlock.Text = "Помилка: Object3.exe не знайдено";
                return;
            }

            _object3Process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    UseShellExecute = true,
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
                }
            };

            _object3Process.Start();
            AddLog("Object3 запущено");
            StatusTextBlock.Text = "Object3 запущено";
        }
        catch (Exception ex)
        {
            AddLog($"Помилка запуску Object3: {ex.Message}");
            StatusTextBlock.Text = $"Помилка: {ex.Message}";
        }
    }

    private void AddLog(string message)
    {
        string timestamp = DateTime.Now.ToString("HH:mm:ss");
        _logText += $"[{timestamp}] {message}\n";
        LogTextBlock.Text = _logText;
    }

    private void Exit_Click(object? sender, RoutedEventArgs e)
    {
        // Закриваємо дочірні процеси
        try
        {
            _object2Process?.Kill();
            _object3Process?.Kill();
        }
        catch { }
        
        _messageServer?.Dispose();
        Close();
    }

    private async void Help_Click(object? sender, RoutedEventArgs e)
    {
        var helpDialog = new Window
        {
            Title = "Довідка",
            Width = 500,
            Height = 300,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new ScrollViewer
            {
                Content = new TextBlock
                {
                    Text = "Лабораторна робота №6\n\n" +
                           "Система складається з трьох програм:\n\n" +
                           "1. Lab6 (ця програма) - менеджер системи\n" +
                           "   Керує роботою інших програм\n\n" +
                           "2. Object2 - генератор даних\n" +
                           "   Створює випадкові точки та записує їх у Clipboard\n\n" +
                           "3. Object3 - візуалізатор\n" +
                           "   Читає дані з Clipboard та відображає графік\n\n" +
                           "Для початку роботи:\n" +
                           "Файл -> Запустити процес\n\n" +
                           "Програми обмінюються повідомленнями через Named Pipes\n" +
                           "та даними через Clipboard Windows.",
                    Margin = new Avalonia.Thickness(20),
                    TextWrapping = Avalonia.Media.TextWrapping.Wrap
                }
            }
        };

        await helpDialog.ShowDialog(this);
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        _messageServer?.Dispose();
        
        try
        {
            _object2Process?.Kill();
            _object3Process?.Kill();
        }
        catch { }
        
        base.OnClosing(e);
    }
}
