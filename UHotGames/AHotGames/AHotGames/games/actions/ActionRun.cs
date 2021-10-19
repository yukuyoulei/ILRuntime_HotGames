using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ActionRun : ActionBase, IActionBase
{
	public ActionRun(Animator handler) : base(handler)
	{
	}

	public void Play()
	{
		handler.SetTrigger("run");
		handler.SetBool("running", true);
	}

	public void Stop()
	{
		handler.SetBool("running", false);
	}
}
