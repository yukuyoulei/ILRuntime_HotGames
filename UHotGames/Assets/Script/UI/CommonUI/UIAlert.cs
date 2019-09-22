using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAlert : UIBase
{
	protected override bool KeepUsingOverlay
	{
		get
		{
			return true;
		}
	}

	Text Content;
	Button btnConfirm;
	Button btnCancel;
	protected override void InitComponents()
	{
		Content = UStaticFuncs.FindChildComponent<Text>(transform, "Content");
		btnConfirm = UStaticFuncs.FindChildComponent<Button>(transform, "btnConfirm");
		btnCancel = UStaticFuncs.FindChildComponent<Button>(transform, "btnCancel");
		btnConfirm.onClick.AddListener(OnConfirm);
		btnCancel.onClick.AddListener(OnCancel);

		DontDestroyOnLoad(gameObject);
	}

	private void OnCancel()
	{
		if (cancelAction != null)
		{
			cancelAction();
		}
		OnCloseUI();
	}

	private Action confirmAction;
	private Action cancelAction;
	private void OnConfirm()
	{
		if (confirmAction != null)
		{
			confirmAction();
		}
		if (dontDestroyTr == transform)
		{
			transform.gameObject.SetActive(false);
		}
		else
		{
			OnCloseUI();
		}
	}

	public static UIAlert Show(string content, Action confirmAction, float autoDestroySec)
	{
		sintance = Show(content, confirmAction, true, true);
		sintance.SetAutoDestroy(autoDestroySec);
		return sintance;
	}

	string buttonContent;
	Text textButton;
	private void SetAutoDestroy(float autoDestroySec)
	{
		textButton = btnConfirm.GetComponentInChildren<Text>();
		buttonContent = textButton.text;
		StartCoroutine(AutoDestroy(autoDestroySec));
	}

	private IEnumerator AutoDestroy(float autoDestroySec)
	{
		yield return new WaitForSeconds(1);
		autoDestroySec -= 1;
		textButton.text = buttonContent + "(" + autoDestroySec + ")";
		if (autoDestroySec < 0)
		{
			if (confirmAction != null)
			{
				confirmAction();
			}
			OnCloseUI();
		}
		else
		{
			StartCoroutine(AutoDestroy(autoDestroySec));
		}
	}

	static Transform dontDestroyTr;
	static UIAlert sintance;
	public static UIAlert Show(string content, Transform prefab = null)
	{
		return Show(content, null, false, true, null, prefab, true);
	}
	public static UIAlert Show(string content, Action confirmAction = null, bool bHideConfirmButton = false, bool bHideCancelButton = false, Action cancelAction = null, Transform prefab = null, bool bDontDestroy = false)
	{
		Debug.Log("alert " + content);
		if (prefab != null)
		{
			if (bDontDestroy)
			{
				dontDestroyTr = prefab;
			}
			prefab.gameObject.SetActive(true);
			sintance = prefab.gameObject.AddComponent<UIAlert>();
		}
		else
		{
			return null;
		}
		sintance.Content.text = content;
		sintance.confirmAction = confirmAction;
		sintance.cancelAction = cancelAction;
		sintance.btnConfirm.gameObject.SetActive(!bHideConfirmButton);
		sintance.btnCancel.gameObject.SetActive(!bHideCancelButton);
		return sintance;
	}
	public static void Hide()
	{
		if (sintance != null)
		{
			sintance.OnCloseUI();
		}
	}
}
