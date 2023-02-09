using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x020003EF RID: 1007
public class EnvSync : PointEntity
{
	// Token: 0x060021E3 RID: 8675 RVA: 0x000D90A9 File Offset: 0x000D72A9
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRepeating(new Action(this.UpdateNetwork), 5f, 5f);
	}

	// Token: 0x060021E4 RID: 8676 RVA: 0x00007338 File Offset: 0x00005538
	private void UpdateNetwork()
	{
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060021E5 RID: 8677 RVA: 0x000D90D0 File Offset: 0x000D72D0
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.environment = Pool.Get<ProtoBuf.Environment>();
		if (TOD_Sky.Instance)
		{
			info.msg.environment.dateTime = TOD_Sky.Instance.Cycle.DateTime.ToBinary();
		}
		info.msg.environment.engineTime = Time.realtimeSinceStartup;
	}

	// Token: 0x060021E6 RID: 8678 RVA: 0x000D913C File Offset: 0x000D733C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.environment == null)
		{
			return;
		}
		if (!TOD_Sky.Instance)
		{
			return;
		}
		if (base.isServer)
		{
			TOD_Sky.Instance.Cycle.DateTime = DateTime.FromBinary(info.msg.environment.dateTime);
		}
	}

	// Token: 0x04001A4C RID: 6732
	private const float syncInterval = 5f;

	// Token: 0x04001A4D RID: 6733
	private const float syncIntervalInv = 0.2f;
}
