public class DailyCheckData : DataBase
{
	public string Desc;//描述
	public int itemID;//道具ID
	public int itemCount;//道具数量
}
public class DailyCheckLoader : SingletonDataLoader<DailyCheckLoader, DailyCheckData>
{
	public override void OnLoadContent(string content)
	{
		LoadContent(content);

		for (var i = 3; i < m_Datas.Count; i++)
		{
			istart = 0;
			var id = GetIntValue(i, istart++);
			if (id < 1)
			{
			continue;
			}

			var data = new DailyCheckData();
			data.id = id;
			data.Desc = GetStringValue(i,istart++);
			data.itemID = GetIntValue(i,istart++);
			data.itemCount = GetIntValue(i,istart++);
			OnAddData(data);
		}
	}
}