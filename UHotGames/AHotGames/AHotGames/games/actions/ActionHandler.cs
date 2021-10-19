using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ActionHandler
{
	private GameObject model;
	private Animator _handler;
	public Animator handler { get { if (_handler) return _handler; _handler = model.GetComponent<Animator>(); return _handler; } }
	public ActionHandler(GameObject model) { this.model = model; }
	private List<ActionBase> actions = new List<ActionBase>();
	public ActionHandler AddAction<A>(A a) where A : ActionBase
	{
		actions.Add(a);
		return this;
	}
	public IActionBase Do<T>() where T : class, IActionBase
	{
		foreach (var a in actions)
		{
			if (a is T) return a as T;
		}
		return null;
	}
}
