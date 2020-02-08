using System.Collections.Generic;
using System.IO;

namespace LibPacket
{
	public class PktLoginRequest : PktBase
	{
		private string _username = "";
		public string username
		{
			get{return _username;}
			set{_username = value;}
		}

		private string _password = "";
		public string password
		{
			get{return _password;}
			set{_password = value;}
		}

		private int _ePartnerID = default(int);
		public int ePartnerID
		{
			get{return _ePartnerID;}
			set{_ePartnerID = value;}
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

	public class PktLoginResult : PktBase
	{
		private int _ePartnerID = default(int);
		public int ePartnerID
		{
			get{return _ePartnerID;}
			set{_ePartnerID = value;}
		}

		private string _uid = "";
		public string uid
		{
			get{return _uid;}
			set{_uid = value;}
		}

		private bool _bSuccess = false;
		public bool bSuccess
		{
			get{return _bSuccess;}
			set{_bSuccess = value;}
		}

		public override void Serialize(MemoryStream ms)
		{
			this.stream = ms;
			if (ePartnerID != 0)
			{
				writer.Write(3);
				writer.Write(ePartnerID);
			}
			if (!string.IsNullOrEmpty(uid))
			{
				writer.Write(1);
				writer.Write(uid);
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
						uid = reader.ReadString();
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

	public class AvatarInfo : PktBase
	{
		private string _avatarID = "";
		public string avatarID
		{
			get{return _avatarID;}
			set{_avatarID = value;}
		}

		private string _avatarName = "";
		public string avatarName
		{
			get{return _avatarName;}
			set{_avatarName = value;}
		}

		private List<ParamInfo> _lInfos = new List<ParamInfo>();
		public List<ParamInfo> lInfos
		{
			get{return _lInfos;}
			set{_lInfos = value;}
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
			if (lInfos.Count > 0)
			{
				writer.Write(3);
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
					case 3:
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
				}
			}
		}
	}

	public class PktEnterGameRequest : PktBase
	{
		public override void Serialize(MemoryStream ms)
		{
		}
		public override void Deserialize(MemoryStream ms)
		{
		}
	}

	public class PktEnterGameResult : PktBase
	{
		private AvatarInfo _info = null;
		public AvatarInfo info
		{
			get{return _info;}
			set{_info = value;}
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

	public class PktServerMessage : PktBase
	{
		private string _message = "";
		public string message
		{
			get{return _message;}
			set{_message = value;}
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

	public class ItemInfo : PktBase
	{
		private int _contentID = default(int);
		public int contentID
		{
			get{return _contentID;}
			set{_contentID = value;}
		}

		private int _num = default(int);
		public int num
		{
			get{return _num;}
			set{_num = value;}
		}

		public override void Serialize(MemoryStream ms)
		{
			this.stream = ms;
			if (contentID != 0)
			{
				writer.Write(1);
				writer.Write(contentID);
			}
			if (num != 0)
			{
				writer.Write(2);
				writer.Write(num);
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
						contentID = reader.ReadInt32();
						break;
					}
					case 2:
					{
						num = reader.ReadInt32();
						break;
					}
				}
			}
		}
	}

	public class PktItemNotify : PktBase
	{
		private List<ItemInfo> _lItems = new List<ItemInfo>();
		public List<ItemInfo> lItems
		{
			get{return _lItems;}
			set{_lItems = value;}
		}

		public override void Serialize(MemoryStream ms)
		{
			this.stream = ms;
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
					case 2:
					{
						var count = reader.ReadInt32();
						for (var i = 0; i < count; i++)
						{
							var __item = new ItemInfo();
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

	public class PktCreateAvatarRequest : PktBase
	{
		private string _avatarName = "";
		public string avatarName
		{
			get{return _avatarName;}
			set{_avatarName = value;}
		}

		private int _sex = default(int);
		public int sex
		{
			get{return _sex;}
			set{_sex = value;}
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

	public class PktCreateAvatarResult : PktBase
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
			get{return _eResult;}
			set{_eResult = value;}
		}

		private AvatarInfo _info = null;
		public AvatarInfo info
		{
			get{return _info;}
			set{_info = value;}
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

	public class ParamInfo : PktBase
	{
		private string _paramName = "";
		public string paramName
		{
			get{return _paramName;}
			set{_paramName = value;}
		}

		private string _paramValue = "";
		public string paramValue
		{
			get{return _paramValue;}
			set{_paramValue = value;}
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

	public class PktParamUpdate : PktBase
	{
		private List<ParamInfo> _lInfos = new List<ParamInfo>();
		public List<ParamInfo> lInfos
		{
			get{return _lInfos;}
			set{_lInfos = value;}
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
				}
			}
		}
	}

}