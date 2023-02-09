using System;
using UnityEngine;

// Token: 0x0200052E RID: 1326
[CreateAssetMenu(menuName = "Rust/Armor Properties")]
public class ArmorProperties : ScriptableObject
{
	// Token: 0x060028AF RID: 10415 RVA: 0x000F7AF1 File Offset: 0x000F5CF1
	public bool Contains(HitArea hitArea)
	{
		return (this.area & hitArea) > (HitArea)0;
	}

	// Token: 0x0400210C RID: 8460
	[InspectorFlags]
	public HitArea area;
}
