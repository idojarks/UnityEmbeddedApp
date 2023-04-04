using System;

namespace WpfLibrary1
{
  public class Class1
  {
    private static readonly Lazy<Class1> _instance = new(() => new Class1());
    public static Class1 Instance = _instance.Value;

    private Class1() { }

    public void Import()
    {
      UnityEditor.AssetDatabase.Refresh();
    }
  }
}
