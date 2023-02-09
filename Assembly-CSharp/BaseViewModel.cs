using System;
using UnityEngine;

// Token: 0x02000937 RID: 2359
public class BaseViewModel : MonoBehaviour
{
	// Token: 0x04003223 RID: 12835
	[Header("BaseViewModel")]
	public LazyAimProperties lazyaimRegular;

	// Token: 0x04003224 RID: 12836
	public LazyAimProperties lazyaimIronsights;

	// Token: 0x04003225 RID: 12837
	public Transform pivot;

	// Token: 0x04003226 RID: 12838
	public bool useViewModelCamera = true;

	// Token: 0x04003227 RID: 12839
	public bool wantsHeldItemFlags;

	// Token: 0x04003228 RID: 12840
	public GameObject[] hideSightMeshes;

	// Token: 0x04003229 RID: 12841
	public bool isGestureViewModel;

	// Token: 0x0400322A RID: 12842
	public Transform MuzzlePoint;

	// Token: 0x0400322B RID: 12843
	[Header("Skin")]
	public SubsurfaceProfile subsurfaceProfile;
}
