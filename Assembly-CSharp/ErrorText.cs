using System;
using System.Diagnostics;
using Facepunch;
using Rust;
using TMPro;
using UnityEngine;

// Token: 0x020007B3 RID: 1971
public class ErrorText : MonoBehaviour
{
	// Token: 0x060033BB RID: 13243 RVA: 0x0013BD00 File Offset: 0x00139F00
	public void OnEnable()
	{
		Output.OnMessage += this.CaptureLog;
	}

	// Token: 0x060033BC RID: 13244 RVA: 0x0013BD13 File Offset: 0x00139F13
	public void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		Output.OnMessage -= this.CaptureLog;
	}

	// Token: 0x060033BD RID: 13245 RVA: 0x0013BD30 File Offset: 0x00139F30
	internal void CaptureLog(string error, string stacktrace, LogType type)
	{
		if (type != LogType.Error && type != LogType.Exception && type != LogType.Assert)
		{
			return;
		}
		if (this.text == null)
		{
			return;
		}
		TextMeshProUGUI textMeshProUGUI = this.text;
		textMeshProUGUI.text = string.Concat(new string[]
		{
			textMeshProUGUI.text,
			error,
			"\n",
			stacktrace,
			"\n\n"
		});
		if (this.text.text.Length > this.maxLength)
		{
			this.text.text = this.text.text.Substring(this.text.text.Length - this.maxLength, this.maxLength);
		}
		this.stopwatch = Stopwatch.StartNew();
	}

	// Token: 0x060033BE RID: 13246 RVA: 0x0013BDEC File Offset: 0x00139FEC
	protected void Update()
	{
		if (this.stopwatch != null && this.stopwatch.Elapsed.TotalSeconds > 30.0)
		{
			this.text.text = string.Empty;
			this.stopwatch = null;
		}
	}

	// Token: 0x04002BAC RID: 11180
	public TextMeshProUGUI text;

	// Token: 0x04002BAD RID: 11181
	public int maxLength = 1024;

	// Token: 0x04002BAE RID: 11182
	private Stopwatch stopwatch;
}
