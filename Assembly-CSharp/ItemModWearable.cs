using System;
using Rust;
using UnityEngine;

// Token: 0x020005D7 RID: 1495
public class ItemModWearable : ItemMod
{
	// Token: 0x1700036C RID: 876
	// (get) Token: 0x06002BFB RID: 11259 RVA: 0x001082EF File Offset: 0x001064EF
	public Wearable targetWearable
	{
		get
		{
			if (this.entityPrefab.isValid)
			{
				return this.entityPrefab.Get().GetComponent<Wearable>();
			}
			return null;
		}
	}

	// Token: 0x06002BFC RID: 11260 RVA: 0x00108310 File Offset: 0x00106510
	private void DoPrepare()
	{
		if (!this.entityPrefab.isValid)
		{
			Debug.LogWarning("ItemModWearable: entityPrefab is null! " + base.gameObject, base.gameObject);
		}
		if (this.entityPrefab.isValid && this.targetWearable == null)
		{
			Debug.LogWarning("ItemModWearable: entityPrefab doesn't have a Wearable component! " + base.gameObject, this.entityPrefab.Get());
		}
	}

	// Token: 0x06002BFD RID: 11261 RVA: 0x00108380 File Offset: 0x00106580
	public override void ModInit()
	{
		if (string.IsNullOrEmpty(this.entityPrefab.resourcePath))
		{
			Debug.LogWarning(this + " - entityPrefab is null or something.. - " + this.entityPrefab.guid);
		}
	}

	// Token: 0x06002BFE RID: 11262 RVA: 0x001083AF File Offset: 0x001065AF
	public bool ProtectsArea(HitArea area)
	{
		return !(this.armorProperties == null) && this.armorProperties.Contains(area);
	}

	// Token: 0x06002BFF RID: 11263 RVA: 0x001083CD File Offset: 0x001065CD
	public bool HasProtections()
	{
		return this.protectionProperties != null;
	}

	// Token: 0x06002C00 RID: 11264 RVA: 0x001083DB File Offset: 0x001065DB
	internal float GetProtection(Item item, DamageType damageType)
	{
		if (this.protectionProperties == null)
		{
			return 0f;
		}
		return this.protectionProperties.Get(damageType) * this.ConditionProtectionScale(item);
	}

	// Token: 0x06002C01 RID: 11265 RVA: 0x00108405 File Offset: 0x00106605
	public float ConditionProtectionScale(Item item)
	{
		if (!item.isBroken)
		{
			return 1f;
		}
		return 0.25f;
	}

	// Token: 0x06002C02 RID: 11266 RVA: 0x0010841A File Offset: 0x0010661A
	public void CollectProtection(Item item, ProtectionProperties protection)
	{
		if (this.protectionProperties == null)
		{
			return;
		}
		protection.Add(this.protectionProperties, this.ConditionProtectionScale(item));
	}

	// Token: 0x06002C03 RID: 11267 RVA: 0x00108440 File Offset: 0x00106640
	private bool IsHeadgear()
	{
		Wearable component = this.entityPrefab.Get().GetComponent<Wearable>();
		return component != null && (component.occupationOver & (Wearable.OccupationSlots.HeadTop | Wearable.OccupationSlots.Face | Wearable.OccupationSlots.HeadBack)) != (Wearable.OccupationSlots)0;
	}

	// Token: 0x06002C04 RID: 11268 RVA: 0x00108474 File Offset: 0x00106674
	public bool IsFootwear()
	{
		Wearable component = this.entityPrefab.Get().GetComponent<Wearable>();
		return component != null && (component.occupationOver & (Wearable.OccupationSlots.LeftFoot | Wearable.OccupationSlots.RightFoot)) != (Wearable.OccupationSlots)0;
	}

	// Token: 0x06002C05 RID: 11269 RVA: 0x001084AC File Offset: 0x001066AC
	public override void OnAttacked(Item item, HitInfo info)
	{
		if (!item.hasCondition)
		{
			return;
		}
		float num = 0f;
		for (int i = 0; i < 25; i++)
		{
			DamageType damageType = (DamageType)i;
			if (info.damageTypes.Has(damageType))
			{
				num += Mathf.Clamp(info.damageTypes.types[i] * this.GetProtection(item, damageType), 0f, item.condition);
				if (num >= item.condition)
				{
					break;
				}
			}
		}
		item.LoseCondition(num);
		if (item != null && item.isBroken && item.GetOwnerPlayer() && this.IsHeadgear() && info.damageTypes.Total() >= item.GetOwnerPlayer().health)
		{
			item.Drop(item.GetOwnerPlayer().transform.position + new Vector3(0f, 1.8f, 0f), item.GetOwnerPlayer().GetInheritedDropVelocity() + Vector3.up * 3f, default(Quaternion)).SetAngularVelocity(UnityEngine.Random.rotation.eulerAngles * 5f);
		}
	}

	// Token: 0x06002C06 RID: 11270 RVA: 0x001085D8 File Offset: 0x001067D8
	public bool CanExistWith(ItemModWearable wearable)
	{
		if (wearable == null)
		{
			return true;
		}
		Wearable targetWearable = this.targetWearable;
		Wearable targetWearable2 = wearable.targetWearable;
		return (targetWearable.occupationOver & targetWearable2.occupationOver) == (Wearable.OccupationSlots)0 && (targetWearable.occupationUnder & targetWearable2.occupationUnder) == (Wearable.OccupationSlots)0;
	}

	// Token: 0x040023DC RID: 9180
	public GameObjectRef entityPrefab = new GameObjectRef();

	// Token: 0x040023DD RID: 9181
	public GameObjectRef entityPrefabFemale = new GameObjectRef();

	// Token: 0x040023DE RID: 9182
	public ProtectionProperties protectionProperties;

	// Token: 0x040023DF RID: 9183
	public ArmorProperties armorProperties;

	// Token: 0x040023E0 RID: 9184
	public ClothingMovementProperties movementProperties;

	// Token: 0x040023E1 RID: 9185
	public UIBlackoutOverlay.blackoutType occlusionType = UIBlackoutOverlay.blackoutType.NONE;

	// Token: 0x040023E2 RID: 9186
	public bool blocksAiming;

	// Token: 0x040023E3 RID: 9187
	public bool emissive;

	// Token: 0x040023E4 RID: 9188
	public float accuracyBonus;

	// Token: 0x040023E5 RID: 9189
	public bool blocksEquipping;

	// Token: 0x040023E6 RID: 9190
	public float eggVision;

	// Token: 0x040023E7 RID: 9191
	public float weight;

	// Token: 0x040023E8 RID: 9192
	public bool equipOnRightClick = true;

	// Token: 0x040023E9 RID: 9193
	public bool npcOnly;

	// Token: 0x040023EA RID: 9194
	public GameObjectRef breakEffect = new GameObjectRef();

	// Token: 0x040023EB RID: 9195
	public GameObjectRef viewmodelAddition;
}
