using System;
using UnityEngine;

namespace Sonar
{
	// Token: 0x020009D6 RID: 2518
	public class SonarObject : MonoBehaviour, IClientComponent
	{
		// Token: 0x04003543 RID: 13635
		[SerializeField]
		private SonarObject.SType sonarType;

		// Token: 0x02000EA7 RID: 3751
		public enum SType
		{
			// Token: 0x04004B7C RID: 19324
			MoonPool,
			// Token: 0x04004B7D RID: 19325
			Sub
		}
	}
}
