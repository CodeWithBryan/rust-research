using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using Rust;

// Token: 0x02000170 RID: 368
public class PhotoEntity : ImageStorageEntity, IUGCBrowserEntity
{
	// Token: 0x170001BE RID: 446
	// (get) Token: 0x060016A1 RID: 5793 RVA: 0x000AB8D5 File Offset: 0x000A9AD5
	// (set) Token: 0x060016A2 RID: 5794 RVA: 0x000AB8DD File Offset: 0x000A9ADD
	public ulong PhotographerSteamId { get; private set; }

	// Token: 0x170001BF RID: 447
	// (get) Token: 0x060016A3 RID: 5795 RVA: 0x000AB8E6 File Offset: 0x000A9AE6
	// (set) Token: 0x060016A4 RID: 5796 RVA: 0x000AB8EE File Offset: 0x000A9AEE
	public uint ImageCrc { get; private set; }

	// Token: 0x170001C0 RID: 448
	// (get) Token: 0x060016A5 RID: 5797 RVA: 0x000AB8F7 File Offset: 0x000A9AF7
	protected override uint CrcToLoad
	{
		get
		{
			return this.ImageCrc;
		}
	}

	// Token: 0x060016A6 RID: 5798 RVA: 0x000AB900 File Offset: 0x000A9B00
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.photo != null)
		{
			this.PhotographerSteamId = info.msg.photo.photographerSteamId;
			this.ImageCrc = info.msg.photo.imageCrc;
		}
	}

	// Token: 0x060016A7 RID: 5799 RVA: 0x000AB950 File Offset: 0x000A9B50
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.photo = Pool.Get<Photo>();
		info.msg.photo.photographerSteamId = this.PhotographerSteamId;
		info.msg.photo.imageCrc = this.ImageCrc;
	}

	// Token: 0x060016A8 RID: 5800 RVA: 0x000AB9A0 File Offset: 0x000A9BA0
	public void SetImageData(ulong steamId, byte[] data)
	{
		this.ImageCrc = FileStorage.server.Store(data, FileStorage.Type.jpg, this.net.ID, 0U);
		this.PhotographerSteamId = steamId;
	}

	// Token: 0x060016A9 RID: 5801 RVA: 0x0007D140 File Offset: 0x0007B340
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		if (!Rust.Application.isQuitting && this.net != null)
		{
			FileStorage.server.RemoveAllByEntity(this.net.ID);
		}
	}

	// Token: 0x170001C1 RID: 449
	// (get) Token: 0x060016AA RID: 5802 RVA: 0x000AB9C7 File Offset: 0x000A9BC7
	public uint[] GetContentCRCs
	{
		get
		{
			if (this.ImageCrc <= 0U)
			{
				return Array.Empty<uint>();
			}
			return new uint[]
			{
				this.ImageCrc
			};
		}
	}

	// Token: 0x060016AB RID: 5803 RVA: 0x000AB9E7 File Offset: 0x000A9BE7
	public void ClearContent()
	{
		this.ImageCrc = 0U;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x170001C2 RID: 450
	// (get) Token: 0x060016AC RID: 5804 RVA: 0x00007074 File Offset: 0x00005274
	public UGCType ContentType
	{
		get
		{
			return UGCType.ImageJpg;
		}
	}

	// Token: 0x170001C3 RID: 451
	// (get) Token: 0x060016AD RID: 5805 RVA: 0x000AB9F7 File Offset: 0x000A9BF7
	public List<ulong> EditingHistory
	{
		get
		{
			if (this.PhotographerSteamId <= 0UL)
			{
				return new List<ulong>();
			}
			return new List<ulong>
			{
				this.PhotographerSteamId
			};
		}
	}

	// Token: 0x170001C4 RID: 452
	// (get) Token: 0x060016AE RID: 5806 RVA: 0x00002E37 File Offset: 0x00001037
	public global::BaseNetworkable UgcEntity
	{
		get
		{
			return this;
		}
	}
}
