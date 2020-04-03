using System.Collections.Generic;
using System.IO;

namespace LibPacket
{
	public class PktLoginRequest : PktBase//52367
	{
		private string _username = "";
		public string username
		{
			get { return _username; }
			set { _username = value; }
		}

		private string _password = "";
		public string password
		{
			get { return _password; }
			set { _password = value; }
		}

		private int _ePartnerID = default(int);
		public int ePartnerID
		{
			get { return _ePartnerID; }
			set { _ePartnerID = value; }
		}

		public override void Serialize(MemoryStream ms)
		{
			this.stream = ms;
			if (!string.IsNullOrEmpty(username))
			{
				writer.Write(1);
				writer.Write(username);
			}
			if (!string.IsNullOrEmpty(password))
			{
				writer.Write(2);
				writer.Write(password);
			}
			if (ePartnerID != 0)
			{
				writer.Write(3);
				writer.Write(ePartnerID);
			}
		}
		public override void Deserialize(MemoryStream ms)
		{
			this.stream = ms;
			var tag = 0;
			while (reader.BaseStream.Position < reader.BaseStream.Length && (tag = reader.ReadInt32()) != 0)
			{
				switch (tag)
				{
					case 1:
						{
							username = reader.ReadString();
							break;
						}
					case 2:
						{
							password = reader.ReadString();
							break;
						}
					case 3:
						{
							ePartnerID = reader.ReadInt32();
							break;
						}
				}
			}
		}
	}

	public class PktLoginResult : PktBase//14886
	{
		private int _ePartnerID = default(int);
		public int ePartnerID
		{
			get { return _ePartnerID; }
			set { _ePartnerID = value; }
		}

		private string _unionid = "";
		public string unionid
		{
			get { return _unionid; }
			set { _unionid = value; }
		}

		private bool _bSuccess = false;
		public bool bSuccess
		{
			get { return _bSuccess; }
			set { _bSuccess = value; }
		}

		public override void Serialize(MemoryStream ms)
		{
			this.stream = ms;
			if (ePartnerID != 0)
			{
				writer.Write(3);
				writer.Write(ePartnerID);
			}
			if (!string.IsNullOrEmpty(unionid))
			{
				writer.Write(1);
				writer.Write(unionid);
			}
			if (bSuccess)
			{
				writer.Write(2);
				writer.Write(bSuccess);
			}
		}
		public override void Deserialize(MemoryStream ms)
		{
			this.stream = ms;
			var tag = 0;
			while (reader.BaseStream.Position < reader.BaseStream.Length && (tag = reader.ReadInt32()) != 0)
			{
				switch (tag)
				{
					case 3:
						{
							ePartnerID = reader.ReadInt32();
							break;
						}
					case 1:
						{
							unionid = reader.ReadString();
							break;
						}
					case 2:
						{
							bSuccess = reader.ReadBoolean();
							break;
						}
				}
			}
		}
	}

	public class AvatarInfo : PktBase//313
	{
		private string _avatarID = "";
		public string avatarID
		{
			get { return _avatarID; }
			set { _avatarID = value; }
		}

		private string _avatarName = "";
		public string avatarName
		{
			get { return _avatarName; }
			set { _avatarName = value; }
		}

		public override void Serialize(MemoryStream ms)
		{
			this.stream = ms;
			if (!string.IsNullOrEmpty(avatarID))
			{
				writer.Write(1);
				writer.Write(avatarID);
			}
			if (!string.IsNullOrEmpty(avatarName))
			{
				writer.Write(2);
				writer.Write(avatarName);
			}
		}
		public override void Deserialize(MemoryStream ms)
		{
			this.stream = ms;
			var tag = 0;
			while (reader.BaseStream.Position < reader.BaseStream.Length && (tag = reader.ReadInt32()) != 0)
			{
				switch (tag)
				{
					case 1:
						{
							avatarID = reader.ReadString();
							break;
						}
					case 2:
						{
							avatarName = reader.ReadString();
							break;
						}
				}
			}
		}
	}

	public class PktEnterGameRequest : PktBase//11066
	{
		public override void Serialize(MemoryStream ms)
		{
		}
		public override void Deserialize(MemoryStream ms)
		{
		}
	}

	public class PktEnterGameResult : PktBase//11923
	{
		private AvatarInfo _info = null;
		public AvatarInfo info
		{
			get { return _info; }
			set { _info = value; }
		}

		public override void Serialize(MemoryStream ms)
		{
			this.stream = ms;
			if (info != null)
			{
				writer.Write(1);
				var m = new MemoryStream();
				info.Serialize(m);
				var bs = m.ToArray();
				writer.Write(bs.Length);
				writer.Write(bs);
			}
		}
		public override void Deserialize(MemoryStream ms)
		{
			this.stream = ms;
			var tag = 0;
			while (reader.BaseStream.Position < reader.BaseStream.Length && (tag = reader.ReadInt32()) != 0)
			{
				switch (tag)
				{
					case 1:
						{
							info = new AvatarInfo();
							var c = reader.ReadInt32();
							info.Deserialize(new MemoryStream(reader.ReadBytes(c)));
							break;
						}
				}
			}
		}
	}

	public class PktServerMessage : PktBase//27462
	{
		private string _message = "";
		public string message
		{
			get { return _message; }
			set { _message = value; }
		}

		public override void Serialize(MemoryStream ms)
		{
			this.stream = ms;
			if (!string.IsNullOrEmpty(message))
			{
				writer.Write(1);
				writer.Write(message);
			}
		}
		public override void Deserialize(MemoryStream ms)
		{
			this.stream = ms;
			var tag = 0;
			while (reader.BaseStream.Position < reader.BaseStream.Length && (tag = reader.ReadInt32()) != 0)
			{
				switch (tag)
				{
					case 1:
						{
							message = reader.ReadString();
							break;
						}
				}
			}
		}
	}

	public class PktCreateAvatarRequest : PktBase//42350
	{
		private string _avatarName = "";
		public string avatarName
		{
			get { return _avatarName; }
			set { _avatarName = value; }
		}

		private int _sex = default(int);
		public int sex
		{
			get { return _sex; }
			set { _sex = value; }
		}

		public override void Serialize(MemoryStream ms)
		{
			this.stream = ms;
			if (!string.IsNullOrEmpty(avatarName))
			{
				writer.Write(1);
				writer.Write(avatarName);
			}
			if (sex != 0)
			{
				writer.Write(2);
				writer.Write(sex);
			}
		}
		public override void Deserialize(MemoryStream ms)
		{
			this.stream = ms;
			var tag = 0;
			while (reader.BaseStream.Position < reader.BaseStream.Length && (tag = reader.ReadInt32()) != 0)
			{
				switch (tag)
				{
					case 1:
						{
							avatarName = reader.ReadString();
							break;
						}
					case 2:
						{
							sex = reader.ReadInt32();
							break;
						}
				}
			}
		}
	}

	public class PktCreateAvatarResult : PktBase//63303
	{
		public enum EResult
		{
			Success,
			MaxCount,
			SameName,
		}
		private EResult _eResult = default(EResult);
		public EResult eResult
		{
			get { return _eResult; }
			set { _eResult = value; }
		}

		private AvatarInfo _info = null;
		public AvatarInfo info
		{
			get { return _info; }
			set { _info = value; }
		}

		public override void Serialize(MemoryStream ms)
		{
			this.stream = ms;
			if (eResult != 0)
			{
				writer.Write(2);
				writer.Write((int)eResult);
			}
			if (info != null)
			{
				writer.Write(1);
				var m = new MemoryStream();
				info.Serialize(m);
				var bs = m.ToArray();
				writer.Write(bs.Length);
				writer.Write(bs);
			}
		}
		public override void Deserialize(MemoryStream ms)
		{
			this.stream = ms;
			var tag = 0;
			while (reader.BaseStream.Position < reader.BaseStream.Length && (tag = reader.ReadInt32()) != 0)
			{
				switch (tag)
				{
					case 2:
						{
							eResult = (EResult)reader.ReadInt32();
							break;
						}
					case 1:
						{
							info = new AvatarInfo();
							var c = reader.ReadInt32();
							info.Deserialize(new MemoryStream(reader.ReadBytes(c)));
							break;
						}
				}
			}
		}
	}

	public class ParamInfo : PktBase//8596
	{
		private string _paramName = "";
		public string paramName
		{
			get { return _paramName; }
			set { _paramName = value; }
		}

		private string _paramValue = "";
		public string paramValue
		{
			get { return _paramValue; }
			set { _paramValue = value; }
		}

		public override void Serialize(MemoryStream ms)
		{
			this.stream = ms;
			if (!string.IsNullOrEmpty(paramName))
			{
				writer.Write(1);
				writer.Write(paramName);
			}
			if (!string.IsNullOrEmpty(paramValue))
			{
				writer.Write(2);
				writer.Write(paramValue);
			}
		}
		public override void Deserialize(MemoryStream ms)
		{
			this.stream = ms;
			var tag = 0;
			while (reader.BaseStream.Position < reader.BaseStream.Length && (tag = reader.ReadInt32()) != 0)
			{
				switch (tag)
				{
					case 1:
						{
							paramName = reader.ReadString();
							break;
						}
					case 2:
						{
							paramValue = reader.ReadString();
							break;
						}
				}
			}
		}
	}

	public class PktParamUpdate : PktBase//62947
	{
		private List<ParamInfo> _lInfos = new List<ParamInfo>();
		public List<ParamInfo> lInfos
		{
			get { return _lInfos; }
			set { _lInfos = value; }
		}

		private string _cakeType = "";
		public string cakeType
		{
			get { return _cakeType; }
			set { _cakeType = value; }
		}

		private string _id = "";
		public string id
		{
			get { return _id; }
			set { _id = value; }
		}

		private string _iid = "";
		public string iid
		{
			get { return _iid; }
			set { _iid = value; }
		}

		public override void Serialize(MemoryStream ms)
		{
			this.stream = ms;
			if (lInfos.Count > 0)
			{
				writer.Write(1);
				writer.Write(lInfos.Count);
				foreach (var __item in lInfos)
				{
					var m = new MemoryStream();
					__item.Serialize(m);
					var bs = m.ToArray();
					writer.Write(bs.Length);
					writer.Write(bs);
				}
			}
			if (!string.IsNullOrEmpty(cakeType))
			{
				writer.Write(2);
				writer.Write(cakeType);
			}
			if (!string.IsNullOrEmpty(id))
			{
				writer.Write(3);
				writer.Write(id);
			}
			if (!string.IsNullOrEmpty(iid))
			{
				writer.Write(4);
				writer.Write(iid);
			}
		}
		public override void Deserialize(MemoryStream ms)
		{
			this.stream = ms;
			var tag = 0;
			while (reader.BaseStream.Position < reader.BaseStream.Length && (tag = reader.ReadInt32()) != 0)
			{
				switch (tag)
				{
					case 1:
						{
							var count = reader.ReadInt32();
							for (var i = 0; i < count; i++)
							{
								var __item = new ParamInfo();
								var c = reader.ReadInt32();
								__item.Deserialize(new MemoryStream(reader.ReadBytes(c)));
								lInfos.Add(__item);
							}
							break;
						}
					case 2:
						{
							cakeType = reader.ReadString();
							break;
						}
					case 3:
						{
							id = reader.ReadString();
							break;
						}
					case 4:
						{
							iid = reader.ReadString();
							break;
						}
				}
			}
		}
	}

	public class Int2 : PktBase//53005
	{
		private int _int1 = default(int);
		public int int1
		{
			get { return _int1; }
			set { _int1 = value; }
		}

		private int _int2 = default(int);
		public int int2
		{
			get { return _int2; }
			set { _int2 = value; }
		}

		public override void Serialize(MemoryStream ms)
		{
			this.stream = ms;
			if (int1 != 0)
			{
				writer.Write(1);
				writer.Write(int1);
			}
			if (int2 != 0)
			{
				writer.Write(2);
				writer.Write(int2);
			}
		}
		public override void Deserialize(MemoryStream ms)
		{
			this.stream = ms;
			var tag = 0;
			while (reader.BaseStream.Position < reader.BaseStream.Length && (tag = reader.ReadInt32()) != 0)
			{
				switch (tag)
				{
					case 1:
						{
							int1 = reader.ReadInt32();
							break;
						}
					case 2:
						{
							int2 = reader.ReadInt32();
							break;
						}
				}
			}
		}
	}

	public class PktCreatePlayer : PktBase//56655
	{
		private string _id = "";
		public string id
		{
			get { return _id; }
			set { _id = value; }
		}

		private int _contentID = default(int);
		public int contentID
		{
			get { return _contentID; }
			set { _contentID = value; }
		}

		private List<ParamInfo> _lParams = new List<ParamInfo>();
		public List<ParamInfo> lParams
		{
			get { return _lParams; }
			set { _lParams = value; }
		}

		private Int2 _pos = null;
		public Int2 pos
		{
			get { return _pos; }
			set { _pos = value; }
		}

		private int _direction = default(int);
		public int direction
		{
			get { return _direction; }
			set { _direction = value; }
		}

		public override void Serialize(MemoryStream ms)
		{
			this.stream = ms;
			if (!string.IsNullOrEmpty(id))
			{
				writer.Write(1);
				writer.Write(id);
			}
			if (contentID != 0)
			{
				writer.Write(2);
				writer.Write(contentID);
			}
			if (lParams.Count > 0)
			{
				writer.Write(3);
				writer.Write(lParams.Count);
				foreach (var __item in lParams)
				{
					var m = new MemoryStream();
					__item.Serialize(m);
					var bs = m.ToArray();
					writer.Write(bs.Length);
					writer.Write(bs);
				}
			}
			if (pos != null)
			{
				writer.Write(4);
				var m = new MemoryStream();
				pos.Serialize(m);
				var bs = m.ToArray();
				writer.Write(bs.Length);
				writer.Write(bs);
			}
			if (direction != 0)
			{
				writer.Write(5);
				writer.Write(direction);
			}
		}
		public override void Deserialize(MemoryStream ms)
		{
			this.stream = ms;
			var tag = 0;
			while (reader.BaseStream.Position < reader.BaseStream.Length && (tag = reader.ReadInt32()) != 0)
			{
				switch (tag)
				{
					case 1:
						{
							id = reader.ReadString();
							break;
						}
					case 2:
						{
							contentID = reader.ReadInt32();
							break;
						}
					case 3:
						{
							var count = reader.ReadInt32();
							for (var i = 0; i < count; i++)
							{
								var __item = new ParamInfo();
								var c = reader.ReadInt32();
								__item.Deserialize(new MemoryStream(reader.ReadBytes(c)));
								lParams.Add(__item);
							}
							break;
						}
					case 4:
						{
							pos = new Int2();
							var c = reader.ReadInt32();
							pos.Deserialize(new MemoryStream(reader.ReadBytes(c)));
							break;
						}
					case 5:
						{
							direction = reader.ReadInt32();
							break;
						}
				}
			}
		}
	}

	public class PktEnterCity : PktBase//19681
	{
		public override void Serialize(MemoryStream ms)
		{
		}
		public override void Deserialize(MemoryStream ms)
		{
		}
	}

	public class PktEnterConta : PktBase//49576
	{
		private int _id = default(int);
		public int id
		{
			get { return _id; }
			set { _id = value; }
		}

		public override void Serialize(MemoryStream ms)
		{
			this.stream = ms;
			if (id != 0)
			{
				writer.Write(1);
				writer.Write(id);
			}
		}
		public override void Deserialize(MemoryStream ms)
		{
			this.stream = ms;
			var tag = 0;
			while (reader.BaseStream.Position < reader.BaseStream.Length && (tag = reader.ReadInt32()) != 0)
			{
				switch (tag)
				{
					case 1:
						{
							id = reader.ReadInt32();
							break;
						}
				}
			}
		}
	}

	public class PktLeaveConta : PktBase//43051
	{
		private string _uid = "";
		public string uid
		{
			get { return _uid; }
			set { _uid = value; }
		}

		public override void Serialize(MemoryStream ms)
		{
			this.stream = ms;
			if (!string.IsNullOrEmpty(uid))
			{
				writer.Write(1);
				writer.Write(uid);
			}
		}
		public override void Deserialize(MemoryStream ms)
		{
			this.stream = ms;
			var tag = 0;
			while (reader.BaseStream.Position < reader.BaseStream.Length && (tag = reader.ReadInt32()) != 0)
			{
				switch (tag)
				{
					case 1:
						{
							uid = reader.ReadString();
							break;
						}
				}
			}
		}
	}

	public class PktContaData : PktBase//19350
	{
		private int _id = default(int);
		public int id
		{
			get { return _id; }
			set { _id = value; }
		}

		public override void Serialize(MemoryStream ms)
		{
			this.stream = ms;
			if (id != 0)
			{
				writer.Write(1);
				writer.Write(id);
			}
		}
		public override void Deserialize(MemoryStream ms)
		{
			this.stream = ms;
			var tag = 0;
			while (reader.BaseStream.Position < reader.BaseStream.Length && (tag = reader.ReadInt32()) != 0)
			{
				switch (tag)
				{
					case 1:
						{
							id = reader.ReadInt32();
							break;
						}
				}
			}
		}
	}

	public class PktSettlement : PktBase//16771
	{
		private bool _ret = false;
		public bool ret
		{
			get { return _ret; }
			set { _ret = value; }
		}

		public override void Serialize(MemoryStream ms)
		{
			this.stream = ms;
			if (ret)
			{
				writer.Write(1);
				writer.Write(ret);
			}
		}
		public override void Deserialize(MemoryStream ms)
		{
			this.stream = ms;
			var tag = 0;
			while (reader.BaseStream.Position < reader.BaseStream.Length && (tag = reader.ReadInt32()) != 0)
			{
				switch (tag)
				{
					case 1:
						{
							ret = reader.ReadBoolean();
							break;
						}
				}
			}
		}
	}

	public class PktEasy : PktBase//11931
	{
		private string _name = "";
		public string name
		{
			get { return _name; }
			set { _name = value; }
		}

		private List<int> _ints = new List<int>();
		public List<int> ints
		{
			get { return _ints; }
			set { _ints = value; }
		}

		private List<string> _strs = new List<string>();
		public List<string> strs
		{
			get { return _strs; }
			set { _strs = value; }
		}

		public override void Serialize(MemoryStream ms)
		{
			this.stream = ms;
			if (!string.IsNullOrEmpty(name))
			{
				writer.Write(1);
				writer.Write(name);
			}
			if (ints.Count > 0)
			{
				writer.Write(2);
				writer.Write(ints.Count);
				foreach (var __item in ints)
				{
					writer.Write(__item);
				}
			}
			if (strs.Count > 0)
			{
				writer.Write(3);
				writer.Write(strs.Count);
				foreach (var __item in strs)
				{
					writer.Write(__item);
				}
			}
		}
		public override void Deserialize(MemoryStream ms)
		{
			this.stream = ms;
			var tag = 0;
			while (reader.BaseStream.Position < reader.BaseStream.Length && (tag = reader.ReadInt32()) != 0)
			{
				switch (tag)
				{
					case 1:
						{
							name = reader.ReadString();
							break;
						}
					case 2:
						{
							var count = reader.ReadInt32();
							for (var i = 0; i < count; i++)
							{
								var __item = reader.ReadInt32();
								ints.Add(__item);
							}
							break;
						}
					case 3:
						{
							var count = reader.ReadInt32();
							for (var i = 0; i < count; i++)
							{
								var __item = reader.ReadString();
								strs.Add(__item);
							}
							break;
						}
				}
			}
		}
	}

	public class PktGetSdata : PktBase//21125
	{
		private string _name = "";
		public string name
		{
			get { return _name; }
			set { _name = value; }
		}

		public override void Serialize(MemoryStream ms)
		{
			this.stream = ms;
			if (!string.IsNullOrEmpty(name))
			{
				writer.Write(1);
				writer.Write(name);
			}
		}
		public override void Deserialize(MemoryStream ms)
		{
			this.stream = ms;
			var tag = 0;
			while (reader.BaseStream.Position < reader.BaseStream.Length && (tag = reader.ReadInt32()) != 0)
			{
				switch (tag)
				{
					case 1:
						{
							name = reader.ReadString();
							break;
						}
				}
			}
		}
	}

	public class PktDailyCheckRequest : PktBase//11914
	{
		public override void Serialize(MemoryStream ms)
		{
		}
		public override void Deserialize(MemoryStream ms)
		{
		}
	}

	public class PktDailyCheckResult : PktBase//38272
	{
		public enum EResult
		{
			Success,
			Failed,
		}
		private EResult _eResult = default(EResult);
		public EResult eResult
		{
			get { return _eResult; }
			set { _eResult = value; }
		}

		private List<Int2> _lItems = new List<Int2>();
		public List<Int2> lItems
		{
			get { return _lItems; }
			set { _lItems = value; }
		}

		public override void Serialize(MemoryStream ms)
		{
			this.stream = ms;
			if (eResult != 0)
			{
				writer.Write(1);
				writer.Write((int)eResult);
			}
			if (lItems.Count > 0)
			{
				writer.Write(2);
				writer.Write(lItems.Count);
				foreach (var __item in lItems)
				{
					var m = new MemoryStream();
					__item.Serialize(m);
					var bs = m.ToArray();
					writer.Write(bs.Length);
					writer.Write(bs);
				}
			}
		}
		public override void Deserialize(MemoryStream ms)
		{
			this.stream = ms;
			var tag = 0;
			while (reader.BaseStream.Position < reader.BaseStream.Length && (tag = reader.ReadInt32()) != 0)
			{
				switch (tag)
				{
					case 1:
						{
							eResult = (EResult)reader.ReadInt32();
							break;
						}
					case 2:
						{
							var count = reader.ReadInt32();
							for (var i = 0; i < count; i++)
							{
								var __item = new Int2();
								var c = reader.ReadInt32();
								__item.Deserialize(new MemoryStream(reader.ReadBytes(c)));
								lItems.Add(__item);
							}
							break;
						}
				}
			}
		}
	}

	public class PktPayRequest : PktBase//56289
	{
		private int _productID = default(int);
		public int productID
		{
			get { return _productID; }
			set { _productID = value; }
		}

		public override void Serialize(MemoryStream ms)
		{
			this.stream = ms;
			if (productID != 0)
			{
				writer.Write(1);
				writer.Write(productID);
			}
		}
		public override void Deserialize(MemoryStream ms)
		{
			this.stream = ms;
			var tag = 0;
			while (reader.BaseStream.Position < reader.BaseStream.Length && (tag = reader.ReadInt32()) != 0)
			{
				switch (tag)
				{
					case 1:
						{
							productID = reader.ReadInt32();
							break;
						}
				}
			}
		}
	}

	public class PktExchangeRequest : PktBase//719
	{
		public enum EType
		{
			Gold = 1,
		}
		private int _count = default(int);
		public int count
		{
			get { return _count; }
			set { _count = value; }
		}

		private EType _eType = default(EType);
		public EType eType
		{
			get { return _eType; }
			set { _eType = value; }
		}

		public override void Serialize(MemoryStream ms)
		{
			this.stream = ms;
			if (count != 0)
			{
				writer.Write(1);
				writer.Write(count);
			}
			if (eType != 0)
			{
				writer.Write(2);
				writer.Write((int)eType);
			}
		}
		public override void Deserialize(MemoryStream ms)
		{
			this.stream = ms;
			var tag = 0;
			while (reader.BaseStream.Position < reader.BaseStream.Length && (tag = reader.ReadInt32()) != 0)
			{
				switch (tag)
				{
					case 1:
						{
							count = reader.ReadInt32();
							break;
						}
					case 2:
						{
							eType = (EType)reader.ReadInt32();
							break;
						}
				}
			}
		}
	}

	public class PktExchangeResult : PktBase//32480
	{
		private bool _bSuccess = false;
		public bool bSuccess
		{
			get { return _bSuccess; }
			set { _bSuccess = value; }
		}

		public override void Serialize(MemoryStream ms)
		{
			this.stream = ms;
			if (bSuccess)
			{
				writer.Write(1);
				writer.Write(bSuccess);
			}
		}
		public override void Deserialize(MemoryStream ms)
		{
			this.stream = ms;
			var tag = 0;
			while (reader.BaseStream.Position < reader.BaseStream.Length && (tag = reader.ReadInt32()) != 0)
			{
				switch (tag)
				{
					case 1:
						{
							bSuccess = reader.ReadBoolean();
							break;
						}
				}
			}
		}
	}

}