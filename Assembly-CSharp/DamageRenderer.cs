using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000453 RID: 1107
public class DamageRenderer : MonoBehaviour, IClientComponent
{
	// Token: 0x04001CEA RID: 7402
	[SerializeField]
	private List<Material> damageShowingMats;

	// Token: 0x04001CEB RID: 7403
	[SerializeField]
	private float maxDamageOpacity = 0.9f;

	// Token: 0x04001CEC RID: 7404
	[SerializeField]
	[HideInInspector]
	private List<DamageRenderer.DamageShowingRenderer> damageShowingRenderers;

	// Token: 0x04001CED RID: 7405
	[SerializeField]
	[HideInInspector]
	private List<GlassPane> damageShowingGlassRenderers;

	// Token: 0x02000CA6 RID: 3238
	[Serializable]
	private struct DamageShowingRenderer
	{
		// Token: 0x06004D2C RID: 19756 RVA: 0x00197130 File Offset: 0x00195330
		public DamageShowingRenderer(Renderer renderer, int[] indices)
		{
			this.renderer = renderer;
			this.indices = indices;
		}

		// Token: 0x04004361 RID: 17249
		public Renderer renderer;

		// Token: 0x04004362 RID: 17250
		public int[] indices;
	}
}
