using System;
using System.Windows;

namespace DreamKit
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
    }

    private void Resize(object sender, EventArgs e)
    {
      UnityWindow.Instance.Resize();
    }

    private void Window_Closed(object sender, EventArgs e)
    {
      UnityWindow.Instance.Close();
    }
  }
}
