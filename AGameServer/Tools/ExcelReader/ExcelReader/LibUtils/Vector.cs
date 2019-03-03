using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibUtils
{
	public struct Vector2
	{
		public double x, y;
		public Vector2(double x, double y)
		{
			this.x = x;
			this.y = y;
		}
		public static double Distance(double lx, double ly, double rx, double ry)
		{
			double deltax = lx - rx;
			double deltay = ly - ry;
			return Math.Sqrt(deltax * deltax + deltay * deltay);
		}
	}
	public struct Vector3
	{
		public double x, y, z;
		public Vector3(double x, double y, double z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}
		public string ToString()
		{
			return x.ToString("f2") + "," + y.ToString("f2") + "," + z.ToString("f2");
		}
		public void FromString(string spos)
		{
			spos = spos.Replace("\"","");
			string[] apos = spos.Split(new char[]{',','|'});
			if (apos.Length >= 3)
			{
				x = typeParser.floatParse(apos[0]);
				y = typeParser.floatParse(apos[1]);
				z = typeParser.floatParse(apos[2]);
			}
		}

		public bool IsZero
		{
			get
			{
				return x == 0 && y == 0 && z == 0;
			}
		}
		public double Distance(Vector3 v)
		{
			double dresult = Distance(v.x, v.y, v.z);
			return dresult;
		}
		public double Distance(double lx, double ly, double lz)
		{
			return Vector3.Distance(x, y, z, lx, ly, lz);
		}
		public static double Distance(double lx, double ly, double lz, double rx, double ry, double rz)
		{
			double deltax = lx - rx;
			double deltay = ly - ry;
			double deltaz = lz - rz;
			return Math.Sqrt(deltax * deltax + deltay * deltay + deltaz * deltaz);
		}
	}
}
