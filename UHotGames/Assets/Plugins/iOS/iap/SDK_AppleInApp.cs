using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Runtime.InteropServices;
public class SDK_AppleInApp : MonoBehaviour
{
	public List<string> productInfo = new List<string>(
		new string[]
		{
			"com.tigermath.ios.price.class1",
		}
		);
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
	private string rawOrderID;
	public static Action actionStartToBuy;
	public void OnBuyProduct(int productIndex, string orderID)
	{
#if UNITY_EDITOR
		return;
#endif
#if UNITY_IOS
		actionStartToBuy?.Invoke();
		rawOrderID = orderID;
		BuyProduct(productInfo[productIndex]);
#endif
	}
	public static Action<string, string> actionReceivedReceiptData;
	private void ProvideContent(string data)
	{
		Debug.Log("ProvideContent " + data);
		actionReceivedReceiptData?.Invoke(rawOrderID, data);
	}
	public static Action actionEndBuying;
	private void UpdateTransactions()
	{
		actionEndBuying?.Invoke();
	}
}
