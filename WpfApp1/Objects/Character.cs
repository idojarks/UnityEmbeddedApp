using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using WpfApp1.Assets;
using WpfApp1.Unity;

namespace WpfApp1.Objects
{
  internal class Character : BaseObj
  {
    readonly MainWindowWpf _mainWindow;
    readonly TreeView _treeView;
    readonly StackPanel _stackPanel;

    int _unityModelId = 0;

    TextBlock _name = new()
    {
      VerticalAlignment = System.Windows.VerticalAlignment.Center
    };

    TextBox _textBoxModelAsset = new()
    {
      Width = 80,
      IsReadOnly = true,
      AllowDrop = true,
    };

    public Character(string name) : base() 
    {
      _mainWindow = (MainWindowWpf)System.Windows.Application.Current.MainWindow;
      _treeView = _mainWindow.Objects;
      _stackPanel = _mainWindow.ObjectProperty;

      _name.Text = name;

      CreateObject();
      SetCharacterPropertyPanel();
    }

    void CreateObject()
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

      dockPanel.Children.Add(_name);

      var item = new TreeViewItem
      {
        Header = dockPanel
      };

      _treeView.Items.Add(item);
    }

    void SetCharacterPropertyPanel()
    {
      _textBoxModelAsset.PreviewDragOver += Box_PreviewDragOver;
      _textBoxModelAsset.PreviewDrop += OnDropCharacterModel;

      _stackPanel.Children.Clear();

      var panel = new StackPanel
      {
        Orientation = Orientation.Horizontal,
        AllowDrop = true
      };

      _stackPanel.Children.Add(panel);

      var text = new TextBlock()
      {
        Text = "Asset",
      };

      panel.Children.Add(text);

      panel.Children.Add(_textBoxModelAsset);
    }

    private void Box_PreviewDragOver(object sender, DragEventArgs e)
    {
      var className = typeof(AssetItem).FullName;

      if (e.Data.GetDataPresent(className))
      {
        e.Effects = DragDropEffects.All;
      }
    }

    private void OnDropCharacterModel(object sender, DragEventArgs e)
    {
      var className = typeof(AssetItem).FullName;

      if (e.Data.GetData(className) is AssetItem assetItem)
      {
        foreach (var item in ObjectBuilder.Instance._objects)
        {
          if (item.Value == this)
          {
            var unityInstanceId = item.Key;
            var unityModelAssetId = assetItem.AssetId;

            PipeClient.Instance.Send($"object.set.character.model {unityInstanceId} {assetItem.Name} {unityModelAssetId}");

            return;
          }
        }

      }
    }

    public void SetModel(string modelName, int unityModelId)
    {
      _textBoxModelAsset.Text = modelName;
      _textBoxModelAsset.Focus();

      _unityModelId = unityModelId;
    }

  }
}
