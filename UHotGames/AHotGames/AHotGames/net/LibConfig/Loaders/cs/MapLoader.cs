public class MapData : DataBase
{
	public string Name;//名字
	public string Desc;//描述
	public string MapClass;//地图类
	public string Prefab;//预设位置
	public int MaxNum;//最大人数
	public int MultiInstance;//可多个
	public string wuxing;//五行
}
public class MapLoader : SingletonDataLoader<MapLoader, MapData>
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

			var data = new MapData();
			data.id = id;
			data.Name = GetStringValue(i,istart++);
			data.Desc = GetStringValue(i,istart++);
			data.MapClass = GetStringValue(i,istart++);
			data.Prefab = GetStringValue(i,istart++);
			data.MaxNum = GetIntValue(i,istart++);
			data.MultiInstance = GetIntValue(i,istart++);
			data.wuxing = GetStringValue(i,istart++);
			OnAddData(data);
		}
	}
}