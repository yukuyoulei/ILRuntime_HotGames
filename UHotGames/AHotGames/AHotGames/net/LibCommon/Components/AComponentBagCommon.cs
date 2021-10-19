using System;
using System.Collections.Generic;
using System.Linq;
using LibCommon;

public class AComponentBagCommon : AComponentBase
{
	public AComponentBagCommon(AGameObj owner) : base(owner)
	{
	}
	public override void InitComponent()
	{
		
	}
	Dictionary<int, int> dItems = new Dictionary<int, int>();
	public virtual void OnAddItem(int itemID, int count)
	{
		DoAddItem(itemID, count);

		if (dItems[itemID] <= 0)
		{
			dItems.Remove(itemID);
		}
	}

	internal void OnRemoveItem(int id, int count)
	{
		if (count > 0)
		{
			count = -count;
		}
		OnAddItem(id, count);
	}

	protected void DoAddItem(int itemID, int count)
	{
		if (dItems.ContainsKey(itemID))
		{
			dItems[itemID] += count;
		}
		else if(count > 0)
		{
			dItems.Add(itemID, count);
		}
	}

	public int OnGetItemCount(int itemID)
	{
		if (dItems.ContainsKey(itemID))
		{
			return dItems[itemID];
		}
		return 0;
	}
}
