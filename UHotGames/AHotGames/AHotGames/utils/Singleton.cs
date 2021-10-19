
using System;
using System.Linq;
using System.Collections.Generic;

public class Singleton<T>
	where T : class, new()
{
	private static T sInstance = null;
	public static T Instance
	{
		get
		{
			if (sInstance == null)
			{
				sInstance = new T();
			}
			return sInstance;
		}
	}
	public static void Dispose()
	{
		sInstance = null;
	}
}

public abstract class LoaderBase
{
	public abstract void OnLoadContent(string sContent);
	public abstract List<DataBase> All { get; }
}
public abstract class SingletonDataLoader<T, DataT> : LoaderBase
	where T : class, new()
	where DataT : DataBase, new()
{
	private static T sInstance = null;
	public static T Instance
	{
		get
		{
			if (sInstance == null)
			{
				sInstance = new T();
			}
			return sInstance;
		}
	}

	protected List<string[]> m_Datas = new List<string[]>();
	protected Dictionary<int, DataBase> dData = new Dictionary<int, DataBase>();
	protected List<DataT> lDataList = new List<DataT>();
	public virtual void Init()
	{

	}
	protected void Load(string sPath)
	{
		CSCSV.OpenCSVFile(ref m_Datas, sPath);
	}
	public void LoadContent(string sContent)
	{
		istart = 0;
		dData.Clear();
		lDataList.Clear();
		m_Datas.Clear();
		CSCSV.ParseFile(ref m_Datas, sContent);
	}
	protected int istart = 0;
	protected bool IsRowValid(int irow)
	{
		istart = 0;
		return GetIntValue(irow, istart) > 0;
	}
	public virtual void OnClear()
	{
		dData.Clear();
		lDataList.Clear();
		m_Datas.Clear();
	}
	public virtual void OnAddData(DataT data)
	{
		dData.Add(data.id, data);
		_AllData = null;
	}
	public virtual void OnAddDataToList(DataT data)
	{
		lDataList.Add(data);
	}
	public virtual DataT OnGetData(int id)
	{
		if (dData.ContainsKey(id))
		{
			return dData[id] as DataT;
		}
		return null;
	}
	public virtual List<DataT> OnGetDataList(int id)
	{
		List<DataT> lResult = new List<DataT>();
		foreach (DataT d in lDataList)
		{
			if (d.id == id)
			{
				lResult.Add(d);
			}
		}
		return lResult;
	}
	public override List<DataBase> All
	{
		get
		{
			return dData.Values.ToList();
		}
	}
	private List<DataT> _AllData;
	public virtual List<DataT> AllData
	{
		get
		{
			if (_AllData == null)
			{
				_AllData = new List<DataT>();
				foreach (DataBase db in dData.Values)
				{
					if (db is DataT)
					{
						_AllData.Add(db as DataT);
					}
				}
			}
			return _AllData;
		}
	}
	public int GetIntValue(int line, int pos)
	{
		return typeParser.intParse(m_Datas[line][pos].ToString());
	}
	public bool GetBoolValue(int line, int pos)
	{
		return GetStringValue(line, pos) == "1";
	}
	public Int64 GetInt64Value(int line, int pos)
	{
		return typeParser.Int64Parse(m_Datas[line][pos].ToString());
	}
	public string GetStringValue(int line, int pos)
	{
		return m_Datas[line][pos].ToString().Replace("\"", "");
	}
	public float GetFloatValue(int line, int pos)
	{
		return typeParser.floatParse(m_Datas[line][pos].ToString());
	}
	public double GetDoubleValue(int line, int pos)
	{
		return typeParser.doubleParse(m_Datas[line][pos].ToString());
	}

	public string[] ToLine(string sContent)
	{
		return sContent.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
	}
	public string[] ToCell(string sContent)
	{
		return sContent.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
	}
}
public class DataBase
{
	public int id;
}
