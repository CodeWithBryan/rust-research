using System;
using Rust.UI;
using UnityEngine;

// Token: 0x02000773 RID: 1907
public class CopyText : MonoBehaviour
{
	// Token: 0x0600335E RID: 13150 RVA: 0x0013B6FB File Offset: 0x001398FB
	public void TriggerCopy()
	{
		if (this.TargetText != null)
		{
			GUIUtility.systemCopyBuffer = this.TargetText.text;
		}
	}

	// Token: 0x040029EC RID: 10732
	public RustText TargetText;
}
