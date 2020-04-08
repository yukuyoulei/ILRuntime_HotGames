using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using System.Threading.Tasks;
using LibClient.GameObj;

public class AEntrance : AHotBase
{
	protected override void InitComponents()
	{
		AOutput.Register(UDebugHotLog.Log);

		var i = UEventListener.Instance;

		LibClient.AClientApp.Init(new AOnlineSubsystem());

		new CakeClient("pinfo", "1", "1");
		PreDownloadResources();

		var SDK_AppleInApp = GameObject.Find("SDK_AppleInApp");
		if (SDK_AppleInApp != null)
			SDK_AppleInApp.AddComponent<SDKInvoker>();
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

	};
	private void PreDownloadResources()
	{
		UHotAssetBundleLoader.Instance.OnDownloadResources(new List<string> { Utils.GetPlatformFolder(Application.platform)
			, Utils.GetPlatformFolder(Application.platform)+ ".manifest" }, () =>
		{
			UHotAssetBundleLoader.Instance.OnDownloadResources(new List<string> { "ui/uidebugconsole.ab" }, () =>
			{
				LoadUI<UIDebugConsole>();

				UHotAssetBundleLoader.Instance.OnDownloadResources(new List<string> { "ui/uiloading.ab" }, () =>
				{
					LoadUI<UILoading>();

					ConfigManager.Instance.DownloadConfig(() =>
					{
						UHotAssetBundleLoader.Instance.OnDownloadResources(new List<string>(preloadResources), () =>
						{
							UILoading.Instance?.OnUnloadThis();

							//LoadUI<UILogin>();
							LoadUI<UIMinerLogin>();
						}, null);
					});
				}, null);
			}, null);
		}, null);
	}
}

