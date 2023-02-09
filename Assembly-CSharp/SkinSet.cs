using System;
using UnityEngine;

// Token: 0x02000732 RID: 1842
[CreateAssetMenu(menuName = "Rust/Skin Set")]
public class SkinSet : ScriptableObject
{
	// Token: 0x060032F4 RID: 13044 RVA: 0x0013AD1C File Offset: 0x00138F1C
	internal Color GetSkinColor(float skinNumber)
	{
		return this.SkinColour.Evaluate(skinNumber);
	}

	// Token: 0x0400295C RID: 10588
	public string Label;

	// Token: 0x0400295D RID: 10589
	public Gradient SkinColour;

	// Token: 0x0400295E RID: 10590
	public HairSetCollection HairCollection;

	// Token: 0x0400295F RID: 10591
	[Header("Models")]
	public GameObjectRef Head;

	// Token: 0x04002960 RID: 10592
	public GameObjectRef Torso;

	// Token: 0x04002961 RID: 10593
	public GameObjectRef Legs;

	// Token: 0x04002962 RID: 10594
	public GameObjectRef Feet;

	// Token: 0x04002963 RID: 10595
	public GameObjectRef Hands;

	// Token: 0x04002964 RID: 10596
	[Header("Censored Variants")]
	public GameObjectRef CensoredTorso;

	// Token: 0x04002965 RID: 10597
	public GameObjectRef CensoredLegs;

	// Token: 0x04002966 RID: 10598
	[Header("Materials")]
	public Material HeadMaterial;

	// Token: 0x04002967 RID: 10599
	public Material BodyMaterial;

	// Token: 0x04002968 RID: 10600
	public Material EyeMaterial;
}
