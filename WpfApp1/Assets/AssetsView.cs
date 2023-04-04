using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfApp1.Unity;

namespace WpfApp1.Assets
{
  internal class AssetsView
  {
    private static readonly Lazy<AssetsView> _instance = new(() => new AssetsView());
    public static AssetsView Instance = _instance.Value;

    private readonly MainWindowWpf _mainWindow;
    private readonly ListView _listView;

    public string UnityResourcesFolder
    {
      set; get;
    }

    private AssetsView() 
    {
      _mainWindow = (MainWindowWpf)System.Windows.Application.Current.MainWindow;
      _listView = _mainWindow.AssetPanel;
      UnityResourcesFolder = "";
    }

    public void OnDropFile(string filePath)
    {
      var fileName = Path.GetFileName(filePath);

      try
      {
        var dir = $"{UnityResourcesFolder}/Assets";

        Directory.CreateDirectory(dir);
        File.Copy(filePath, $"{dir}/{fileName}", true);
      }
      catch (Exception e)
      {
        _mainWindow.StatusBarText.Text = $"[{this}.{System.Reflection.MethodBase.GetCurrentMethod()?.Name}] {e.Message}";
        return;
      }

      PipeClient.Instance.Send($"resource.load.assetbundle {fileName}");
    }

    public void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      ListBox parent = (ListBox)sender;
      var dragSource = parent;
      var mydata = GetDataFromListBox(dragSource, e.GetPosition(parent));

      if (mydata != null)
      {
        DragDrop.DoDragDrop(parent, mydata, DragDropEffects.Move);
      }
    }

    private static object? GetDataFromListBox(ListBox source, System.Windows.Point point)
    {
      UIElement? element = source.InputHitTest(point) as UIElement;
      if (element != null)
      {
        object data = DependencyProperty.UnsetValue;
        while (data == DependencyProperty.UnsetValue)
        {
          data = source.ItemContainerGenerator.ItemFromContainer(element);

          if (data == DependencyProperty.UnsetValue)
          {
            element = VisualTreeHelper.GetParent(element) as UIElement;
          }

          if (element == source)
          {
            return null;
          }
        }

        if (data != DependencyProperty.UnsetValue)
        {
          return data;
        }
      }

      return null;
    }

    public void Parse(List<string> commands, List<string> parameters)
    {
      var command = commands[0];
      commands.RemoveAt(0);

      switch (command)
      {
        case "load":
          {
            ParseLoadingResult(commands, parameters);
          }
          break;

        default:
          break;
      }
    }

    public void ParseLoadingResult(List<string> commands, List<string> parameters)
    {
      BitmapImage bitmap;
      string fileName;
      string assetType;
      string assetId;

      var which = commands.FirstOrDefault();
      commands.RemoveAt(0);

      switch (which)
      {
        case "assetbundle":
          {
            var result = commands.FirstOrDefault();

            if (result == "succeed")
            {
              if (parameters.Count != 3)
              {
                _mainWindow.Dispatcher.Invoke(() =>
                {
                  _mainWindow.StatusBarText.Text = $"[{this}] insufficient parameters count {string.Join(" ", parameters)}";
                });

                return;
              }

              fileName = parameters[0];
              assetType = parameters[1];
              assetId = parameters[2];
            }
            else
            {
              _mainWindow.Dispatcher.Invoke(() =>
              {
                _mainWindow.StatusBarText.Text = $"[{this}] failed to load asset: {string.Join(" ", parameters)}";
              });

              return;
            }

            var imageUri = assetType switch
            {
              "Model" => "/Resources/PORTRT0.bmp",
              "Animation" => "/Resources/PORTRT0.bmp",
              _ => "/Resources/PORTRT0.bmp"
            };

            bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(imageUri, UriKind.RelativeOrAbsolute);
            bitmap.DecodePixelWidth = 20;
            bitmap.EndInit();
          }
          break;

        default:
          {
            _mainWindow.Dispatcher.Invoke(() =>
            {
              _mainWindow.StatusBarText.Text = $"[{this}] unknown asset type: {which}";
            });
          }
          return;
      }

      var items = new List<AssetItem>
      {
        new AssetItem
        {
          Image = bitmap,
          Name = fileName.Split('.').FirstOrDefault(),
          AssetId = assetId,
        }
      };

      _listView.ItemsSource = items;
    }
  }

  public class AssetItem
  {
    public BitmapImage? Image { get; set; }
    public string? Name { get; set; }
    public string? AssetId { get; set; }
  }
}
