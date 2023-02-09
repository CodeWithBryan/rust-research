using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;

// Token: 0x020003B9 RID: 953
public class SignContent : ImageStorageEntity, IUGCBrowserEntity
{
	// Token: 0x1700027E RID: 638
	// (get) Token: 0x0600209A RID: 8346 RVA: 0x000D4632 File Offset: 0x000D2832
	protected override uint CrcToLoad
	{
		get
		{
			return this.textureIDs[0];
		}
	}

	// Token: 0x1700027F RID: 639
	// (get) Token: 0x0600209B RID: 8347 RVA: 0x00007074 File Offset: 0x00005274
	protected override FileStorage.Type StorageType
	{
		get
		{
			return FileStorage.Type.png;
		}
	}

	// Token: 0x17000280 RID: 640
	// (get) Token: 0x0600209C RID: 8348 RVA: 0x00003A54 File Offset: 0x00001C54
	public UGCType ContentType
	{
		get
		{
			return UGCType.ImagePng;
		}
	}

	// Token: 0x0600209D RID: 8349 RVA: 0x000D463C File Offset: 0x000D283C
	public void CopyInfoFromSign(ISignage s, IUGCBrowserEntity b)
	{
		uint[] textureCRCs = s.GetTextureCRCs();
		this.textureIDs = new uint[textureCRCs.Length];
		textureCRCs.CopyTo(this.textureIDs, 0);
		this.editHistory.Clear();
		foreach (ulong item in b.EditingHistory)
		{
			this.editHistory.Add(item);
		}
		FileStorage.server.ReassignEntityId(s.NetworkID, this.net.ID);
	}

	// Token: 0x0600209E RID: 8350 RVA: 0x000D46DC File Offset: 0x000D28DC
	public void CopyInfoToSign(ISignage s, IUGCBrowserEntity b)
	{
		FileStorage.server.ReassignEntityId(this.net.ID, s.NetworkID);
		s.SetTextureCRCs(this.textureIDs);
		b.EditingHistory.Clear();
		foreach (ulong item in this.editHistory)
		{
			b.EditingHistory.Add(item);
		}
	}

	// Token: 0x0600209F RID: 8351 RVA: 0x000D4768 File Offset: 0x000D2968
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.paintableSign == null)
		{
			info.msg.paintableSign = Pool.Get<PaintableSign>();
		}
		info.msg.paintableSign.crcs = Pool.GetList<uint>();
		foreach (uint item in this.textureIDs)
		{
			info.msg.paintableSign.crcs.Add(item);
		}
	}

	// Token: 0x060020A0 RID: 8352 RVA: 0x000D47DD File Offset: 0x000D29DD
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		FileStorage.server.RemoveAllByEntity(this.net.ID);
	}

	// Token: 0x060020A1 RID: 8353 RVA: 0x000D47FC File Offset: 0x000D29FC
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.paintableSign != null)
		{
			this.textureIDs = new uint[info.msg.paintableSign.crcs.Count];
			for (int i = 0; i < info.msg.paintableSign.crcs.Count; i++)
			{
				this.textureIDs[i] = info.msg.paintableSign.crcs[i];
			}
		}
	}

	// Token: 0x17000281 RID: 641
	// (get) Token: 0x060020A2 RID: 8354 RVA: 0x000D487B File Offset: 0x000D2A7B
	public uint[] GetContentCRCs
	{
		get
		{
			return this.textureIDs;
		}
	}

	// Token: 0x060020A3 RID: 8355 RVA: 0x000029D4 File Offset: 0x00000BD4
	public void ClearContent()
	{
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x17000282 RID: 642
	// (get) Token: 0x060020A4 RID: 8356 RVA: 0x000D4883 File Offset: 0x000D2A83
	public FileStorage.Type FileType
	{
		get
		{
			return this.StorageType;
		}
	}

	// Token: 0x17000283 RID: 643
	// (get) Token: 0x060020A5 RID: 8357 RVA: 0x000D488B File Offset: 0x000D2A8B
	public List<ulong> EditingHistory
	{
		get
		{
			return this.editHistory;
		}
	}

	// Token: 0x17000284 RID: 644
	// (get) Token: 0x060020A6 RID: 8358 RVA: 0x00002E37 File Offset: 0x00001037
	public global::BaseNetworkable UgcEntity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x04001959 RID: 6489
	private uint[] textureIDs = new uint[1];

	// Token: 0x0400195A RID: 6490
	private List<ulong> editHistory = new List<ulong>();
}
