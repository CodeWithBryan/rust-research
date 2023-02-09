using System;
using Facepunch;
using ProtoBuf;

// Token: 0x02000193 RID: 403
public class VehicleVendor : NPCTalking
{
	// Token: 0x0600176A RID: 5994 RVA: 0x000AEE4C File Offset: 0x000AD04C
	public override string GetConversationStartSpeech(global::BasePlayer player)
	{
		if (base.ProviderBusy())
		{
			return "startbusy";
		}
		return "intro";
	}

	// Token: 0x0600176B RID: 5995 RVA: 0x000AEE61 File Offset: 0x000AD061
	public VehicleSpawner GetVehicleSpawner()
	{
		if (!this.spawnerRef.IsValid(base.isServer))
		{
			return null;
		}
		return this.spawnerRef.Get(base.isServer).GetComponent<VehicleSpawner>();
	}

	// Token: 0x0600176C RID: 5996 RVA: 0x000AEE90 File Offset: 0x000AD090
	public override void UpdateFlags()
	{
		base.UpdateFlags();
		VehicleSpawner vehicleSpawner = this.GetVehicleSpawner();
		bool b = vehicleSpawner != null && vehicleSpawner.IsPadOccupied();
		base.SetFlag(global::BaseEntity.Flags.Reserved1, b, false, true);
	}

	// Token: 0x0600176D RID: 5997 RVA: 0x000AEECC File Offset: 0x000AD0CC
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.spawnerRef.IsValid(true) && this.vehicleSpawner == null)
		{
			this.vehicleSpawner = this.GetVehicleSpawner();
			return;
		}
		if (this.vehicleSpawner != null && !this.spawnerRef.IsValid(true))
		{
			this.spawnerRef.Set(this.vehicleSpawner);
		}
	}

	// Token: 0x0600176E RID: 5998 RVA: 0x000AEF35 File Offset: 0x000AD135
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.vehicleVendor = Pool.Get<ProtoBuf.VehicleVendor>();
		info.msg.vehicleVendor.spawnerRef = this.spawnerRef.uid;
	}

	// Token: 0x0600176F RID: 5999 RVA: 0x000AEF69 File Offset: 0x000AD169
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.vehicleVendor != null)
		{
			this.spawnerRef.id_cached = info.msg.vehicleVendor.spawnerRef;
		}
	}

	// Token: 0x06001770 RID: 6000 RVA: 0x0007BEA0 File Offset: 0x0007A0A0
	public override ConversationData GetConversationFor(global::BasePlayer player)
	{
		return this.conversations[0];
	}

	// Token: 0x0400105E RID: 4190
	public EntityRef spawnerRef;

	// Token: 0x0400105F RID: 4191
	public VehicleSpawner vehicleSpawner;
}
