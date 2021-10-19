public class PaymentData : DataBase
{
	public string Name;//名字
	public string Desc;//描述
	public int Price;//价格
	public string AppStoreID;//AppStoreID
}
public class PaymentLoader : SingletonDataLoader<PaymentLoader, PaymentData>
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

			var data = new PaymentData();
			data.id = id;
			data.Name = GetStringValue(i,istart++);
			data.Desc = GetStringValue(i,istart++);
			data.Price = GetIntValue(i,istart++);
			data.AppStoreID = GetStringValue(i,istart++);
			OnAddData(data);
		}
	}
}