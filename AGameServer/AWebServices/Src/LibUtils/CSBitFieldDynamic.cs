using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CSBitFieldDynamic
{
	private int m_uintSize;

	private List<uint> m_Bitfield = new List<uint>();
	public CSBitFieldDynamic()
	{
		m_uintSize = 32;
	}

	public bool Set(uint uBitIndex, bool bValue)
	{
		if (uBitIndex >= m_Bitfield.Count * m_uintSize)
		{
			// - out of need to grow/resize
			while (m_Bitfield.Count * m_uintSize <= uBitIndex)
			{
				m_Bitfield.Add(new uint());
			}
		}

		if (bValue)
		{
			// - set bit
			m_Bitfield[(int)(uBitIndex / m_uintSize)] |= (uint)(1 << ((int)uBitIndex % m_uintSize));
		}
		else
		{
			// - clear bit
			m_Bitfield[(int)(uBitIndex / m_uintSize)] &= ~(uint)(1 << ((int)uBitIndex % m_uintSize));
		}

		return true;

	}
	public bool Test(uint uBitIndex)
	{
		if (!InRange(uBitIndex))
		{
			// - out of range
			return false;
		}

		return 0 != (m_Bitfield[(int)uBitIndex / m_uintSize] & (1 << ((int)uBitIndex % m_uintSize)));
	}
	private bool InRange(uint uBitIndex)
	{
		if (uBitIndex >= m_Bitfield.Count * m_uintSize)
		{
			// - out of range
			return false;
		}

		return true;
	}
	public override string ToString()
	{
		string sResult = "";

		for (int ii = m_Bitfield.Count - 1; ii >= 0; ii--)
		{
			uint uBits = m_Bitfield[ii];

			sResult += GetHexValue((uBits & 0xf0000000) >> 28).ToString()
				+ GetHexValue((uBits & 0x0f000000) >> 24).ToString()
				+ GetHexValue((uBits & 0x00f00000) >> 20).ToString()
				+ GetHexValue((uBits & 0x000f0000) >> 16).ToString()
				+ GetHexValue((uBits & 0x0000f000) >> 12).ToString()
				+ GetHexValue((uBits & 0x00000f00) >> 8).ToString()
				+ GetHexValue((uBits & 0x000000f0) >> 4).ToString()
				+ GetHexValue((uBits & 0x0000000f)).ToString();
		}

		return sResult;
	}
	private string GetHexValue(uint uNibble)
	{
		switch (uNibble)
		{
			case 0: return "0";
			case 1: return "1";
			case 2: return "2";
			case 3: return "3";
			case 4: return "4";
			case 5: return "5";
			case 6: return "6";
			case 7: return "7";
			case 8: return "8";
			case 9: return "9";
			case 10: return "A";
			case 11: return "B";
			case 12: return "C";
			case 13: return "D";
			case 14: return "E";
			case 15: return "F";
			default:
				return "-";
		}
	}
	public bool FromString(string sValue)
	{
		if (!IsValidStr(sValue))
		{
			return false;
		}

		int nLen = sValue.Length;

		// 			m_Bitfield.Capacity = 0;
		// 			m_Bitfield.Capacity = (nLen+7) / 8;
		// 
		uint uBit = 0;

		for (int ii = nLen - 1; ii >= 0; ii--)
		{
			string cHex = sValue.Substring(ii, 1);
			uint uNibble = GetNibbleValue(cHex);

			Set(uBit + 0, 0 != (uNibble & 0x01));
			Set(uBit + 1, 0 != (uNibble & 0x02));
			Set(uBit + 2, 0 != (uNibble & 0x04));
			Set(uBit + 3, 0 != (uNibble & 0x08));

			uBit += 4;
		}

		return true;
	}
	private bool IsValidStr(string szValue)
	{
		uint uPos = 0;

		while (uPos < szValue.Length)
		{
			char cChar = szValue.ToCharArray((int)uPos, 1)[0];
			uPos++;

			if (cChar >= '0' && cChar <= '9')
			{
				continue;
			}

			if (cChar >= 'A' && cChar <= 'F')
			{
				continue;
			}

			if (cChar >= 'a' && cChar <= 'f')
			{
				continue;
			}

			return false;
		}

		return true;
	}
	private uint GetNibbleValue(string cHex)
	{
		switch (cHex)
		{
			case "0": return 0;
			case "1": return 1;
			case "2": return 2;
			case "3": return 3;
			case "4": return 4;
			case "5": return 5;
			case "6": return 6;
			case "7": return 7;
			case "8": return 8;
			case "9": return 9;
			case "A": return 10;
			case "B": return 11;
			case "C": return 12;
			case "D": return 13;
			case "E": return 14;
			case "F": return 15;
			case "a": return 10;
			case "b": return 11;
			case "c": return 12;
			case "d": return 13;
			case "e": return 14;
			case "f": return 15;
			default:
				return 0;
		}
	}
}
