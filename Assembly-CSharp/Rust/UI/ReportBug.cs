using System;
using UnityEngine;
using UnityEngine.UI;

namespace Rust.UI
{
	// Token: 0x02000ACA RID: 2762
	public class ReportBug : UIDialog
	{
		// Token: 0x04003B00 RID: 15104
		public GameObject GetInformation;

		// Token: 0x04003B01 RID: 15105
		public GameObject Finished;

		// Token: 0x04003B02 RID: 15106
		public RustInput Subject;

		// Token: 0x04003B03 RID: 15107
		public RustInput Message;

		// Token: 0x04003B04 RID: 15108
		public RustButton ReportButton;

		// Token: 0x04003B05 RID: 15109
		public RustButtonGroup Category;

		// Token: 0x04003B06 RID: 15110
		public RustIcon ProgressIcon;

		// Token: 0x04003B07 RID: 15111
		public RustText ProgressText;

		// Token: 0x04003B08 RID: 15112
		public RawImage ScreenshotImage;

		// Token: 0x04003B09 RID: 15113
		public GameObject ScreenshotRoot;

		// Token: 0x04003B0A RID: 15114
		public UIBackgroundBlur BlurController;

		// Token: 0x04003B0B RID: 15115
		public RustButton SubmitButton;

		// Token: 0x04003B0C RID: 15116
		public GameObject SubmitErrorRoot;

		// Token: 0x04003B0D RID: 15117
		public RustText CooldownText;

		// Token: 0x04003B0E RID: 15118
		public RustText ContentMissingText;
	}
}
