using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class SVNUtils
{
    public static List<string> drives = new List<string>() { "c:", "d:", "e:", "f:" };
    public static string svnPath = @"\Program Files\TortoiseSVN\bin\";
    public static string svnProc = @"TortoiseProc.exe";
    public static string svnProcPath = "";

    [MenuItem("Assets/SVN更新 %&e")]
    public static void UpdateFromSVN()
    {
        if (string.IsNullOrEmpty(svnProcPath))
            svnProcPath = GetSvnProcPath();
        var dir = new DirectoryInfo(Application.dataPath);
        var path = dir.Parent.FullName.Replace('/', '\\');
        var para = "/command:update /path:\"" + path + "\" /closeonend:0";
        System.Diagnostics.Process.Start(svnProcPath, para);
    }

    [MenuItem("Assets/SVN提交 %&r")]
    public static void CommitToSVN()
    {
        if (string.IsNullOrEmpty(svnProcPath))
            svnProcPath = GetSvnProcPath();
        var dir = new DirectoryInfo(Application.dataPath);
        var path = dir.Parent.FullName.Replace('/', '\\');
        var para = "/command:commit /path:\"" + path + "\"";
        System.Diagnostics.Process.Start(svnProcPath, para);
    }

    [MenuItem("Assets/回滚本地修改（更新时看到红色字请点我！） %&t")]
    public static void RevertFromSVN()
    {
        if (string.IsNullOrEmpty(svnProcPath))
            svnProcPath = GetSvnProcPath();
        var path = Application.dataPath.Replace('/', '\\');
        var para = "/command:revert /path:\"" + path + "\"";
        System.Diagnostics.Process.Start(svnProcPath, para);
    }

    /*[MenuItem("Assets/SVN更新策划数据 %&i")]
    public static void UpdateDataFromSVN()
    {
        if (string.IsNullOrEmpty(svnProcPath))
            svnProcPath = GetSvnProcPath();
        var path = (Application.dataPath + "/Resources/data/xml").Replace('/', '\\');
        var para = "/command:update /path:\"" + path + "\" /closeonend:0";
        System.Diagnostics.Process.Start(svnProcPath, para);
    }*/

    [MenuItem("Assets/SVN添加 %&u")]
    public static void AddToSVN()
    {
        if (string.IsNullOrEmpty(svnProcPath))
            svnProcPath = GetSvnProcPath();
        var path = Application.dataPath.Replace('/', '\\');
        var para = "/command:add /path:\"" + path + "\"";
        System.Diagnostics.Process.Start(svnProcPath, para);
    }

    [MenuItem("Assets/清理SVN %&y")]
    public static void CleanUpFromSVN()
    {
        if (string.IsNullOrEmpty(svnProcPath))
            svnProcPath = GetSvnProcPath();
        var path = Application.dataPath.Replace('/', '\\');
        var para = "/command:cleanup /path:\"" + path + "\"";
        System.Diagnostics.Process.Start(svnProcPath, para);
    }

    public static string GetSvnProcPath()
    {
        foreach (var item in drives)
        {
            var path = string.Concat(item, svnPath, svnProc);
            if (File.Exists(path))
                return path;
        }
        return EditorUtility.OpenFilePanel("Select TortoiseProc.exe", "c:\\", "exe");
    }
}