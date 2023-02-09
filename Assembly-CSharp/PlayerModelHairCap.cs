using System;
using UnityEngine;

// Token: 0x020002BC RID: 700
public class PlayerModelHairCap : MonoBehaviour
{
	// Token: 0x06001C7F RID: 7295 RVA: 0x000C3F6C File Offset: 0x000C216C
	public void SetupHairCap(SkinSetCollection skin, float hairNum, float meshNum, MaterialPropertyBlock block)
	{
		int index = skin.GetIndex(meshNum);
		SkinSet skinSet = skin.Skins[index];
		if (skinSet == null)
		{
			return;
		}
		for (int i = 0; i < 5; i++)
		{
			if ((this.hairCapMask & (HairCapMask)(1 << i)) != (HairCapMask)0)
			{
				float typeNum;
				float seed;
				PlayerModelHair.GetRandomVariation(hairNum, i, index, out typeNum, out seed);
				HairType hairType = (HairType)i;
				HairSetCollection.HairSetEntry hairSetEntry = skinSet.HairCollection.Get(hairType, typeNum);
				if (!(hairSetEntry.HairSet == null))
				{
					HairDyeCollection hairDyeCollection = hairSetEntry.HairDyeCollection;
					if (!(hairDyeCollection == null))
					{
						HairDye hairDye = hairDyeCollection.Get(seed);
						if (hairDye != null)
						{
							hairDye.ApplyCap(hairDyeCollection, hairType, block);
						}
					}
				}
			}
		}
	}

	// Token: 0x040015F4 RID: 5620
	[InspectorFlags]
	public HairCapMask hairCapMask;
}
