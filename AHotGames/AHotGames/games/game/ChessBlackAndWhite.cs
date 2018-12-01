using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 
/// Written by FS
/// 
/// </summary>

public class ChessBlackAndWhite : AGameBase
{
    protected override void InitComponents()
    {
        Restart();
        gameObj.AddComponent<UOnGUIer>().onOnGUI = () =>
        {
            OnGUI();
        };
    }
    private int rowNum = 10;
    private int colNum = 10;
    private int leftEdge = 10;
    private int topEdge = 100;
    private int size = 10;
    private int cellsep = 5;
    void Restart()
    {
        lTable.Clear();
        for (int i = 0; i < colNum; i++)
        {
            lTable.Add(new List<ChessCell>());
            for (int j = 0; j < rowNum; j++)
            {
                lTable[i].Add(new ChessCell());
                if (i == rowNum / 2 - 1 && j == colNum / 2 - 1)
                {
                    lTable[i][j].iPlayer = 1;
                }
                if (i == rowNum / 2 && j == colNum / 2 - 1)
                {
                    lTable[i][j].iPlayer = 2;
                }
                if (i == rowNum / 2 - 1 && j == colNum / 2)
                {
                    lTable[i][j].iPlayer = 2;
                }
                if (i == rowNum / 2 && j == colNum / 2)
                {
                    lTable[i][j].iPlayer = 1;
                }

            }
        }
        size = ((Screen.width > Screen.height ? (Screen.height - topEdge) : Screen.width) - (2 * leftEdge)) / rowNum - cellsep;

        lasti = -1;
        lastj = -1;

        curPlayer = 1;
    }

    private int curPlayer = 1;
    private void ChangePlayer()
    {
        curPlayer = curPlayer == 1 ? 2 : 1;
    }
    private string curPlayerName
    {
        get
        {
            return curPlayer == 1 ? "O" : "X";
        }
    }

    public bool bPC = true;
    private int lasti = -1;
    private int lastj = -1;
    List<List<ChessCell>> lTable = new List<List<ChessCell>>();
    private int player1count = 2;
    private int player2count = 2;
    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 30), "Cur Player:" + curPlayerName);
        if (GUI.Button(new Rect(220, 10, 80, 40), "restart"))
        {
            Restart();
        }

        if (lasti >= 0)
        {
            GUI.Box(new Rect(leftEdge + lasti * (size + cellsep), topEdge + lastj * (size + cellsep), size, size), "");
        }
        int most1 = 0;
        for (int ii = 0; ii < colNum; ii++)
        {
            for (int jj = 0; jj < rowNum; jj++)
            {
                if (lTable[ii][jj].iPlayer != 0)
                {
                    continue;
                }
                int icount = CheckValid(jj, ii, false);
                if (most1 < icount)
                {
                    most1 = icount;
                }
            }
        }
        if (most1 == 0)
        {
            ShowResult();
        }
        for (int i = 0; i < colNum; i++)
        {
            for (int j = 0; j < rowNum; j++)
            {
                if (lTable[i][j].iPlayer == 1)
                {
                    GUI.Button(new Rect(leftEdge + i * (size + cellsep), topEdge + j * (size + cellsep), size, size), "O");
                }
                else if (lTable[i][j].iPlayer == 2)
                {
                    GUI.Button(new Rect(leftEdge + i * (size + cellsep), topEdge + j * (size + cellsep), size, size), "X");
                }
                else
                {
                    if (GUI.Button(new Rect(leftEdge + i * (size + cellsep), topEdge + j * (size + cellsep), size, size), ""))
                    {
                        if (!bPC || curPlayer == 1)
                        {
                            CheckValid(j, i, true);
                            if (bPC && curPlayer == 2)
                            {
                                int most = 0;
                                int mii = 0;
                                int mjj = 0;
                                for (int ii = 0; ii < colNum; ii++)
                                {
                                    for (int jj = 0; jj < rowNum; jj++)
                                    {
                                        if (lTable[ii][jj].iPlayer != 0)
                                        {
                                            continue;
                                        }
                                        int icount = CheckValid(jj, ii, false);
                                        if (most < icount)
                                        {
                                            most = icount;
                                            mii = ii;
                                            mjj = jj;
                                        }
                                    }
                                }
                                if (most > 0)
                                {
                                    CheckValid(mjj, mii, true);
                                    lasti = mii;
                                    lastj = mjj;
                                }
                                else
                                {
                                    ShowResult();
                                }
                            }
                        }
                    }
                }
            }
        }

        player1count = 0;
        player2count = 0;
        for (int ii = 0; ii < colNum; ii++)
        {
            for (int jj = 0; jj < rowNum; jj++)
            {
                if (lTable[ii][jj].iPlayer == 1)
                {
                    player1count++;
                }
                else if (lTable[ii][jj].iPlayer == 2)
                {
                    player2count++;
                }
            }
        }

        GUI.Label(new Rect(10, 55, 300, 40), "P1 O:" + player1count + "    P2 X:" + player2count);
    }
    void ShowResult()
    {
        GUI.Box(new Rect(220, 55, 100, 40), "");
        GUI.Button(new Rect(220, 55, 100, 40), player1count > player2count ? "P1 WIN" : "P2 WIN");
    }
    int CheckValid(int row, int col, bool bSet)
    {
        List<ChessCell> lSetCells = new List<ChessCell>();
        List<ChessCell> lPendingCells = new List<ChessCell>();
        //test left
        for (int i = col - 1; i >= 0; i--)
        {
            if (lTable[i][row].iPlayer == 0)
            {
                break;
            }
            if (lTable[i][row].iPlayer != curPlayer)
            {
                lPendingCells.Add(lTable[i][row]);
            }
            if (lTable[i][row].iPlayer == curPlayer)
            {
                lSetCells.AddRange(lPendingCells);
                break;
            }
        }
        //test right
        lPendingCells.Clear();
        for (int i = col + 1; i < colNum; i++)
        {
            if (lTable[i][row].iPlayer == 0)
            {
                break;
            }
            if (lTable[i][row].iPlayer != curPlayer)
            {
                lPendingCells.Add(lTable[i][row]);
            }
            if (lTable[i][row].iPlayer == curPlayer)
            {
                lSetCells.AddRange(lPendingCells);
                break;
            }
        }
        //test up
        lPendingCells.Clear();
        for (int i = row - 1; i >= 0; i--)
        {
            if (lTable[col][i].iPlayer == 0)
            {
                break;
            }
            if (lTable[col][i].iPlayer != curPlayer)
            {
                lPendingCells.Add(lTable[col][i]);
            }
            if (lTable[col][i].iPlayer == curPlayer)
            {
                lSetCells.AddRange(lPendingCells);
                break;
            }
        }
        //test down
        lPendingCells.Clear();
        for (int i = row + 1; i < rowNum; i++)
        {
            if (lTable[col][i].iPlayer == 0)
            {
                break;
            }
            if (lTable[col][i].iPlayer != curPlayer)
            {
                lPendingCells.Add(lTable[col][i]);
            }
            if (lTable[col][i].iPlayer == curPlayer)
            {
                lSetCells.AddRange(lPendingCells);
                break;
            }
        }
        //test lefttop
        lPendingCells.Clear();
        for (int i = col - 1, j = row - 1; i >= 0 && j >= 0; i--, j--)
        {
            if (lTable[i][j].iPlayer == 0)
            {
                break;
            }
            if (lTable[i][j].iPlayer != curPlayer)
            {
                lPendingCells.Add(lTable[i][j]);
            }
            if (lTable[i][j].iPlayer == curPlayer)
            {
                lSetCells.AddRange(lPendingCells);
                break;
            }
        }
        //test righttop
        lPendingCells.Clear();
        for (int i = col + 1, j = row - 1; i < colNum && j >= 0; i++, j--)
        {
            if (lTable[i][j].iPlayer == 0)
            {
                break;
            }
            if (lTable[i][j].iPlayer != curPlayer)
            {
                lPendingCells.Add(lTable[i][j]);
            }
            if (lTable[i][j].iPlayer == curPlayer)
            {
                lSetCells.AddRange(lPendingCells);
                break;
            }
        }
        //test leftbottom
        lPendingCells.Clear();
        for (int i = col - 1, j = row + 1; i >= 0 && j < rowNum; i--, j++)
        {
            if (lTable[i][j].iPlayer == 0)
            {
                break;
            }
            if (lTable[i][j].iPlayer != curPlayer)
            {
                lPendingCells.Add(lTable[i][j]);
            }
            if (lTable[i][j].iPlayer == curPlayer)
            {
                lSetCells.AddRange(lPendingCells);
                break;
            }
        }
        //test rightbottom
        lPendingCells.Clear();
        for (int i = col + 1, j = row + 1; i < colNum && j < rowNum; i++, j++)
        {
            if (lTable[i][j].iPlayer == 0)
            {
                break;
            }
            if (lTable[i][j].iPlayer != curPlayer)
            {
                lPendingCells.Add(lTable[i][j]);
            }
            if (lTable[i][j].iPlayer == curPlayer)
            {
                lSetCells.AddRange(lPendingCells);
                break;
            }
        }

        if (lSetCells.Count > 0)
        {
            if (bSet)
            {
                lTable[col][row].iPlayer = curPlayer;
                foreach (ChessCell cc in lSetCells)
                {
                    cc.iPlayer = curPlayer;
                }
                ChangePlayer();
            }
        }
        return lSetCells.Count;
    }
}

class ChessCell
{
    public int iPlayer = 0;
}
