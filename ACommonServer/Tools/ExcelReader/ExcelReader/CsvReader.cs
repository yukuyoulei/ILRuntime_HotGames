using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExcelReader
{
	public partial class CsvReader : Form
	{
		AIniLoader iniLoader;
		public CsvReader()
		{
			InitializeComponent();

			this.AllowDrop = true;

			iniLoader = new AIniLoader();
			iniLoader.LoadIniFile("config.ini");

			var saveScript = iniLoader.OnGetValue("saveScript");
			if (!string.IsNullOrEmpty(saveScript))
			{
				inputSaveScript.Text = saveScript;
			}
		}

		private void CsvReader_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = DragDropEffects.Copy;
		}

		private void CsvReader_DragDrop(object sender, DragEventArgs e)
		{
			var data = (string[])e.Data.GetData(DataFormats.FileDrop);
			if (data.Length == 0)
			{
				return;
			}
			listFiles.Items.Clear();
			foreach (var d in data)
			{
				listFiles.Items.Add(d);
			}
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(inputSaveScript.Text))
			{
				iniLoader.OnSetValue("saveScript", inputSaveScript.Text);
				iniLoader.OnSaveBack();
			}
		}

		private void btnParse_Click(object sender, EventArgs e)
		{
			if (listFiles.Items.Count == 0)
			{
				return;
			}
			var saveScriptPath = inputSaveScript.Text;
			if (string.IsNullOrEmpty(saveScriptPath))
			{
				saveScriptPath = Environment.CurrentDirectory;
			}
			if (!Directory.Exists(saveScriptPath))
			{
				Directory.CreateDirectory(saveScriptPath);
			}
			foreach (var file in listFiles.Items)
			{
				if (!File.Exists(file.ToString()))
				{
					continue;
				}
				ParseFile(file.ToString(), saveScriptPath);
			}
			btnSave_Click(null, null);
			Process.Start(saveScriptPath);
		}

		private void ParseFile(string file, string saveScriptPath)
		{
			var fileInfo = new FileInfo(file);
			var outputPath = saveScriptPath + "/" + fileInfo.Name.Replace(".csv", "").Replace(".txt", "") + "Loader.cs";
			var sep = new char[] { ' ' };
			if (file.EndsWith(".csv"))
			{
				sep = new char[] { ',' };
			}
			else
			{
				sep = new char[] { ',', '\t' };
			}
			var lines = File.ReadAllLines(file, Encoding.UTF8);
			Dictionary<string, string> dHeads = new Dictionary<string, string>();
			if (lines.Length < 3)
			{
				MessageBox.Show(file + "不够三行，不能解析");
				return;
			}
			var line0 = lines[0].Split(sep, StringSplitOptions.None);
			var line1 = lines[1].Split(sep, StringSplitOptions.None);
			var line2 = lines[2].Split(sep, StringSplitOptions.None);
			if (line0.Length != line1.Length || line1.Length != line2.Length)
			{
				MessageBox.Show(file + "前三行列数需一致，第一行为注释，第二行为变量名，第三行为变量类型。解析失败！");
				return;
			}
			var className = fileInfo.Name.Split('.')[0];
			var loaderName = className + "Loader";
			var result = "public class " + className + ":DataBase"
				+ "\r\n" + "{"
				;
			for (var i = 1; i < line1.Length; i++)//跳过id
			{
				result += "\r\n\t" + "public " + line2[i] + " " + line1[i] + ";//" + line0[i];
			}
			result += "\r\n}";
			result += "\r\npublic class " + loaderName + " : SingletonDataLoader<" + loaderName + ", " + className + ">"
				+ "\r\n" + "{"
				+ "\r\n" + "\tpublic override void OnLoadContent(string content)"
				+ "\r\n" + "\t{"
				+ "\r\n" + "\t\tLoadContent(content);"
				+ "\r\n"
				+ "\r\n" + "\t\tfor (var i = 3; i < m_Datas.Count; i++)"
				+ "\r\n" + "\t\t{"
                + "\r\n" + "\t\t\tistart = 0;"
                + "\r\n" + "\t\t\tvar id = GetIntValue(i, istart++);"
				+ "\r\n" + "\t\t\tif (id < 1)"
				+ "\r\n" + "\t\t\t{"
				+ "\r\n" + "\t\t\tcontinue;"
				+ "\r\n" + "\t\t\t}"
				+ "\r\n" + ""
				+ "\r\n" + "\t\t\tvar data = new " + className + "();"
				+ "\r\n" + "\t\t\tdata.id = id;"
				;
			for (var i = 1; i < line1.Length; i++)
			{
				result += "\r\n" + "\t\t\tdata." + line1[i] + " = Get" + line2[i].Substring(0, 1).ToUpper() + line2[i].Substring(1) + "Value(i,istart++);";
			}
			result += "\r\n" + "\t\t\tOnAddData(data);"
				+ "\r\n" + "\t\t}"
				+ "\r\n" + "\t}"
				+ "\r\n" + "}"
				;
			File.WriteAllText(outputPath, result);
		}
	}
}
