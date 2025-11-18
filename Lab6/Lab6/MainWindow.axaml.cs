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
    private ParametersDialog.Parameters? _lastParameters;

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
                // Коли Object2 готовий — надсилаємо останні параметри, якщо вони є
                if (_lastParameters is not null)
                {
                    _ = SendParamsToObject2Async(_lastParameters);
                }
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

        // Опціонально збережемо параметри у файл як fallback (Object2 все одно отримає їх через pipe)
        await SaveParametersToFile(result);

        // Запускаємо/забезпечуємо Object2
        _lastParameters = result;
        await StartObject2();
        // Спроба надіслати параметри одразу; якщо сервер ще не готовий — OBJECT2_READY перешле їх пізніше
        _ = SendParamsToObject2Async(result);
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
            var (fileName, args, workingDir, exists) = ResolveExecutableOrDll("Object2");
            
            // Перевіряємо чи Object2 вже запущений
            if (_object2Process != null && !_object2Process.HasExited)
            {
                AddLog("Object2 вже запущений, надсилаємо сигнал для повторної обробки");
                await MessageClient.SendMessageAsync("Object2_Pipe", "START");
                return;
            }

            if (!exists)
            {
                AddLog($"ПОМИЛКА: Не знайдено виконуваний файл або DLL Object2");
                StatusTextBlock.Text = "Помилка: Object2.exe не знайдено";
                return;
            }

            _object2Process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = args ?? string.Empty,
                    UseShellExecute = string.IsNullOrEmpty(args),
                    WorkingDirectory = workingDir
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
            
            var (fileName, args, workingDir, exists) = ResolveExecutableOrDll("Object3");
            
            // Перевіряємо чи Object3 вже запущений
            if (_object3Process != null && !_object3Process.HasExited)
            {
                AddLog("Object3 вже запущений, надсилаємо сигнал для оновлення");
                await MessageClient.SendMessageAsync("Object3_Pipe", "UPDATE");
                return;
            }

            if (!exists)
            {
                AddLog($"ПОМИЛКА: Не знайдено виконуваний файл або DLL Object3");
                StatusTextBlock.Text = "Помилка: Object3.exe не знайдено";
                return;
            }

            _object3Process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = args ?? string.Empty,
                    UseShellExecute = string.IsNullOrEmpty(args),
                    WorkingDirectory = workingDir
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

    private static (string fileName, string? args, string workingDir, bool exists) ResolveExecutableOrDll(string baseName)
    {
        // Locate executable/DLL or fall back to `dotnet run` for development layouts.
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;

        string CandidateHost(string dir) =>
            File.Exists(Path.Combine(dir, baseName + ".exe"))
                ? Path.Combine(dir, baseName + ".exe")
                : Path.Combine(dir, baseName);

        var host = CandidateHost(baseDir);
        if (File.Exists(host)) return (host, null, baseDir, true);

        var dll = Path.Combine(baseDir, baseName + ".dll");
        if (File.Exists(dll)) return ("dotnet", dll, baseDir, true);

        // Try sibling project bin path (development layout)
        var netDir = new DirectoryInfo(baseDir);
        var configDir = netDir.Parent;
        var binDir = configDir?.Parent;
        var projDir = binDir?.Parent;
        var solutionLab6Dir = projDir?.Parent;
        if (solutionLab6Dir != null && configDir != null)
        {
            string cfg = configDir.Name;
            string framework = netDir.Name;

            string TryPath(string projectName)
                => Path.Combine(solutionLab6Dir.FullName, projectName, "bin", cfg, framework);

            string candidateDir = TryPath(baseName);
            var host2 = CandidateHost(candidateDir);
            if (File.Exists(host2)) return (host2, null, candidateDir, true);
            var dll2 = Path.Combine(candidateDir, baseName + ".dll");
            if (File.Exists(dll2)) return ("dotnet", dll2, candidateDir, true);

            var csproj = Path.Combine(solutionLab6Dir.FullName, baseName, baseName + ".csproj");
            if (File.Exists(csproj))
            {
                var argsRun = $"run --project \"{csproj}\" -c {cfg}";
                return ("dotnet", argsRun, solutionLab6Dir.FullName, true);
            }
        }
        // Try a published out folder one level up (Lab6/out)
        var outDir = Path.Combine(solutionLab6Dir?.FullName ?? baseDir, "out");
        var host3 = CandidateHost(outDir);
        if (File.Exists(host3)) return (host3, null, outDir, true);
        var dll3 = Path.Combine(outDir, baseName + ".dll");
        if (File.Exists(dll3)) return ("dotnet", dll3, outDir, true);

        // Not found
        return (Path.Combine(baseDir, baseName + ".exe"), null, baseDir, false);
    }

    private static async Task<bool> SendParamsToObject2Async(ParametersDialog.Parameters p)
    {
        // до 5 спроб із невеликою затримкою
        var payload = $"PARAMS:{p.NPoints};{p.XMin};{p.XMax};{p.YMin};{p.YMax}";
        for (int i = 0; i < 5; i++)
        {
            if (await MessageClient.SendMessageAsync("Object2_Pipe", payload))
                return true;
            await Task.Delay(400);
        }
        return false;
    }

    private void AddLog(string message)
    {
        string timestamp = DateTime.Now.ToString("HH:mm:ss");
        _logText += $"[{timestamp}] {message}\n";
        LogTextBlock.Text = _logText;
    }

    private void Exit_Click(object? sender, RoutedEventArgs e)
    {
        // Надсилаємо команди EXIT у дочірні процеси для акуратного завершення
        _ = MessageClient.SendMessageAsync("Object2_Pipe", "EXIT");
        _ = MessageClient.SendMessageAsync("Object3_Pipe", "EXIT");
        
        // Fallback: якщо не завершилися — примусово закриємо
        try { _object2Process?.Kill(); } catch { }
        try { _object3Process?.Kill(); } catch { }
        
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
        // Надсилаємо EXIT й закриваємо як fallback
        _ = MessageClient.SendMessageAsync("Object2_Pipe", "EXIT");
        _ = MessageClient.SendMessageAsync("Object3_Pipe", "EXIT");
        try { _object2Process?.Kill(); } catch { }
        try { _object3Process?.Kill(); } catch { }
        
        base.OnClosing(e);
    }
}
