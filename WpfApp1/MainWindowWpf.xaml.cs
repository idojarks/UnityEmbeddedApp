using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using WpfApp1.Assets;
using WpfApp1.Objects;
using WpfApp1.Unity;

namespace WpfApp1
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindowWpf : Window
  {
    public MainWindowWpf()
    {
      InitializeComponent();

      Activated += MainWindow_Activated;
      System.Windows.Application.Current.Exit += Current_Exit;
    }

    private void MainWindow_Activated(object? sender, EventArgs e)
    {
#if EMBEDDED_UNITY_PLAYER
      Unity.PipeClient.Instance.Connect();
#endif
    }

    private void Resize(object sender, EventArgs e)
    {
#if EMBEDDED_UNITY_PLAYER
      Unity.UnityWindow.Instance.Resize();
#endif
    }

    private void Window_Closed(object sender, EventArgs e)
    {
    }

    private void Current_Exit(object sender, ExitEventArgs e)
    {
#if EMBEDDED_UNITY_PLAYER
      Unity.PipeClient.Instance.Disconnect();
      Unity.UnityWindow.Instance.Close();
#endif 
    }

    private void Objects_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      ObjectBuilder.Instance.OnMouseDown(e);
    }

    private void ObjectsContextMenu_Opened(object sender, RoutedEventArgs e)
    {
      ObjectBuilder.Instance.OnContextMenuOpened();
    }

    private void ResourceView_Drop(object sender, DragEventArgs e)
    {
      if (e.Data.GetDataPresent(DataFormats.FileDrop))
      {
        var data = e.Data.GetData(DataFormats.FileDrop);

        if (data is string[] files)
        {
          foreach (var file in files)
          {
            AssetsView.Instance.OnDropFile(file);
          }
        }
      }
    }

    private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
    {
      switch (e.Key)
      {
        case Key.Q:
          {
            var msg = $"asset.import.fbx mesh BasicMotionsDummyModel.fbx 324ljkvfdfgd0fg";
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
                  }
                  break;
                case "asset":
                  {
                    AssetsView.Instance.Parse(command, tokens);
                  }
                  break;
              }
            }
          }
          break;
        case Key.C:
          {
            if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
            {

            }
          }
          break;
        case Key.V:
          {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {

            }
          }
          break;
        default:
          break;
      }
    }

    private void AssetPanel_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      AssetsView.Instance.OnMouseLeftButtonDown(sender, e);
    }
  }
}
