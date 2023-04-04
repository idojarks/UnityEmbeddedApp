using System;
using System.IO;
using System.Xml.Linq;
using UnityEngine;

namespace WpfLibrary3
{
  public class Class1
  {
    private static readonly Lazy<Class1> _instance = new Lazy<Class1>(() => new Class1());
    public static Class1 Instance = _instance.Value;

    private Class1() { }

    public void LoadAssetBundle()
    {
      var path = Path.Combine(Application.dataPath, "AssetBundles/AssetBundles/mesh");
      var myLoadedAssetBundle = AssetBundle.LoadFromFile(path);

      if (myLoadedAssetBundle == null)
      {
        Debug.Log("Failed to load AssetBundle!");
        return;
      }

      foreach (var name in myLoadedAssetBundle.GetAllAssetNames())
      {
        var prefab = myLoadedAssetBundle.LoadAsset(name);
        GameObject.Instantiate(prefab);
      }
    }
  }
}
