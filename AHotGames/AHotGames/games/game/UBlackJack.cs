using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 
/// Written by FS
/// 
/// </summary>
public class UBlackJack : AGameBase
{
    protected override void InitComponents()
    {
        Start1();
        gameObj.AddComponent<UOnGUIer>().onOnGUI = () =>
        {
            OnGUI();
        };
    }
    private List<int> lAllCards;
    private List<int> lLastCards;
    void Start1()
    {
        lAllCards = new List<int>();
        for (int i = 1; i <= 52; i++)
        {
            lAllCards.Add(i);
        }

        SwipeCardsX = Screen.width / 2 - cardWidth / 2;
        SwipeCardsY = Screen.height - cardHeight * 1.5f;
        LeftCardsX = cardWidth * 0.5f;
        LeftCardsY = cardHeight * 0.5f;
        RightCardX = Screen.width - cardWidth * 1.5f;
        RightCardY = cardHeight * 0.5f;
    }


    int btnWidth = 160;
    int btnHeight = 120;
    bool bStarted = false;

    private List<int> lLeftCards = new List<int>();
    private int RightCard;

    private Vector2 rawPos;
    private float SwipeCardsX, SwipeCardsY;
    private float SwipingCardsX, SwipingCardsY;
    private float LeftCardsX, LeftCardsY;
    private float TargetLeftCardsX, TargetLeftCardsY;
    private float RightCardX, RightCardY;
    bool bAddToLeft;
    bool bSetToRight;
    private int SwipingCard;
    bool bBlackJack;
    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), results[0] + ((results[1] > 0 && results[1] != results[0]) ? "|" + results[1] : ""));

        for (int i = 0; i < lLeftCards.Count; i++)
        {
            DrawCard(LeftCardsX + i % 5 * cardWidth, LeftCardsY + i / 5 * cardHeight * 0.2f, lLeftCards[i]);
        }
        if (RightCard > 0)
        {
            DrawCard(RightCardX, RightCardY, RightCard);
        }

        if (Result != 0)
        {
            if (bBlackJack)
            {
                GUI.Label(new Rect(Screen.width / 2 - btnWidth / 2, Screen.height / 2 - btnHeight / 2 - 40, btnWidth, btnHeight), "黑杰克！");
            }
            GUI.Label(new Rect(Screen.width / 2 - btnWidth / 2, Screen.height / 2 - btnHeight / 2 - 20, btnWidth, btnHeight), Result == 1 ? "成功" : "失败");
            if (GUI.Button(new Rect((Screen.width - btnWidth) / 2, (Screen.height - btnHeight) / 2, btnWidth, btnHeight), "继  续"))
            {
                DoStart();
            }
            return;
        }

        DrawCard(SwipeCardsX, SwipeCardsY, 0);
        if (!bStarted)
        {
            GUI.Label(new Rect(Screen.width / 2 - btnWidth / 2, Screen.height / 2 - btnHeight / 2 - 20, btnWidth * 2, btnHeight), "左滑小于等于21，右滑大于21");
            if (GUI.Button(new Rect((Screen.width - btnWidth) / 2, (Screen.height - btnHeight) / 2, btnWidth, btnHeight), "开  始"))
            {
                DoStart();
            }
            return;
        }

        if (bAddToLeft)
        {
            SwipingCardsX += (TargetLeftCardsX - SwipingCardsX) * 0.1f;
            SwipingCardsY += (TargetLeftCardsY - SwipingCardsY) * 0.1f;
            DrawCard(SwipingCardsX, SwipingCardsY, 0);
            if (Mathf.Abs(TargetLeftCardsX - SwipingCardsX) < 1)
            {
                bAddToLeft = false;
                lLeftCards.Add(SwipingCard);

                DoCalculate();
            }
            return;
        }
        if (bSetToRight)
        {
            SwipingCardsX += (RightCardX - SwipingCardsX) * 0.1f;
            SwipingCardsY += (RightCardY - SwipingCardsY) * 0.1f;
            DrawCard(SwipingCardsX, SwipingCardsY, 0);
            if (Mathf.Abs(RightCardY - SwipingCardsY) < 1)
            {
                bSetToRight = false;
                RightCard = SwipingCard;

                DoCalculate();
            }
            return;
        }

        if (Input.GetMouseButtonDown(0)
            && Input.mousePosition.x > SwipeCardsX
            && Input.mousePosition.y < Screen.height - SwipeCardsY
            && Input.mousePosition.x < SwipeCardsX + cardWidth)
        {
            rawPos = Input.mousePosition;
        }
        if (Input.GetMouseButton(0) && rawPos != Vector2.zero)
        {
            SwipingCardsX = SwipeCardsX + Input.mousePosition.x - rawPos.x;
            SwipingCardsY = SwipeCardsY + rawPos.y - Input.mousePosition.y;
            DrawCard(SwipingCardsX, SwipingCardsY, 0);
        }
        if (Input.GetMouseButtonUp(0) && rawPos != Vector2.zero)
        {
            if (SwipeCardsX - SwipingCardsX > cardWidth * 0.5f)
            {
                TargetLeftCardsX = LeftCardsX + lLeftCards.Count % 5 * cardWidth;
                TargetLeftCardsY = LeftCardsY + lLeftCards.Count / 5 * cardHeight * 0.2f;
                bAddToLeft = true;
                GetRandomCard();
            }
            else if (SwipingCardsX - SwipeCardsX - cardWidth > cardWidth * 0.5f && lLeftCards.Count > 0)
            {
                bSetToRight = true;
                GetRandomCard();
            }

            rawPos = Vector2.zero;
        }
    }

    int Result = 0;
    int[] results = new int[] { 0, 0 };
    private int DoCalculate()
    {
        results[0] = 0;
        results[1] = 0;
        foreach (int icard in lLeftCards)
        {
            int inum = ToCardNum(icard);
            if (inum == 1)
            {
                results[0] += 1;
                results[1] += 11;
            }
            else
            {
                results[0] += inum;
                results[1] += inum;
            }
            if (results[1] > 0 && results[1] != results[0])
            {
                if (results[1] > 21)
                {
                    results[1] = 0;
                }
            }
        }

        if (results[0] == 21 || results[1] == 21)
        {
            results[0] = 21;
            results[1] = 0;

            bBlackJack = true;
            Result = 1;
            return Result;
        }

        if (results[0] > 21)
        {
            Result = -1;
            return Result;
        }

        if (RightCard > 0)
        {
            results[1] = 0;
            results[0] += ToCardNum(RightCard);
            if (results[0] > 21)
            {
                Result = 1;
                return Result;
            }
            else
            {
                Result = -1;
                return Result;
            }
        }
        return Result;
    }

    void GetRandomCard()
    {
        int idx = UnityEngine.Random.Range(0, lLastCards.Count);
        Debug.Log("SwipingCard:" + idx);
        SwipingCard = lLastCards[idx];
        lLastCards.RemoveAt(idx);
    }

    float cardWidth = 80;
    float cardHeight = 120;
    void DrawCard(float x, float y, int card)
    {
        GUI.Box(new Rect(x, y, cardWidth, cardHeight), "");
        if (card > 0)
        {
            GUI.Label(new Rect(x + 5, y + 5, 20, 20), ToCardColor(card));
            GUI.Label(new Rect(x + 25, y + 5, 20, 20), ToCardName(card));
        }
    }

    private void DoStart()
    {
        bStarted = true;
        Result = 0;
        bBlackJack = false;

        lLeftCards.Clear();
        RightCard = 0;

        results[0] = 0;
        results[1] = 0;

        if (lLastCards == null
            || lLastCards.Count < 10)
        {
            lLastCards = new List<int>(lAllCards);
        }
    }

    string[] sCardColor = new string[] { "♠", "♥", "♣", "♦" };
    string ToCardColor(int card)
    {
        return sCardColor[(card - 1) / 13];
    }
    int ToCardNum(int card)
    {
        int inum = card % 13;
        if (inum == 0 || inum > 10)
        {
            return 10;
        }
        return inum;
    }
    string ToCardName(int card)
    {
        int inum = card % 13;
        if (inum == 0 || inum > 10)
        {
            switch (inum)
            {
                case 11:
                    return "J";
                case 12:
                    return "Q";
                case 0:
                    return "K";
            }
        }
        return inum.ToString();
    }
}