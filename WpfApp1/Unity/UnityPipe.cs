using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace WpfApp1.Unity
{
  internal sealed class UnityPipe
  {
    private static readonly Lazy<UnityPipe> _instance = new Lazy<UnityPipe>(() => new UnityPipe());
    public static UnityPipe Instance = _instance.Value;

    private readonly MainWindow mainWindow;
    private readonly NamedPipeClientStream pipeClient;

    private bool isConnected = false;
    private byte[] buffer = new byte[1024];
    private object pipeState = new object();

    private UnityPipe() 
    {
      mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
      pipeClient = new NamedPipeClientStream(".", "dreamkit", PipeDirection.InOut);
    }

    public void Connect()
    {
      if (isConnected == true)
      {
        return;
      }

      try
      {
        pipeClient.Connect(5000);

        isConnected = true;
        
        mainWindow.StatusBarText.Text = Receive();

        pipeClient.BeginRead(buffer, 0, buffer.Length, ReadCallback, null);
      }
      catch (Exception e)
      {
        mainWindow.StatusBarText.Text = $"connect to unity has failed: {e.Message}";
      }
    }

    void ReadCallback(IAsyncResult ar)
    {
      var bytesRead = 0;

      try
      {
        bytesRead = pipeClient.EndRead(ar);
      }
      catch (Exception e)
      {
        mainWindow.Dispatcher.Invoke(() =>
        {
          mainWindow.StatusBarText.Text = $"reading exception: {e.Message}";
        });
      }

      if (bytesRead > 0)
      {
        mainWindow.Dispatcher.Invoke(() =>
        {
          mainWindow.StatusBarText.Text = Encoding.Unicode.GetString(buffer, 0, bytesRead);
        });
      }

      pipeClient.BeginRead(buffer, 0, buffer.Length, ReadCallback, null);
    }

    public void Disconnect()
    {
      if (isConnected == true)
      {
        isConnected = false;
        pipeClient.Dispose();
      }
    }

    void Send(string message)
    {
      var outBuffer = Encoding.Unicode.GetBytes(message);
      var len = outBuffer.Length;

      pipeClient.WriteByte((byte)(len / 256));
      pipeClient.WriteByte((byte)(len % 256));
      pipeClient.Write(outBuffer, 0, len);
      pipeClient.Flush();
    }

    string Receive()
    {
      var len = pipeClient.ReadByte() * 256;
      len += pipeClient.ReadByte();
      var inBuffer = new byte[len];
      pipeClient.Read(inBuffer, 0, len);

      return Encoding.Unicode.GetString(inBuffer);
    }
  }
}
