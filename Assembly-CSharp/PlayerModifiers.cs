using System;
using Facepunch;
using Network;
using ProtoBuf;

// Token: 0x020000AC RID: 172
public class PlayerModifiers : BaseModifiers<global::BasePlayer>
{
	// Token: 0x06000FAE RID: 4014 RVA: 0x00082134 File Offset: 0x00080334
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PlayerModifiers.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000FAF RID: 4015 RVA: 0x00082174 File Offset: 0x00080374
	public override void ServerUpdate(BaseCombatEntity ownerEntity)
	{
		base.ServerUpdate(ownerEntity);
		this.SendChangesToClient();
	}

	// Token: 0x06000FB0 RID: 4016 RVA: 0x00082184 File Offset: 0x00080384
	public ProtoBuf.PlayerModifiers Save()
	{
		ProtoBuf.PlayerModifiers playerModifiers = Pool.Get<ProtoBuf.PlayerModifiers>();
		playerModifiers.modifiers = Pool.GetList<ProtoBuf.Modifier>();
		foreach (global::Modifier modifier in this.All)
		{
			if (modifier != null)
			{
				playerModifiers.modifiers.Add(modifier.Save());
			}
		}
		return playerModifiers;
	}

	// Token: 0x06000FB1 RID: 4017 RVA: 0x000821F8 File Offset: 0x000803F8
	public void Load(ProtoBuf.PlayerModifiers m)
	{
		base.RemoveAll();
		if (m == null || m.modifiers == null)
		{
			return;
		}
		foreach (ProtoBuf.Modifier modifier in m.modifiers)
		{
			if (modifier != null)
			{
				global::Modifier modifier2 = new global::Modifier();
				modifier2.Init((global::Modifier.ModifierType)modifier.type, (global::Modifier.ModifierSource)modifier.source, modifier.value, modifier.duration, modifier.timeRemaing);
				base.Add(modifier2);
			}
		}
	}

	// Token: 0x06000FB2 RID: 4018 RVA: 0x0008228C File Offset: 0x0008048C
	public void SendChangesToClient()
	{
		if (!this.dirty)
		{
			return;
		}
		base.SetDirty(false);
		using (ProtoBuf.PlayerModifiers playerModifiers = this.Save())
		{
			base.baseEntity.ClientRPCPlayer<ProtoBuf.PlayerModifiers>(null, base.baseEntity, "UpdateModifiers", playerModifiers);
		}
	}
}
