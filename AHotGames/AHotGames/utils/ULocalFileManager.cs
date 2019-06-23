using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ULocalFileManager : Singleton<ULocalFileManager>
{
    Dictionary<string, UFileInfo> dFiles = new Dictionary<string, UFileInfo>();
    public void OnAddFile(string name, string version)
    {
        name = name.Replace("\\", "/");
        OnRemoveFile(name);
        dFiles.Add(name, new UFileInfo(name, version));
    }
    public UFileInfo OnGetFile(string name)
    {
        name = name.Replace("\\", "/");
        if (dFiles.ContainsKey(name))
        {
            return dFiles[name];
        }
        if (File.Exists(Utils.ConfigSaveDir + name))
        {
            var fi = new UFileInfo(name, Utils.GetMD5HashFromFile(Utils.ConfigSaveDir + name));
            dFiles.Add(name, fi);
            return fi;
        }
        return null;
    }
    public void OnRemoveFile(string name)
    {
        name = name.Replace("\\", "/");
        if (dFiles.ContainsKey(name))
        {
            dFiles.Remove(name);
        }
    }


}

public class UFileInfo
{
    public UFileInfo(string name, string version)
    {
        this.name = name;
        this.version = version;
    }
    public string name { get; private set; }
    public string version
    {
        get
        {
            return PlayerPrefs.GetString(name + "ver");
        }
        private set
        {
            PlayerPrefs.SetString(name + "ver", value);
        }
    }
}