using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

public class UMMO : AHotBase
{
	Button btnGetQuestion;
	Button btnAnswer;
	Button btnReturn;
	InputField inputAnswer;
	WExpSlider expSlider;

	Transform Cave0;
	protected override void InitComponents()
	{
		Cave0 = FindWidget<Transform>("Cave0");
		var actionHandler = new ActionHandler(Cave0.gameObject);
		actionHandler.AddAction(new ActionRun(actionHandler.handler));
		addUpdateAction(() =>
		{
			if (Input.GetMouseButtonDown(0))
				actionHandler.Do<ActionRun>().Play();
			if (Input.GetMouseButtonUp(0))
				actionHandler.Do<ActionRun>().Stop();
			return false;
		});

		var tr = FindWidget<Transform>("expBg");
		expSlider = new WExpSlider();
		expSlider.SetGameObj(tr.gameObject);

		FindWidget<Text>("textAvatarname").text = URemoteData.AvatarName;
		ShowLevel();

		inputAnswer = FindWidget<InputField>("inputAnswer");

		btnReturn = FindWidget<Button>("btnReturn");
		btnReturn.onClick.AddListener(() =>
		{
			OnUnloadThis();

			LoadAnotherUI<UIMain>();
		});

		btnGetQuestion = FindWidget<Button>("btnGetQuestion");
		btnGetQuestion.onClick.AddListener(() =>
		{
			btnGetQuestion.gameObject.SetActive(false);
			UStaticWebRequests.OnWebRequest("Question/GetOne", "username=" + UILogin.CachedUsername + "&token=" + UILogin.token, jobj =>
			{
				ShowWidget("question", true);
				FindWidget<Text>("textQuestion").text = jobj["q"].ToString();
			},
			jfail =>
			{
				ShowWidget("question", false);
				btnGetQuestion.gameObject.SetActive(true);
			});
		});

		ShowWidget("question", false);
		btnAnswer = FindWidget<Button>("btnAnswer");
		btnAnswer.onClick.AddListener(() =>
		{
			if (string.IsNullOrEmpty(inputAnswer.text))
			{
				return;
			}
			UStaticWebRequests.OnWebRequest("Question/Answer", "username=" + UILogin.CachedUsername + "&token=" + UILogin.token + "&answer=" + inputAnswer.text, jobj =>
			{
				if (jobj.ContainsKey("avatar"))
				{
					ShowWidget("question", false);
					URemoteData.OnReceiveAvatarData(jobj["avatar"].ToString());
					UIAlert.Show("回答正确！");
					btnGetQuestion.gameObject.SetActive(true);
				}
				else
				{
					UIAlert.Show("回答错误，再好好想想吧。");
				}
			},
			jfail =>
			{
				UIAlert.Show("回答错误，再好好想想吧。");
			});
		});


		URemoteData.ListeningParam(InfoNameDefs.AvatarLevel, ShowLevel);

	}
	protected override void OnDestroy()
	{
		URemoteData.CancelListeningParam(InfoNameDefs.AvatarLevel, ShowLevel);
	}

	private void ShowLevel()
	{
		FindWidget<Text>("textLevel").text = URemoteData.AvatarLevel;
	}
}

