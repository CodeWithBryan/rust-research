using System;
using UnityEngine;

// Token: 0x02000772 RID: 1906
public class ConvarToggleChildren : MonoBehaviour
{
	// Token: 0x0600335A RID: 13146 RVA: 0x0013B5EC File Offset: 0x001397EC
	protected void Awake()
	{
		this.Command = ConsoleSystem.Index.Client.Find(this.ConvarName);
		if (this.Command == null)
		{
			this.Command = ConsoleSystem.Index.Server.Find(this.ConvarName);
		}
		if (this.Command != null)
		{
			this.SetState(this.Command.String == this.ConvarEnabled);
		}
	}

	// Token: 0x0600335B RID: 13147 RVA: 0x0013B648 File Offset: 0x00139848
	protected void Update()
	{
		if (this.Command != null)
		{
			bool flag = this.Command.String == this.ConvarEnabled;
			if (this.state != flag)
			{
				this.SetState(flag);
			}
		}
	}

	// Token: 0x0600335C RID: 13148 RVA: 0x0013B684 File Offset: 0x00139884
	private void SetState(bool newState)
	{
		foreach (object obj in base.transform)
		{
			((Transform)obj).gameObject.SetActive(newState);
		}
		this.state = newState;
	}

	// Token: 0x040029E8 RID: 10728
	public string ConvarName;

	// Token: 0x040029E9 RID: 10729
	public string ConvarEnabled = "True";

	// Token: 0x040029EA RID: 10730
	private bool state;

	// Token: 0x040029EB RID: 10731
	private ConsoleSystem.Command Command;
}
