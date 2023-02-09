using System;
using UnityEngine;

// Token: 0x02000733 RID: 1843
[CreateAssetMenu(menuName = "Rust/Skin Set Collection")]
public class SkinSetCollection : ScriptableObject
{
	// Token: 0x060032F6 RID: 13046 RVA: 0x0013AD2A File Offset: 0x00138F2A
	public int GetIndex(float MeshNumber)
	{
		return Mathf.Clamp(Mathf.FloorToInt(MeshNumber * (float)this.Skins.Length), 0, this.Skins.Length - 1);
	}

	// Token: 0x060032F7 RID: 13047 RVA: 0x0013AD4C File Offset: 0x00138F4C
	public SkinSet Get(float MeshNumber)
	{
		return this.Skins[this.GetIndex(MeshNumber)];
	}

	// Token: 0x04002969 RID: 10601
	public SkinSet[] Skins;
}
