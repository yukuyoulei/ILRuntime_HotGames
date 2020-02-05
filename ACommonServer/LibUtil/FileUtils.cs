using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class FileUtils
{
	public static string FileSizeToString(long fileSize, int floatSize = 2)
	{
		if (fileSize < 1024)
		{
			return fileSize + "B";
		}
		if (fileSize < 1024 * 1024)
		{
			return (fileSize / 1024.0).ToString("f" + floatSize) + "K";
		}
		if (fileSize < 1024 * 1024 * 1024)
		{
			return (fileSize / 1024.0 / 1024).ToString("f" + floatSize) + "M";
		}
		if (fileSize < (long)1024 * 1024 * 1024 * 1024)
		{
			return (fileSize / 1024.0 / 1024 / 1024).ToString("f" + floatSize) + "G";
		}
		return (fileSize / 1024.0 / 1024 / 1024 / 1024).ToString("f" + floatSize) + "T";
	}
	public static void CopyDirectory(string source, string target)
	{
		// 创建目的文件夹
		if (!Directory.Exists(target))
		{
			Directory.CreateDirectory(target);
		}

		// 拷贝文件
		DirectoryInfo sDir = new DirectoryInfo(source);
		FileInfo[] fileArray = sDir.GetFiles();
		foreach (FileInfo file in fileArray)
		{
			file.CopyTo(target + "/" + file.Name, true);
		}

		// 循环子文件夹
		DirectoryInfo dDir = new DirectoryInfo(target);
		DirectoryInfo[] subDirArray = sDir.GetDirectories();
		foreach (DirectoryInfo subDir in subDirArray)
		{
			CopyDirectory(subDir.FullName, target + "/" + subDir.Name);
		}
	}
}
