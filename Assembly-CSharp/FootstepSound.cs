using System;
using UnityEngine;

// Token: 0x02000213 RID: 531
public class FootstepSound : MonoBehaviour, IClientComponent
{
	// Token: 0x04001324 RID: 4900
	public SoundDefinition lightSound;

	// Token: 0x04001325 RID: 4901
	public SoundDefinition medSound;

	// Token: 0x04001326 RID: 4902
	public SoundDefinition hardSound;

	// Token: 0x04001327 RID: 4903
	private const float panAmount = 0.05f;

	// Token: 0x02000C22 RID: 3106
	public enum Hardness
	{
		// Token: 0x040040DF RID: 16607
		Light = 1,
		// Token: 0x040040E0 RID: 16608
		Medium,
		// Token: 0x040040E1 RID: 16609
		Hard
	}
}
