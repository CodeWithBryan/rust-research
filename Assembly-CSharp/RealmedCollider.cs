using System;
using UnityEngine;

// Token: 0x02000534 RID: 1332
public class RealmedCollider : BasePrefab
{
	// Token: 0x060028BB RID: 10427 RVA: 0x000F7DF8 File Offset: 0x000F5FF8
	public override void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.PreProcess(process, rootObj, name, serverside, clientside, bundling);
		if (this.ServerCollider != this.ClientCollider)
		{
			if (clientside)
			{
				if (this.ServerCollider)
				{
					process.RemoveComponent(this.ServerCollider);
					this.ServerCollider = null;
				}
			}
			else if (this.ClientCollider)
			{
				process.RemoveComponent(this.ClientCollider);
				this.ClientCollider = null;
			}
		}
		process.RemoveComponent(this);
	}

	// Token: 0x04002117 RID: 8471
	public Collider ServerCollider;

	// Token: 0x04002118 RID: 8472
	public Collider ClientCollider;
}
