using System;
using ConVar;
using Rust;
using UnityEngine;

// Token: 0x02000402 RID: 1026
public class SupplyDrop : LootContainer
{
	// Token: 0x0600228A RID: 8842 RVA: 0x000DD170 File Offset: 0x000DB370
	public override void ServerInit()
	{
		base.ServerInit();
		if (!Rust.Application.isLoadingSave)
		{
			if (this.parachutePrefab.isValid)
			{
				this.parachute = GameManager.server.CreateEntity(this.parachutePrefab.resourcePath, default(Vector3), default(Quaternion), true);
			}
			if (this.parachute)
			{
				this.parachute.SetParent(this, "parachute_attach", false, false);
				this.parachute.Spawn();
			}
		}
		this.isLootable = false;
		base.Invoke(new Action(this.MakeLootable), 300f);
		base.InvokeRepeating(new Action(this.CheckNightLight), 0f, 30f);
	}

	// Token: 0x0600228B RID: 8843 RVA: 0x000DD22A File Offset: 0x000DB42A
	protected override void OnChildAdded(BaseEntity child)
	{
		base.OnChildAdded(child);
		if (base.isServer && Rust.Application.isLoadingSave)
		{
			if (this.parachute != null)
			{
				Debug.LogWarning("More than one child entity was added to SupplyDrop! Expected only the parachute.", this);
			}
			this.parachute = child;
		}
	}

	// Token: 0x0600228C RID: 8844 RVA: 0x000DD262 File Offset: 0x000DB462
	private void RemoveParachute()
	{
		if (this.parachute)
		{
			this.parachute.Kill(BaseNetworkable.DestroyMode.None);
			this.parachute = null;
		}
	}

	// Token: 0x0600228D RID: 8845 RVA: 0x000DD284 File Offset: 0x000DB484
	public void MakeLootable()
	{
		this.isLootable = true;
	}

	// Token: 0x0600228E RID: 8846 RVA: 0x000DD28D File Offset: 0x000DB48D
	private void OnCollisionEnter(Collision collision)
	{
		if ((1 << collision.collider.gameObject.layer & 1084293393) > 0)
		{
			this.RemoveParachute();
			this.MakeLootable();
		}
	}

	// Token: 0x0600228F RID: 8847 RVA: 0x000DD2BB File Offset: 0x000DB4BB
	private void CheckNightLight()
	{
		base.SetFlag(BaseEntity.Flags.Reserved1, Env.time > 20f || Env.time < 7f, false, true);
	}

	// Token: 0x04001B05 RID: 6917
	public GameObjectRef parachutePrefab;

	// Token: 0x04001B06 RID: 6918
	private const BaseEntity.Flags FlagNightLight = BaseEntity.Flags.Reserved1;

	// Token: 0x04001B07 RID: 6919
	private BaseEntity parachute;
}
