using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WpfApp1.Unity;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WpfApp1.Objects
{
  internal class ObjectsControl
  {
    private static readonly Lazy<ObjectsControl> _instance = new Lazy<ObjectsControl>(() => new ObjectsControl());
    public static ObjectsControl Instance = _instance.Value;

    readonly MainWindow mainWindow;
    readonly System.Windows.Controls.TreeView treeView;

    private ObjectsControl() 
    {
      mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
      treeView = mainWindow.Objects;
    }

    public void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
    {
      if (e.RightButton == System.Windows.Input.MouseButtonState.Pressed)
      {
        //var point = e.GetPosition(treeView);
        mainWindow.ObjectsContextMenu.IsOpen = !mainWindow.ObjectsContextMenu.IsOpen;
      }
    }

    public void OnContextMenuOpened()
    {
      mainWindow.ObjectsContextMenu.Items.Clear();

      var menuItem = new MenuItem()
      {
        Header = "Character",
      };
      menuItem.Click += (o, e) =>
      {
        CreateCharacter();
      };
      mainWindow.ObjectsContextMenu.Items.Add(menuItem);
    }

    public void CreateCharacter()
    {
      try
      {
        var dockPanel = new DockPanel
        {
          LastChildFill = true,
          HorizontalAlignment = HorizontalAlignment.Stretch,
        };

        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.UriSource = new Uri("/Resources/PORTRT0.bmp", UriKind.RelativeOrAbsolute);
        bitmap.DecodePixelWidth = 20;
        bitmap.EndInit();

        var icon = new System.Windows.Controls.Image
        {
          VerticalAlignment = System.Windows.VerticalAlignment.Center,
          Source = bitmap,
          MaxWidth = 20,
        };
        dockPanel.Children.Add(icon);

        var name = new TextBlock
        {
          VerticalAlignment = System.Windows.VerticalAlignment.Center,
          Text = "Character"
        };
        dockPanel.Children.Add(name);

        var item = new TreeViewItem
        {
          Header = dockPanel
        };

        treeView.Items.Add(item);
      }
      catch (Exception e)
      {
        mainWindow.StatusBarText.Text = $"ObjectsControl.CreateCharacter() exception: {e.Message}";
      }
    }
  }
}
