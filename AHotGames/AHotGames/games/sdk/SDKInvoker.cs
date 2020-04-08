using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SDKInvoker : MonoBehaviour
{
	void Start()
	{
	}
	private void UpdateTransactions()
	{
		UIAlert.Show($"SDKInvoker UpdateTransactions");
	}
	private void FailedTransactions(string str)
	{
		UIAlert.Show($"SDKInvoker FailedTransactions {str}");
	}
}
