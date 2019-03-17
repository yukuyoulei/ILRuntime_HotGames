public class Singleton<T>
	where T : class,new()
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
}
