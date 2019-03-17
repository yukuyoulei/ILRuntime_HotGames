using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class UMRemoteDataManager : Singleton<UMRemoteDataManager>
{
    private Dictionary<Type, UMRemoteDataBase> dDatas = new Dictionary<Type, UMRemoteDataBase>();
    public void OnAdd<T>(T t) where T : UMRemoteDataBase
    {
        var tp = t.GetType();
        if (dDatas.ContainsKey(tp))
        {
            dDatas.Remove(tp);
        }
        dDatas.Add(tp, t);
    }
    public T OnGet<T>() where T : UMRemoteDataBase
    {
        var tp = typeof(T);
        if (dDatas.ContainsKey(tp))
        {
            return dDatas[tp] as T;
        }
        return null;
    }
}
public abstract class UMRemoteDataBase
{
    protected Dictionary<string, string> dValues = new Dictionary<string, string>();
    public void SetValue(string key, string value) { if (dValues.ContainsKey(key)) dValues[key] = value; dValues.Add(key, value); }
    protected string GetStringValue(string key) { return dValues.ContainsKey(key) ? dValues[key] : ""; }
    protected int GetIntValue(string key) { return dValues.ContainsKey(key) ? typeParser.intParse(dValues[key]) : 0; }
    protected Int64 GetInt64Value(string key) { return dValues.ContainsKey(key) ? typeParser.Int64Parse(dValues[key]) : 0; }
    public void OnFormat(JObject jres)
    {
        if (jres != null)
        {
            if (jres.ContainsKey("err") && jres["err"].ToString() == "0")
            {
                JsonFormate(jres);
            }
            else if (jres.ContainsKey("err") && jres["err"].ToString() != "0")
            {
                UIAlert.Show("解析Json结果出错：" + jres["err"]);
            }
        }
    }
    public abstract void JsonFormate(JObject jres);
}
public class UMRemoteAvatarData : UMRemoteDataBase
{
    public static UMRemoteAvatarData data { get { return UMRemoteDataManager.Instance.OnGet<UMRemoteAvatarData>(); } }
    private string[] dataValues = new string[] { "name", "gold", "sex", "Money", "head", "dcc", "dcct" };
    public string AvatarName { get { return GetStringValue("name"); } }
    public long AvatarGold { get { return GetInt64Value("gold"); } }
    public int AvatarSex { get { return GetIntValue("sex"); } }
    public long AvatarMoney { get { return GetInt64Value("Money"); } }
    public int AvatarHead { get { return GetIntValue("head"); } }
    public int DailyCheckCount { get { return GetIntValue("dcc"); } }
    public long LastDailyCheckTime { get { return GetInt64Value("dcct"); } }

    public override void JsonFormate(JObject jres)
    {
        foreach (var v in dataValues)
        {
            if (jres.ContainsKey(v))
            {
                SetValue(v, jres[v].ToString());
            }
        }
    }
}
