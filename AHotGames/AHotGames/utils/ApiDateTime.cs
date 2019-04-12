using System;

public class ApiDateTime
{
    /// <summary>
    /// 封装这个函数是为了以后做国外版本时修改时间参数
    /// </summary>
    private static DateTime _Now = new DateTime(1969,1,1);
    public static DateTime Now
    {
        get
        {
            if (_Now.Year != 1969)
            {
                return _Now.AddSeconds((DateTime.Now - startTime).TotalSeconds);
            }
            return DateTime.Now;
        }
    }
    private static DateTime startTime;
    public static void SetTime(int secondsFromStart)
    {
        startTime = DateTime.Now;
        _Now = TimeFromSeconds(secondsFromStart);
    }

    public static int _StartMonday030Seconds;
    public static int StartMonday030Seconds()
    {
        if (_StartMonday030Seconds == 0)
        {
            _StartMonday030Seconds = SecondsFromBegin(time_monday030);
        }
        return _StartMonday030Seconds;
    }

    private static int _weekly_seconds;
    public static int weekly_seconds
    {
        get
        {
            if (_weekly_seconds == 0)
            {
                _weekly_seconds = 7 * 24 * 3600;
            }
            return _weekly_seconds;
        }
    }
    public static int GetWeekArg()
    {
        int secondSep = (int)(ApiDateTime.Now - time_monday030).TotalSeconds;
        return (int)(secondSep / ApiDateTime.weekly_seconds);
    }

    private static readonly DateTime time_monday030 = new DateTime(2016, 1, 4, 3, 0, 0);
    private static readonly DateTime time_begin = new DateTime(2018, 10, 1);
    public static int SecondsFromBegin()
    {
        return (int)(ApiDateTime.Now - time_begin).TotalSeconds;
    }
    public static int SecondsFromBeginUTC()
    {
        return (int)(DateTime.UtcNow - time_begin).TotalSeconds;
    }
    public static DateTime TimeFromSeconds(int seconds)
    {
        if (seconds < 0)
        {
            seconds = 0;
        }
        DateTime dt = time_begin;
        return dt.AddSeconds(seconds);
    }

    public static int SecondsFromBegin(DateTime dateTime)
    {
        return (int)(dateTime - time_begin).TotalSeconds;
    }

    public static int FirstSecondOfWeek()
    {
        const int FOURDAY_SECONDS = 4 * 24 * 3600;
        const int WEEK_SECONDS = 7 * 24 * 3600;
        if (SecondsFromBegin() < FOURDAY_SECONDS)
        {
            return 0;
        }
        else
        {
            return (SecondsFromBegin() - FOURDAY_SECONDS) / WEEK_SECONDS * WEEK_SECONDS + FOURDAY_SECONDS;
        }
    }

    //返回：判断现在与参数是否在同周
    //参数iSeconds：任意的秒数
    public static bool IsSameWeek(int iSeconds)
    {
        const int THREEDAY_SECONDS = 3 * 24 * 3600;
        const int WEEK_SECONDS = 7 * 24 * 3600;
        iSeconds += THREEDAY_SECONDS;
        int iSecondsOther = SecondsFromBegin() + THREEDAY_SECONDS;
        return iSeconds / WEEK_SECONDS == iSecondsOther / WEEK_SECONDS;
    }

    //判断两个时间点是否在同周  added by haoshubin at 2012-11-17
    public static bool IsSameWeek(int iSeconds1, int iSeconds2)
    {
        const int THREEDAY_SECONDS = 3 * 24 * 3600;
        const int WEEK_SECONDS = 7 * 24 * 3600;
        iSeconds1 += THREEDAY_SECONDS;
        iSeconds2 += THREEDAY_SECONDS;
        return iSeconds1 / WEEK_SECONDS == iSeconds2 / WEEK_SECONDS;
    }

    //返回：判断现在与参数是否在同天
    //参数iSeconds：任意的秒数
    public static bool IsSameDay(int iSeconds, int deltaSeconds = 10800)
    {
        const int DAY_SECONDS = 24 * 3600;
        return (SecondsFromBegin() - deltaSeconds) / DAY_SECONDS == (iSeconds - deltaSeconds) / DAY_SECONDS;
    }

    //返回：判断现在到参数超过1天
    //参数iSeconds：任意的秒数
    public static bool IsMoreThanOneDay(int iSeconds, int deltaSeconds = 10800)
    {
        const int DAY_SECONDS = 24 * 3600;
        return (SecondsFromBegin() - deltaSeconds) / DAY_SECONDS == (iSeconds - deltaSeconds + DAY_SECONDS) / DAY_SECONDS || (SecondsFromBegin() - deltaSeconds) / DAY_SECONDS == (iSeconds - deltaSeconds) / DAY_SECONDS;
    }

    public static int FirstSecondOfTwoDays()
    {
        const int TWODAY_SECONDS = 2 * 24 * 3600;
        return (SecondsFromBegin()) / TWODAY_SECONDS * TWODAY_SECONDS;
    }

    //以两天为一个时间间隔
    //返回：判断现在与参数是否在同一个时间间隔
    //参数iSeconds：任意的秒数
    public static bool IsSameTwoDays(int iSeconds)
    {
        const int TWODAY_SECONDS = 2 * 24 * 3600;
        return iSeconds / TWODAY_SECONDS == SecondsFromBegin() / TWODAY_SECONDS;
    }

    //以两天为一个时间间隔
    //返回：判断现在是否在一个时间间隔的前五分钟
    public static bool IsFiveMinuteBeforeOneDay()
    {
        const int TWODAY_SECONDS = 2 * 24 * 3600;
        const int FIVEMINUTE_SECONDS = 5 * 60;
        return SecondsFromBegin() / TWODAY_SECONDS != (SecondsFromBegin() + FIVEMINUTE_SECONDS) / TWODAY_SECONDS;
    }

    //返回现在距今天零点已过去多少秒
    public static int SecondsFromToday()
    {
        DateTime now = DateTime.Now;
        return now.Hour * 60 * 60 + now.Minute * 60 + now.Second;
    }

    //返回指定时间距当天零点已过去多少秒
    public static int SecondsFromToday(DateTime dateTime)
    {
        return dateTime.Hour * 60 * 60 + dateTime.Minute * 60 + dateTime.Second;
    }

    //返回是否在两天内
    public static bool IsBetweenTwoDays(int iSeconds)
    {
        const int TWODAY_SECONDS = 2 * 24 * 3600;
        return Math.Abs((int)(iSeconds - SecondsFromBegin())) <= TWODAY_SECONDS;
    }

    //返回是否在连续的两天内
    public static bool IsInContinuousTwoDays(int iSeconds)
    {
        const int ONEDAY_SECONDS = 24 * 3600;
        return Math.Abs((int)(iSeconds / ONEDAY_SECONDS - SecondsFromBegin() / ONEDAY_SECONDS)) == 1;
    }
}
