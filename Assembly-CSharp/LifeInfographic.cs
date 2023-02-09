using System;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000832 RID: 2098
public class LifeInfographic : MonoBehaviour
{
	// Token: 0x04002E97 RID: 11927
	[NonSerialized]
	public PlayerLifeStory life;

	// Token: 0x04002E98 RID: 11928
	public GameObject container;

	// Token: 0x04002E99 RID: 11929
	public RawImage AttackerAvatarImage;

	// Token: 0x04002E9A RID: 11930
	public Image DamageSourceImage;

	// Token: 0x04002E9B RID: 11931
	public LifeInfographicStat[] Stats;

	// Token: 0x04002E9C RID: 11932
	public Animator[] AllAnimators;

	// Token: 0x04002E9D RID: 11933
	public GameObject WeaponRoot;

	// Token: 0x04002E9E RID: 11934
	public GameObject DistanceRoot;

	// Token: 0x04002E9F RID: 11935
	public GameObject DistanceDivider;

	// Token: 0x04002EA0 RID: 11936
	public Image WeaponImage;

	// Token: 0x04002EA1 RID: 11937
	public LifeInfographic.DamageSetting[] DamageDisplays;

	// Token: 0x04002EA2 RID: 11938
	public bool ShowDebugData;

	// Token: 0x02000E2E RID: 3630
	[Serializable]
	public struct DamageSetting
	{
		// Token: 0x0400496B RID: 18795
		public DamageType ForType;

		// Token: 0x0400496C RID: 18796
		public string Display;

		// Token: 0x0400496D RID: 18797
		public Sprite DamageSprite;
	}
}
