using System.Collections.Generic;
using System.IO;

namespace LibPacket
{
	public class PktLoginRequest : PktBase
	{
		public enum EPartnerID
		{
			Test,
			Normal,
		}
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

		private EPartnerID _ePartnerID = default(EPartnerID);
		public EPartnerID ePartnerID
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
				writer.Write((int)ePartnerID);
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
						ePartnerID = (EPartnerID)reader.ReadInt32();
						break;
					}
				}
			}
		}
	}

	public class PktLoginResult : PktBase
	{
		private PktLoginRequest.EPartnerID _ePartnerID = default(PktLoginRequest.EPartnerID);
		public PktLoginRequest.EPartnerID ePartnerID
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
				writer.Write((int)ePartnerID);
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
						ePartnerID = (PktLoginRequest.EPartnerID)reader.ReadInt32();
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

	public class PktEnterGameRequest : PktBase
	{
		private PktLoginRequest.EPartnerID _ePartnerID = default(PktLoginRequest.EPartnerID);
		public PktLoginRequest.EPartnerID ePartnerID
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

		public override void Serialize(MemoryStream ms)
		{
			this.stream = ms;
			if (ePartnerID != 0)
			{
				writer.Write(3);
				writer.Write((int)ePartnerID);
			}
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
					case 3:
					{
						ePartnerID = (PktLoginRequest.EPartnerID)reader.ReadInt32();
						break;
					}
					case 1:
					{
						uid = reader.ReadString();
						break;
					}
				}
			}
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
				foreach (var _item in lItems)
				{
					var m = new MemoryStream();
					_item.Serialize(m);
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
							var _item = new ItemInfo();
							var c = reader.ReadInt32();
							_item.Deserialize(new MemoryStream(reader.ReadBytes(c)));
							lItems.Add(_item);
						}
						break;
					}
				}
			}
		}
	}

}