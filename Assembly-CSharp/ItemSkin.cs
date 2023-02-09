using System;
using Rust.Workshop;
using UnityEngine;

// Token: 0x02000726 RID: 1830
[CreateAssetMenu(menuName = "Rust/ItemSkin")]
public class ItemSkin : SteamInventoryItem
{
	// Token: 0x060032C6 RID: 12998 RVA: 0x00139C6B File Offset: 0x00137E6B
	public void ApplySkin(GameObject obj)
	{
		if (this.Skinnable == null)
		{
			return;
		}
		Skin.Apply(obj, this.Skinnable, this.Materials);
	}

	// Token: 0x060032C7 RID: 12999 RVA: 0x00139C90 File Offset: 0x00137E90
	public override bool HasUnlocked(ulong playerId)
	{
		if (this.Redirect != null && this.Redirect.isRedirectOf != null && this.Redirect.isRedirectOf.steamItem != null)
		{
			BasePlayer basePlayer = BasePlayer.FindByID(playerId);
			if (basePlayer != null && basePlayer.blueprints.CheckSkinOwnership(this.Redirect.isRedirectOf.steamItem.id, basePlayer.userID))
			{
				return true;
			}
		}
		if (this.UnlockedViaSteamItem != null)
		{
			BasePlayer basePlayer2 = BasePlayer.FindByID(playerId);
			if (basePlayer2 != null && basePlayer2.blueprints.CheckSkinOwnership(this.UnlockedViaSteamItem.id, basePlayer2.userID))
			{
				return true;
			}
		}
		return base.HasUnlocked(playerId);
	}

	// Token: 0x04002917 RID: 10519
	public Skinnable Skinnable;

	// Token: 0x04002918 RID: 10520
	public Material[] Materials;

	// Token: 0x04002919 RID: 10521
	[Tooltip("If set, whenever we make an item with this skin, we'll spawn this item without a skin instead")]
	public ItemDefinition Redirect;

	// Token: 0x0400291A RID: 10522
	public SteamInventoryItem UnlockedViaSteamItem;
}
