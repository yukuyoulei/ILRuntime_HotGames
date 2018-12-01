#if ENABLE_NETWORK
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System.Collections;

public static class Util_Zip
{
	private static string Pd = "^$*&(!";
	public static string CreateZip(string zipPath, string dirPath)
	{
		ZipFile czip = ZipFile.Create(zipPath);
		czip.Password = Pd;
		DiskArchiveStorage myDisk = new DiskArchiveStorage(czip);
		czip.BeginUpdate(myDisk);
		var files = Directory.EnumerateFiles(dirPath);
		foreach (string file in files)
		{
			if (!file.EndsWith(".txt"))
			{
				continue;
			}
			if (!File.Exists(file))
			{
				AOutput.Log("File not found!");
			}
			czip.Add(file, Path.GetFileName(file));
		}
		czip.CommitUpdate();
		czip.Close();
		return zipPath;
	}
	public static string ExtractZip(string zipPath, string dirPath)
	{
		ZipConstants.DefaultCodePage = 0;
		FastZip zip = new FastZip();
		zip.Password = Pd;
		zip.ExtractZip(zipPath, dirPath, "");
		return dirPath;
	}
	public static string ExtractZip(Stream zipStream, string dirPath)
	{
		FastZip zip = new FastZip();
		zip.Password = Pd;
		zip.ExtractZip(zipStream, dirPath, FastZip.Overwrite.Always, null, "", "", false, true);
		return dirPath;
	}
}

#endif