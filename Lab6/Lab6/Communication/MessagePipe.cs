using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

namespace Lab6.Communication;

public class MessageClient
{
    public static async Task<bool> SendMessageAsync(string pipeName, string message)
    {
        try
        {
            using var client = new NamedPipeClientStream(".", pipeName, PipeDirection.Out);
            await client.ConnectAsync(2000); // 2 second timeout
            
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            await client.WriteAsync(buffer, 0, buffer.Length);
            await client.FlushAsync();
            
            return true;
        }
        catch
        {
            return false;
        }
    }
}

public class MessageServer : IDisposable
{
    private readonly string _pipeName;
    private NamedPipeServerStream? _server;
    private bool _isRunning;
    private Task? _listenTask;

    public event Action<string>? MessageReceived;

    public MessageServer(string pipeName)
    {
        _pipeName = pipeName;
    }

    public void Start()
    {
        if (_isRunning) return;
        
        _isRunning = true;
        _listenTask = Task.Run(ListenAsync);
    }

    private async Task ListenAsync()
    {
        while (_isRunning)
        {
            try
            {
                _server = new NamedPipeServerStream(_pipeName, PipeDirection.In, 1, 
                    PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
                
                await _server.WaitForConnectionAsync();
                
                using var reader = new StreamReader(_server, Encoding.UTF8);
                string message = await reader.ReadToEndAsync();
                
                MessageReceived?.Invoke(message);
                
                _server.Disconnect();
                _server.Dispose();
            }
            catch
            {
                // Ignore connection errors
            }
        }
    }

    public void Stop()
    {
        _isRunning = false;
        _server?.Dispose();
    }

    public void Dispose()
    {
        Stop();
    }
}
