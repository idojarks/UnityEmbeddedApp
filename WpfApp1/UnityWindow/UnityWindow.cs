using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WpfApp1.UnityWindow
{
  internal class UnityWindow
  {
    [DllImport("User32.dll")]
    static extern bool MoveWindow(IntPtr handle, int x, int y, int width, int height, bool redraw);

    [DllImport("User32.dll")]
    static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int width, int height, uint uflags);

    internal delegate int WindowEnumProc(IntPtr hwnd, IntPtr lparam);
    [DllImport("user32.dll")]
    internal static extern bool EnumChildWindows(IntPtr hwnd, WindowEnumProc func, IntPtr lParam);

    [DllImport("user32.dll")]
    static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32")]
    private static extern IntPtr SetParent(IntPtr hWnd, IntPtr hWndParent);

    private IntPtr unityHWND = IntPtr.Zero;
    private readonly Panel parent;

    private const int WM_ACTIVATE = 0x0006;
    private readonly IntPtr WA_ACTIVE = new IntPtr(1);
    private readonly IntPtr WA_INACTIVE = new IntPtr(0);
    private const int SWP_NOMOVE = 0x0002;
    private const int SWP_NOOWNERZORDER = 0x0200;

    private Process process;

    public UnityWindow(Panel parentPanel) 
    {
      parent = parentPanel;

      process = new Process();
      process.StartInfo.FileName = "D:\\study\\unity\\uwp\\output2\\uwp.exe";
      process.StartInfo.Arguments = "-parentHWND " + parentPanel.Handle.ToInt32() + " " + Environment.CommandLine;
      process.StartInfo.UseShellExecute = true;
      process.StartInfo.CreateNoWindow = true;
      process.Start();

      process.WaitForInputIdle();

      SetParent(process.Handle, parentPanel.Handle);

      EnumChildWindows(parentPanel.Handle, WindowEnum, IntPtr.Zero);

      using (AnonymousPipeServerStream pipeServer = 
        new AnonymousPipeServerStream(PipeDirection.Out, System.IO.HandleInheritability.Inheritable))
      {

      }
    }

    private int WindowEnum(IntPtr hwnd, IntPtr lparam)
    {
      unityHWND = hwnd;
      ActivateUnityWindow();
      return 0;
    }

    private void ActivateUnityWindow()
    {
      SendMessage(unityHWND, WM_ACTIVATE, WA_ACTIVE, IntPtr.Zero);
    }

    private void DeactivateUnityWindow()
    {
      SendMessage(unityHWND, WM_ACTIVATE, WA_INACTIVE, IntPtr.Zero);
    }

    public void Resize()
    {
      MoveWindow(unityHWND, 0, 0, parent.Width, parent.Height, true);
      ActivateUnityWindow();
    }

    public void Close()
    {
      try
      {
        process.CloseMainWindow();

        Thread.Sleep(1000);

        while (process.HasExited == false) 
        {
          process.Kill();
        }
      }
      catch (Exception)
      {

        throw;
      }
    }
  }
}
