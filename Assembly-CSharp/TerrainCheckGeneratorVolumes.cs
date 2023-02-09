using System;
using UnityEngine;

// Token: 0x02000665 RID: 1637
public class TerrainCheckGeneratorVolumes : MonoBehaviour, IEditorComponent
{
	// Token: 0x06002E40 RID: 11840 RVA: 0x00115705 File Offset: 0x00113905
	protected void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 1f);
		GizmosUtil.DrawWireCircleY(base.transform.position, this.PlacementRadius);
	}

	// Token: 0x04002600 RID: 9728
	public float PlacementRadius;
}
