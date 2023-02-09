using System;
using UnityEngine.UI;

// Token: 0x02000128 RID: 296
public class TimerConfig : UIDialog
{
	// Token: 0x04000E33 RID: 3635
	[NonSerialized]
	private CustomTimerSwitch timerSwitch;

	// Token: 0x04000E34 RID: 3636
	public InputField input;

	// Token: 0x04000E35 RID: 3637
	public static float minTime = 0.25f;

	// Token: 0x04000E36 RID: 3638
	public float seconds;
}
