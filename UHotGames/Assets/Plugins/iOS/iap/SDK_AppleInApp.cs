using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Runtime.InteropServices;
public class SDK_AppleInApp : MonoBehaviour
{
#if UNITY_IOS

	[DllImport("__Internal")]
	private static extern void InitIAPManager();//初始化

	[DllImport("__Internal")]
	private static extern bool IsProductAvailable();//判断是否可以购买

	[DllImport("__Internal")]
	private static extern void RequstProductInfo(string s);//获取商品信息

	[DllImport("__Internal")]
	private static extern void BuyProduct(string s);//购买商品
#endif

	private void Awake()
	{
#if UNITY_EDITOR
		return;
#endif
#if UNITY_IOS
		InitIAPManager();
#endif
	}
	public void OnBuyProduct(string productInfo)
	{
#if UNITY_EDITOR
		return;
#endif
#if UNITY_IOS
		BuyProduct(productInfo);
#endif
	}
	private void ProvideContent(string data)//ReceivedReceiptData
	{
		ILRuntimeHandler.Instance.EmitMessage($"ProvideContent {data}");
	}
	private void UpdateTransactions()
	{
		ILRuntimeHandler.Instance.EmitMessage($"UpdateTransactions");
	}
}
