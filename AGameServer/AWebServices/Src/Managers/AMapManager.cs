using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class AMapManager : Singleton<AMapManager>
{
    public bool CheckCanMoveTo(int x, int y)
    {
        //to do, map check
        return true;
    }
    public MonsterCell OnGetMonster(int x, int y)
    {
        //to do, generate monster
        return null;
    }
}
[Serializable]
public class MonsterCell
{
    public int Level;
    public int Model;
}