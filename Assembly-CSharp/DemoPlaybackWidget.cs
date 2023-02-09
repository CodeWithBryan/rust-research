using System;
using Rust.UI;
using UnityEngine;

// Token: 0x02000780 RID: 1920
public class DemoPlaybackWidget : MonoBehaviour
{
	// Token: 0x04002A00 RID: 10752
	public RustSlider DemoProgress;

	// Token: 0x04002A01 RID: 10753
	public RustText DemoName;

	// Token: 0x04002A02 RID: 10754
	public RustText DemoDuration;

	// Token: 0x04002A03 RID: 10755
	public RustText DemoCurrentTime;

	// Token: 0x04002A04 RID: 10756
	public GameObject PausedRoot;

	// Token: 0x04002A05 RID: 10757
	public GameObject PlayingRoot;

	// Token: 0x04002A06 RID: 10758
	public RectTransform DemoPlaybackHandle;

	// Token: 0x04002A07 RID: 10759
	public RectTransform ShotPlaybackWindow;

	// Token: 0x04002A08 RID: 10760
	public RustButton LoopButton;

	// Token: 0x04002A09 RID: 10761
	public GameObject ShotButtonRoot;

	// Token: 0x04002A0A RID: 10762
	public RustText ShotNameText;

	// Token: 0x04002A0B RID: 10763
	public GameObject ShotNameRoot;

	// Token: 0x04002A0C RID: 10764
	public RectTransform ShotRecordWindow;
}
