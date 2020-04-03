public class Items:DataBase
{
	public string Name;//道具名
	public string Desc;//描述
	public string param;//属性
	public int dropID;//掉落ID
}
public class ItemsLoader : SingletonDataLoader<ItemsLoader, Items>
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

			var data = new Items();
			data.id = id;
			data.Name = GetStringValue(i,istart++);
			data.Desc = GetStringValue(i,istart++);
			data.param = GetStringValue(i,istart++);
			data.dropID = GetIntValue(i,istart++);
			OnAddData(data);
		}
	}
}