public class Map:DataBase
{
	public string Name;//名字
	public string Desc;//描述
	public string Prefab;//预设位置
}
public class MapLoader : SingletonDataLoader<MapLoader, Map>
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

			var data = new Map();
			data.id = id;
			data.Name = GetStringValue(i,istart++);
			data.Desc = GetStringValue(i,istart++);
			data.Prefab = GetStringValue(i,istart++);
			OnAddData(data);
		}
	}
}