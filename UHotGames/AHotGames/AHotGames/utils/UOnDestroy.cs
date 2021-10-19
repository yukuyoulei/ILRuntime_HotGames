using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class UOnDestroy : MonoBehaviour
{
	public Action actionOnDestroy;
	private void Start()
	{
		
	}
	private void OnDestroy()
	{
		actionOnDestroy?.Invoke();
	}
}
