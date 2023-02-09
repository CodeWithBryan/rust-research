using System;
using UnityEngine;

// Token: 0x02000323 RID: 803
public class FirstPersonEffect : MonoBehaviour, IEffect
{
	// Token: 0x04001770 RID: 6000
	public bool isGunShot;

	// Token: 0x04001771 RID: 6001
	[HideInInspector]
	public EffectParentToWeaponBone parentToWeaponComponent;
}
