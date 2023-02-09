using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200087C RID: 2172
public class ProgressBar : UIBehaviour
{
	// Token: 0x04003032 RID: 12338
	public static ProgressBar Instance;

	// Token: 0x04003033 RID: 12339
	private Action<BasePlayer> action;

	// Token: 0x04003034 RID: 12340
	private float timeFinished;

	// Token: 0x04003035 RID: 12341
	private float timeCounter;

	// Token: 0x04003036 RID: 12342
	public GameObject scaleTarget;

	// Token: 0x04003037 RID: 12343
	public Image progressField;

	// Token: 0x04003038 RID: 12344
	public Image iconField;

	// Token: 0x04003039 RID: 12345
	public Text leftField;

	// Token: 0x0400303A RID: 12346
	public Text rightField;

	// Token: 0x0400303B RID: 12347
	public SoundDefinition clipOpen;

	// Token: 0x0400303C RID: 12348
	public SoundDefinition clipCancel;

	// Token: 0x0400303D RID: 12349
	public bool IsOpen;
}
