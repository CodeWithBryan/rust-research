using System;
using UnityEngine;

// Token: 0x020008CC RID: 2252
public class LayerCullDistance : MonoBehaviour
{
	// Token: 0x06003634 RID: 13876 RVA: 0x001439E8 File Offset: 0x00141BE8
	protected void OnEnable()
	{
		Camera component = base.GetComponent<Camera>();
		float[] layerCullDistances = component.layerCullDistances;
		layerCullDistances[LayerMask.NameToLayer(this.Layer)] = this.Distance;
		component.layerCullDistances = layerCullDistances;
	}

	// Token: 0x0400312B RID: 12587
	public string Layer = "Default";

	// Token: 0x0400312C RID: 12588
	public float Distance = 1000f;
}
