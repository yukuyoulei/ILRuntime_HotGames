using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

public class USanXiao : AHotBase
{
    const int rc = 9;
    readonly Color[] colors = new Color[] { Color.red, Color.green, Color.yellow, Color.blue, Color.black };

    Dictionary<int, RawImage> dCells = new Dictionary<int, RawImage>();
    Dictionary<int, int> dColors = new Dictionary<int, int>();
    bool bmoving = false;
    protected override void InitComponents()
    {
        RegisterReturnButton();

        var grid = FindWidget<Transform>("grid");
        for (var i = 0; i < grid.childCount; i++)
        {
            var pos = i;
            var image = grid.GetChild(pos).GetComponent<RawImage>();
            dCells.Add(pos, image);
            image.color = Color.white;

            var ic = 0;
            do
            {
                ic = random.Next(colors.Length);
            }
            while (checkTriple(dColors.Count, ic));

            dColors.Add(pos, ic);
            RenderImage(image, pos);

            Vector3 beginPos = Vector3.zero;
            Vector3 rawMousePos = Vector3.zero;
            int hv = 0; //±1 = horizon, ± rc = vertical
            RegistDragFunc(image, null
                , () =>
                {
                    if (bmoving) return;
                    hv = 0;
                    image.transform.SetAsLastSibling();
                    beginPos = image.transform.position;
                    rawMousePos = Input.mousePosition;
                }
                , () =>
                {
                    if (bmoving) return;
                    image.transform.position = beginPos;
                }
                , null
                , () =>
                {
                    if (bmoving) return;
                    var fh = Input.mousePosition.x - rawMousePos.x;
                    var fv = Input.mousePosition.y - rawMousePos.y;
                    if (hv == 0)
                    {
                        if (Math.Abs(fh) < 1 && Math.Abs(fv) < 1) return;
                        if (Math.Abs(fh) > Math.Abs(fv))
                            hv = 1 * (fh > 0 ? 1 : -1);
                        else
                            hv = rc * (fv > 0 ? -1 : 1);
                    }
                    else
                    {
                        var cpos = pos + hv;
                        if (!dColors.ContainsKey(cpos))
                        {
                            image.transform.position = beginPos;
                            return;
                        }

                        var fhv = 0f;
                        if (hv == -1 || hv == 1)
                            fhv = fh;
                        else
                            fhv = fv;

                        if (Math.Abs(fhv) > 40)
                        {
                            var tripled = StartToCheck(image, pos, ic, cpos, dColors[cpos]);
                            if (tripled)
                            {
                                SwapPos(pos, cpos);
                            }
                            else
                            {
                                tripled = StartToCheck(image, cpos, dColors[cpos], pos, ic);
                                if (tripled)
                                {
                                    SwapPos(pos, cpos);
                                }
                                else
                                {
                                    bmoving = true;
                                    MoveTo(image.transform, beginPos, 0.2f, Space.World, () =>
                                    {
                                        bmoving = false;
                                        image.raycastTarget = true;
                                    });
                                }
                            }
                        }
                        if (hv == -1 || hv == 1)
                        {
                            var p = image.transform.position;
                            p.y = beginPos.y;
                            image.transform.position = p;
                        }
                        else
                        {
                            var p = image.transform.position;
                            p.x = beginPos.x;
                            image.transform.position = p;
                        }
                    }
                });
        }

        ShowWidget("alert", false);
    }

    private void SwapPos(int pos, int cpos)
    {
        AOutput.Log($"SwapPos {pos}, {cpos}");
    }

    private bool StartToCheck(RawImage image, int pos, int ic, int temppos, int tempc)
    {
        image.raycastTarget = false;
        AOutput.Log($"StartToCheck {pos}:{ic}, {temppos}:{tempc}");
        return checkTriple(pos, ic, temppos, tempc);
    }

    private void RenderImage(RawImage image, int pos)
    {
        image.color = colors[dColors[pos]];
    }

    private bool checkTriple(int pos, int ic, int temppos = -1, int tempc = -1)
    {
        if (dColors.Count < 3) return false;

        var icount = 0;
        for (var i = pos - 1; i >= pos - 2 && i >= pos / rc * rc && i < dColors.Count; i--)
        {
            var c = dColors[i];
            if (temppos != -1 && temppos == i) c = tempc;
            if (ic != c) break;
            icount++;
            if (icount == 2) return true;
        }
        icount = 0;
        for (var i = pos + 1; i <= pos + 2 && i <= (pos / rc + 1) * rc && i < dColors.Count; i++)
        {
            var c = dColors[i];
            if (temppos != -1 && temppos == i) c = tempc;
            if (ic != c) break;
            icount++;
            if (icount == 2) return true;
        }
        if (pos >= rc * 2)
        {
            icount = 0;
            for (var i = 1; i < 3; i++)
            {
                var c = dColors[pos - rc * i];
                if (temppos != -1 && temppos == i) c = tempc;
                if (ic != c) break;
                icount++;
                if (icount == 2) return true;
            }
            icount = 0;
            for (var i = 1; i < 3; i++)
            {
                var p = pos + rc * i;
                if (p >= dColors.Count) return false;
                var c = dColors[p];
                if (temppos != -1 && temppos == p) c = tempc;
                if (ic != c) break;
                icount++;
                if (icount == 2) return true;
            }
        }
        return false;
    }
}

