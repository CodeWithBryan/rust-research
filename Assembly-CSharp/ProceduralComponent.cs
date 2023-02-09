using System;
using UnityEngine;

// Token: 0x02000687 RID: 1671
public abstract class ProceduralComponent : MonoBehaviour
{
	// Token: 0x170003B0 RID: 944
	// (get) Token: 0x06002FC8 RID: 12232 RVA: 0x00007074 File Offset: 0x00005274
	public virtual bool RunOnCache
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06002FC9 RID: 12233 RVA: 0x0011D52A File Offset: 0x0011B72A
	public bool ShouldRun()
	{
		return (!World.Cached || this.RunOnCache) && (this.Mode & ProceduralComponent.Realm.Server) != (ProceduralComponent.Realm)0;
	}

	// Token: 0x06002FCA RID: 12234
	public abstract void Process(uint seed);

	// Token: 0x04002688 RID: 9864
	[InspectorFlags]
	public ProceduralComponent.Realm Mode = (ProceduralComponent.Realm)(-1);

	// Token: 0x04002689 RID: 9865
	public string Description = "Procedural Component";

	// Token: 0x02000D7F RID: 3455
	public enum Realm
	{
		// Token: 0x040046C3 RID: 18115
		Client = 1,
		// Token: 0x040046C4 RID: 18116
		Server
	}
}
