using LibPacket;
using System;
using System.Collections.Generic;
using System.Text;

public class Entity
{
	public Entity(Context conta)
	{
		this.conta = conta;
		this.conta.add(this);
	}
	public Action<string> toclient;
	public Context conta { get; private set; }
	public string id { get; set; }
	private string _uid;
	public string uid { get { if (string.IsNullOrEmpty(_uid)) _uid = Guid.NewGuid().ToString(); return _uid; } }

	public Dictionary<string, int> dAttrs = new Dictionary<string, int>();

	public KeyValuePair<int, int> pos = new KeyValuePair<int, int>();
	public LibCommon.EDirection eDirection;

	internal void destroy()
	{
		foreach (var g in lGroups)
		{
			group_out(g);
		}
		lGroups.Clear();
		conta = null;
	}

	List<string> lGroups = new List<string>();
	public void group_in(string name)
	{
		if (lGroups.Contains(name)) return;
		conta.group_in(name, this);
		lGroups.Add(name);
	}
	private void group_out(string name)
	{
		if (!lGroups.Contains(name)) return;
		conta.group_out(name, this);
		lGroups.Remove(name);
	}

	public bool isplayer;
	public bool ismonster;
}
