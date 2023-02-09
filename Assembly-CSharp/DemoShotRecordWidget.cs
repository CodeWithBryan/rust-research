using System;
using Rust.UI;
using UnityEngine;

// Token: 0x02000787 RID: 1927
public class DemoShotRecordWidget : MonoBehaviour
{
	// Token: 0x04002A23 RID: 10787
	public RustInput NameInput;

	// Token: 0x04002A24 RID: 10788
	public GameObject RecordingRoot;

	// Token: 0x04002A25 RID: 10789
	public GameObject PreRecordingRoot;

	// Token: 0x04002A26 RID: 10790
	public RustButton CountdownToggle;

	// Token: 0x04002A27 RID: 10791
	public RustButton PauseOnSaveToggle;

	// Token: 0x04002A28 RID: 10792
	public RustButton ReturnToStartToggle;

	// Token: 0x04002A29 RID: 10793
	public RustButton RecordDofToggle;

	// Token: 0x04002A2A RID: 10794
	public RustOption FolderDropdown;

	// Token: 0x04002A2B RID: 10795
	public GameObject RecordingUnderlay;

	// Token: 0x04002A2C RID: 10796
	public AudioSource CountdownAudio;

	// Token: 0x04002A2D RID: 10797
	public GameObject ShotRecordTime;

	// Token: 0x04002A2E RID: 10798
	public RustText ShotRecordTimeText;

	// Token: 0x04002A2F RID: 10799
	public RustText ShotNameText;

	// Token: 0x04002A30 RID: 10800
	public GameObject RecordingInProcessRoot;

	// Token: 0x04002A31 RID: 10801
	public GameObject CountdownActiveRoot;

	// Token: 0x04002A32 RID: 10802
	public GameObject CountdownActiveSliderRoot;

	// Token: 0x04002A33 RID: 10803
	public RustSlider CountdownActiveSlider;

	// Token: 0x04002A34 RID: 10804
	public RustText CountdownActiveText;
}
