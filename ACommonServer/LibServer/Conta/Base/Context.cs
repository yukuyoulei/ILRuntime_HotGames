using System;
using System.Collections.Generic;
using System.Text;

public abstract class Context
{
	public int conf_id;
	public string ownerID { get; private set; }
	public bool gameover = false;
	public string failed_reason = "";
	protected DateTime startTime;
	protected int lastSeconds = -1;
	public bool enabled = false;
	public bool IsTimeout
	{
		get
		{
			if (lastSeconds == -1) return false;
			return (DateTime.Now - startTime).TotalSeconds >= lastSeconds;
		}
	}
	protected void OnStart()
	{
		startTime = DateTime.Now;
	}
	public Context(int confid)
	{
		this.OnStart();
		this.conf_id = confid;
	}
	public Context(int confid, string ownerID)
	{
		this.ownerID = ownerID;
		this.conf_id = confid;
	}
	public MapData mapConf
	{
		get
		{
			return MapLoader.Instance.OnGetData(conf_id);
		}
	}
	public int PlayerCountMax
	{
		get
		{
			return mapConf.MaxNum;
		}
	}
	public bool CanEnter(string psid)
	{
		if (mapConf.MultiInstance == 0 && psid != ownerID) return false;
		return dAllPlayers.Count + 1 <= PlayerCountMax;
	}
	protected Dictionary<Type, SystemBase> dSystems = new Dictionary<Type, SystemBase>();
	protected void AddSystem(SystemBase system)
	{
		var type = system.GetType();
		if (dSystems.ContainsKey(type)) throw new Exception($"Duplicate system {type}");
		dSystems.Add(type, system);
	}
	protected Dictionary<string, Entity> dAllPlayers = new Dictionary<string, Entity>();
	public void AddPlayer(Entity entity)
	{
		if (dAllPlayers.ContainsKey(entity.psid)) return;
		dAllPlayers.Add(entity.psid, entity);
	}
	public Dictionary<string, Entity> entities = new Dictionary<string, Entity>();
	internal void add(Entity entity)
	{
		entities.Add(entity.uid, entity);
	}
	internal void remove(Entity entity)
	{
		if (dAllPlayers.ContainsKey(entity.psid)) dAllPlayers.Remove(entity.psid);
		entities.Remove(entity.uid);
	}

	protected T GetSystem<T>() where T : SystemBase
	{
		var type = typeof(T);
		return dSystems[type] as T;
	}

	Dictionary<string, List<Entity>> dGroups = new Dictionary<string, List<Entity>>();
	internal void group_in(string name, Entity entity)
	{
		if (!dGroups.ContainsKey(name))
			dGroups.Add(name, new List<Entity>());
		dGroups[name].Add(entity);
	}

	internal void group_out(string name, Entity entity)
	{
		dGroups[name].Remove(entity);
	}
}
