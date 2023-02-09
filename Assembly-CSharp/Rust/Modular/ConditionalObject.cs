using System;
using UnityEngine;

namespace Rust.Modular
{
	// Token: 0x02000ADF RID: 2783
	[Serializable]
	public class ConditionalObject
	{
		// Token: 0x170005C7 RID: 1479
		// (get) Token: 0x060042E0 RID: 17120 RVA: 0x00185617 File Offset: 0x00183817
		// (set) Token: 0x060042E1 RID: 17121 RVA: 0x0018561F File Offset: 0x0018381F
		public bool? IsActive { get; private set; }

		// Token: 0x060042E2 RID: 17122 RVA: 0x00185628 File Offset: 0x00183828
		public ConditionalObject(GameObject conditionalGO, GameObject ownerGO, int socketsTaken)
		{
			this.gameObject = conditionalGO;
			this.ownerGameObject = ownerGO;
			this.socketSettings = new ConditionalSocketSettings[socketsTaken];
		}

		// Token: 0x060042E3 RID: 17123 RVA: 0x0018564C File Offset: 0x0018384C
		public void SetActive(bool active)
		{
			if (this.IsActive != null && active == this.IsActive.Value)
			{
				return;
			}
			this.gameObject.SetActive(active);
			this.IsActive = new bool?(active);
		}

		// Token: 0x060042E4 RID: 17124 RVA: 0x00185694 File Offset: 0x00183894
		public void RefreshActive()
		{
			if (this.IsActive == null)
			{
				return;
			}
			this.gameObject.SetActive(this.IsActive.Value);
		}

		// Token: 0x04003B7E RID: 15230
		public GameObject gameObject;

		// Token: 0x04003B7F RID: 15231
		public GameObject ownerGameObject;

		// Token: 0x04003B80 RID: 15232
		public ConditionalSocketSettings[] socketSettings;

		// Token: 0x04003B81 RID: 15233
		public bool restrictOnHealth;

		// Token: 0x04003B82 RID: 15234
		public float healthRestrictionMin;

		// Token: 0x04003B83 RID: 15235
		public float healthRestrictionMax;

		// Token: 0x04003B84 RID: 15236
		public bool restrictOnAdjacent;

		// Token: 0x04003B85 RID: 15237
		public ConditionalObject.AdjacentCondition adjacentRestriction;

		// Token: 0x04003B86 RID: 15238
		public ConditionalObject.AdjacentMatchType adjacentMatch;

		// Token: 0x04003B87 RID: 15239
		public bool restrictOnLockable;

		// Token: 0x04003B88 RID: 15240
		public bool lockableRestriction;

		// Token: 0x02000F29 RID: 3881
		public enum AdjacentCondition
		{
			// Token: 0x04004D78 RID: 19832
			SameInFront,
			// Token: 0x04004D79 RID: 19833
			SameBehind,
			// Token: 0x04004D7A RID: 19834
			DifferentInFront,
			// Token: 0x04004D7B RID: 19835
			DifferentBehind,
			// Token: 0x04004D7C RID: 19836
			BothDifferent,
			// Token: 0x04004D7D RID: 19837
			BothSame
		}

		// Token: 0x02000F2A RID: 3882
		public enum AdjacentMatchType
		{
			// Token: 0x04004D7F RID: 19839
			GroupOrExact,
			// Token: 0x04004D80 RID: 19840
			ExactOnly,
			// Token: 0x04004D81 RID: 19841
			GroupNotExact
		}
	}
}
