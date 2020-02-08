using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoGen
{
	class Program
	{
		static Dictionary<string, int> dProtoIDs = new Dictionary<string, int>();
		public static List<string> lProtoNames = new List<string>();
		public static List<string> lEnumNames = new List<string>();
		static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				printHelp();
				End();
				return;
			}
			Dictionary<string, string> dInfos = new Dictionary<string, string>();
			foreach (string sarg in args)
			{
				string[] aarg = sarg.Split(new string[] { ":" }, 2, StringSplitOptions.RemoveEmptyEntries);
				if (aarg.Length < 2)
				{
					continue;
				}
				if (!dInfos.ContainsKey(aarg[0]))
				{
					dInfos.Add(aarg[0], aarg[1]);
				}
			}
			if (dInfos.ContainsKey("debug"))
			{
				Debugger.Launch();
			}
			if (!dInfos.ContainsKey("file") || !dInfos.ContainsKey("out-cs"))
			{
				printHelp();
				End();
				return;
			}
			string sreadfile = FormatPath(dInfos["file"]);
			if (!File.Exists(sreadfile))
			{
				Console.WriteLine("找不到文件" + sreadfile);
				End();
				return;
			}

			List<ProtoCell> lProtoCells = new List<ProtoCell>();
			bool bHasError = false;

			StreamReader sr = new StreamReader(sreadfile);
			int lineNum = 0;
			string line = null;
			ProtoCell curProtoCell = new ProtoCell();
			bool bLeftDet = false;
			bool bRightDet = true;
			while ((line = sr.ReadLine()) != null)
			{
				lineNum++;
				line = line.Trim();
				if (string.IsNullOrEmpty(line))
				{
					continue;
				}

				if (!curProtoCell.bBegan)
				{
					if (!bRightDet)
					{
						bHasError = true;
						LineError(lineNum, "期待一个单独的'}'符号");
						break;
					}

					string[] aline = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
					if (aline[0] == "message")
					{
						if (aline.Length < 2)
						{
							bHasError = true;
							LineError(lineNum, "没有找到协议名");
							break;
						}
						if (lProtoNames.Contains(aline[1]))
						{
							bHasError = true;
							LineError(lineNum, "重复的协议名" + aline[1]);
							break;
						}
						curProtoCell.pktName = aline[1];
						lProtoNames.Add(curProtoCell.pktName);
						curProtoCell.id = CRC.CRC16(curProtoCell.pktName);
						lProtoCells.Add(curProtoCell);
						dProtoIDs.Add(curProtoCell.pktName, curProtoCell.id);
						curProtoCell.bBegan = true;
					}
					else if (aline[0] == "enum")
					{
						bHasError = true;
						LineError(lineNum, "枚举只能定义到message内部，暂不支持message外部的枚举" + line);
						break;
					}
					else
					{
						bHasError = true;
						LineError(lineNum, "找不到message关键字：" + line);
						break;
					}
				}
				else if (!bLeftDet)
				{
					if (line == "{")
					{
						bLeftDet = true;
						bRightDet = false;
					}
					else
					{
						bHasError = true;
						LineError(lineNum, "期待一个单独的'{'符号");
						break;
					}
				}
				else
				{
					if (line == "}")
					{
						if (curProtoCell.bEnumBegin)
						{
							curProtoCell.AddEnum(line);
							curProtoCell.bEnumBegin = false;
						}
						else
						{
							bRightDet = true;
							curProtoCell = new ProtoCell();
							bLeftDet = false;
						}
						continue;
					}
					else
					{
						string[] aline = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
						if (aline[0] == "enum" || curProtoCell.bEnumBegin)
						{
							if (aline[0] == "}")
							{
								curProtoCell.AddEnum(line.Trim());
								curProtoCell.bEnumBegin = false;
							}
							else
							{
								if (aline[0] == "enum")
									lEnumNames.Add(aline[1]);
								curProtoCell.bEnumBegin = true;
								curProtoCell.AddEnum(line.Trim());
							}
						}
						else
						{
							if (line.IndexOf(";") != line.Length - 1)
							{
								bHasError = true;
								LineError(lineNum, "需要以;结尾");
								break;
							}
							line = line.Substring(0, line.Length - 1);

							aline = line.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
							if (aline.Length == 1)
							{
								bHasError = true;
								LineError(lineNum, "找不到=");
								break;
							}

							string[] aparam = aline[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
							if (aparam.Length != 2)
							{
								bHasError = true;
								LineError(lineNum, "错误的属性");
								break;
							}
							int paramIndex = 0;
							int.TryParse(aline[1], out paramIndex);
							string serror = curProtoCell.AddParam(aparam[0], aparam[1], paramIndex);
							if (serror != null)
							{
								bHasError = true;
								LineError(lineNum, serror);
								break;
							}
						}
					}
				}
			}
			sr.Close();

			if (!bHasError)
			{
				string csfile = FormatPath(dInfos["out-cs"]);
				if (csfile.LastIndexOf(".cs") != csfile.Length - 3)
				{
					Console.WriteLine(csfile + " 不是一个合法的cs脚本");
					End();
					return;
				}

				FormatCSProtos(lProtoCells, csfile);
			}

		}

		private static void End()
		{
			Console.Write("Press any key to exit.");
			Console.ReadKey();
		}

		static string FormatCSProtos(List<ProtoCell> lProtos, string csfile)
		{
			Console.WriteLine($"generating csfile {csfile}");
			string sproto = "using System.Collections.Generic;"
				+ "\n" + "using System.IO;"
				+ "\n\n" + "namespace LibPacket"
				+ "\n{";
			foreach (ProtoCell cell in lProtos)
			{
				sproto += "\n\tpublic class " + cell.pktName + " : PktBase"
					+ "\n\t{"
					;
				int i = 0;
				foreach (string senum in cell.lEnumLines)
				{
					sproto += "\n\t\t";
					if (i == 0)
					{
						sproto += "public ";
					}
					else if (i != 1 && i != cell.lEnumLines.Count - 1)
					{
						sproto += "\t";
					}
					sproto += senum.Replace(";", ",");
					i++;
				}

				var sserialize = "";
				var sdeserialize = "";
				foreach (ProtoParamCell ppc in cell.lParams)
				{
					if (string.IsNullOrEmpty(sserialize))
					{
						sserialize = "\n\t\t\tthis.stream = ms;";
					}
					if (string.IsNullOrEmpty(sdeserialize))
					{
						sdeserialize = "\n\t\t\tthis.stream = ms;" +
								"\n\t\t\tvar tag = 0;" +
								"\n\t\t\twhile (reader.BaseStream.Position < reader.BaseStream.Length && (tag = reader.ReadInt32()) != 0)" +
								"\n\t\t\t{" +
								"\n\t\t\t\tswitch (tag)" +
								"\n\t\t\t\t{"
								;
					}
					string sparam = "\n\t\t" + "private " + ppc.paramType + " _" + ppc.paramName;
					sdeserialize += $"\n\t\t\t\t\tcase {ppc.paramIndex}:"
						+ "\n\t\t\t\t\t{";
					if (sparam.IndexOf("List") != -1)
					{
						sparam += " = new " + ppc.paramType + "();";
						sserialize += $"\n\t\t\tif ({ppc.paramName}.Count > 0)" +
							"\n\t\t\t{" +
							$"\n\t\t\t\twriter.Write({ppc.paramIndex});" +
							$"\n\t\t\t\twriter.Write({ppc.paramName}.Count);" +
							$"\n\t\t\t\tforeach (var __item in {ppc.paramName})" +
							"\n\t\t\t\t{"
							;
						sdeserialize += "\n\t\t\t\t\t\tvar count = reader.ReadInt32();" +
										"\n\t\t\t\t\t\tfor (var i = 0; i < count; i++)" +
										"\n\t\t\t\t\t\t{";
						var t = ppc.paramType.Substring(5, ppc.paramType.Length - 6);
						if (lProtoNames.Contains(t))
						{
							sserialize += "\n\t\t\t\t\tvar m = new MemoryStream();"
										+ "\n\t\t\t\t\t__item.Serialize(m);"
										+ "\n\t\t\t\t\tvar bs = m.ToArray();"
										+ "\n\t\t\t\t\twriter.Write(bs.Length);"
										+ "\n\t\t\t\t\twriter.Write(bs);";
							sdeserialize += $"\n\t\t\t\t\t\t\tvar __item = new {t}();"
										+ "\n\t\t\t\t\t\t\tvar c = reader.ReadInt32();"
										+ $"\n\t\t\t\t\t\t\t__item.Deserialize(new MemoryStream(reader.ReadBytes(c)));"
										;

						}
						else
						{
							sserialize += $"\n\t\t\t\twriter.Write({ppc.paramName});";
							sdeserialize += $"\n\t\t\t\t\tvar __item = reader.Read{GetReaderSuffix(t)}();";
						}
						sserialize += "\n\t\t\t\t}"
							+ "\n\t\t\t}";
						sdeserialize += $"\n\t\t\t\t\t\t\t{ppc.paramName}.Add(__item);"
							+ "\n\t\t\t\t\t\t}";
					}
					else if (lProtoNames.Contains(ppc.paramType))
					{
						sparam += " = null;";
						sserialize += $"\n\t\t\tif ({ppc.paramName} != null)" +
							"\n\t\t\t{" +
							$"\n\t\t\t\twriter.Write({ppc.paramIndex});" +
							"\n\t\t\t\tvar m = new MemoryStream();" +
							$"\n\t\t\t\t{ppc.paramName}.Serialize(m);" +
							"\n\t\t\t\tvar bs = m.ToArray();" +
							"\n\t\t\t\twriter.Write(bs.Length);" +
							"\n\t\t\t\twriter.Write(bs);" +
							"\n\t\t\t}"
							;
						sdeserialize += $"\n\t\t\t\t\t\t{ppc.paramName} = new {ppc.paramType}();"
										+ "\n\t\t\t\t\t\tvar c = reader.ReadInt32();"
										+ $"\n\t\t\t\t\t\t{ppc.paramName}.Deserialize(new MemoryStream(reader.ReadBytes(c)));"
									;
					}
					else if (ppc.paramType.ToLower() == "string")
					{
						sparam += " = \"\";";
						sserialize += $"\n\t\t\tif (!string.IsNullOrEmpty({ppc.paramName}))" +
							"\n\t\t\t{" +
							$"\n\t\t\t\twriter.Write({ppc.paramIndex});" +
							$"\n\t\t\t\twriter.Write({ppc.paramName});" +
							"\n\t\t\t}"
							;
						sdeserialize += $"\n\t\t\t\t\t\t{ppc.paramName} = reader.Read{GetReaderSuffix(ppc.paramType)}();";
					}
					else if (ppc.paramType == "bool")
					{
						sparam += " = false;";
						sserialize += $"\n\t\t\tif ({ppc.paramName})" +
							"\n\t\t\t{" +
							$"\n\t\t\t\twriter.Write({ppc.paramIndex});" +
							$"\n\t\t\t\twriter.Write({ppc.paramName});" +
							"\n\t\t\t}"
							;
						sdeserialize += $"\n\t\t\t\t\t\t{ppc.paramName} = reader.Read{GetReaderSuffix(ppc.paramType)}();";
					}
					else
					{
						sparam += " = default(" + ppc.paramType + ");";
						if (lEnumNames.Contains(ppc.paramType) || ppc.paramType.Contains("."))
						{
							sserialize += $"\n\t\t\tif ({ppc.paramName} != 0)" +
											"\n\t\t\t{" +
											$"\n\t\t\t\twriter.Write({ppc.paramIndex});" +
											$"\n\t\t\t\twriter.Write((int){ppc.paramName});" +
											"\n\t\t\t}"
											;
							sdeserialize += $"\n\t\t\t\t\t\t{ppc.paramName} = ({ppc.paramType})reader.ReadInt32();";
						}
						else
						{
							sserialize += $"\n\t\t\tif ({ppc.paramName} != 0)" +
											"\n\t\t\t{" +
											$"\n\t\t\t\twriter.Write({ppc.paramIndex});" +
											$"\n\t\t\t\twriter.Write({ppc.paramName});" +
											"\n\t\t\t}"
											;
							sdeserialize += $"\n\t\t\t\t\t\t{ppc.paramName} = reader.Read{GetReaderSuffix(ppc.paramType)}();";
						}
					}
					sdeserialize += "\n\t\t\t\t\t\tbreak;"
						+ "\n\t\t\t\t\t}";
					sparam += "\n\t\t" + "public " + ppc.paramType + " " + ppc.paramName
						+ "\n\t\t" + "{"
						+ "\n\t\t\t" + "get{" + "return _" + ppc.paramName + ";}"
						+ "\n\t\t\t" + "set{" + "_" + ppc.paramName + " = value;}"
						+ "\n\t\t" + "}";
					sproto += sparam;
					sproto += "\n";
				}

				if (!string.IsNullOrEmpty(sdeserialize))
				{
					sdeserialize += "\n\t\t\t\t}" +
									"\n\t\t\t}";
				}
				sproto += "\n\t\tpublic override void Serialize(MemoryStream ms)" +
					"\n\t\t{" +
					sserialize +
					"\n\t\t}" +
					"\n\t\tpublic override void Deserialize(MemoryStream ms)" +
					"\n\t\t{" +
					sdeserialize +
					"\n\t\t}"
					;
				sproto += "\n\t}\n";
			}
			sproto += "\n}";

			StreamWriter sw = new StreamWriter(csfile);
			sw.Write(sproto);
			sw.Close();

			return sproto;
		}

		private static string GetReaderSuffix(string t)
		{
			switch (t)
			{
				case "int":
				case "Int32":
					return "Int32";
				case "float":
				case "Single":
					return "Single";
				case "string":
				case "String":
					return "String";
				case "long":
				case "Int64":
					return "Int64";
				case "bool":
					return "Boolean";
				case "byte":
					return "Byte";
				case "char":
					return "Char";
			}
			return t;
		}

		static string FormatPath(string sinput)
		{
			string sCurrentDir = Directory.GetCurrentDirectory();
			DirectoryInfo dir = new DirectoryInfo(sCurrentDir);
			string sfile = sinput;
			if (sinput.Contains(":")) return sinput;
			int indexOf = 0;
			while (indexOf != -1)
			{
				int iOf = sfile.IndexOf("../", indexOf);
				if (iOf == -1)
				{
					iOf = sfile.IndexOf("..\\", indexOf);
				}
				indexOf = iOf + 1;
				if (indexOf <= 0)
				{
					break;
				}
				dir = dir.Parent;
			}
			sfile = sfile.Replace("../", "");
			sfile = sfile.Replace("..\\", "");
			string sreadfile = dir.FullName + "\\" + sfile;
			return sreadfile;
		}

		static void LineError(int lineNum, string sError)
		{
			if (string.IsNullOrEmpty(sError))
			{
				return;
			}
			Console.BackgroundColor = ConsoleColor.Red;
			Console.ForegroundColor = ConsoleColor.Black;
			Console.WriteLine("Line " + lineNum + " error:" + sError);
			Console.BackgroundColor = ConsoleColor.Black;
			Console.ForegroundColor = ConsoleColor.White;
		}

		static void printHelp()
		{
			Console.WriteLine("file:proto file path");
			Console.WriteLine("out-cs:cs proto file out path");
			Console.WriteLine("out-lua:lua proto file out path");
		}
	}

	class ProtoCell
	{
		public int id = 0;
		public string pktName;
		public List<ProtoParamCell> lParams = new List<ProtoParamCell>();
		public List<string> lEnumLines = new List<string>();
		List<int> lIndexes = new List<int>();
		public bool bEnumBegin;

		public bool bBegan;

		static bool TestType(string stype)
		{
			// 			if(stype.IndexOf("List<") == 0 && Program.lProtoNames.Contains(stype.Substring(5, stype.Length - 6)))
			// 			{
			// 				return true;
			// 			}
			// 			if (stype != "int" && stype != "string" && stype != "long" && stype != "float" && stype != "double"
			// 				&& stype != "List<int>" && stype != "List<string>" && stype != "List<long>" && stype != "List<float>" && stype != "List<double>"
			// 				&& !Program.lProtoNames.Contains(stype))
			// 			{
			// 				return false;
			// 			}
			return true;
		}

		public string AddEnum(string enumLine)
		{
			if (lEnumLines.Contains(enumLine.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0]))
			{
				return "重复的枚举值:" + enumLine;
			}
			lEnumLines.Add(enumLine.Trim());
			return null;
		}
		public string AddParam(string sparamtype, string sparamname, int paramindex)
		{
			if (!TestType(sparamtype))
			{
				return "不支持的属性" + sparamtype;
			}
			if (lIndexes.Contains(paramindex))
			{
				return "属性序号重复" + paramindex;
			}
			ProtoParamCell ppc = new ProtoParamCell();
			ppc.paramType = sparamtype;
			ppc.paramName = sparamname;
			ppc.paramIndex = paramindex;
			lParams.Add(ppc);
			lIndexes.Add(paramindex);
			return null;
		}
	}
	class ProtoParamCell
	{
		public string paramType;
		public string paramName;
		public int paramIndex;
	}
}
