using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using System.Threading.Tasks;
using LibClient.GameObj;
using System.Resources;

/*
xcopy /r /f /y $(TargetDir)\$(ProjectName).dll $(TargetDir)..\..\..\ab1\AHotGames.bytes
xcopy /r /f /y $(TargetDir)\$(ProjectName).pdb $(TargetDir)..\..\..\ab1\AHotGames.pdb
 * */
//https://docs.unity3d.com/Manual/ClassIDReference.html  //Classes Ordered by ID Number
public class AEntrance
{
	public static void Initialize(string platform)
	{
		AOutput.Register(UDebugHotLog.Log);

		var i = UEventListener.Instance;

		if (!HotEnvironment.bUsingLocalCDN)
			LibClient.AClientApp.SetEndpoint("69.51.23.197", 999);
		AOutput.Log($"LibClient.AClientApp Connect {LibClient.AClientApp.ip}:{LibClient.AClientApp.port}");

		LibClient.AClientApp.Init(new AOnlineSubsystem());

		PreDownloadResources();
	}

	private static string[] preloadResources = new string[] {
		"ui/uiwait.ab"
		, "ui/uialert.ab"
		, "ui/uiregister.ab"
		, "ui/uicommontips.ab"
		, "ui/uicommonwait.ab"
    };
	private static void PreDownloadResources()
	{
		UHotAssetBundleLoader.Instance.OnDownloadResources(new List<string> { "/" + Utils.GetPlatformFolder()
			, "/" + Utils.GetPlatformFolder()+ ".manifest" }, () =>
		{
			UHotAssetBundleLoader.Instance.OnDownloadResources(new List<string> { "ui/uidebugconsole.ab" }, () =>
			{
				AHotBase.LoadUI<UIDebugConsole>();

				UHotAssetBundleLoader.Instance.OnDownloadResources(new List<string> { "ui/uiloading.ab" }, () =>
				{
					AHotBase.LoadUI<UILoading>();

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
		}, null, false);
	}

	private static void DoInit()
	{
		//LoadUI<UILogin>();
		UBattleNetMgr.Instance.Init();
		AHotBase.LoadUI<UIMinerLogin>();
	}
}

