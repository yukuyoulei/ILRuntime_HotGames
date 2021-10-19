public class JinData : DataBase
{
	public int Name;//关卡ID
}
public class JinLoader : SingletonDataLoader<JinLoader, JinData>
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

			var data = new JinData();
			data.id = id;
			data.Name = GetIntValue(i,istart++);
			OnAddData(data);
		}
	}
}