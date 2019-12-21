using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using System.Threading.Tasks;

public class U2048 : AHotBase
{
	const int rc = 4;
	const int slotCount = rc * rc;
	int[] numbers = new int[slotCount];
	bool bStart = false;

	Transform blockTemp;
	Transform[] slots = new Transform[slotCount];
	Dictionary<int, Transform> dBlocks = new Dictionary<int, Transform>();
	Queue<Transform> blockPools = new Queue<Transform>();

	Text labelAlert;
	Button btnRestart;
	protected override void InitComponents()
	{
		RegisterReturnButton();

		labelAlert = FindWidget<Text>("labelAlert");
		btnRestart = FindWidget<Button>("btnRestart");
		btnRestart.onClick.AddListener(ClearCells);

		blockTemp = FindWidget<Transform>("block");
		blockTemp.gameObject.SetActive(false);

		var grid = FindWidget<Transform>("grid");
		for (var i = 0; i < slots.Length; i++)
		{
			slots[i] = grid.transform.GetChild(i);
			numbers[i] = 0;
		}

		bStart = true;
		SpawnNewCell();

		addUpdateAction(ListenSwap);
	}

	Vector2 downPos;
	bool bMoving;
	private bool ListenSwap()
	{
		if (bMoving) return false;
		if (!bStart) return false;
		if (Input.GetMouseButtonDown(0))
		{
			downPos = Input.mousePosition;
		}
		else if (Input.GetMouseButtonUp(0))
		{
			var ih = Input.mousePosition.x - downPos.x;
			var iv = downPos.y - Input.mousePosition.y;
			if (Math.Abs(ih) > Math.Abs(iv))
			{
				if (Math.Abs(ih) < 20) return false;
				if (ih < 0) OnLeft();
				else OnRight();
			}
			else
			{
				if (Math.Abs(iv) < 20) return false;
				if (iv < 0) OnUp();
				else OnDown();
			}

			if (bMoving)
			{
				DelayDoSth(moveTime, () =>
				{
					bMoving = false;
					SpawnNewCell();
				});
			}
		}
		return false;
	}

	private bool OnUp(bool check = false)
	{
		var bHasEmptySlot = false;
		for (var j = 0; j < rc; j++)
		{
			for (var i = 1; i < rc; i++)
				if (numbers[i * rc + j] != 0 && numbers[(i - 1) * rc + j] == 0)
				{
					bHasEmptySlot = true;
					break;
				}
			if (bHasEmptySlot)
				break;
		}
		if (!bHasEmptySlot)
		{
			var bHasSameNumber = false;
			for (var j = 0; j < rc; j++)
			{
				for (var i = 1; i < rc; i++)
					if (numbers[i * rc + j] != 0
						&& numbers[(i - 1) * rc + j] == numbers[i * rc + j])
					{
						bHasSameNumber = true;
						break;
					}
				if (bHasSameNumber)
					break;
			}
			if (!bHasSameNumber)
				return false;
		}

		if (check) return true;

		for (var j = 0; j < rc; j++)
		{
			for (var i = 1; i < rc; i++)
			{
				var ito = (i - 1) * rc + j;
				var ifrom = i * rc + j;
				if (numbers[ifrom] == 0
					|| (numbers[ito] != 0 && numbers[ito] != numbers[ifrom]))
					continue;
				numbers[ito] += numbers[ifrom];
				numbers[ifrom] = 0;
				AOutput.Log($"ifrom {ifrom} ito {ito}");

				slots[ifrom].SetAsLastSibling();
				var inum = numbers[ito];
				var endTr = slots[ito];
				var replaceTr = dBlocks.ContainsKey(ito) ? dBlocks[ito] : null;
				BlockFromTo(ifrom, ito);
				MoveTo(dBlocks[ito], endTr.position, moveTime, Space.World, () =>
				{
					SetNumber(ito, inum, dBlocks[ito]);
					if (replaceTr != null) ReturnBlockToPool(replaceTr);
				});
			}
		}
		return true;
	}
	private bool OnDown(bool check = false)
	{
		var bHasEmptySlot = false;
		for (var j = 0; j < rc; j++)
		{
			for (var i = rc - 2; i >= 0; i--)
				if (numbers[i * rc + j] != 0 && numbers[(i + 1) * rc + j] == 0)
				{
					bHasEmptySlot = true;
					break;
				}
			if (bHasEmptySlot)
				break;
		}
		if (!bHasEmptySlot)
		{
			var bHasSameNumber = false;
			for (var j = 0; j < rc; j++)
			{
				for (var i = rc - 2; i >= 0; i--)
					if (numbers[i * rc + j] != 0
						&& numbers[(i + 1) * rc + j] == numbers[i * rc + j])
					{
						bHasSameNumber = true;
						break;
					}
				if (bHasSameNumber)
					break;
			}
			if (!bHasSameNumber)
				return false;
		}

		if (check) return true;

		for (var j = 0; j < rc; j++)
		{
			for (var i = rc - 2; i >= 0; i--)
			{
				var ito = (i + 1) * rc + j;
				var ifrom = i * rc + j;
				if (numbers[ifrom] == 0
					|| (numbers[ito] != 0 && numbers[ito] != numbers[ifrom]))
					continue;
				numbers[ito] += numbers[ifrom];
				numbers[ifrom] = 0;

				slots[ifrom].SetAsLastSibling();
				var inum = numbers[ito];
				var endTr = slots[ito];
				var replaceTr = dBlocks.ContainsKey(ito) ? dBlocks[ito] : null;
				BlockFromTo(ifrom, ito);
				MoveTo(dBlocks[ito], endTr.position, moveTime, Space.World, () =>
				{
					SetNumber(ito, inum, dBlocks[ito]);
					if (replaceTr != null) ReturnBlockToPool(replaceTr);
				});
			}
		}
		return true;
	}
	private bool OnLeft(bool check = false)
	{
		var bHasEmptySlot = false;
		for (var j = 1; j < rc; j++)
		{
			for (var i = 0; i < rc; i++)
				if (numbers[i * rc + j] != 0 && numbers[i * rc + j - 1] == 0)
				{
					bHasEmptySlot = true;
					break;
				}
			if (bHasEmptySlot)
				break;
		}
		if (!bHasEmptySlot)
		{
			var bHasSameNumber = false;
			for (var j = 1; j < rc; j++)
			{
				for (var i = 0; i < rc; i++)
					if (numbers[i * rc + j] != 0
						&& numbers[i * rc + j - 1] == numbers[i * rc + j])
					{
						bHasSameNumber = true;
						break;
					}
				if (bHasSameNumber)
					break;
			}
			if (!bHasSameNumber)
				return false;
		}

		if (check) return true;

		for (var j = 1; j < rc; j++)
		{
			for (var i = 0; i < rc; i++)
			{
				var ito = i * rc + j - 1;
				var ifrom = i * rc + j;
				if (numbers[ifrom] == 0
					|| (numbers[ito] != 0 && numbers[ito] != numbers[ifrom]))
					continue;
				numbers[ito] += numbers[ifrom];
				numbers[ifrom] = 0;
				AOutput.Log($"ifrom {ifrom} ito {ito}");

				slots[ifrom].SetAsLastSibling();
				var inum = numbers[ito];
				var endTr = slots[ito];
				var replaceTr = dBlocks.ContainsKey(ito) ? dBlocks[ito] : null;
				BlockFromTo(ifrom, ito);
				MoveTo(dBlocks[ito], endTr.position, moveTime, Space.World, () =>
				{
					SetNumber(ito, inum, dBlocks[ito]);
					if (replaceTr != null) ReturnBlockToPool(replaceTr);
				});
			}
		}
		return true;
	}
	private bool OnRight(bool check = false)
	{
		var bHasEmptySlot = false;
		for (var j = rc - 2; j >= 0; j--)
		{
			for (var i = 0; i < rc; i++)
				if (numbers[i * rc + j] != 0 && numbers[i * rc + j + 1] == 0)
				{
					bHasEmptySlot = true;
					break;
				}
			if (bHasEmptySlot)
				break;
		}
		if (!bHasEmptySlot)
		{
			var bHasSameNumber = false;
			for (var j = rc - 2; j >= 0; j--)
			{
				for (var i = 0; i < rc; i++)
					if (numbers[i * rc + j] != 0
						&& numbers[i * rc + j + 1] == numbers[i * rc + j])
					{
						bHasSameNumber = true;
						break;
					}
				if (bHasSameNumber)
					break;
			}
			if (!bHasSameNumber)
				return false;
		}

		if (check) return true;

		for (var j = rc - 2; j >= 0; j--)
		{
			for (var i = 0; i < rc; i++)
			{
				var ito = i * rc + j + 1;
				var ifrom = i * rc + j;
				if (numbers[ifrom] == 0
					|| (numbers[ito] != 0 && numbers[ito] != numbers[ifrom]))
					continue;
				numbers[ito] += numbers[ifrom];
				numbers[ifrom] = 0;
				AOutput.Log($"ifrom {ifrom} ito {ito}");

				slots[ifrom].SetAsLastSibling();
				var inum = numbers[ito];
				var endTr = slots[ito];
				var replaceTr = dBlocks.ContainsKey(ito) ? dBlocks[ito] : null;
				BlockFromTo(ifrom, ito);
				MoveTo(dBlocks[ito], endTr.position, moveTime, Space.World, () =>
				{
					SetNumber(ito, inum, dBlocks[ito]);
					if (replaceTr != null) ReturnBlockToPool(replaceTr);
				});
			}
		}
		return true;
	}

	private void BlockFromTo(int ifrom, int ito)
	{
		bMoving = true;
		dBlocks.Remove(ito);
		dBlocks.Add(ito, dBlocks[ifrom]);
		dBlocks.Remove(ifrom);
	}

	float moveTime = 0.2f;
	private void ReturnBlockToPool(Transform block)
	{
		blockPools.Enqueue(block);
		block.gameObject.SetActive(false);
	}

	List<int> emptyCells = new List<int>();
	private void SpawnNewCell()
	{
		ShowWidget("alert", false);

		if (!bStart) return;

		emptyCells.Clear();
		for (var i = 0; i < slotCount; i++)
		{
			if (numbers[i] == 0)
				emptyCells.Add(i);
		}

		if (emptyCells.Count == 0)
		{
			OnGameOver(false);
			return;
		}

		var ic = emptyCells[random.Next(emptyCells.Count)];
		var inum = random.Next(2) + 1;
		var b = GetOneBlockFromPool();
		dBlocks.Add(ic, b);
		SetNumber(ic, inum, b);

		if (dBlocks.Count == slotCount)
		{
			DelayDoSth(0.5f, () =>
			{
				if (OnUp(true))
					return;
				if (OnLeft(true))
					return;
				if (OnDown(true))
					return;
				if (OnRight(true))
					return;
				OnGameOver(false);
			});
		}
	}

	private void SetNumber(int ic, int inum, Transform b)
	{
		numbers[ic] = inum;
		FindWidget<Text>(b, "number").text = inum.ToString();
		b.transform.SetParent(slots[ic]);
		b.transform.localPosition = Vector3.zero;
		b.gameObject.SetActive(true);
		AOutput.Log($"Set {ic} Number {inum}");

		if (inum >= 2048)
		{
			DelayDoSth(0.5f, () =>
			{
				OnGameOver(true);
			});
		}
	}

	private Transform GetOneBlockFromPool()
	{
		if (blockPools.Count > 0)
		{
			return blockPools.Dequeue();
		}
		return GameObject.Instantiate(blockTemp, blockTemp.parent);
	}

	private void OnGameOver(bool bSuccess)
	{
		ShowWidget("alert", true);

		bStart = false;
		bMoving = false;
		labelAlert.text = bSuccess ? "成功。" : "失败。";
	}

	private void ClearCells()
	{
		for (var i = 0; i < slotCount; i++)
		{
			numbers[i] = 0;
			if (i < dBlocks.Count)
			{
				dBlocks[i].gameObject.SetActive(false);
				blockPools.Enqueue(dBlocks[i]);
			}
		}
		dBlocks.Clear();

		bStart = true;
		bMoving = true;
		SpawnNewCell();

		DelayDoSth(0.1f, () =>
		{
			bMoving = false;
		});
	}
}
