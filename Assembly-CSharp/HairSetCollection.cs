using System;
using UnityEngine;

// Token: 0x02000725 RID: 1829
[CreateAssetMenu(menuName = "Rust/Hair Set Collection")]
public class HairSetCollection : ScriptableObject
{
	// Token: 0x060032C1 RID: 12993 RVA: 0x00139BC0 File Offset: 0x00137DC0
	public HairSetCollection.HairSetEntry[] GetListByType(HairType hairType)
	{
		switch (hairType)
		{
		case HairType.Head:
			return this.Head;
		case HairType.Eyebrow:
			return this.Eyebrow;
		case HairType.Facial:
			return this.Facial;
		case HairType.Armpit:
			return this.Armpit;
		case HairType.Pubic:
			return this.Pubic;
		default:
			return null;
		}
	}

	// Token: 0x060032C2 RID: 12994 RVA: 0x00139C0D File Offset: 0x00137E0D
	public int GetIndex(HairSetCollection.HairSetEntry[] list, float typeNum)
	{
		return Mathf.Clamp(Mathf.FloorToInt(typeNum * (float)list.Length), 0, list.Length - 1);
	}

	// Token: 0x060032C3 RID: 12995 RVA: 0x00139C28 File Offset: 0x00137E28
	public int GetIndex(HairType hairType, float typeNum)
	{
		HairSetCollection.HairSetEntry[] listByType = this.GetListByType(hairType);
		return this.GetIndex(listByType, typeNum);
	}

	// Token: 0x060032C4 RID: 12996 RVA: 0x00139C48 File Offset: 0x00137E48
	public HairSetCollection.HairSetEntry Get(HairType hairType, float typeNum)
	{
		HairSetCollection.HairSetEntry[] listByType = this.GetListByType(hairType);
		return listByType[this.GetIndex(listByType, typeNum)];
	}

	// Token: 0x04002912 RID: 10514
	public HairSetCollection.HairSetEntry[] Head;

	// Token: 0x04002913 RID: 10515
	public HairSetCollection.HairSetEntry[] Eyebrow;

	// Token: 0x04002914 RID: 10516
	public HairSetCollection.HairSetEntry[] Facial;

	// Token: 0x04002915 RID: 10517
	public HairSetCollection.HairSetEntry[] Armpit;

	// Token: 0x04002916 RID: 10518
	public HairSetCollection.HairSetEntry[] Pubic;

	// Token: 0x02000E06 RID: 3590
	[Serializable]
	public struct HairSetEntry
	{
		// Token: 0x040048C6 RID: 18630
		public HairSet HairSet;

		// Token: 0x040048C7 RID: 18631
		public GameObjectRef HairPrefab;

		// Token: 0x040048C8 RID: 18632
		public HairDyeCollection HairDyeCollection;
	}
}
