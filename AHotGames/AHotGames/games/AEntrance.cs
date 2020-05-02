using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using System.Threading.Tasks;
using LibClient.GameObj;
using System.Resources;

public class AEntrance : AHotBase
{
	protected override void InitComponents()
	{
		AOutput.Register(UDebugHotLog.Log);

		var i = UEventListener.Instance;

		if (!Environment.bUsingLocalCDN)
			LibClient.AClientApp.SetEndpoint("69.51.23.197", 999);
		AOutput.Log($"LibClient.AClientApp Connect {LibClient.AClientApp.ip}:{LibClient.AClientApp.port}");

		LibClient.AClientApp.Init(new AOnlineSubsystem());

		PreDownloadResources();
	}

	private string[] preloadResources = new string[] {
		  "ui/uilogin.ab"

		, "ui/uirank.ab"
		, "ui/uimain.ab"
		, "ui/uiwait.ab"
		, "ui/uialert.ab"
		, "ui/uiregister.ab"
		, "ui/uicommontips.ab"
		, "ui/uicommonwait.ab"
		, "ui/uicreateavatar.ab"
		, "ui/uiminermain.ab"
    };
	private void PreDownloadResources()
	{
		UHotAssetBundleLoader.Instance.OnDownloadResources(new List<string> { Utils.GetPlatformFolder()
			, Utils.GetPlatformFolder()+ ".manifest" }, () =>
		{
			UHotAssetBundleLoader.Instance.OnDownloadResources(new List<string> { "ui/uidebugconsole.ab" }, () =>
			{
				LoadUI<UIDebugConsole>();

				UHotAssetBundleLoader.Instance.OnDownloadResources(new List<string> { "ui/uiloading.ab" }, () =>
				{
					LoadUI<UILoading>();

					ConfigManager.Instance.DownloadConfig(() =>
					{
						var lall = preloadResources.ToList();
						foreach (var d in MapLoader.Instance.AllData)
						{
							lall.Add(d.Prefab);
						}
						UHotAssetBundleLoader.Instance.OnDownloadResources(lall, () =>
						{
							UILoading.Instance?.OnUnloadThis();

							DoInit();
						}, null);
					});
				}, null);
			}, null);
		}, null);
	}

	private void DoInit()
	{
		//LoadUI<UILogin>();
		UBattleNetMgr.Instance.Init();
		LoadUI<UIMinerLogin>();
	}
}

