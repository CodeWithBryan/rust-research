using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007DC RID: 2012
public class UIRecordingInfo : SingletonComponent<UIRecordingInfo>
{
	// Token: 0x0600341D RID: 13341 RVA: 0x0013B4E2 File Offset: 0x001396E2
	private void Start()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x04002C9F RID: 11423
	public RustText CountdownText;

	// Token: 0x04002CA0 RID: 11424
	public Slider TapeProgressSlider;

	// Token: 0x04002CA1 RID: 11425
	public GameObject CountdownRoot;

	// Token: 0x04002CA2 RID: 11426
	public GameObject RecordingRoot;

	// Token: 0x04002CA3 RID: 11427
	public Transform Spinner;

	// Token: 0x04002CA4 RID: 11428
	public float SpinSpeed = 180f;

	// Token: 0x04002CA5 RID: 11429
	public Image CassetteImage;
}
