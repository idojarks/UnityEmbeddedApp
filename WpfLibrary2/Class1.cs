using System;
using System.IO;
using System.Windows.Controls;
using UnityEditor;

namespace WpfLibrary2
{
  public class Class1
  {
    private static readonly Lazy<Class1> _instance = new(() => new Class1());
    public static Class1 Instance = _instance.Value;

    private Class1() { }

    public void Import(string filePath)
    {
      var dir = Path.GetDirectoryName(filePath);
      
      try
      {
        var q = new UnityEngine.Vector2(1, 1);

        /*var guids = AssetDatabase.FindAssets("t:mesh", new[] { $"{dir}" });
        AssetDatabase.ImportAsset(filePath);*/
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
    }
  }
}
