using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// 
/// Written by FS
/// 
/// </summary>
public class Snake : AGameBase
{
    protected override void InitComponents()
    {
        gameObj.AddComponent<UOnGUIer>().onOnGUI = () =>
        {
            OnGUI();
        };
        gameObj.AddComponent<UUpdater>().onUpdate = () => { Update(); };
    }

    List<List<Cell>> lCells = new List<List<Cell>>();
    List<SnakeCell> lSnakeCells = new List<SnakeCell>();
    FoodCell foodCell = new FoodCell();
    System.Random rdm = new System.Random();
    private void ResetCells()
    {
        lCells.Clear();
        lSnakeCells.Clear();

        int r = Screen.height / Cell.isize - 2;
        int c = Screen.width / Cell.isize - 2;
        for (int i = 0; i < r; i++)
        {
            lCells.Add(new List<Cell>());
            for (int j = 0; j < c; j++)
            {
                if (i == 0 || j == 0 || i == r - 1 || j == c - 1)
                {
                    WallCell wc = new WallCell();
                    wc.ix = j;
                    wc.iy = i;
                    lCells[i].Add(wc);
                }
                else
                {
                    EmptyCell wc = new EmptyCell();
                    wc.ix = j;
                    wc.iy = i;
                    lCells[i].Add(wc);
                }
            }
        }

        for (int i = 0; i < 5; i++)
        {
            int ix = 5 + i;
            int iy = 5;
            SnakeCell sc = new SnakeCell();
            sc.ix = ix;
            sc.iy = iy;
            lSnakeCells.Insert(0, sc);
            lCells[iy][ix] = sc;
        }

        RandomOneFood();

        bGameOver = false;

        iDirectX = 1;
        iDirectY = 0;
        moveSep = 1;
    }
    void RandomOneFood()
    {
        while (true)
        {
            int iy = rdm.Next(lCells.Count);
            int ix = rdm.Next(lCells[iy].Count);
            if (lCells[iy][ix] is EmptyCell)
            {
                foodCell.ix = ix;
                foodCell.iy = iy;
                lCells[iy][ix] = foodCell;
                break;
            }
        }
    }

    void OnGUI()
    {

        for (int i = 0; i < lCells.Count; i++)
        {
            for (int j = 0; j < lCells[i].Count; j++)
            {
                //GUI.Label(new Rect(Cell.isize * j + Cell.borderWidth, Cell.isize * i + Cell.borderWidth, Cell.isize, Cell.isize), i + "," + j);
                lCells[i][j].OnDraw();
            }
        }

        if (bGameOver)
        {
            if (GUI.Button(new Rect(Screen.width / 2 - 30, Screen.height / 2 - 25, 60, 50), "Restart"))
            {
                ResetCells();
            }
        }
        if (!bStarting)
        {
            if (GUI.Button(new Rect(Screen.width / 2 - 30, Screen.height / 2 - 25, 60, 50), "Start"))
            {
                ResetCells();
                bStarting = true;
            }
        }
        if (bPause)
        {
            if (GUI.Button(new Rect(Screen.width / 2 - 30, Screen.height / 2 - 25, 60, 50), "Continue"))
            {
                bPause = false;
            }
        }
    }

    int iDirectX = 0;
    int iDirectY = 0;
    bool bStarting;
    bool bGameOver;
    float deltaSec = 0;
    float moveSep = 1;
    bool bPause;
    bool bDirectionLock = false;
    void Update()
    {
        if (bGameOver)
        {
            return;
        }
        if (!bStarting)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && iDirectY == 0 && !bDirectionLock)
        {
            Debug.Log("up");
            iDirectX = 0;
            iDirectY = -1;
            bDirectionLock = true;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && iDirectY == 0 && !bDirectionLock)
        {
            Debug.Log("down");
            iDirectX = 0;
            iDirectY = 1;
            bDirectionLock = true;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && iDirectX == 0 && !bDirectionLock)
        {
            Debug.Log("left");
            iDirectX = -1;
            iDirectY = 0;
            bDirectionLock = true;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) && iDirectX == 0 && !bDirectionLock)
        {
            Debug.Log("right");
            iDirectX = 1;
            iDirectY = 0;
            bDirectionLock = true;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (bGameOver)
            {
                bGameOver = false;
                ResetCells();
            }
            else if (!bStarting)
            {
                bStarting = true;
                ResetCells();
            }
            else
            {
                bPause = !bPause;
            }
        }

        if (bPause)
        {
            return;
        }

        deltaSec += Time.deltaTime;
        if (deltaSec < moveSep)
        {
            return;
        }
        deltaSec -= moveSep;

        Vector2 lastPos = Vector2.zero;
        foreach (SnakeCell sc in lSnakeCells)
        {
            if (lastPos.x == 0 || lastPos.y == 0)
            {
                lastPos.y = sc.iy + iDirectY;
                lastPos.x = sc.ix + iDirectX;
            }
            if (lCells[(int)lastPos.y][(int)lastPos.x].IsBlock)
            {
                bGameOver = true;
                break;
            }
            if (lCells[(int)lastPos.y][(int)lastPos.x] is FoodCell)
            {
                SnakeCell c = new SnakeCell();
                c.ix = (int)lastPos.x;
                c.iy = (int)lastPos.y;
                lSnakeCells.Insert(0, c);
                lCells[(int)lastPos.y][(int)lastPos.x] = c;

                RandomOneFood();

                moveSep -= moveSep * 0.07f;
                break;
            }

            Vector2 pos;
            pos.x = sc.ix;
            pos.y = sc.iy;
            lCells[sc.iy][sc.ix] = new EmptyCell();
            sc.ix = (int)lastPos.x;
            sc.iy = (int)lastPos.y;
            lCells[sc.iy][sc.ix] = sc;
            lastPos = pos;
        }
        bDirectionLock = false;
    }
}

abstract class Cell
{
    public const int isize = 30;
    public const int borderWidth = 30;

    public int ix, iy;
    public virtual void OnDraw()
    {
    }
    public virtual bool IsBlock { get { return true; } }
}
class EmptyCell : Cell
{
    public override void OnDraw()
    {

    }
    public override bool IsBlock
    {
        get
        {
            return false;
        }
    }
}
class WallCell : Cell
{
    public override void OnDraw()
    {
        GUI.Box(new Rect(isize * ix + borderWidth, isize * iy + borderWidth, isize, isize), "");
        GUI.Box(new Rect(isize * ix + borderWidth, isize * iy + borderWidth, isize, isize), "");
        GUI.Box(new Rect(isize * ix + borderWidth, isize * iy + borderWidth, isize, isize), "");
    }
    public override bool IsBlock
    {
        get
        {
            return true;
        }
    }
}

class FoodCell : Cell
{
    public override void OnDraw()
    {
        GUI.Box(new Rect(isize * ix + borderWidth, isize * iy + borderWidth, isize, isize), "F");
    }
    public override bool IsBlock
    {
        get
        {
            return false;
        }
    }
}

class SnakeCell : Cell
{
    public override void OnDraw()
    {
        GUI.Box(new Rect(isize * ix + borderWidth, isize * iy + borderWidth, isize, isize), "");
        GUI.Box(new Rect((isize) * ix + 4 + borderWidth, (isize) * iy + 4 + borderWidth, isize - 8, isize - 8), "");
    }
    public override bool IsBlock
    {
        get
        {
            return true;
        }
    }
}