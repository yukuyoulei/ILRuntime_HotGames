using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UICurve : MaskableGraphic
{
	List<int> lCurveIDs = new List<int>();
	public void AddCurveData(int id, UICurveData curveData)
	{
		lCurveIDs.Add(id);
		dCurveData.Add(id, curveData);
		SetAllDirty();
	}
	public int CurveCount
	{
		get
		{
			return dCurveData.Count;
		}
	}
	public void Clear()
	{
		dCurveData.Clear();
		SetAllDirty();
	}
	private Dictionary<int, UICurveData> dCurveData = new Dictionary<int, UICurveData>();
	protected override void OnPopulateMesh(VertexHelper vh)
	{
		vh.Clear();
		if (dCurveData == null)
		{
			return;
		}
		foreach (var cd in dCurveData)
		{
			var lCurveData = cd.Value;
			if (lCurveData.pos.Count < 2)
			{
				continue;
			}
			for (int i = 1; i < lCurveData.pos.Count; i++)
			{
				UIVertex[] verts = new UIVertex[4];

				float x1 = lCurveData.pos[i - 1].x;
				float y1 = lCurveData.pos[i - 1].y;
				float x2 = lCurveData.pos[i].x;
				float y2 = lCurveData.pos[i].y;
				float xd = (y2 - y1) / Mathf.Sqrt(Mathf.Pow(x2 - x1, 2) + Mathf.Pow(y2 - y1, 2)) * cd.Value.thickness / 2;
				float yd = (x2 - x1) / Mathf.Sqrt(Mathf.Pow(x2 - x1, 2) + Mathf.Pow(y2 - y1, 2)) * cd.Value.thickness / 2;

				int idx = 0;
				verts[idx].position = new Vector3(lCurveData.pos[i - 1].x - xd, lCurveData.pos[i - 1].y + yd);
				verts[idx].color = lCurveData.color;
				verts[idx].uv0 = Vector2.zero;

				idx++;
				verts[idx].position = new Vector3(lCurveData.pos[i].x - xd, lCurveData.pos[i].y + yd);
				verts[idx].color = lCurveData.color;
				verts[idx].uv0 = Vector2.zero;

				idx++;
				verts[idx].position = new Vector3(lCurveData.pos[i].x + xd, lCurveData.pos[i].y - yd);
				verts[idx].color = lCurveData.color;
				verts[idx].uv0 = Vector2.zero;

				idx++;
				verts[idx].position = new Vector3(lCurveData.pos[i - 1].x + xd, lCurveData.pos[i - 1].y - yd);
				verts[idx].color = lCurveData.color;
				verts[idx].uv0 = Vector2.zero;
				vh.AddUIVertexQuad(verts);
			}
		}
	}

	internal void RemovePointIDs(params int[] remotePointIDs)
	{
		RemovePointIDs(remotePointIDs.ToList());
	}
	internal void RemovePointIDs(List<int> lRemotePointIDs)
	{
		foreach (var i in lRemotePointIDs)
		{
			if (!dCurveData.ContainsKey(i))
			{
				continue;
			}
			dCurveData.Remove(i);
			lCurveIDs.Remove(i);
		}
		SetAllDirty();
	}

	internal void DequeueData()
	{
		if (lCurveIDs.Count == 0)
		{
			return;
		}
		var id = lCurveIDs[lCurveIDs.Count - 1];
		if (dCurveData.ContainsKey(id))
		{
			dCurveData.Remove(id);
		}
		lCurveIDs.Remove(id);
		SetAllDirty();
	}
}

public class UICurveData
{
	public List<Vector2> pos = new List<Vector2>();
	public Color color;
	public float thickness = 2;
	public void AddPos(Vector2 v)
	{
		pos.Add(v);
	}
	public void AddPos(float x, float y)
	{
		AddPos(new Vector2(x, y));
	}
}