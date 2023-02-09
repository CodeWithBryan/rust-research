using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200040E RID: 1038
public class ProjectileWeaponMod : BaseEntity
{
	// Token: 0x060022DC RID: 8924 RVA: 0x000DE50A File Offset: 0x000DC70A
	public override void ServerInit()
	{
		base.SetFlag(BaseEntity.Flags.Disabled, true, false, true);
		base.ServerInit();
	}

	// Token: 0x060022DD RID: 8925 RVA: 0x000DE51D File Offset: 0x000DC71D
	public override void PostServerLoad()
	{
		base.limitNetworking = base.HasFlag(BaseEntity.Flags.Disabled);
	}

	// Token: 0x060022DE RID: 8926 RVA: 0x000DE530 File Offset: 0x000DC730
	public static float Mult(BaseEntity parentEnt, Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier> selector_modifier, Func<ProjectileWeaponMod.Modifier, float> selector_value, float def)
	{
		if (parentEnt.children == null)
		{
			return def;
		}
		IEnumerable<float> mods = ProjectileWeaponMod.GetMods(parentEnt, selector_modifier, selector_value);
		float num = 1f;
		foreach (float num2 in mods)
		{
			num *= num2;
		}
		return num;
	}

	// Token: 0x060022DF RID: 8927 RVA: 0x000DE590 File Offset: 0x000DC790
	public static float Sum(BaseEntity parentEnt, Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier> selector_modifier, Func<ProjectileWeaponMod.Modifier, float> selector_value, float def)
	{
		if (parentEnt.children == null)
		{
			return def;
		}
		IEnumerable<float> mods = ProjectileWeaponMod.GetMods(parentEnt, selector_modifier, selector_value);
		if (mods.Count<float>() != 0)
		{
			return mods.Sum();
		}
		return def;
	}

	// Token: 0x060022E0 RID: 8928 RVA: 0x000DE5C0 File Offset: 0x000DC7C0
	public static float Average(BaseEntity parentEnt, Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier> selector_modifier, Func<ProjectileWeaponMod.Modifier, float> selector_value, float def)
	{
		if (parentEnt.children == null)
		{
			return def;
		}
		IEnumerable<float> mods = ProjectileWeaponMod.GetMods(parentEnt, selector_modifier, selector_value);
		if (mods.Count<float>() != 0)
		{
			return mods.Average();
		}
		return def;
	}

	// Token: 0x060022E1 RID: 8929 RVA: 0x000DE5F0 File Offset: 0x000DC7F0
	public static float Max(BaseEntity parentEnt, Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier> selector_modifier, Func<ProjectileWeaponMod.Modifier, float> selector_value, float def)
	{
		if (parentEnt.children == null)
		{
			return def;
		}
		IEnumerable<float> mods = ProjectileWeaponMod.GetMods(parentEnt, selector_modifier, selector_value);
		if (mods.Count<float>() != 0)
		{
			return mods.Max();
		}
		return def;
	}

	// Token: 0x060022E2 RID: 8930 RVA: 0x000DE620 File Offset: 0x000DC820
	public static float Min(BaseEntity parentEnt, Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier> selector_modifier, Func<ProjectileWeaponMod.Modifier, float> selector_value, float def)
	{
		if (parentEnt.children == null)
		{
			return def;
		}
		IEnumerable<float> mods = ProjectileWeaponMod.GetMods(parentEnt, selector_modifier, selector_value);
		if (mods.Count<float>() != 0)
		{
			return mods.Min();
		}
		return def;
	}

	// Token: 0x060022E3 RID: 8931 RVA: 0x000DE650 File Offset: 0x000DC850
	public static IEnumerable<float> GetMods(BaseEntity parentEnt, Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier> selector_modifier, Func<ProjectileWeaponMod.Modifier, float> selector_value)
	{
		return (from x in (from ProjectileWeaponMod x in parentEnt.children
		where x != null && (!x.needsOnForEffects || x.HasFlag(BaseEntity.Flags.On))
		select x).Select(selector_modifier)
		where x.enabled
		select x).Select(selector_value);
	}

	// Token: 0x060022E4 RID: 8932 RVA: 0x000DE6BC File Offset: 0x000DC8BC
	public static bool HasBrokenWeaponMod(BaseEntity parentEnt)
	{
		if (parentEnt.children == null)
		{
			return false;
		}
		return parentEnt.children.Cast<ProjectileWeaponMod>().Any((ProjectileWeaponMod x) => x != null && x.IsBroken());
	}

	// Token: 0x04001B2F RID: 6959
	[Header("Silencer")]
	public GameObjectRef defaultSilencerEffect;

	// Token: 0x04001B30 RID: 6960
	public bool isSilencer;

	// Token: 0x04001B31 RID: 6961
	[Header("Weapon Basics")]
	public ProjectileWeaponMod.Modifier repeatDelay;

	// Token: 0x04001B32 RID: 6962
	public ProjectileWeaponMod.Modifier projectileVelocity;

	// Token: 0x04001B33 RID: 6963
	public ProjectileWeaponMod.Modifier projectileDamage;

	// Token: 0x04001B34 RID: 6964
	public ProjectileWeaponMod.Modifier projectileDistance;

	// Token: 0x04001B35 RID: 6965
	[Header("Recoil")]
	public ProjectileWeaponMod.Modifier aimsway;

	// Token: 0x04001B36 RID: 6966
	public ProjectileWeaponMod.Modifier aimswaySpeed;

	// Token: 0x04001B37 RID: 6967
	public ProjectileWeaponMod.Modifier recoil;

	// Token: 0x04001B38 RID: 6968
	[Header("Aim Cone")]
	public ProjectileWeaponMod.Modifier sightAimCone;

	// Token: 0x04001B39 RID: 6969
	public ProjectileWeaponMod.Modifier hipAimCone;

	// Token: 0x04001B3A RID: 6970
	[Header("Light Effects")]
	public bool isLight;

	// Token: 0x04001B3B RID: 6971
	[Header("MuzzleBrake")]
	public bool isMuzzleBrake;

	// Token: 0x04001B3C RID: 6972
	[Header("MuzzleBoost")]
	public bool isMuzzleBoost;

	// Token: 0x04001B3D RID: 6973
	[Header("Scope")]
	public bool isScope;

	// Token: 0x04001B3E RID: 6974
	public float zoomAmountDisplayOnly;

	// Token: 0x04001B3F RID: 6975
	[Header("Magazine")]
	public ProjectileWeaponMod.Modifier magazineCapacity;

	// Token: 0x04001B40 RID: 6976
	public bool needsOnForEffects;

	// Token: 0x04001B41 RID: 6977
	[Header("Burst")]
	public int burstCount = -1;

	// Token: 0x04001B42 RID: 6978
	public float timeBetweenBursts;

	// Token: 0x02000C8E RID: 3214
	[Serializable]
	public struct Modifier
	{
		// Token: 0x040042DF RID: 17119
		public bool enabled;

		// Token: 0x040042E0 RID: 17120
		[Tooltip("1 means no change. 0.5 is half.")]
		public float scalar;

		// Token: 0x040042E1 RID: 17121
		[Tooltip("Added after the scalar is applied.")]
		public float offset;
	}
}
