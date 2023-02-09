using System;
using Facepunch;
using ProtoBuf;

// Token: 0x020001FA RID: 506
public class HorseCorpse : global::LootableCorpse
{
	// Token: 0x17000201 RID: 513
	// (get) Token: 0x06001A39 RID: 6713 RVA: 0x000BA95F File Offset: 0x000B8B5F
	public override string playerName
	{
		get
		{
			return this.lootPanelTitle.translated;
		}
	}

	// Token: 0x06001A3A RID: 6714 RVA: 0x000BA96C File Offset: 0x000B8B6C
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.horse = Pool.Get<ProtoBuf.Horse>();
		info.msg.horse.breedIndex = this.breedIndex;
	}

	// Token: 0x06001A3B RID: 6715 RVA: 0x000BA99B File Offset: 0x000B8B9B
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.horse != null)
		{
			this.breedIndex = info.msg.horse.breedIndex;
		}
	}

	// Token: 0x04001298 RID: 4760
	public int breedIndex;

	// Token: 0x04001299 RID: 4761
	public Translate.Phrase lootPanelTitle;
}
