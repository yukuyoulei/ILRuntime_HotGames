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
	InputField inputAnswer;
	Image expProgress;
	Text textExp;
	protected override void InitComponents()
	{
		FindWidget<Text>("textAvatarname").text = URemoteData.AvatarName;
		FindWidget<Text>("textLevel").text = URemoteData.AvatarLevel;

		expProgress = FindWidget<Image>("expProgress");
		textExp = FindWidget<Text>("textExp");
		textExp.text = "";

		inputAnswer = FindWidget<InputField>("inputAnswer");

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

		URemoteData.ListeningParam(InfoNameDefs.CurExp, ShowCurExp);
		URemoteData.ListeningParam(InfoNameDefs.MaxExp, ShowCurExp);

		RefreshUI();
	}

	private void RefreshUI()
	{
		ShowCurExp();
	}

	private void ShowCurExp()
	{
		if (URemoteData.MaxExp == 0)
		{
			return;
		}
		textExp.text = URemoteData.CurExp + "/" + URemoteData.MaxExp;
		expProgress.fillAmount = URemoteData.CurExp / (float)URemoteData.MaxExp;
	}
}

