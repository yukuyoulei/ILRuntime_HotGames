using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;

public class UCopyFilesFromStreamingAssets : MonoBehaviour
{
    Action actionOnComp;
    public void OnCopy(string fileListName, string toDir, Action actOnComp)
    {
        this.actionOnComp = actOnComp;

        StartCoroutine(DoLoadFileList(fileListName));
    }

    private List<string> lfiles = new List<string>();
    private IEnumerator DoLoadFileList(string fileListName)
    {
        lfiles.Clear();
        var www = new WWW(UStaticFuncs.GetStreamAssetPath() + UStaticFuncs.GetPlatformFolder(Application.platform) + "/" + fileListName);
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
            var atext = www.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var t in atext)
            {
                var fname = "";
                var av = t.Split('|');
                if (av.Length <= 2)
                {
                    continue;
                }
                fname = av[0];
                if (fname.StartsWith("/") || fname.StartsWith("\\"))
                {
                    fname = fname.Substring(1);
                }
                fname = fname.Replace("\\", "/");
                if (fname.EndsWith(".meta"))
                {
                    continue;
                }
                lfiles.Add(fname);
            }

            totalCount = lfiles.Count;

            OnCopyFile();
        }
        else
        {
            AOutput.Log("DoCopyFile failed:" + www.error + " for file " + www.url);
            actionOnComp?.Invoke();
        }
    }

    float totalCount;
    private void OnCopyFile()
    {
        if (lfiles.Count == 0)
        {
            actionOnComp?.Invoke();
            return;
        }
        var fname = lfiles[0];
        lfiles.RemoveAt(0);

        StartCoroutine(DoCopyFile(fname));
    }

    private IEnumerator DoCopyFile(string fname)
    {
        var www = new WWW(UStaticFuncs.GetStreamAssetPath() + UStaticFuncs.GetPlatformFolder(Application.platform) + "/" + fname);
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
            var finfo = new FileInfo(UStaticFuncs.ConfigSaveDir + fname);
            if (!finfo.Directory.Exists)
            {
                finfo.Directory.Create();
            }
            System.IO.File.WriteAllBytes(finfo.FullName, www.bytes);
        }
        else
        {
            AOutput.Log("DoCopyFile failed:" + www.error + " for file " + www.url);
        }
        AOutput.Log("正在释放资源，不消耗流量。");
        OnCopyFile();
    }
}
