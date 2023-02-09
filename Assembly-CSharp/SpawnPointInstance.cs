using System;
using Rust;
using UnityEngine;

// Token: 0x0200054D RID: 1357
public class SpawnPointInstance : MonoBehaviour
{
	// Token: 0x0600294C RID: 10572 RVA: 0x000FAC02 File Offset: 0x000F8E02
	public void Notify()
	{
		if (!this.parentSpawnPointUser.IsUnityNull<ISpawnPointUser>())
		{
			this.parentSpawnPointUser.ObjectSpawned(this);
		}
		if (this.parentSpawnPoint)
		{
			this.parentSpawnPoint.ObjectSpawned(this);
		}
	}

	// Token: 0x0600294D RID: 10573 RVA: 0x000FAC36 File Offset: 0x000F8E36
	public void Retire()
	{
		if (!this.parentSpawnPointUser.IsUnityNull<ISpawnPointUser>())
		{
			this.parentSpawnPointUser.ObjectRetired(this);
		}
		if (this.parentSpawnPoint)
		{
			this.parentSpawnPoint.ObjectRetired(this);
		}
	}

	// Token: 0x0600294E RID: 10574 RVA: 0x000FAC6A File Offset: 0x000F8E6A
	protected void OnDestroy()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		this.Retire();
	}

	// Token: 0x04002185 RID: 8581
	internal ISpawnPointUser parentSpawnPointUser;

	// Token: 0x04002186 RID: 8582
	internal BaseSpawnPoint parentSpawnPoint;
}
