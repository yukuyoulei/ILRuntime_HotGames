using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class ActionBase
{
	protected Animator handler;
	public ActionBase(Animator handler) { this.handler = handler; }
}
