using System;
using UnityEngine;

namespace Facepunch.UI
{
	// Token: 0x02000AAD RID: 2733
	public class ESPCanvas : MonoBehaviour
	{
		// Token: 0x04003A5C RID: 14940
		[Tooltip("Max amount of elements to show at once")]
		public int MaxElements;

		// Token: 0x04003A5D RID: 14941
		[Tooltip("Amount of times per second we should update the visible panels")]
		public float RefreshRate = 5f;

		// Token: 0x04003A5E RID: 14942
		[Tooltip("This object will be duplicated in place")]
		public ESPPlayerInfo Source;

		// Token: 0x04003A5F RID: 14943
		[Tooltip("Entities this far away won't be overlayed")]
		public float MaxDistance = 64f;

		// Token: 0x04003A60 RID: 14944
		[ClientVar(ClientAdmin = true)]
		public static float OverrideMaxDisplayDistance;

		// Token: 0x04003A61 RID: 14945
		[ClientVar(ClientAdmin = true)]
		public static bool DisableOcclusionChecks;

		// Token: 0x04003A62 RID: 14946
		[ClientVar(ClientAdmin = true)]
		public static bool ShowHealth;

		// Token: 0x04003A63 RID: 14947
		[ClientVar(ClientAdmin = true)]
		public static bool ColourCodeTeams;
	}
}
