using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x02000011 RID: 17
public class BaseFirework : BaseCombatEntity, IIgniteable
{
	// Token: 0x06000029 RID: 41 RVA: 0x000028BF File Offset: 0x00000ABF
	public bool IsLit()
	{
		return base.HasFlag(BaseEntity.Flags.OnFire);
	}

	// Token: 0x0600002A RID: 42 RVA: 0x000028C8 File Offset: 0x00000AC8
	public bool IsExhausted()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved8);
	}

	// Token: 0x0600002B RID: 43 RVA: 0x000028D5 File Offset: 0x00000AD5
	public static int NumActiveFireworks()
	{
		return BaseFirework._activeFireworks.Count;
	}

	// Token: 0x0600002C RID: 44 RVA: 0x000028E4 File Offset: 0x00000AE4
	public virtual void TryLightFuse()
	{
		if (this.IsExhausted() || this.IsLit())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.OnFire, true, false, true);
		base.EnableGlobalBroadcast(true);
		base.Invoke(new Action(this.Begin), this.fuseLength);
		this.pickup.enabled = false;
		base.EnableSaving(false);
	}

	// Token: 0x0600002D RID: 45 RVA: 0x0000293F File Offset: 0x00000B3F
	public virtual void Begin()
	{
		base.SetFlag(BaseEntity.Flags.OnFire, false, false, true);
		base.SetFlag(BaseEntity.Flags.On, true, false, false);
		base.SendNetworkUpdate_Flags();
		base.Invoke(new Action(this.OnExhausted), this.activityLength);
	}

	// Token: 0x0600002E RID: 46 RVA: 0x00002974 File Offset: 0x00000B74
	public virtual void OnExhausted()
	{
		base.SetFlag(BaseEntity.Flags.Reserved8, true, false, false);
		base.SetFlag(BaseEntity.Flags.OnFire, false, false, false);
		base.SetFlag(BaseEntity.Flags.On, false, false, false);
		base.EnableGlobalBroadcast(false);
		base.SendNetworkUpdate_Flags();
		base.Invoke(new Action(this.Cleanup), this.corpseDuration);
		BaseFirework._activeFireworks.Remove(this);
	}

	// Token: 0x0600002F RID: 47 RVA: 0x000029D4 File Offset: 0x00000BD4
	public void Cleanup()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06000030 RID: 48 RVA: 0x000029DD File Offset: 0x00000BDD
	internal override void DoServerDestroy()
	{
		BaseFirework._activeFireworks.Remove(this);
		base.DoServerDestroy();
	}

	// Token: 0x06000031 RID: 49 RVA: 0x000029F1 File Offset: 0x00000BF1
	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		if (!base.isServer)
		{
			return;
		}
		if (info.damageTypes.Has(DamageType.Heat))
		{
			this.StaggeredTryLightFuse();
		}
	}

	// Token: 0x06000032 RID: 50 RVA: 0x00002A17 File Offset: 0x00000C17
	public void Ignite(Vector3 fromPos)
	{
		this.StaggeredTryLightFuse();
	}

	// Token: 0x06000033 RID: 51 RVA: 0x00002A20 File Offset: 0x00000C20
	public void StaggeredTryLightFuse()
	{
		if (this.IsExhausted() || this.IsLit())
		{
			return;
		}
		if (this.limitActiveCount)
		{
			if (BaseFirework.NumActiveFireworks() >= BaseFirework.maxActiveFireworks)
			{
				base.SetFlag(BaseEntity.Flags.OnFire, true, false, true);
				base.Invoke(new Action(this.StaggeredTryLightFuse), 0.35f);
				return;
			}
			BaseFirework._activeFireworks.Add(this);
			base.SetFlag(BaseEntity.Flags.OnFire, false, false, false);
		}
		base.Invoke(new Action(this.TryLightFuse), UnityEngine.Random.Range(0.1f, 0.3f));
	}

	// Token: 0x06000034 RID: 52 RVA: 0x00002AAC File Offset: 0x00000CAC
	public bool CanIgnite()
	{
		return !this.IsExhausted() && !this.IsLit();
	}

	// Token: 0x06000035 RID: 53 RVA: 0x00002AC1 File Offset: 0x00000CC1
	public override bool CanPickup(BasePlayer player)
	{
		return !this.IsExhausted() && base.CanPickup(player) && !this.IsLit();
	}

	// Token: 0x04000034 RID: 52
	public float fuseLength = 3f;

	// Token: 0x04000035 RID: 53
	public float activityLength = 10f;

	// Token: 0x04000036 RID: 54
	public const BaseEntity.Flags Flag_Spent = BaseEntity.Flags.Reserved8;

	// Token: 0x04000037 RID: 55
	public float corpseDuration = 15f;

	// Token: 0x04000038 RID: 56
	public bool limitActiveCount;

	// Token: 0x04000039 RID: 57
	[ServerVar]
	public static int maxActiveFireworks = 25;

	// Token: 0x0400003A RID: 58
	public static HashSet<BaseFirework> _activeFireworks = new HashSet<BaseFirework>();
}
