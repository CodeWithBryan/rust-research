using System;
using UnityEngine;

// Token: 0x020005E0 RID: 1504
[CreateAssetMenu(menuName = "Rust/MaterialSound")]
public class MaterialSound : ScriptableObject
{
	// Token: 0x0400240A RID: 9226
	public SoundDefinition DefaultSound;

	// Token: 0x0400240B RID: 9227
	public MaterialSound.Entry[] Entries;

	// Token: 0x02000D2E RID: 3374
	[Serializable]
	public class Entry
	{
		// Token: 0x0400454E RID: 17742
		public PhysicMaterial Material;

		// Token: 0x0400454F RID: 17743
		public SoundDefinition Sound;
	}
}
