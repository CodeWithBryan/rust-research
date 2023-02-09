using System;
using Rust.UI;
using UnityEngine;

// Token: 0x020007EB RID: 2027
public class ChangelogButton : MonoBehaviour
{
	// Token: 0x06003430 RID: 13360 RVA: 0x0013D540 File Offset: 0x0013B740
	private void Update()
	{
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(false);
		if (activeGameMode != null)
		{
			if (this.CanvasGroup.alpha != 1f)
			{
				this.CanvasGroup.alpha = 1f;
				this.CanvasGroup.blocksRaycasts = true;
				this.Button.Text.SetPhrase(new Translate.Phrase(activeGameMode.shortname, activeGameMode.shortname));
				return;
			}
		}
		else if (this.CanvasGroup.alpha != 0f)
		{
			this.CanvasGroup.alpha = 0f;
			this.CanvasGroup.blocksRaycasts = false;
		}
	}

	// Token: 0x04002CFA RID: 11514
	public RustButton Button;

	// Token: 0x04002CFB RID: 11515
	public CanvasGroup CanvasGroup;
}
