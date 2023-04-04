using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WpfApp1.Assets;
using WpfApp1.Unity;
using static System.Net.Mime.MediaTypeNames;

namespace WpfApp1.Objects
{
  internal class ObjectBuilder
  {
    private static readonly Lazy<ObjectBuilder> _instance = new(() => new ObjectBuilder());
    public static ObjectBuilder Instance = _instance.Value;

    readonly MainWindowWpf _mainWindow;
    readonly System.Windows.Controls.TreeView _treeView;
    readonly StackPanel _stackPanel;

    enum ObjectType
    {
      Character,
    }

    public Dictionary<int, BaseObj> _objects = new();

    private ObjectBuilder() 
    {
      _mainWindow = (MainWindowWpf)System.Windows.Application.Current.MainWindow;
      _treeView = _mainWindow.Objects;
      _stackPanel = _mainWindow.ObjectProperty;
      _stackPanel.AllowDrop = true;
    }

    public void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
    {
      if (e.RightButton == System.Windows.Input.MouseButtonState.Pressed)
      {
        _mainWindow.ObjectsContextMenu.IsOpen = !_mainWindow.ObjectsContextMenu.IsOpen;
      }
    }

    public void OnContextMenuOpened()
    {
      _mainWindow.ObjectsContextMenu.Items.Clear();

      var character = new MenuItem()
      {
        Header = "Character",
      };
      character.Click += (o, e) =>
      {
        PipeClient.Instance.Send("object.create.character character");
      };

      _mainWindow.ObjectsContextMenu.Items.Add(character);
    }

    public void OnResponse(List<string> commmand, List<string> parameters)
    {
      var f = commmand[0];
      var t = commmand[1];

      if (f == "create")
      {
        if (t == "character")
        {
          var r = commmand[2];

          if (r == "failed")
          {
            _mainWindow.Dispatcher.Invoke(() =>
            {
              _mainWindow.StatusBarText.Text = $"{this}.{MethodBase.GetCurrentMethod()?.Name} object build failed: {string.Join(" ", parameters)}";
            });

            return;
          }

          var characterName = parameters[0];
          var unityInstanceId = Convert.ToInt32(parameters[1]);

          if (_objects.TryGetValue(unityInstanceId, out var context))
          {
            _mainWindow.Dispatcher.Invoke(() =>
            {
              _mainWindow.StatusBarText.Text = $"{this}.{MethodBase.GetCurrentMethod()?.Name} unity instance already exists";
            });

            return;
          }

          _objects.Add(unityInstanceId, new Character(characterName));
        }
      }
      else if (f == "set")
      {
        if (t == "charater")
        {
          var o = commmand[2];
          var r = commmand[3];

          if (o == "model")
          {
            if (r == "succeed")
            {
              var unityInstanceId = Convert.ToInt32(parameters[0]);
              var modelName = parameters[1];
              var unityModelId = Convert.ToInt32(parameters[2]);

              if (_objects.TryGetValue(unityInstanceId, out var obj))
              {
                if (obj is Character c)
                {
                  c.SetModel(modelName, unityModelId);
                }
              }
            }
          }
        }
      }
    }

    
  }
}
