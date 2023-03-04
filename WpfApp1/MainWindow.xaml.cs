using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private readonly UnityWindow.UnityWindow? unityWindow = null;

    public MainWindow()
    {
      InitializeComponent();

      unityWindow = new UnityWindow.UnityWindow(UnityView);
    }

    private void Resize(object sender, EventArgs e)
    {
      unityWindow?.Resize();
    }

    private void Window_Closed(object sender, EventArgs e)
    {
      unityWindow?.Close();
    }
  }
}
