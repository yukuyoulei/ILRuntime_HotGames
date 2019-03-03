using System;

public class SameDay
{
	private const UInt32 DAY_TOTALSECONDS = 24 * 3600;

	//参数time：创建日定时器时间。
	public SameDay(DateTime time)
	{
		DateTime beginTime = new DateTime(1970, 1, 1);
		if (0 > DateTime.Compare(time, beginTime))
		{
			//SystemLogger.Log.Error("SameDay's construction failed: Param is before 1970.1.1");
			m_iSeconds = 0;
		}
		else
		{
			m_iSeconds = (UInt32)(time - beginTime).TotalSeconds;
		}
	}

	//参数iSeconds：创建日定时器时间（从1970.1.1日午夜开始的秒）
	public SameDay(UInt32 iSeconds)
	{
		m_iSeconds = iSeconds;
	}

	//参数now：需要比较是否为同一天的时间。
	//参数iDemarcation：每日分界点，以秒为单位。
	public bool CheckSameDay(DateTime now, UInt32 iDemarcation)
	{
		DateTime beginTime = new DateTime(1970, 1, 1);
		if (0 > DateTime.Compare(now, beginTime))
		{
			//SystemLogger.Log.Error("SameDay.CheckSameDay failed: Param is before 1970.1.1");
			return false;
		}
		else
		{
			UInt32 iSecondsToNow = (UInt32)(now - beginTime).TotalSeconds;

			UInt32 iDays1 = (iSecondsToNow + DAY_TOTALSECONDS - iDemarcation) / DAY_TOTALSECONDS;
			UInt32 iDays = (m_iSeconds + DAY_TOTALSECONDS - iDemarcation) / DAY_TOTALSECONDS;
			if (iDays < iDays1)
			{
				m_iSeconds = iSecondsToNow;
			}
			return iDays1 == iDays;
		}
	}

	//获得上次日定时器执行的时间（从1970.1.1日午夜开始的秒）
	public UInt32 Seconds
	{
		get
		{
			return m_iSeconds;
		}
	}

	private UInt32 m_iSeconds;
}
