using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace AWebServices.asmx
{
    /// <summary>
    /// Wiki 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class Wiki : System.Web.Services.WebService
    {
        public const string TableName = "twiki";
        public static string dbConnect = ConfigurationManager.AppSettings["wikiconnect"];
        public static string dbName = ConfigurationManager.AppSettings["wikidbname"];

        public static ADBAccessor adb
        {
            get
            {
                var db = ADBManager.Get(dbConnect, dbName);
                if (db == null)
                {
                    throw new Exception("cannot connect to database");
                }
                return db;
            }
        }
        void SendError(int error)
        {
            SendError(error.ToString());
        }
        void SendError(params string[] values)
        {
            Context.Response.Write(AWebServerUtils.OnGetJsonError(values));
        }


        static int _totalcount = -1;
        static int totalcount
        {
            get
            {
                if (_totalcount == -1)
                {
                    var r = adb.FindOneData(TableName, ADBAccessor.filter_eq("t", "count")
                        , ADBAccessor.projections("count"));
                    if (r != null && r.Contains("count"))
                    {
                        _totalcount = r["count"].AsInt32;
                    }
                    else
                    {
                        _totalcount = 0;
                        adb.UpdateOneData(TableName
                            , ADBAccessor.filter_eq("t", "count")
                            , ADBAccessor.updates_build(ADBAccessor.update("t", "count")
                                , ADBAccessor.update("count", _totalcount))
                            , true);
                    }
                }
                return _totalcount;
            }
            set
            {
                _totalcount = value;
                adb.UpdateOneData(TableName
                    , ADBAccessor.filter_eq("t", "count")
                    , ADBAccessor.updates_build(ADBAccessor.update("t", "count")
                        , ADBAccessor.update("count", _totalcount))
                    , true);
            }
        }
        [WebMethod]
        public void wikiadd(string subject, string content)
        {
            var id = ++totalcount;
            var t = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            adb.UpdateOneData(TableName
                , ADBAccessor.filter_eq("i", id)
                , ADBAccessor.updates_build(
                      ADBAccessor.update("i", id)
                    , ADBAccessor.update("s", subject)
                    , ADBAccessor.update("c", content)
                    , ADBAccessor.update("t", t)
                    )
                , true);
            SendError("s", subject, "c", content, "t", t);
        }
        class AWikiCell
        {
            public int id;
            public string subject;
            public string content;
            public string time;
            public string lastmodifytime;

            internal string[] ToArray()
            {
                var l = new List<string>();
                l.Add("i");
                l.Add(id.ToString());
                l.Add("s");
                l.Add(subject);
                l.Add("c");
                l.Add(HttpUtility.UrlEncode(content));
                l.Add("t");
                l.Add(time);
                l.Add("lt");
                l.Add(lastmodifytime);
                return l.ToArray();
            }
        }
        static Dictionary<int, AWikiCell> dWikis = new Dictionary<int, AWikiCell>();
        [WebMethod]
        public void wikilist(string page, string count)
        {
            var ipage = typeParser.intParse(page);
            var icount = typeParser.intParse(count);
            List<List<string>> lr = new List<List<string>>();
            for (var i = (ipage + 1) * icount; i > ipage * icount; i--)
            {
                AWikiCell cell = null;
                if (dWikis.ContainsKey(i))
                {
                    cell = dWikis[i];
                }
                else
                {
                    var r = adb.FindOneData(TableName
                        , ADBAccessor.filter_eq("i", i)
                        , ADBAccessor.projections("i", "s", "t"));
                    if (r == null)
                    {
                        continue;
                    }
                    cell = new AWikiCell();
                    cell.id = r["i"].AsInt32;
                    cell.subject = r["s"].AsString;
                    cell.time = r["t"].AsString;
                }

                var l = new List<string>();
                l.Add("i");
                l.Add(cell.id.ToString());
                l.Add("s");
                l.Add(cell.subject);
                l.Add("t");
                l.Add(cell.time);
                lr.Add(l);
            }
            SendError("r", AWebServerUtils.ToJsonArray(lr.ToArray()));
        }
        AWikiCell OnGetWiki(string id)
        {
            var i = typeParser.intParse(id);
            if (dWikis.ContainsKey(i))
            {
                return dWikis[i];
            }
            return null;
        }
        AWikiCell OnLoadWikiCellWithContent(string id)
        {
            var i = typeParser.intParse(id);
            AWikiCell cell = OnGetWiki(id);
            if (cell == null || string.IsNullOrEmpty(cell.content))
            {
                var r = adb.FindOneData(TableName, ADBAccessor.filter_eq("i", i)
                    , ADBAccessor.projections("s", "c", "t"));
                if (r == null)
                {
                    SendError("1");
                    return null;
                }
                if (cell == null)
                {
                    cell = new AWikiCell();
                }
                cell.id = i;
                cell.content = r["c"].AsString;
                cell.time = r["t"].AsString;
                cell.subject = r["s"].AsString;
                if (r.Contains("lt"))
                {
                    cell.lastmodifytime = r["lt"].AsString;
                }
                if (dWikis.ContainsKey(cell.id))
                {
                    dWikis[cell.id] = cell;
                }
                else
                {
                    dWikis.Add(cell.id, cell);
                }
            }
            return cell;
        }
        AWikiCell OnLoadWikiCellWithoutContent(string id)
        {
            var i = typeParser.intParse(id);
            AWikiCell cell = OnGetWiki(id);
            if (cell == null)
            {
                var r = adb.FindOneData(TableName, ADBAccessor.filter_eq("i", i)
                    , ADBAccessor.projections("i", "s", "t"));
                if (r == null)
                {
                    SendError("1");
                    return null;
                }
                cell = new AWikiCell();
                cell.id = i;
                cell.time = r["t"].AsString;
                cell.subject = r["s"].AsString;
                dWikis.Add(cell.id, cell);
            }
            return cell;
        }
        [WebMethod]
        public void wikiread(string id)
        {
            AWikiCell cell = OnLoadWikiCellWithContent(id);
            if (cell == null)
            {
                SendError("1");
                return;
            }
            SendError(cell.ToArray());
        }
        [WebMethod]
        public void wikimodify(string id, string content)
        {
            AWikiCell cell = OnLoadWikiCellWithoutContent(id);
            if (cell == null)
            {
                SendError("1");
                return;
            }
            cell.content = content;
            var t = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            adb.UpdateOneData(TableName
                , ADBAccessor.filter_eq("i", cell.id)
                , ADBAccessor.updates_build(
                      ADBAccessor.update("c", content)
                    , ADBAccessor.update("lt", t)
                    ));
            SendError(cell.ToArray());
        }
    }
}
