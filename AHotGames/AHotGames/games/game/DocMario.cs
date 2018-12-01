using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
/// <summary>
/// 
/// Written by FS
/// 
/// </summary>

public class DocMario : AGameBase
{
    protected override void InitComponents()
    {
        DoInit();
        gameObj.AddComponent<UOnGUIer>().onOnGUI = () =>
        {
            OnGUI();
        };
        gameObj.AddComponent<UUpdater>().onUpdate = () => { Update(); };
    }
    public int columnCount = 10;
    public int rowCount = 20;
    private int[][] lTable;
    private Dictionary<int, int> dWorms = new Dictionary<int, int>();//row * 100 + column, type

    System.Random rdm = new System.Random();

    int CurLevel = 0;
    const int MinimumWormCount = 3;

    int[] medicineInHand;
    private Dictionary<int, int> medicineDroping = new Dictionary<int, int>();//row * 100 + column, type
    List<int[]> lCells = new List<int[]>();

    void DoInit()
    {
        dropingCells = null;
        lCells.Clear();
        iTransform = 0;
        dWorms.Clear();
        medicineInHand = new int[2];
        medicineDroping.Clear();
        bSmallMedicineDroping = false;

        lTable = new int[columnCount][];
        for (int i = 0; i < lTable.Length; i++)
        {
            lTable[i] = new int[rowCount];
        }

        for (int i = 0; i < CurLevel + MinimumWormCount; i++)
        {
            while (true)
            {
                int irow = rdm.Next(rowCount - 10) + 10;
                int icolumn = rdm.Next(columnCount);
                if (lTable[icolumn][irow] == 0)
                {
                    int itype = GetWormType();
                    lTable[icolumn][irow] = itype;
                    dWorms.Add(irow * 100 + icolumn, itype);
                    break;
                }
            }
        }

        DoRandomMedicineInHand();

        doMoveAfterTime = DateTime.Now;
        doDropAction = () => { DoDrop(doMoveAfter); };

        gameObj.AddComponent<UUpdater>().onUpdate = () =>
        {
            if (doDropAction != null)
            {
                doDropAction();
            }
        };
    }
    Action doDropAction;
    DateTime doMoveAfterTime;
    void DoDrop(float fDoMoveAfter)
    {
        if ((DateTime.Now - doMoveAfterTime).TotalSeconds < doMoveAfter)
        {
            return;
        }
        doDropAction = null;

        if (medicineDroping.Count == 0)
        {
            for (int i = 0; i < medicineInHand.Length; i++)
            {
                int icolumn = columnCount / 2 - 1 + i;
                medicineDroping.Add(icolumn, medicineInHand[i]);
                lTable[icolumn][0] = medicineInHand[i];
            }
            iTransform = 0;
            DoRandomMedicineInHand();
        }
        else if (bSmallMedicineDroping)
        {
            List<int[]> remove = new List<int[]>();
            foreach (int[] acell in dropingCells)
            {
                bool bBlocked = false;
                foreach (int pos in acell)
                {
                    int row = pos / 100 + 1;
                    if (row >= rowCount)
                    {
                        bBlocked = true;
                        break;
                    }
                    int column = pos % 100;
                    if (medicineDroping.ContainsKey(row * 100 + column))
                    {
                        continue;
                    }
                    if (lTable[column][row] != 0)
                    {
                        bBlocked = true;
                        break;
                    }
                }
                if (bBlocked)
                {
                    foreach (int ipos in acell)
                    {
                        medicineDroping.Remove(ipos);
                    }
                    remove.Add(acell);
                }
                else
                {
                    foreach (int pos in acell)
                    {
                        int row = pos / 100;
                        int column = pos % 100;
                        lTable[column][row] = 0;
                    }
                    List<int> lCell = new List<int>();
                    foreach (int pos in acell)
                    {
                        int row = pos / 100 + 1;
                        int column = pos % 100;
                        lTable[column][row] = medicineDroping[pos];
                        medicineDroping.Remove(pos);
                        lCell.Add(row * 100 + column);
                    }
                    for (int i = 0; i < lCell.Count; i++)
                    {
                        medicineDroping.Add(lCell[i], lTable[lCell[i] % 100][lCell[i] / 100]);
                        acell[i] = lCell[i];
                    }
                }

            }
            foreach (int[] acell in remove)
            {
                lCells.Remove(acell);
            }
            bSmallMedicineDroping = medicineDroping.Count > 0;
            DoCheckTrible();
        }
        else
        {
            bool bBlocked = false;
            foreach (int pos in medicineDroping.Keys)
            {
                int row = pos / 100 + 1;
                if (row >= rowCount)
                {
                    bBlocked = true;
                    break;
                }
                int column = pos % 100;
                if (medicineDroping.ContainsKey(row * 100 + column))
                {
                    continue;
                }
                if (lTable[column][row] != 0)
                {
                    bBlocked = true;
                    break;
                }
            }
            if (bBlocked)
            {
                DoPut();
            }
            else
            {
                foreach (int pos in medicineDroping.Keys)
                {
                    int row = pos / 100;
                    int column = pos % 100;
                    lTable[column][row] = 0;
                }
                Dictionary<int, int> dNewPoss = new Dictionary<int, int>();
                foreach (int pos in medicineDroping.Keys)
                {
                    int row = pos / 100 + 1;
                    if (row >= rowCount)
                    {
                        row = rowCount - 1;
                    }
                    int column = pos % 100;
                    lTable[column][row] = medicineDroping[pos];
                    dNewPoss.Add(row * 100 + column, medicineDroping[pos]);
                }
                medicineDroping = dNewPoss;
            }
        }

        if (bLevelUp)
        {
            bLevelUp = false;
        }
        else
        {
            doMoveAfterTime = DateTime.Now;
            doDropAction = () => { DoDrop(doMoveAfter); };
        }
    }

    void DoPut()
    {
        doMoveAfter = cRawMoveSpeed;
        int[] atc = new int[2];
        foreach (int pos in medicineDroping.Keys)
        {
            if (atc[0] == 0)
            {
                atc[0] = pos;
            }
            else
            {
                atc[1] = pos;
                lCells.Add(atc);
            }
        }

        if (!DoCheckTrible())
        {
            DoPrepareForNewMedicine();
        }
    }

    bool bSmallMedicineDroping = false;
    void DoPrepareForNewMedicine()
    {
        medicineDroping.Clear();
        iTransform = 0;
    }

    const int canDestroySameTypeCount = 3;
    int iTransform = 0;
    bool bLevelUp = false;
    private bool DoCheckTrible()
    {
        List<int> waitForDestroy = new List<int>();
        for (int i = 0; i < columnCount; i++)
        {
            List<int> dependForDestroy = new List<int>();
            int sameType = 0;
            for (int j = 0; j < rowCount; j++)
            {
                if (lTable[i][j] != 0 && lTable[i][j] == sameType)
                {
                    dependForDestroy.Add(j * 100 + i);
                }
                else
                {
                    if (dependForDestroy.Count >= canDestroySameTypeCount)
                    {
                        waitForDestroy.AddRange(dependForDestroy);
                    }
                    sameType = lTable[i][j];
                    dependForDestroy.Clear();
                    if (sameType != 0)
                    {
                        dependForDestroy.Add(j * 100 + i);
                    }
                }
            }
            if (dependForDestroy.Count >= canDestroySameTypeCount)
            {
                waitForDestroy.AddRange(dependForDestroy);
            }
        }
        for (int j = 0; j < rowCount; j++)
        {
            List<int> dependForDestroy = new List<int>();
            int sameType = 0;
            for (int i = 0; i < columnCount; i++)
            {
                if (lTable[i][j] != 0 && lTable[i][j] == sameType)
                {
                    dependForDestroy.Add(j * 100 + i);
                }
                else
                {
                    if (dependForDestroy.Count >= canDestroySameTypeCount)
                    {
                        waitForDestroy.AddRange(dependForDestroy);
                    }
                    sameType = lTable[i][j];
                    dependForDestroy.Clear();
                    if (sameType != 0)
                    {
                        dependForDestroy.Add(j * 100 + i);
                    }
                }
            }
            if (dependForDestroy.Count >= canDestroySameTypeCount)
            {
                waitForDestroy.AddRange(dependForDestroy);
            }
        }
        if (waitForDestroy.Count == 0)
        {
            return false;
        }
        else
        {
            medicineDroping.Clear();
            for (int i = 0; i < waitForDestroy.Count; i++)
            {
                int irow = waitForDestroy[i] / 100;
                int icol = waitForDestroy[i] % 100;
                lTable[icol][irow] = 0;

                if (dWorms.ContainsKey(waitForDestroy[i]))
                {
                    dWorms.Remove(waitForDestroy[i]);
                    if (dWorms.Count <= 0)
                    {
                        CurLevel++;
                        bLevelUp = true;
                        DoInit();
                        return true;
                    }
                }

                int testRow = irow;
                while (true)
                {
                    testRow--;
                    if (testRow < 0)
                    {
                        break;
                    }
                    if (lTable[icol][testRow] == 0)
                    {
                        break;
                    }
                    int pos = testRow * 100 + icol;
                    if (waitForDestroy.Contains(pos))
                    {
                        continue;
                    }
                    if (!medicineDroping.ContainsKey(pos))
                    {
                        medicineDroping.Add(pos, lTable[icol][testRow]);
                    }
                }

                testRow = irow;
                while (true)
                {
                    testRow++;
                    if (testRow >= rowCount)
                    {
                        break;
                    }
                    if (lTable[icol][testRow] == 0)
                    {
                        break;
                    }
                    int pos = testRow * 100 + icol;
                    if (waitForDestroy.Contains(pos))
                    {
                        continue;
                    }
                    if (!medicineDroping.ContainsKey(pos))
                    {
                        medicineDroping.Add(pos, lTable[icol][testRow]);
                    }
                }
                int testCol = icol;
                while (true)
                {
                    testCol--;
                    if (testCol < 0)
                    {
                        break;
                    }
                    if (lTable[testCol][irow] == 0)
                    {
                        break;
                    }
                    int pos = irow * 100 + testCol;
                    if (waitForDestroy.Contains(pos))
                    {
                        continue;
                    }
                    if (!medicineDroping.ContainsKey(pos))
                    {
                        medicineDroping.Add(pos, lTable[testCol][irow]);
                    }
                }
                testCol = icol;
                while (true)
                {
                    testCol++;
                    if (testCol >= columnCount)
                    {
                        break;
                    }
                    if (lTable[testCol][irow] == 0)
                    {
                        break;
                    }
                    int pos = irow * 100 + testCol;
                    if (waitForDestroy.Contains(pos))
                    {
                        continue;
                    }
                    if (!medicineDroping.ContainsKey(pos))
                    {
                        medicineDroping.Add(pos, lTable[testCol][irow]);
                    }
                }
            }
            bSmallMedicineDroping = medicineDroping.Count != 0;

            if (bSmallMedicineDroping)
            {
                List<int[]> newCells = new List<int[]>();
                for (int icell = 0; icell < lCells.Count; icell++)
                {
                    int[] apos = lCells[icell];
                    List<int> newCell = new List<int>();
                    foreach (int ipos in apos)
                    {
                        int irow = ipos / 100;
                        int icol = ipos % 100;
                        if (irow < 0 || irow >= rowCount || icol < 0 || icol >= columnCount)
                        {
                            Debug.LogError("invalid pos " + ipos);
                        }
                        if (lTable[ipos % 100][ipos / 100] != 0)
                        {
                            newCell.Add(ipos);
                        }
                    }
                    if (newCell.Count > 0)
                    {
                        newCells.Add(newCell.ToArray());
                    }
                }
                lCells = newCells;

                dropingCells = new List<int[]>();
                for (int icell = 0; icell < lCells.Count; icell++)
                {
                    foreach (int ipos in lCells[icell])
                    {
                        if (medicineDroping.ContainsKey(ipos))
                        {
                            dropingCells.Add(lCells[icell]);
                            continue;
                        }
                    }
                }

                medicineDroping.Clear();
                foreach (int[] acell in dropingCells)
                {
                    foreach (int cell in acell)
                    {
                        if (medicineDroping.ContainsKey(cell))
                        {
                            continue;
                        }
                        medicineDroping.Add(cell, lTable[cell % 100][cell / 100]);
                    }
                }

            }
            return true;
        }
    }
    List<int[]> dropingCells;

    void DoRandomMedicineInHand()
    {
        bSmallMedicineDroping = false;
        for (int i = 0; i < medicineInHand.Length; i++)
        {
            int itype = GetWormType();
            medicineInHand[i] = itype;
        }
    }
    int GetWormType()
    {
        return rdm.Next(MinimumWormCount) + 1;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 30), "Cur Level:" + CurLevel);
        GUI.Label(new Rect(120, 10, 100, 30), "Left Worms:" + dWorms.Count);

        for (int i = 0; i < columnCount; i++)
        {
            for (int j = 0; j < rowCount; j++)
            {
                DrawTableCell(lTable[i][j], i, j);
            }
        }
        for (int i = 0; i < medicineInHand.Length; i++)
        {
            DrawTableCell(medicineInHand[i], columnCount + 3 + i, 1);
        }
    }

    const int size = 35;
    private void DrawTableCell(int type, int icolumn, int irow)
    {
        string sContent = "";
        switch (type)
        {
            case 0:
                break;
            case 1:
                if (dWorms.ContainsKey(irow * 100 + icolumn))
                {
                    sContent = "○○\r\n○○";
                }
                else
                {
                    sContent = "●●\r\n●●";
                }
                break;
            case 2:
                if (dWorms.ContainsKey(irow * 100 + icolumn))
                {
                    sContent = "□□\r\n□□";
                }
                else
                {
                    sContent = "■■\r\n■■";
                }
                break;
            case 3:
                if (dWorms.ContainsKey(irow * 100 + icolumn))
                {
                    sContent = "△△\r\n△△";
                }
                else
                {
                    sContent = "▲▲\r\n▲▲";
                }
                break;
        }
        if (type != 0)
        {
            GUI.Box(new Rect(icolumn * size + 10, irow * size + 100, size, size), "");
        }
        GUI.Box(new Rect(icolumn * size + 10, irow * size + 100, size, size), sContent);
    }

    void Update()
    {
        if (bSmallMedicineDroping)
        {
            return;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            DoLeft();
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            DoRight();
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            DoTransform();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            doMoveAfter = 0.1f;
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            doMoveAfter = cRawMoveSpeed;
        }
    }
    float doMoveAfter = 1;
    float cRawMoveSpeed = 1;

    private void DoTransform()
    {
        iTransform++;
        int itrans = iTransform % 4;
        List<int> aPoses = medicineDroping.Keys.ToList();
        aPoses.Sort();
        List<int> targetPoses = new List<int>();
        Dictionary<int, int> newToOld = new Dictionary<int, int>();
        int newPos = 0;
        switch (itrans)
        {
            case 0:
                newPos = (aPoses[1] / 100 - 1) * 100 + aPoses[1] % 100 - 1;
                newToOld.Add(newPos, aPoses[1]);
                targetPoses.Add(newPos);
                targetPoses.Add(aPoses[0]);
                break;
            case 1:
                newPos = (aPoses[0] / 100 - 1) * 100 + aPoses[0] % 100 + 1;
                newToOld.Add(newPos, aPoses[0]);
                targetPoses.Add(newPos);
                targetPoses.Add(aPoses[1]);
                break;
            case 2:
                newPos = (aPoses[0] / 100 + 1) * 100 + aPoses[0] % 100 + 1;
                newToOld.Add(newPos, aPoses[0]);
                targetPoses.Add(newPos);
                targetPoses.Add(aPoses[1]);
                break;
            case 3:
                newPos = (aPoses[1] / 100 + 1) * 100 + aPoses[1] % 100 - 1;
                newToOld.Add(newPos, aPoses[1]);
                targetPoses.Add(newPos);
                targetPoses.Add(aPoses[0]);
                break;
        }
        for (int i = 0; i < targetPoses.Count; i++)
        {
            int irow = targetPoses[i] / 100;
            int icol = targetPoses[i] % 100;
            if (irow < 0 || irow >= rowCount || icol < 0 || icol >= columnCount)
            {
                iTransform--;
                //blocked, return
                return;
            }
            if (medicineDroping.ContainsKey(targetPoses[i]))
            {
                continue;
            }
            if (lTable[icol][irow] != 0)
            {
                iTransform--;
                //blocked, return
                return;
            }
        }
        foreach (int pos in medicineDroping.Keys)
        {
            int row = pos / 100;
            int column = pos % 100;
            lTable[column][row] = 0;
        }
        Dictionary<int, int> dNewPoss = new Dictionary<int, int>();
        for (int i = 0; i < targetPoses.Count; i++)
        {
            int irow = targetPoses[i] / 100;
            int icol = targetPoses[i] % 100;
            int itype = newToOld.ContainsKey(targetPoses[i]) ? medicineDroping[newToOld[targetPoses[i]]] : medicineDroping[targetPoses[i]];
            dNewPoss.Add(irow * 100 + icol, itype);
            lTable[icol][irow] = itype;
        }
        medicineDroping = dNewPoss;
    }

    private void DoRight()
    {
        foreach (int pos in medicineDroping.Keys)
        {
            int row = pos / 100;
            int column = pos % 100 + 1;
            if (column >= columnCount)
            {
                return;
            }
            if (medicineDroping.ContainsKey(row * 100 + column))
            {
                continue;
            }
            if (lTable[column][row] != 0)
            {
                return;
            }
        }
        foreach (int pos in medicineDroping.Keys)
        {
            int row = pos / 100;
            int column = pos % 100;
            lTable[column][row] = 0;
        }
        Dictionary<int, int> dNewPoss = new Dictionary<int, int>();
        foreach (int pos in medicineDroping.Keys)
        {
            int row = pos / 100;
            int column = pos % 100 + 1;
            lTable[column][row] = medicineDroping[pos];
            dNewPoss.Add(row * 100 + column, medicineDroping[pos]);
        }
        medicineDroping = dNewPoss;
    }

    private void DoLeft()
    {
        foreach (int pos in medicineDroping.Keys)
        {
            int row = pos / 100;
            int column = pos % 100 - 1;
            if (column < 0)
            {
                return;
            }
            if (medicineDroping.ContainsKey(row * 100 + column))
            {
                continue;
            }
            if (lTable[column][row] != 0)
            {
                return;
            }
        }
        foreach (int pos in medicineDroping.Keys)
        {
            int row = pos / 100;
            int column = pos % 100;
            lTable[column][row] = 0;
        }
        Dictionary<int, int> dNewPoss = new Dictionary<int, int>();
        foreach (int pos in medicineDroping.Keys)
        {
            int row = pos / 100;
            int column = pos % 100 - 1;
            lTable[column][row] = medicineDroping[pos];
            dNewPoss.Add(row * 100 + column, medicineDroping[pos]);
        }
        medicineDroping = dNewPoss;
    }
}
