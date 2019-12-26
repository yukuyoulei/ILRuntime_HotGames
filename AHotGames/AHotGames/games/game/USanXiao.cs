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

    Dictionary<int, Transform> dCells = new Dictionary<int, Transform>();
    Dictionary<int, RawImage> dImages = new Dictionary<int, RawImage>();
    Dictionary<int, int> dColors = new Dictionary<int, int>();
    bool bmoving = false;
    protected override void InitComponents()
    {
        RegisterReturnButton();

        var grid = FindWidget<Transform>("grid");
        for (var i = 0; i < grid.childCount; i++)
        {
            var pos = i;
            var tr = grid.GetChild(pos);
            dCells.Add(pos, tr);
            var image = tr.GetChild(0).GetComponent<RawImage>();
            dImages.Add(pos, image);
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
                    tr.transform.SetAsLastSibling();
                    beginPos = tr.transform.position;
                    rawMousePos = Input.mousePosition;
                }
                , () =>
                {
                    if (bmoving) return;
                    tr.transform.position = beginPos;
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
                            tr.transform.position = beginPos;
                            return;
                        }

                        var fhv = 0f;
                        if (hv == -1 || hv == 1)
                            fhv = fh;
                        else
                            fhv = fv;

                        if (Math.Abs(fhv) > 40)
                        {
                            bmoving = false;
                            image.raycastTarget = false;
                            var tripled = StartToCheck(cpos, dColors[pos], pos, dColors[cpos]);
                            if (tripled)
                            {
                                SwapPos(pos, cpos);
                            }
                            else
                            {
                                image.raycastTarget = false;
                                tripled = StartToCheck(pos, dColors[cpos], cpos, dColors[pos]);
                                if (tripled)
                                {
                                    SwapPos(pos, cpos);
                                }
                                else
                                {
                                    bmoving = true;
                                    MoveTo(tr.transform, beginPos, 0.2f, Space.World, () =>
                                    {
                                        bmoving = false;
                                        image.raycastTarget = true;
                                    });
                                }
                            }
                        }

                        if (hv == -1 || hv == 1)
                        {
                            var p = tr.transform.position;
                            p.y = beginPos.y;
                            tr.transform.position = p;
                        }
                        else
                        {
                            var p = tr.transform.position;
                            p.x = beginPos.x;
                            tr.transform.position = p;
                        }
                    }
                });
        }

        ShowWidget("alert", false);
    }

    private void SwapPos(int pos, int cpos)
    {
        bmoving = true;
        var p = dCells[pos].transform.position;
        var cp = dCells[cpos].transform.position;
        MoveTo(dImages[cpos].transform, p, 0.2f, Space.World);
        MoveTo(dImages[pos].transform, cp, 0.2f, Space.World, () =>
        {
            var image = dImages[pos];
            dImages[pos] = dImages[cpos];
            dImages[cpos] = image;
            var c = dColors[pos];
            dColors[pos] = dColors[cpos];
            dColors[cpos] = c;

            bmoving = false;
            VanishTriplesFrom(pos, cpos);
        });
    }

    List<KeyValuePair<int, RawImage>> pendingImages = new List<KeyValuePair<int, RawImage>>();
    private void VanishTriplesFrom(params int[] poses)
    {
        Dictionary<int, RawImage> dVanishingImage = new Dictionary<int, RawImage>();
        Dictionary<int, RawImage> dTempVanishingImage = new Dictionary<int, RawImage>();
        for (var idx = 0; idx < poses.Length; idx++)
        {
            dTempVanishingImage.Clear();
            var pos = poses[idx];
            var ic = dColors[pos];
            for (var i = pos - 2; i <= pos + 1; i++)
            {
                if (!dColors.ContainsKey(i)) continue;
                if (dColors[i] != ic)
                {
                    if (dTempVanishingImage.Count >= 3)
                        break;
                    dTempVanishingImage.Clear();
                    continue;
                }
                dTempVanishingImage.Add(i, dImages[i]);
            }
            if (dTempVanishingImage.Count >= 3)
            {
                foreach (var kv in dTempVanishingImage)
                {
                    if (dVanishingImage.ContainsKey(kv.Key)) continue;
                    dVanishingImage.Add(kv.Key, kv.Value);
                }
            }
            dTempVanishingImage.Clear();
            for (var i = pos - 1; i <= pos + 2; i++)
            {
                if (!dColors.ContainsKey(i)) continue;
                if (dColors[i] != ic)
                {
                    if (dTempVanishingImage.Count >= 3)
                        break;
                    dTempVanishingImage.Clear();
                    continue;
                }
                dTempVanishingImage.Add(i, dImages[i]);
            }
            if (dTempVanishingImage.Count >= 3)
            {
                foreach (var kv in dTempVanishingImage)
                {
                    if (dVanishingImage.ContainsKey(kv.Key)) continue;
                    dVanishingImage.Add(kv.Key, kv.Value);
                }
            }
            dTempVanishingImage.Clear();
            for (var i = -1; i <= 2; i++)
            {
                var p = pos + rc * i;
                if (!dColors.ContainsKey(p)) continue;
                if (dColors[p] != ic)
                {
                    if (dTempVanishingImage.Count >= 3)
                        break;
                    dTempVanishingImage.Clear();
                    continue;
                }
                dTempVanishingImage.Add(p, dImages[p]);
            }
            if (dTempVanishingImage.Count >= 3)
            {
                foreach (var kv in dTempVanishingImage)
                {
                    if (dVanishingImage.ContainsKey(kv.Key)) continue;
                    dVanishingImage.Add(kv.Key, kv.Value);
                }
            }
            dTempVanishingImage.Clear();
            for (var i = -2; i <= 1; i++)
            {
                var p = pos + rc * i;
                if (!dColors.ContainsKey(p)) continue;
                if (dColors[p] != ic)
                {
                    if (dTempVanishingImage.Count >= 3)
                        break;
                    dTempVanishingImage.Clear();
                    continue;
                }
                dTempVanishingImage.Add(p, dImages[p]);
            }
            if (dTempVanishingImage.Count >= 3)
            {
                foreach (var kv in dTempVanishingImage)
                {
                    if (dVanishingImage.ContainsKey(kv.Key)) continue;
                    dVanishingImage.Add(kv.Key, kv.Value);
                }
            }
        }

        foreach (var kv in dVanishingImage)
        {
            kv.Value.enabled = false;
            dColors[kv.Key] = -1;
            pendingImages.Add(kv);
        }

        StartDrop();
    }

    private void StartDrop()
    {
        List<int> droping = new List<int>();
        foreach (var p in pendingImages)
        {
            var pos = p.Key;
            while (pos > -1)
            {
                pos -= rc;
                if (!dColors.ContainsKey(pos)) continue;
                if (dColors[pos] == -1) continue;
                if (droping.Contains(pos)) continue;
                var img = dImages[pos];
                dImages.Remove(pos);
                dImages[pos + rc] = img;
                var c = dColors[pos];
                dColors[pos] = -1;
                dColors[pos + rc] = c;
                bmoving = true;
                droping.Add(pos);
                MoveTo(img.transform, dCells[pos + rc].position, 0.2f, Space.World, () =>
                {
                    droping.Remove(pos);
                    bmoving = droping.Count == 0;
                });
            }
        }
    }

    private bool StartToCheck(int pos, int ic, int temppos, int tempc)
    {
        return checkTriple(pos, ic, temppos, tempc);
    }

    private void RenderImage(RawImage image, int pos)
    {
        image.color = colors[dColors[pos]];
    }

    private bool checkTriple(int pos, int ic, int temppos = -1, int tempc = -1)
    {
        if (dColors.Count < 3) return false;
        if (tempc == ic) return false;

        int icount = 0;
        for (int i = pos - 1; i >= pos - 2 && i >= pos / rc * rc && i < dColors.Count; i--)
        {
            if (!dColors.ContainsKey(i)) break;
            if (temppos == i) break;

            int c = dColors[i];
            if (ic != c) break;

            icount++;
            if (icount == 2) return true;
        }
        icount = 0;
        for (int i = pos + 1; i <= pos + 2 && i <= (pos / rc + 1) * rc && i < dColors.Count; i++)
        {
            if (!dColors.ContainsKey(i)) break;
            if (temppos == i) break;

            int c = dColors[i];
            if (temppos == i) c = tempc;
            if (ic != c) break;

            icount++;
            if (icount == 2) return true;
        }
        icount = 0;
        for (int i = pos - 1; i <= pos + 1 && i <= (pos / rc + 1) * rc && i < dColors.Count; i += 2)
        {
            if (!dColors.ContainsKey(i)) break;
            if (temppos == i) break;

            int c = dColors[i];
            if (temppos == i) c = tempc;
            if (ic != c) break;

            icount++;
            if (icount == 2) return true;
        }
        if (pos >= rc * 2)
        {
            icount = 0;
            for (int i = 1; i < 3; i++)
            {
                int p = pos - rc * i;
                if (!dColors.ContainsKey(p)) break;
                if (temppos == p) break;

                int c = dColors[p];
                if (temppos == i) c = tempc;
                if (ic != c) break;

                icount++;
                if (icount == 2) return true;
            }
            icount = 0;
            for (int i = 1; i < 3; i++)
            {
                int p = pos + rc * i;
                if (!dColors.ContainsKey(p)) break;
                if (temppos == p) break;

                int c = dColors[p];
                if (temppos == p) c = tempc;
                if (ic != c) break;

                icount++;
                if (icount == 2) return true;
            }
            icount = 0;
            for (int i = -1; i < 2; i += 2)
            {
                int p = pos + rc * i;
                if (!dColors.ContainsKey(p)) break;
                if (temppos == p) break;

                int c = dColors[p];
                if (temppos == p) c = tempc;
                if (ic != c) break;

                icount++;
                if (icount == 2) return true;
            }
        }
        return false;
    }
}

