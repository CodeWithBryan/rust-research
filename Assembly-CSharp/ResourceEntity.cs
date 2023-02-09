using System;
using Facepunch;
using ProtoBuf;
using UnityEngine.Serialization;

// Token: 0x02000437 RID: 1079
public class ResourceEntity : global::BaseEntity
{
	// Token: 0x0600237D RID: 9085 RVA: 0x000E0EEB File Offset: 0x000DF0EB
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.resource == null)
		{
			return;
		}
		this.health = info.msg.resource.health;
	}

	// Token: 0x0600237E RID: 9086 RVA: 0x000E0F18 File Offset: 0x000DF118
	public override void InitShared()
	{
		base.InitShared();
		if (base.isServer)
		{
			DecorComponent[] components = PrefabAttribute.server.FindAll<DecorComponent>(this.prefabID);
			base.transform.ApplyDecorComponentsScaleOnly(components);
		}
	}

	// Token: 0x0600237F RID: 9087 RVA: 0x000E0F50 File Offset: 0x000DF150
	public override void ServerInit()
	{
		base.ServerInit();
		this.resourceDispenser = base.GetComponent<ResourceDispenser>();
		if (this.health == 0f)
		{
			this.health = this.startHealth;
		}
	}

	// Token: 0x06002380 RID: 9088 RVA: 0x000E0F7D File Offset: 0x000DF17D
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			info.msg.resource = Pool.Get<BaseResource>();
			info.msg.resource.health = this.Health();
		}
	}

	// Token: 0x06002381 RID: 9089 RVA: 0x000E0FB4 File Offset: 0x000DF1B4
	public override float MaxHealth()
	{
		return this.startHealth;
	}

	// Token: 0x06002382 RID: 9090 RVA: 0x000E0FBC File Offset: 0x000DF1BC
	public override float Health()
	{
		return this.health;
	}

	// Token: 0x06002383 RID: 9091 RVA: 0x000059DD File Offset: 0x00003BDD
	protected virtual void OnHealthChanged()
	{
	}

	// Token: 0x06002384 RID: 9092 RVA: 0x000E0FC4 File Offset: 0x000DF1C4
	public override void OnAttacked(HitInfo info)
	{
		if (base.isServer && !this.isKilled)
		{
			if (this.resourceDispenser != null)
			{
				this.resourceDispenser.OnAttacked(info);
			}
			if (!info.DidGather)
			{
				if (this.baseProtection)
				{
					this.baseProtection.Scale(info.damageTypes, 1f);
				}
				float num = info.damageTypes.Total();
				this.health -= num;
				if (this.health <= 0f)
				{
					this.OnKilled(info);
					return;
				}
				this.OnHealthChanged();
			}
		}
	}

	// Token: 0x06002385 RID: 9093 RVA: 0x000E105E File Offset: 0x000DF25E
	public virtual void OnKilled(HitInfo info)
	{
		this.isKilled = true;
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06002386 RID: 9094 RVA: 0x000062DD File Offset: 0x000044DD
	public override float BoundsPadding()
	{
		return 1f;
	}

	// Token: 0x04001C31 RID: 7217
	[FormerlySerializedAs("health")]
	public float startHealth;

	// Token: 0x04001C32 RID: 7218
	[FormerlySerializedAs("protection")]
	public ProtectionProperties baseProtection;

	// Token: 0x04001C33 RID: 7219
	protected float health;

	// Token: 0x04001C34 RID: 7220
	internal ResourceDispenser resourceDispenser;

	// Token: 0x04001C35 RID: 7221
	[NonSerialized]
	protected bool isKilled;
}
