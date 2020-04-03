using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public class AIniLoader
{
	private Dictionary<string, string> dIniNodes = new Dictionary<string, string>();
	private Dictionary<string, string> dDefaultValues = new Dictionary<string, string>();
	public void OnSetDefaultValue(string key, int value)
	{
		OnSetDefaultValue(key, value.ToString());
	}
	public void OnSetDefaultValue(string key, string value)
	{
		if (dDefaultValues.ContainsKey(key))
		{
			dDefaultValues[key] = value;
		}
		else
		{
			dDefaultValues.Add(key, value);
		}
	}
	public List<string> AllKeys
	{
		get
		{
			return dIniNodes.Keys.ToList();
		}
	}
	private string fileName = "";
	public void LoadIniFile(string sFileName)
	{
		fileName = sFileName;

		if (!File.Exists(sFileName))
		{
			File.Create(sFileName).Close();
		}
		else
		{
			StreamReader mysr = null;
			try
			{
				mysr = new StreamReader(sFileName, System.Text.Encoding.UTF8);
				string strline = "";
				while ((strline = mysr.ReadLine()) != null)
				{
					DoAddLine(strline);
				}
			}
			catch
			{

			}
			finally
			{
				mysr.Close();
			}
		}
	}
	public void LoadContext(string content)
	{
		string[] acontent = content.Split(new char[] { '\n', '&' }, StringSplitOptions.RemoveEmptyEntries);
		foreach (string scontent in acontent)
		{
			DoAddLine(scontent);
		}
	}

	private void DoAddLine(string strline)
	{
		if (string.IsNullOrEmpty(strline))
		{
			return;
		}
		if (strline.StartsWith("//") || strline.StartsWith("#"))
		{
			return;
		}
		string[] astr = strline.Split(new char[] { '=' }, 2);
		if (astr.Length != 2)
		{
			return;
			//throw new Exception("Invalid line " + strline);
		}

		dIniNodes.Add(astr[0].Trim(), astr[1].Trim());
	}

	public int OnGetIntValue(string sNodeName, int defaultValue = -1)
	{
		return typeParser.intParse(OnGetValue(sNodeName));
	}

	public void OnSetValue(string sNodeName, int Value)
	{
		OnSetValue(sNodeName, Value.ToString());
	}
	public void OnSetValue(string sNodeName, string sValue)
	{
		if (dIniNodes.ContainsKey(sNodeName))
		{
			dIniNodes[sNodeName] = sValue;
		}
		else
		{
			dIniNodes.Add(sNodeName, sValue);
		}
	}

	internal bool ContainsKey(string v)
	{
		return dIniNodes.ContainsKey(v);
	}

	public string OnGetValue(string sNodeName)
	{
		if (!dIniNodes.ContainsKey(sNodeName))
		{
			if (dDefaultValues.ContainsKey(sNodeName))
			{
				return dDefaultValues[sNodeName];
			}
			return "";
		}
		return dIniNodes[sNodeName];
	}
    public bool ContainsNode(string sNodeName)
    {
        return dIniNodes.ContainsKey(sNodeName);
    }

	public void OnSaveBack()
	{
		string shistory = "";
		foreach (string str in dIniNodes.Keys)
		{
			shistory += str + "=" + dIniNodes[str] + "\n";
		}
		foreach (string str in dDefaultValues.Keys)
		{
			if (dIniNodes.ContainsKey(str))
			{
				continue;
			}
			shistory += str + "=" + dDefaultValues[str] + "\n";
		}
		StreamWriter sw = new StreamWriter(fileName);
		sw.Write(shistory);
		sw.Close();
	}

	internal string GetAllText()
	{
		return File.ReadAllText(fileName);
	}
}
