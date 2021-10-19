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
			get { if (_info == null) _info = new AvatarInfo(); return _info; }
			set { _info = value; }
		}

		public override void Serialize(MemoryStream ms)
		{
			this.stream = ms;
			if (_info != null)
			{
				writer.Write(1);
				var m = new MemoryStream();
				_info.Serialize(m);
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
			get { if (_info == null) _info = new AvatarInfo(); return _info; }
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
			if (_info != null)
			{
				writer.Write(1);
				var m = new MemoryStream();
				_info.Serialize(m);
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
		private string _uid = "";
		public string uid
		{
			get { return _uid; }
			set { _uid = value; }
		}

		private string _psid = "";
		public string psid
		{
			get { return _psid; }
			set { _psid = value; }
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
			get { if (_pos == null) _pos = new Int2(); return _pos; }
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
			if (!string.IsNullOrEmpty(uid))
			{
				writer.Write(1);
				writer.Write(uid);
			}
			if (!string.IsNullOrEmpty(psid))
			{
				writer.Write(6);
				writer.Write(psid);
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
			if (_pos != null)
			{
				writer.Write(4);
				var m = new MemoryStream();
				_pos.Serialize(m);
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
							uid = reader.ReadString();
							break;
						}
					case 6:
						{
							psid = reader.ReadString();
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

	public class PData : PktBase//42270
	{
		private int _intArg = default(int);
		public int intArg
		{
			get { return _intArg; }
			set { _intArg = value; }
		}

		private string _strArg = "";
		public string strArg
		{
			get { return _strArg; }
			set { _strArg = value; }
		}

		private List<int> _lInt = new List<int>();
		public List<int> lInt
		{
			get { return _lInt; }
			set { _lInt = value; }
		}

		private List<string> _lString = new List<string>();
		public List<string> lString
		{
			get { return _lString; }
			set { _lString = value; }
		}

		private PData _data = null;
		public PData data
		{
			get { if (_data == null) _data = new PData(); return _data; }
			set { _data = value; }
		}

		public override void Serialize(MemoryStream ms)
		{
			this.stream = ms;
			if (intArg != 0)
			{
				writer.Write(4);
				writer.Write(intArg);
			}
			if (!string.IsNullOrEmpty(strArg))
			{
				writer.Write(5);
				writer.Write(strArg);
			}
			if (lInt.Count > 0)
			{
				writer.Write(1);
				writer.Write(lInt.Count);
				foreach (var __item in lInt)
				{
					writer.Write(__item);
				}
			}
			if (lString.Count > 0)
			{
				writer.Write(2);
				writer.Write(lString.Count);
				foreach (var __item in lString)
				{
					writer.Write(__item);
				}
			}
			if (_data != null)
			{
				writer.Write(3);
				var m = new MemoryStream();
				_data.Serialize(m);
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
					case 4:
						{
							intArg = reader.ReadInt32();
							break;
						}
					case 5:
						{
							strArg = reader.ReadString();
							break;
						}
					case 1:
						{
							var count = reader.ReadInt32();
							for (var i = 0; i < count; i++)
							{
								var __item = reader.ReadInt32();
								lInt.Add(__item);
							}
							break;
						}
					case 2:
						{
							var count = reader.ReadInt32();
							for (var i = 0; i < count; i++)
							{
								var __item = reader.ReadString();
								lString.Add(__item);
							}
							break;
						}
					case 3:
						{
							data = new PData();
							var c = reader.ReadInt32();
							data.Deserialize(new MemoryStream(reader.ReadBytes(c)));
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

		private List<PData> _lDatas = new List<PData>();
		public List<PData> lDatas
		{
			get { return _lDatas; }
			set { _lDatas = value; }
		}

		public override void Serialize(MemoryStream ms)
		{
			this.stream = ms;
			if (id != 0)
			{
				writer.Write(1);
				writer.Write(id);
			}
			if (lDatas.Count > 0)
			{
				writer.Write(2);
				writer.Write(lDatas.Count);
				foreach (var __item in lDatas)
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
							id = reader.ReadInt32();
							break;
						}
					case 2:
						{
							var count = reader.ReadInt32();
							for (var i = 0; i < count; i++)
							{
								var __item = new PData();
								var c = reader.ReadInt32();
								__item.Deserialize(new MemoryStream(reader.ReadBytes(c)));
								lDatas.Add(__item);
							}
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

		private PData _pData = null;
		public PData pData
		{
			get { if (_pData == null) _pData = new PData(); return _pData; }
			set { _pData = value; }
		}

		public override void Serialize(MemoryStream ms)
		{
			this.stream = ms;
			if (ret)
			{
				writer.Write(1);
				writer.Write(ret);
			}
			if (_pData != null)
			{
				writer.Write(2);
				var m = new MemoryStream();
				_pData.Serialize(m);
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
							ret = reader.ReadBoolean();
							break;
						}
					case 2:
						{
							pData = new PData();
							var c = reader.ReadInt32();
							pData.Deserialize(new MemoryStream(reader.ReadBytes(c)));
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

	public class PktCreateOrderRequest : PktBase//18986
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

	public class PktCreateOrderResult : PktBase//16368
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

		private string _orderID = "";
		public string orderID
		{
			get { return _orderID; }
			set { _orderID = value; }
		}

		private string _extraInfo = "";
		public string extraInfo
		{
			get { return _extraInfo; }
			set { _extraInfo = value; }
		}

		public override void Serialize(MemoryStream ms)
		{
			this.stream = ms;
			if (eResult != 0)
			{
				writer.Write(1);
				writer.Write((int)eResult);
			}
			if (!string.IsNullOrEmpty(orderID))
			{
				writer.Write(2);
				writer.Write(orderID);
			}
			if (!string.IsNullOrEmpty(extraInfo))
			{
				writer.Write(3);
				writer.Write(extraInfo);
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
							orderID = reader.ReadString();
							break;
						}
					case 3:
						{
							extraInfo = reader.ReadString();
							break;
						}
				}
			}
		}
	}

	public class PktCommonRequest : PktBase//56792
	{
		private int _method = default(int);
		public int method
		{
			get { return _method; }
			set { _method = value; }
		}

		private PData _pData = null;
		public PData pData
		{
			get { if (_pData == null) _pData = new PData(); return _pData; }
			set { _pData = value; }
		}

		public override void Serialize(MemoryStream ms)
		{
			this.stream = ms;
			if (method != 0)
			{
				writer.Write(1);
				writer.Write(method);
			}
			if (_pData != null)
			{
				writer.Write(2);
				var m = new MemoryStream();
				_pData.Serialize(m);
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
							method = reader.ReadInt32();
							break;
						}
					case 2:
						{
							pData = new PData();
							var c = reader.ReadInt32();
							pData.Deserialize(new MemoryStream(reader.ReadBytes(c)));
							break;
						}
				}
			}
		}
	}

	public class PktRemoveEntity : PktBase//23518
	{
		private string _uid = "";
		public string uid
		{
			get { return _uid; }
			set { _uid = value; }
		}

		private string _reason = "";
		public string reason
		{
			get { return _reason; }
			set { _reason = value; }
		}

		public override void Serialize(MemoryStream ms)
		{
			this.stream = ms;
			if (!string.IsNullOrEmpty(uid))
			{
				writer.Write(1);
				writer.Write(uid);
			}
			if (!string.IsNullOrEmpty(reason))
			{
				writer.Write(2);
				writer.Write(reason);
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
					case 2:
						{
							reason = reader.ReadString();
							break;
						}
				}
			}
		}
	}

}