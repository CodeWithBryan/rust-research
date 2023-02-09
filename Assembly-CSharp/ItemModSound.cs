using System;
using Rust;
using UnityEngine;

// Token: 0x020005D0 RID: 1488
public class ItemModSound : ItemMod
{
	// Token: 0x06002BED RID: 11245 RVA: 0x00107C80 File Offset: 0x00105E80
	public override void OnParentChanged(Item item)
	{
		if (Rust.Application.isLoadingSave)
		{
			return;
		}
		if (this.actionType == ItemModSound.Type.OnAttachToWeapon)
		{
			if (item.parentItem == null)
			{
				return;
			}
			if (item.parentItem.info.category != ItemCategory.Weapon)
			{
				return;
			}
			BasePlayer ownerPlayer = item.parentItem.GetOwnerPlayer();
			if (ownerPlayer == null)
			{
				return;
			}
			if (ownerPlayer.IsNpc)
			{
				return;
			}
			Effect.server.Run(this.effect.resourcePath, ownerPlayer, 0U, Vector3.zero, Vector3.zero, null, false);
		}
	}

	// Token: 0x040023C6 RID: 9158
	public GameObjectRef effect = new GameObjectRef();

	// Token: 0x040023C7 RID: 9159
	public ItemModSound.Type actionType;

	// Token: 0x02000D27 RID: 3367
	public enum Type
	{
		// Token: 0x0400453E RID: 17726
		OnAttachToWeapon
	}
}
