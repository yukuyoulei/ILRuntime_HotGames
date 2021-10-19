using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
namespace LibNet.Uitlity
{
	public class CExcelRow
	{
		public CExcelRow(UInt32 iColumNum)
		{
			m_vCellData = new String[iColumNum];
		}
		public UInt32 CellNum() { return (UInt32)m_vCellData.Length; }
		public String StringAt(UInt32 iIndex) { return m_vCellData[iIndex]; }
		public Int32 Atoi(UInt32 iIndex)
		{
			return Int32.Parse(m_vCellData[iIndex]);
		}
		public float Atof(UInt32 iIndex)
		{
			return float.Parse(m_vCellData[iIndex]);
		}
		public void SetAt(UInt32 iIndex, String sValue)
		{
			if (iIndex >= 0 && iIndex < m_vCellData.Length)
			{
				m_vCellData[iIndex] = sValue;
			}
		}
		public void SetAt(UInt32 iIndex, Int32 iValue)
		{
			SetAt(iIndex, iValue.ToString());
		}
		public void SetAt(UInt32 iIndex, float fValue)
		{
			SetAt(iIndex, fValue.ToString());
		}
		private String[] m_vCellData;
	}

	public class CExcelReader
	{
		public bool ReadFromFile(String sFileFullPath)
		{
			XmlDocument doc = new XmlDocument();
			XmlTextReader reader = new XmlTextReader(sFileFullPath);
			doc.Load(reader);
			reader.Close();
			//XmlNode tableNode = doc.SelectSingleNode("/Workbook/Worksheet/Table[1]");
			XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
			nsmgr.AddNamespace("excel", "urn:schemas-microsoft-com:office:spreadsheet");
			XmlNode tableNode = doc.SelectSingleNode("/excel:Workbook/excel:Worksheet/excel:Table[1]", nsmgr);
			if (tableNode == null) return false;

			m_iColumnNum = UInt32.Parse(tableNode.Attributes["ss:ExpandedColumnCount"].Value);
			if (m_iColumnNum == 0) return false;

			List<CExcelRow> vRows = new List<CExcelRow>();
			m_iRowNum = 0;
			foreach (XmlNode rowNode in tableNode.ChildNodes)
			{
				if (rowNode.Name != "Row") continue;
				CExcelRow row = new CExcelRow(m_iColumnNum);
				UInt32 iCurrentCellIndex = 0;
				foreach (XmlNode cellNode in rowNode.ChildNodes)
				{
					XmlAttribute attr = (XmlAttribute)cellNode.Attributes.GetNamedItem("ss:Index");
					if (null != attr)
					{
						iCurrentCellIndex = UInt32.Parse(attr.Value) - 1;
					}
					XmlNode dataNode = cellNode.SelectSingleNode("./excel:Data", nsmgr);
					if (null != dataNode)
					{
						row.SetAt(iCurrentCellIndex, dataNode.InnerText);
					}
					iCurrentCellIndex++;
				}
				vRows.Add(row);
				m_iRowNum++;
			}
			m_vRows = vRows.ToArray();
			return true;
		}
		public UInt32 RowNum()
		{
			return m_iRowNum;
		}
		public UInt32 ColumnNum()
		{
			return m_iColumnNum;
		}
		public CExcelRow RowAt(UInt32 iIndex)
		{
			return m_vRows[iIndex];
		}

		private CExcelRow[] m_vRows;
		private UInt32 m_iRowNum;
		private UInt32 m_iColumnNum;
	};

}
