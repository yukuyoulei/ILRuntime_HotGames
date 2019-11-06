using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct ACards
{
	public ACards(int content)
	{
		this.content = content;
	}
	public int content;
	public string resourcePath
	{
		get
		{
			return $"Images/Pai/{content + 1}";
		}
	}
	public EColor color
	{
		get
		{
			return (EColor)(content / 13);
		}
	}
	public int number
	{
		get
		{
			return content >= 52 ? (13 + content % 13) : content % 13;
		}
	}
	public enum EColor
	{
		Spade,
		Heart,
		Diamond,
		Club,
		Joker,
	}
	public enum ENumber
	{
		NA, N2, N3, N4, N5, N6, N7, N8, N9, N10, NJ, NQ, NK, NXJoker, NDJoker,
	}
}
