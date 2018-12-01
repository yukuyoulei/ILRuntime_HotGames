using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 
/// Written by FS
/// 
/// </summary>

using LlkList = System.Collections.Generic.List<LlkCell>;
public class GameLianliankan : AGameBase
{
    protected override void InitComponents()
    {
        Reinit();

        gameObj.AddComponent<UOnGUIer>().onOnGUI = () =>
        {
            OnGUI();
        };
        gameObj.AddComponent<UUpdater>().onUpdate = () => { Update(); };
    }
    private const int size = 30;

    private int maxRow = 16;
    private int maxCol = 12;
    private int cellNum;
    private int clearedCellNum;
    private int level = 1;

    private List<LlkList> llkCells = new List<LlkList>();
    private LlkList lSelectedCells = new LlkList();

    private int iDiff = 3;

    void OnGUI()
    {
        GUILayout.Label("等级：" + level + "  倒计时：" + ((int)(lastSecond)).ToString());

        GUI.Box(new Rect(posOffset, posOffset, maxCol * size, maxRow * size), "");

        foreach (LlkList ll in llkCells)
        {
            foreach (LlkCell lc in ll)
            {
                if (lc.bCleared)
                {
                    continue;
                }

                int lcx = lc.x;
                int lcy = lc.y;
                int lcsize = size;
                if (lc.bSelected)
                {
                    GUI.Box(new Rect(lc.x - 2, lc.y - 2, size + 4, size + 4), "");
                    lcx += 2;
                    lcy += 2;
                    lcsize -= 4;
                }

                if (GUI.Button(new Rect(lcx, lcy, lcsize, lcsize), lc.iType.ToString()))
                {
                    if (bGameOver || bLevelUp)
                    {
                        return;
                    }
                    SetSelect(lc);
                }
            }
        }

        if (bGameOver && GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 30, 200, 60), "Game Over!"))
        {
            Reinit();
        }
        if (bLevelUp && GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 30, 200, 60), "Level Up!"))
        {
            iDiff++;
            level++;
            Reinit();
        }
    }

    private const int posOffset = 50;
    private const int fullSeconds = 60;
    private float lastSecond = fullSeconds;
    private void Reinit()
    {
        bGameOver = false;
        bLevelUp = false;
        lastSecond = fullSeconds;

        maxRow = iDiff * 2 + 4;
        maxCol = iDiff * 2 + 2;
        clearedCellNum = 0;
        cellNum = 0;

        llkCells.Clear();
        lSelectedCells.Clear();

        for (int i = 0; i < maxRow; i++)
        {
            LlkList ll = new LlkList();
            for (int j = 0; j < maxCol; j++)
            {
                LlkCell lc = new LlkCell();
                ll.Add(lc);
                lc.x = size * j + posOffset;
                lc.y = size * i + posOffset;
                lc.row = i;
                lc.col = j;
                lc.bCleared = true;
            }
            llkCells.Add(ll);
        }

        for (int i = 0; i < (maxRow - 2) * (maxCol - 2) / 2; i++)
        {
            int rt = Random.Range(0, iDiff * 2);
            int rr;
            int rc;

            do
            {
                rr = Random.Range(0, maxRow - 2) + 1;
                rc = Random.Range(0, maxCol - 2) + 1;
            }
            while (llkCells[rr][rc].iType != -1);
            llkCells[rr][rc].SetType(rt);

            do
            {
                rr = Random.Range(0, maxRow - 2) + 1;
                rc = Random.Range(0, maxCol - 2) + 1;
            }
            while (llkCells[rr][rc].iType != -1);
            llkCells[rr][rc].SetType(rt);

            cellNum += 2;
        }
    }

    private void SetSelect(LlkCell lc)
    {
        if (lSelectedCells.Count == 0)
        {
            lc.bSelected = true;
            lSelectedCells.Add(lc);
        }
        else if (lSelectedCells.Count == 1)
        {
            if (lc == lSelectedCells[0])
            {
                lc.bSelected = false;
                lSelectedCells.Clear();
                return;
            }

            CheckCells(lc, lSelectedCells[0]);
        }
    }

    private void CheckCells(LlkCell lc1, LlkCell lc2)
    {
        if (lc1.iType != lc2.iType)
        {
            lc1.bSelected = false;
            lc2.bSelected = false;
            lSelectedCells.Clear();
            return;
        }

        if (CheckOneLine(lc1, lc2)
            || CheckTwoLines(lc1, lc2)
            || CheckThreeLines(lc1, lc2))
        {
            lc1.bCleared = true;
            lc2.bCleared = true;

            clearedCellNum += 2;
            lastSecond += 1;
        }
        else
        {
            lc1.bSelected = false;
            lc2.bSelected = false;
        }
        lSelectedCells.Clear();
    }

    private bool CheckOneLine(LlkCell lc1, LlkCell lc2)
    {
        int i;
        int ilarger;
        int ismaller;
        LlkCell tc;

        if (lc1.col == lc2.col)
        {
            ilarger = lc1.row > lc2.row ? lc1.row : lc2.row;
            ismaller = lc1.row < lc2.row ? lc1.row : lc2.row;
            for (i = ismaller + 1; i < ilarger; i++)
            {
                tc = llkCells[i][lc1.col];
                if (!tc.bCleared)
                {
                    return false;
                }
            }
            return true;
        }
        else if (lc1.row == lc2.row)
        {
            ilarger = lc1.col > lc2.col ? lc1.col : lc2.col;
            ismaller = lc1.col < lc2.col ? lc1.col : lc2.col;
            for (i = ismaller + 1; i < ilarger; i++)
            {
                tc = llkCells[lc1.row][i];
                if (!tc.bCleared)
                {
                    return false;
                }
            }
            return true;
        }

        return false;
    }
    private bool CheckTwoLines(LlkCell lc1, LlkCell lc2)
    {
        if (llkCells[lc1.row][lc2.col].bCleared
            && CheckOneLine(lc1, llkCells[lc1.row][lc2.col])
            && CheckOneLine(llkCells[lc1.row][lc2.col], lc2))
        {
            return true;
        }
        if (llkCells[lc2.row][lc1.col].bCleared
            && CheckOneLine(lc1, llkCells[lc2.row][lc1.col])
            && CheckOneLine(llkCells[lc2.row][lc1.col], lc2))
        {
            return true;
        }

        return false;
    }
    private bool CheckThreeLines(LlkCell lc1, LlkCell lc2)
    {
        int i;
        LlkCell c;
        for (i = lc1.row - 1; i >= 0; i--)
        {
            c = llkCells[i][lc1.col];
            if (c.bCleared)
            {
                if (CheckTwoLines(c, lc2))
                {
                    return true;
                }
            }
            else
            {
                break;
            }
        }

        for (i = lc1.row + 1; i < maxRow; i++)
        {
            c = llkCells[i][lc1.col];
            if (c.bCleared)
            {
                if (CheckTwoLines(c, lc2))
                {
                    return true;
                }
            }
            else
            {
                break;
            }
        }

        for (i = lc1.col - 1; i >= 0; i--)
        {
            c = llkCells[lc1.row][i];
            if (c.bCleared)
            {
                if (CheckTwoLines(c, lc2))
                {
                    return true;
                }
            }
            else
            {
                break;
            }
        }

        for (i = lc1.col + 1; i < maxCol; i++)
        {
            c = llkCells[lc1.row][i];
            if (c.bCleared)
            {
                if (CheckTwoLines(c, lc2))
                {
                    return true;
                }
            }
            else
            {
                break;
            }
        }
        return false;
    }

    private bool bGameOver;
    private bool bLevelUp;
    // Update is called once per frame
    void Update()
    {
        if (bLevelUp)
        {
            return;
        }

        if (bGameOver)
        {
            return;
        }

        if (clearedCellNum >= cellNum)
        {
            bLevelUp = true;
            return;
        }

        lastSecond -= Time.deltaTime;
        if (lastSecond <= 0)
        {
            bGameOver = true;
        }
    }
}

public class LlkCell
{
    public int iType = -1;
    public bool bSelected;
    public int row = -1;
    public int col = -1;
    public int x;
    public int y;

    public bool bCleared;

    public void SetType(int type)
    {
        iType = type;
        bCleared = false;
    }

    public bool CanGoThrough()
    {
        return bCleared || iType == 0;
    }


}