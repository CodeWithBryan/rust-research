using System;
using UnityEngine;

// Token: 0x02000739 RID: 1849
[CreateAssetMenu(menuName = "Rust/Underwear")]
public class Underwear : ScriptableObject
{
	// Token: 0x0600330B RID: 13067 RVA: 0x0013B0D2 File Offset: 0x001392D2
	public uint GetID()
	{
		return StringPool.Get(this.shortname);
	}

	// Token: 0x0600330C RID: 13068 RVA: 0x0013B0DF File Offset: 0x001392DF
	public bool HasMaleParts()
	{
		return this.replacementsMale.Length != 0;
	}

	// Token: 0x0600330D RID: 13069 RVA: 0x0013B0EB File Offset: 0x001392EB
	public bool HasFemaleParts()
	{
		return this.replacementsFemale.Length != 0;
	}

	// Token: 0x0600330E RID: 13070 RVA: 0x0013B0F8 File Offset: 0x001392F8
	public bool ValidForPlayer(BasePlayer player)
	{
		if (this.HasMaleParts() && this.HasFemaleParts())
		{
			return true;
		}
		bool flag = Underwear.IsFemale(player);
		return (flag && this.HasFemaleParts()) || (!flag && this.HasMaleParts());
	}

	// Token: 0x0600330F RID: 13071 RVA: 0x0013B13C File Offset: 0x0013933C
	public static bool IsFemale(BasePlayer player)
	{
		ulong userID = player.userID;
		ulong num = 4332UL;
		UnityEngine.Random.State state = UnityEngine.Random.state;
		UnityEngine.Random.InitState((int)(num + userID));
		float num2 = UnityEngine.Random.Range(0f, 1f);
		UnityEngine.Random.state = state;
		return num2 > 0.5f;
	}

	// Token: 0x06003310 RID: 13072 RVA: 0x0013B184 File Offset: 0x00139384
	public static bool Validate(Underwear underwear, BasePlayer player)
	{
		if (underwear == null)
		{
			return true;
		}
		if (!underwear.ValidForPlayer(player))
		{
			return false;
		}
		if (underwear.adminOnly && (!player.IsAdmin || !player.IsDeveloper))
		{
			return false;
		}
		bool flag = underwear.steamItem == null || player.blueprints.steamInventory.HasItem(underwear.steamItem.id);
		bool flag2 = false;
		if (player.isServer && (underwear.steamDLC == null || underwear.steamDLC.HasLicense(player.userID)))
		{
			flag2 = true;
		}
		return flag && flag2;
	}

	// Token: 0x04002989 RID: 10633
	public string shortname = "";

	// Token: 0x0400298A RID: 10634
	public Translate.Phrase displayName;

	// Token: 0x0400298B RID: 10635
	public Sprite icon;

	// Token: 0x0400298C RID: 10636
	public Sprite iconFemale;

	// Token: 0x0400298D RID: 10637
	public SkinReplacement[] replacementsMale;

	// Token: 0x0400298E RID: 10638
	public SkinReplacement[] replacementsFemale;

	// Token: 0x0400298F RID: 10639
	[Tooltip("User can craft this item on any server if they have this steam item")]
	public SteamInventoryItem steamItem;

	// Token: 0x04002990 RID: 10640
	[Tooltip("User can craft this item if they have this DLC purchased")]
	public SteamDLCItem steamDLC;

	// Token: 0x04002991 RID: 10641
	public bool adminOnly;
}
