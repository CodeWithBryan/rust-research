using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000432 RID: 1074
public class PlayerCorpse : global::LootableCorpse
{
	// Token: 0x0600236B RID: 9067 RVA: 0x000035EB File Offset: 0x000017EB
	public bool IsBuoyant()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved6);
	}

	// Token: 0x0600236C RID: 9068 RVA: 0x000E0C30 File Offset: 0x000DEE30
	public override bool OnStartBeingLooted(global::BasePlayer baseEntity)
	{
		return (!baseEntity.InSafeZone() || baseEntity.userID == this.playerSteamID) && base.OnStartBeingLooted(baseEntity);
	}

	// Token: 0x0600236D RID: 9069 RVA: 0x000E0C54 File Offset: 0x000DEE54
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.buoyancy == null)
		{
			Debug.LogWarning("Player corpse has no buoyancy assigned, searching at runtime :" + base.name);
			this.buoyancy = base.GetComponent<Buoyancy>();
		}
		if (this.buoyancy != null)
		{
			this.buoyancy.SubmergedChanged = new Action<bool>(this.BuoyancyChanged);
			this.buoyancy.forEntity = this;
		}
	}

	// Token: 0x0600236E RID: 9070 RVA: 0x000E0CC7 File Offset: 0x000DEEC7
	public void BuoyancyChanged(bool isSubmerged)
	{
		if (this.IsBuoyant())
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved6, isSubmerged, false, false);
		base.SendNetworkUpdate_Flags();
	}

	// Token: 0x0600236F RID: 9071 RVA: 0x000E0CE8 File Offset: 0x000DEEE8
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.lootableCorpse != null)
		{
			info.msg.lootableCorpse.underwearSkin = this.underwearSkin;
		}
		if (base.isServer && this.containers != null && this.containers.Length > 1 && !info.forDisk)
		{
			info.msg.storageBox = Pool.Get<StorageBox>();
			info.msg.storageBox.contents = this.containers[1].Save();
		}
	}

	// Token: 0x06002370 RID: 9072 RVA: 0x000E0D6F File Offset: 0x000DEF6F
	public override string Categorize()
	{
		return "playercorpse";
	}

	// Token: 0x04001C26 RID: 7206
	public Buoyancy buoyancy;

	// Token: 0x04001C27 RID: 7207
	public const global::BaseEntity.Flags Flag_Buoyant = global::BaseEntity.Flags.Reserved6;

	// Token: 0x04001C28 RID: 7208
	public uint underwearSkin;
}
