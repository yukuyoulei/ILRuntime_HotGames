using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibUtils
{
	public class ADictionary<TKey, TValue>
		where TKey : IComparable
	{
		private List<Pair<TKey, TValue>> values = new List<Pair<TKey, TValue>>();
		public void Add(TKey k, TValue v)
		{
			Pair<TKey, TValue> aimKey = null;
			foreach (Pair<TKey, TValue> p in values)
			{
				if (p.left.CompareTo(k) == 0)
				{
					throw new Exception("Already has a same key " + k);
					//return;
				}
			}
			if (aimKey == null)
			{
				Pair<TKey, TValue> p = new Pair<TKey, TValue>(k, v);
				values.Add(p);
			}
		}
		public TValue this[TKey k]
		{
			get
			{
				foreach (Pair<TKey, TValue> p in values)
				{
					if (p.left.CompareTo(k) == 0)
					{
						return p.right;
					}
				}
				return default(TValue);
			}
		}
		public bool ContainsKey(TKey k)
		{
			foreach (Pair<TKey, TValue> p in values)
			{
				if (p.left.CompareTo(k) == 0)
				{
					return true;
				}
			}
			return false;
		}
		public List<TValue> Values
		{
			get
			{
				List<TValue> result = new List<TValue>();
				foreach (Pair<TKey, TValue> p in values)
				{
					result.Add(p.right);
				}
				return result;
			}
		}
		public List<TKey> Keys
		{
			get
			{
				List<TKey> result = new List<TKey>();
				foreach (Pair<TKey, TValue> p in values)
				{
					result.Add(p.left);
				}
				return result;
			}
		}
	}
}
