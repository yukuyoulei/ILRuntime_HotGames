using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class UUpdater : MonoBehaviour
{
	public void Start()
	{
		//这个方法是必须的，否则Update方法不会被调用
	}
	public Action onUpdate;
	public void Update()
	{
		if (onUpdate != null)
		{
			onUpdate();
		}
	}
}

