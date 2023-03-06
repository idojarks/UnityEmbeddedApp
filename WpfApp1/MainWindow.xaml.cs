using System;
using System.Windows;
using WpfApp1.Objects;

namespace WpfApp1
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();

      Activated += MainWindow_Activated;
    }

    private void MainWindow_Activated(object? sender, EventArgs e)
    {
      Unity.UnityPipe.Instance.Connect();
    }

    private void Resize(object sender, EventArgs e)
    {
      Unity.UnityWindow.Instance.Resize();
    }

    private void Window_Closed(object sender, EventArgs e)
    {
      Unity.UnityPipe.Instance.Disconnect();
      Unity.UnityWindow.Instance.Close();
    }

    private void Objects_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      ObjectsControl.Instance.OnMouseDown(e);
    }

    private void ObjectsContextMenu_Opened(object sender, RoutedEventArgs e)
    {
      ObjectsControl.Instance.OnContextMenuOpened();
    }
  }
}
