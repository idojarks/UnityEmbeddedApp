using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows.Threading;
using WpfApp1.Assets;
using WpfApp1.Objects;

namespace WpfApp1.Unity
{
  internal sealed class PipeClient
  {
    private static readonly Lazy<PipeClient> _instance = new(() => new PipeClient());
    public static PipeClient Instance = _instance.Value;

    private readonly MainWindowWpf _mainWindow;
    private readonly NamedPipeClientStream _pipeClient;

    private bool isConnected = false;
    private readonly byte[] buffer = new byte[1024];

    private PipeClient() 
    {
      _mainWindow = (MainWindowWpf)System.Windows.Application.Current.MainWindow;
      _pipeClient = new NamedPipeClientStream(".", "dreamkit", PipeDirection.InOut, PipeOptions.Asynchronous);
    }

    public void Connect()
    {
      if (isConnected == true)
      {
        return;
      }

      try
      {
        _pipeClient.Connect(5000);

        isConnected = true;

        //_mainWindow.StatusBarText.Text = Receive();

        AssetsView.Instance.UnityResourcesFolder = Receive();

        _pipeClient.BeginRead(buffer, 0, buffer.Length, ReadCallback, null);
      }
      catch (Exception e)
      {
        _mainWindow.StatusBarText.Text = $"connect to unity has failed: {e.Message}";
      }
    }

    void ReadCallback(IAsyncResult ar)
    {
      if (!isConnected)
      {
        return;
      }

      var bytesRead = 0;

      try
      {
        bytesRead = _pipeClient.EndRead(ar);
      }
      catch (Exception e)
      {
        _mainWindow.Dispatcher.Invoke(() =>
        {
          _mainWindow.StatusBarText.Text = $"reading exception: {e.Message}";
        });
      }

      if (bytesRead > 0)
      {
        _mainWindow.Dispatcher.Invoke(() =>
        {
          ParseMessage(Encoding.Unicode.GetString(buffer, 0, bytesRead));
        });

        _pipeClient.BeginRead(buffer, 0, buffer.Length, ReadCallback, null);
      }
    }

    void ParseMessage(string msg)
    {
      var tokens = msg.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
      var command = tokens[0].Split('.', StringSplitOptions.RemoveEmptyEntries).ToList();

      tokens.RemoveAt(0);

      if (command.Count > 0)
      {
        var category = command[0];
        command.RemoveAt(0);

        switch (category)
        {
          case "object":
            {
              ObjectBuilder.Instance.OnResponse(command, tokens);
            }
            break;
          case "resource":
            {
              AssetsView.Instance.Parse(command, tokens);
            }
            break;
        }
      }
    }

    public void Disconnect()
    {
      if (isConnected == true)
      {
        isConnected = false;

        _pipeClient.Dispose();
        _pipeClient.Close();
      }
    }

    public void Send(string message)
    {
      try
      {
        var outBuffer = Encoding.Unicode.GetBytes(message);
        var len = outBuffer.Length;

        _pipeClient.WriteByte((byte)(len / 256));
        _pipeClient.WriteByte((byte)(len % 256));
        _pipeClient.Write(outBuffer, 0, len);
        _pipeClient.Flush();
      }
      catch (Exception e)
      {
        _mainWindow.StatusBarText.Text = $"pipe send exception: {e.Message}";
      }
    }

    string Receive()
    {
      var len = _pipeClient.ReadByte() * 256;
      len += _pipeClient.ReadByte();
      var inBuffer = new byte[len];
      _pipeClient.Read(inBuffer, 0, len);

      return Encoding.Unicode.GetString(inBuffer);

      /*var response = new StringBuilder();

      do
      {
        var len = pipeClient.ReadByte() * 256;
        len += pipeClient.ReadByte();
        var inBuffer = new byte[len];
        pipeClient.Read(inBuffer, 0, len);

        response.Append(Encoding.Unicode.GetString(inBuffer));
      } while (!pipeClient.IsConnected);

      return response.ToString();*/
    }
  }
}
