using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

public class UMinerMap : AHotBase
{
	RectTransform map;
	Transform bgcell;
	List<Transform> lCells = new List<Transform>();
	List<MapCell> lMapCells = new List<MapCell>();
	RectTransform role;
	Animator role01;
	RectTransform bombTemplate;

	public const string DirUp = "u";
	public const string DirDown = "d";
	public const string DirLeft = "l";
	public const string DirRight = "r";
	bool bmoving = false;
	bool bNeedStop = false;
	const int MaxX = 12;
	const int MaxY = 16;
	const int Size = 32;
	int bombMaxCount = 1;
	int curBombCount = 0;
	int curBombMaxLength = 1;
	int cannotDestroyBlockType = 0;
	Dictionary<int, BombCell> dBombs = new Dictionary<int, BombCell>();
	protected override void InitComponents()
	{
		map = FindWidget<RectTransform>("map");
		var cells = FindWidget<Transform>("cells");
		bgcell = cells.GetChild(0);
		for (var i = 1; i < cells.childCount; i++)
		{
			lCells.Add(cells.GetChild(i));
		}

		bombTemplate = FindWidget<RectTransform>("bomb");
		role = FindWidget<RectTransform>("role");
		role01 = FindWidget<Animator>(role, "role01");
		addUpdateAction(() =>
		{
			if (Input.GetKey(KeyCode.LeftArrow))
			{
				if (bmoving) return false;
				bNeedStop = false;
				DoMove(DirLeft);
			}
			else if (Input.GetKeyUp(KeyCode.LeftArrow))
				bNeedStop = true;
			else if (Input.GetKey(KeyCode.RightArrow))
			{
				if (bmoving) return false;
				bNeedStop = false;
				DoMove(DirRight);
			}
			else if (Input.GetKeyUp(KeyCode.RightArrow))
				bNeedStop = true;
			else if (Input.GetKey(KeyCode.UpArrow))
			{
				if (bmoving) return false;
				bNeedStop = false;
				DoMove(DirUp);
			}
			else if (Input.GetKeyUp(KeyCode.UpArrow))
				bNeedStop = true;
			else if (Input.GetKey(KeyCode.DownArrow))
			{
				if (bmoving) return false;
				bNeedStop = false;
				DoMove(DirDown);
			}
			else if (Input.GetKeyUp(KeyCode.DownArrow))
				bNeedStop = true;
			else if (Input.GetKey(KeyCode.Space))
				DoSetBomb();
			return false;
		});

		cannotDestroyBlockType = lCells.Count - 1;
		for (var i = 0; i < MaxY; i++)
		{
			for (var j = 0; j < MaxX; j++)
			{
				var irdm = 0;
				if (i % 2 == 1 && j % 2 == 1)
				{
					irdm = cannotDestroyBlockType;
				}
				else if (i + j > 2)
				{
					irdm = random.Next(lCells.Count - 1);
				}

				var model = GameObject.Instantiate(lCells[irdm], map) as RectTransform;
				model.anchoredPosition = new Vector3(j * Size, -i * Size);
				lMapCells.Add(new MapCell(model, irdm, j, i));
			}
		}
		role.position = lMapCells[0].model.position;
	}

	private void DoSetBomb()
	{
		if (curBombCount >= bombMaxCount) return;
		var bmodel = GameObject.Instantiate(bombTemplate, map.parent);
		bmodel.gameObject.SetActive(true);
		bmodel.anchoredPosition = role.anchoredPosition;
		bmodel.SetParent(map);
		curBombCount++;
		var bomb = new BombCell(bmodel, x, y, curBombMaxLength);
		var fire = FindWidget<RectTransform>(bombTemplate.transform, "fire");
		dBombs.Add(bomb.ID, bomb);
		var id = bomb.ID;
		var cx = x;
		var cy = y;
		DelayDoSth(3, () =>
		{
			if (!dBombs.ContainsKey(id)) return;
			curBombCount--;
			dBombs.Remove(id);
			var insfire = GameObject.Instantiate(fire, map);
			insfire.anchoredPosition = bmodel.anchoredPosition;
			insfire.gameObject.SetActive(true);
			DelayDoSth(0.1f, () =>
			{
				DoFire(insfire, insfire, insfire, insfire, 1, bomb.maxLength, cx, cy);
				DelayDoSth(0.1f, () =>
				{
					DisableObj(insfire.gameObject);
				});
			});

			DisableObj(bmodel.gameObject);
		});
	}
	void DisableObj(GameObject obj)
	{
		GameObject.Destroy(obj);
	}

	private void DoFire(RectTransform insfire0, RectTransform insfire1, RectTransform insfire2, RectTransform insfire3, int curLength, int maxLength, int cx, int cy)
	{
		if (curLength > maxLength) return;
		if (insfire0 != null)
		{
			if (cx - 1 < 0) goto to1;
			if (GetBlockType(cx - 1, cy) != 0)
			{
				DestroyBlock(cx - 1, cy);
				goto to1;
			}
			var ifire = GameObject.Instantiate(insfire0, map);
			ifire.gameObject.SetActive(true);
			ifire.anchoredPosition = new Vector2(insfire0.anchoredPosition.x - Size, insfire0.anchoredPosition.y);
			DelayDoSth(0.1f, () =>
			{
				DoFire(ifire, null, null, null, curLength + 1, maxLength, cx - 1, cy);
				DisableObj(ifire.gameObject);
			});
		}
	to1:
		if (insfire1 != null)
		{
			if (cx + 1 >= MaxX) goto to2;
			if (GetBlockType(cx + 1, cy) != 0)
			{
				DestroyBlock(cx + 1, cy);
				goto to2;
			}
			var ifire = GameObject.Instantiate(insfire1, map);
			ifire.gameObject.SetActive(true);
			ifire.anchoredPosition = new Vector2(insfire1.anchoredPosition.x + Size, insfire1.anchoredPosition.y);
			if (ifire)
			{
				DelayDoSth(0.1f, () =>
				{
					DoFire(null, ifire, null, null, curLength + 1, maxLength, cx + 1, cy);
					DisableObj(ifire.gameObject);
				});
			}
		}
	to2:
		if (insfire2 != null)
		{
			if (cy - 1 < 0) goto to3;
			if (GetBlockType(cx, cy - 1) != 0)
			{
				DestroyBlock(cx, cy - 1);
				goto to3;
			}
			var ifire = GameObject.Instantiate(insfire2, map);
			ifire.gameObject.SetActive(true);
			ifire.anchoredPosition = new Vector2(insfire2.anchoredPosition.x, insfire2.anchoredPosition.y + Size);
			if (ifire)
			{
				DelayDoSth(0.1f, () =>
				{
					DoFire(null, null, ifire, null, curLength + 1, maxLength, cx, cy - 1);
					DisableObj(ifire.gameObject);
				});
			}
		}
	to3:
		if (insfire3 != null)
		{
			if (cy + 1 >= MaxY) return;
			if (GetBlockType(cx, cy + 1) != 0)
			{
				DestroyBlock(cx, cy + 1);
				return;
			}
			var ifire = GameObject.Instantiate(insfire3, map);
			ifire.gameObject.SetActive(true);
			ifire.anchoredPosition = new Vector2(insfire3.anchoredPosition.x, insfire3.anchoredPosition.y - Size);
			if (ifire)
			{
				DelayDoSth(0.1f, () =>
				{
					DoFire(null, null, null, ifire, curLength + 1, maxLength, cx, cy + 1);
					DisableObj(ifire.gameObject);
				});
			}
		}
	}

	private void DestroyBlock(int x, int y)
	{
		var m = x + y * MaxX;
		if (lMapCells[m].blockType == cannotDestroyBlockType) return;
		lMapCells[m].blockType = 0;
		lMapCells[m].model.GetComponent<Graphic>().enabled = false;
	}

	private void StopMove(string direction)
	{
		curDirection = "";
		role01.SetTrigger("i" + direction);
		bmoving = false;
	}
	private int x, y;
	private string curDirection;
	private void DoMove(string direction)
	{
		var ancpos = map.anchoredPosition;
		switch (direction)
		{
			case DirLeft:
				if (!CheckX(-1))
				{
					StopMove(direction);
					return;
				}
				ancpos = new Vector2(ancpos.x + Size, ancpos.y);
				break;
			case DirRight:
				if (!CheckX(1))
				{
					StopMove(direction);
					return;
				}
				ancpos = new Vector2(ancpos.x - Size, ancpos.y);
				break;
			case DirUp:
				if (!CheckY(-1))
				{
					StopMove(direction);
					return;
				}
				ancpos = new Vector2(ancpos.x, ancpos.y - Size);
				break;
			case DirDown:
				if (!CheckY(1))
				{
					StopMove(direction);
					return;
				}
				ancpos = new Vector2(ancpos.x, ancpos.y + Size);
				break;
		}
		var delay = 0f;
		if (direction != curDirection)
			delay = 0.2f;
		curDirection = direction;

		role01.SetTrigger(direction);
		bmoving = true;
		DelayDoSth(delay, () =>
		{
			MoveTo(map, ancpos, 0.6f, () =>
			{
				if (bNeedStop)
					StopMove(direction);
				else
					DoMove(direction);
			});
		});
	}

	int GetBlockType(int x, int y)
	{
		var m = x + y * MaxX;
		return lMapCells[m].blockType;
	}
	private bool CheckX(int plus)
	{
		if (x + plus < 0) return false;
		if (x + plus >= MaxX) return false;
		if (GetBlockType(x + plus, y) > 0) return false;
		foreach (var b in dBombs)
		{
			if (b.Value.x == x + plus && b.Value.y == y) return false;
		}
		x += plus;
		return true;
	}
	private bool CheckY(int plus)
	{
		if (y + plus < 0) return false;
		if (y + plus >= MaxY) return false;
		if (GetBlockType(x, y + plus) > 0) return false;
		foreach (var b in dBombs)
		{
			if (b.Value.x == x && b.Value.y == y + plus) return false;
		}
		y += plus;
		return true;
	}
}
public abstract class CellBase
{
	public RectTransform model;
	public int x, y;
}
public class MapCell : CellBase
{
	public MapCell(RectTransform model, int blockType, int x, int y)
	{
		this.model = model;
		this.blockType = blockType;
		this.x = x;
		this.y = y;
	}
	public int blockType;
}
public class BombCell : CellBase
{
	private static int s_BombID;
	public BombCell(RectTransform model, int x, int y, int maxLength)
	{
		this.model = model;
		ID = ++s_BombID;
		this.x = x;
		this.y = y;
		this.maxLength = maxLength;
	}
	public int ID;
	public int maxLength;
}

