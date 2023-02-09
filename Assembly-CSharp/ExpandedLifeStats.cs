using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007B4 RID: 1972
public class ExpandedLifeStats : MonoBehaviour
{
	// Token: 0x04002BAF RID: 11183
	public GameObject DisplayRoot;

	// Token: 0x04002BB0 RID: 11184
	public GameObjectRef GenericStatRow;

	// Token: 0x04002BB1 RID: 11185
	[Header("Resources")]
	public Transform ResourcesStatRoot;

	// Token: 0x04002BB2 RID: 11186
	public List<ExpandedLifeStats.GenericStatDisplay> ResourceStats;

	// Token: 0x04002BB3 RID: 11187
	[Header("Weapons")]
	public GameObjectRef WeaponStatRow;

	// Token: 0x04002BB4 RID: 11188
	public Transform WeaponsRoot;

	// Token: 0x04002BB5 RID: 11189
	[Header("Misc")]
	public Transform MiscRoot;

	// Token: 0x04002BB6 RID: 11190
	public List<ExpandedLifeStats.GenericStatDisplay> MiscStats;

	// Token: 0x04002BB7 RID: 11191
	public LifeInfographic Infographic;

	// Token: 0x04002BB8 RID: 11192
	public RectTransform MoveRoot;

	// Token: 0x04002BB9 RID: 11193
	public Vector2 OpenPosition;

	// Token: 0x04002BBA RID: 11194
	public Vector2 ClosedPosition;

	// Token: 0x04002BBB RID: 11195
	public GameObject OpenButtonRoot;

	// Token: 0x04002BBC RID: 11196
	public GameObject CloseButtonRoot;

	// Token: 0x04002BBD RID: 11197
	public GameObject ScrollGradient;

	// Token: 0x04002BBE RID: 11198
	public ScrollRect Scroller;

	// Token: 0x02000E1E RID: 3614
	[Serializable]
	public struct GenericStatDisplay
	{
		// Token: 0x0400494A RID: 18762
		public string statKey;

		// Token: 0x0400494B RID: 18763
		public Sprite statSprite;

		// Token: 0x0400494C RID: 18764
		public Translate.Phrase displayPhrase;
	}
}
