using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x020004D6 RID: 1238
public class EffectDictionary
{
	// Token: 0x06002798 RID: 10136 RVA: 0x000F3116 File Offset: 0x000F1316
	public static string GetParticle(string impactType, string materialName)
	{
		return EffectDictionary.LookupEffect("impacts", impactType, materialName);
	}

	// Token: 0x06002799 RID: 10137 RVA: 0x000F3124 File Offset: 0x000F1324
	public static string GetParticle(DamageType damageType, string materialName)
	{
		switch (damageType)
		{
		case DamageType.Bullet:
			return EffectDictionary.GetParticle("bullet", materialName);
		case DamageType.Slash:
			return EffectDictionary.GetParticle("slash", materialName);
		case DamageType.Blunt:
			return EffectDictionary.GetParticle("blunt", materialName);
		case DamageType.Fall:
		case DamageType.Radiation:
		case DamageType.Bite:
			break;
		case DamageType.Stab:
			return EffectDictionary.GetParticle("stab", materialName);
		default:
			if (damageType == DamageType.Arrow)
			{
				return EffectDictionary.GetParticle("bullet", materialName);
			}
			break;
		}
		return EffectDictionary.GetParticle("blunt", materialName);
	}

	// Token: 0x0600279A RID: 10138 RVA: 0x000F31A4 File Offset: 0x000F13A4
	public static string GetDecal(string impactType, string materialName)
	{
		return EffectDictionary.LookupEffect("decals", impactType, materialName);
	}

	// Token: 0x0600279B RID: 10139 RVA: 0x000F31B4 File Offset: 0x000F13B4
	public static string GetDecal(DamageType damageType, string materialName)
	{
		switch (damageType)
		{
		case DamageType.Bullet:
			return EffectDictionary.GetDecal("bullet", materialName);
		case DamageType.Slash:
			return EffectDictionary.GetDecal("slash", materialName);
		case DamageType.Blunt:
			return EffectDictionary.GetDecal("blunt", materialName);
		case DamageType.Fall:
		case DamageType.Radiation:
		case DamageType.Bite:
			break;
		case DamageType.Stab:
			return EffectDictionary.GetDecal("stab", materialName);
		default:
			if (damageType == DamageType.Arrow)
			{
				return EffectDictionary.GetDecal("bullet", materialName);
			}
			break;
		}
		return EffectDictionary.GetDecal("blunt", materialName);
	}

	// Token: 0x0600279C RID: 10140 RVA: 0x000F3234 File Offset: 0x000F1434
	public static string GetDisplacement(string impactType, string materialName)
	{
		return EffectDictionary.LookupEffect("displacement", impactType, materialName);
	}

	// Token: 0x0600279D RID: 10141 RVA: 0x000F3244 File Offset: 0x000F1444
	private static string LookupEffect(string category, string effect, string material)
	{
		if (EffectDictionary.effectDictionary == null)
		{
			EffectDictionary.effectDictionary = GameManifest.LoadEffectDictionary();
		}
		string format = "assets/bundled/prefabs/fx/{0}/{1}/{2}";
		string[] array;
		if (!EffectDictionary.effectDictionary.TryGetValue(StringFormatCache.Get(format, category, effect, material), out array) && !EffectDictionary.effectDictionary.TryGetValue(StringFormatCache.Get(format, category, effect, "generic"), out array))
		{
			return string.Empty;
		}
		return array[UnityEngine.Random.Range(0, array.Length)];
	}

	// Token: 0x04001FCE RID: 8142
	private static Dictionary<string, string[]> effectDictionary;
}
