using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x020008C7 RID: 2247
public class ConvarComponent : MonoBehaviour
{
	// Token: 0x0600362B RID: 13867 RVA: 0x00143890 File Offset: 0x00141A90
	protected void OnEnable()
	{
		if (!this.ShouldRun())
		{
			return;
		}
		foreach (ConvarComponent.ConvarEvent convarEvent in this.List)
		{
			convarEvent.OnEnable();
		}
	}

	// Token: 0x0600362C RID: 13868 RVA: 0x001438EC File Offset: 0x00141AEC
	protected void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		if (!this.ShouldRun())
		{
			return;
		}
		foreach (ConvarComponent.ConvarEvent convarEvent in this.List)
		{
			convarEvent.OnDisable();
		}
	}

	// Token: 0x0600362D RID: 13869 RVA: 0x00143950 File Offset: 0x00141B50
	private bool ShouldRun()
	{
		return this.runOnServer;
	}

	// Token: 0x04003121 RID: 12577
	public bool runOnServer = true;

	// Token: 0x04003122 RID: 12578
	public bool runOnClient = true;

	// Token: 0x04003123 RID: 12579
	public List<ConvarComponent.ConvarEvent> List = new List<ConvarComponent.ConvarEvent>();

	// Token: 0x02000E52 RID: 3666
	[Serializable]
	public class ConvarEvent
	{
		// Token: 0x06005047 RID: 20551 RVA: 0x001A124C File Offset: 0x0019F44C
		public void OnEnable()
		{
			this.cmd = ConsoleSystem.Index.Client.Find(this.convar);
			if (this.cmd == null)
			{
				this.cmd = ConsoleSystem.Index.Server.Find(this.convar);
			}
			if (this.cmd == null)
			{
				return;
			}
			this.cmd.OnValueChanged += this.cmd_OnValueChanged;
			this.cmd_OnValueChanged(this.cmd);
		}

		// Token: 0x06005048 RID: 20552 RVA: 0x001A12B0 File Offset: 0x0019F4B0
		private void cmd_OnValueChanged(ConsoleSystem.Command obj)
		{
			if (this.component == null)
			{
				return;
			}
			bool flag = obj.String == this.on;
			if (this.component.enabled == flag)
			{
				return;
			}
			this.component.enabled = flag;
		}

		// Token: 0x06005049 RID: 20553 RVA: 0x001A12F9 File Offset: 0x0019F4F9
		public void OnDisable()
		{
			if (Rust.Application.isQuitting)
			{
				return;
			}
			if (this.cmd == null)
			{
				return;
			}
			this.cmd.OnValueChanged -= this.cmd_OnValueChanged;
		}

		// Token: 0x04004A14 RID: 18964
		public string convar;

		// Token: 0x04004A15 RID: 18965
		public string on;

		// Token: 0x04004A16 RID: 18966
		public MonoBehaviour component;

		// Token: 0x04004A17 RID: 18967
		internal ConsoleSystem.Command cmd;
	}
}
