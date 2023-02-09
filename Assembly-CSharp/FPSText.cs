using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007B5 RID: 1973
public class FPSText : MonoBehaviour
{
	// Token: 0x060033C1 RID: 13249 RVA: 0x0013BE4C File Offset: 0x0013A04C
	protected void Update()
	{
		if (this.fpsTimer.Elapsed.TotalSeconds < 0.5)
		{
			return;
		}
		this.text.enabled = true;
		this.fpsTimer.Reset();
		this.fpsTimer.Start();
		string text = Performance.current.frameRate + " FPS";
		this.text.text = text;
	}

	// Token: 0x04002BBF RID: 11199
	public Text text;

	// Token: 0x04002BC0 RID: 11200
	private Stopwatch fpsTimer = Stopwatch.StartNew();
}
