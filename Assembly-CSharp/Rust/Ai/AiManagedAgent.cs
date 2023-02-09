using System;
using UnityEngine;

namespace Rust.Ai
{
	// Token: 0x02000AF7 RID: 2807
	[DefaultExecutionOrder(-102)]
	public class AiManagedAgent : FacepunchBehaviour, IServerComponent
	{
		// Token: 0x06004379 RID: 17273 RVA: 0x001877BB File Offset: 0x001859BB
		private void OnEnable()
		{
			this.isRegistered = false;
			if (SingletonComponent<AiManager>.Instance == null || !SingletonComponent<AiManager>.Instance.enabled || AiManager.nav_disable)
			{
				base.enabled = false;
				return;
			}
		}

		// Token: 0x0600437A RID: 17274 RVA: 0x001877EC File Offset: 0x001859EC
		private void DelayedRegistration()
		{
			if (!this.isRegistered)
			{
				this.isRegistered = true;
			}
		}

		// Token: 0x0600437B RID: 17275 RVA: 0x001877FD File Offset: 0x001859FD
		private void OnDisable()
		{
			if (Application.isQuitting)
			{
				return;
			}
			if (!(SingletonComponent<AiManager>.Instance == null) && SingletonComponent<AiManager>.Instance.enabled)
			{
				bool flag = this.isRegistered;
			}
		}

		// Token: 0x04003BF8 RID: 15352
		[Tooltip("TODO: Replace with actual agent type id on the NavMeshAgent when we upgrade to 5.6.1 or above.")]
		public int AgentTypeIndex;

		// Token: 0x04003BF9 RID: 15353
		[ReadOnly]
		[NonSerialized]
		public Vector2i NavmeshGridCoord;

		// Token: 0x04003BFA RID: 15354
		private bool isRegistered;
	}
}
