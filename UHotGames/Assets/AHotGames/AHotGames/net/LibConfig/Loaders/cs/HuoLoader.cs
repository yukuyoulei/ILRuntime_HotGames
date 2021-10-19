public class HuoData : DataBase
{
	public int Name;//关卡ID
}
public class HuoLoader : SingletonDataLoader<HuoLoader, HuoData>
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

			var data = new HuoData();
			data.id = id;
			data.Name = GetIntValue(i,istart++);
			OnAddData(data);
		}
	}
}