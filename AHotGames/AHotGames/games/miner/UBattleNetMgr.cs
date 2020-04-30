using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class UBattleNetMgr : Singleton<UBattleNetMgr>
{
	public void Init()
	{
		UEventListener.Instance.OnRegisterEvent(UEvents.EnterGame, OnEnterGameCb);
		UEventListener.Instance.OnRegisterEvent(UEvents.ContaData, OnContaDataCb);
		UEventListener.Instance.OnRegisterEvent(UEvents.ServerDisconnected, OnServerDisconnectedCb);
		UEventListener.Instance.OnRegisterEvent(UEvents.CreatePlayer, OnCreatePlayer);
		UEventListener.Instance.OnRegisterEvent(UEvents.Settlement, OnSettlement);
	}

	public Action settlementAction;
	public void RegisterSettlement(Action action)
	{
		settlementAction = action;
	}
	private void OnSettlement(UEventBase obj)
	{
		if (settlementAction != null)
		{
			settlementAction();
			settlementAction = null;
			return;
		}
		var eb = obj as EventSettlement;
		AHotBase.LoadUI<UISingleSettlement>(ui => { ui.SetData(eb); });
	}

	private void OnCreatePlayer(UEventBase obj)
	{
		var eb = obj as EventCreatePlayer;
		if (eb.pkt.psid == LibClient.GameObj.CakeAvatar.myID)
			UILoading.Instance.OnUnloadThis();
		AClientApis.OnBeginFight();
	}

	private static void OnServerDisconnectedCb(UEventBase obj)
	{
		AHotBase.UnloadAllClasses();
		AHotBase.LoadAnotherUI<UIMinerLogin>();
	}

	private void OnEnterGameCb(UEventBase obj)
	{
		AHotBase.UnloadAllClasses();
		var eb = obj as EventEnterGame;
		if (eb.info == null)
		{
			AHotBase.LoadAnotherUI<UIMinerCreateAvatar>();
		}
		else
		{
			AClientApis.OnGetSdata();
		}
	}
	private void OnContaDataCb(UEventBase obj)
	{
		AHotBase.UnloadAllClasses();
		AHotBase.LoadUI<UILoading>();

		var data = obj as EventContaData;
		var id = data.id;
		var map = MapLoader.Instance.OnGetData(id);
		UICommonWait.Show();
		UHotAssetBundleLoader.Instance.OnDownloadResources(() =>
		{
			UICommonWait.Hide();
			AHotBase.LoadAnotherClass(map.MapClass, map.Prefab);
		}, map.Prefab);
	}
}
