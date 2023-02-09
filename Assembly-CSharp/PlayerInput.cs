using System;
using Rust;

// Token: 0x02000421 RID: 1057
public class PlayerInput : EntityComponent<BasePlayer>
{
	// Token: 0x06002335 RID: 9013 RVA: 0x000DFCD0 File Offset: 0x000DDED0
	protected void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		this.state.Clear();
	}

	// Token: 0x04001B9E RID: 7070
	public InputState state = new InputState();

	// Token: 0x04001B9F RID: 7071
	[NonSerialized]
	public bool hadInputBuffer = true;
}
