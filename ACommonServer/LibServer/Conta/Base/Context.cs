using System;
using System.Collections.Generic;
using System.Text;

public abstract class Context
{
	protected Dictionary<Type, SystemBase> dSystems = new Dictionary<Type, SystemBase>();
	protected void AddSystem(SystemBase system)
	{
		var type = system.GetType();
		if (dSystems.ContainsKey(type)) throw new Exception($"Duplicate system {type}");
		dSystems.Add(type, system);
	}

	public Dictionary<string, Entity> entities = new Dictionary<string, Entity>();
	internal void add(Entity entity)
	{
		entities.Add(entity.uid, entity);
	}
	internal void remove(Entity entity)
	{
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
