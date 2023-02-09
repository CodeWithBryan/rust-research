using System;
using UnityEngine;

// Token: 0x02000505 RID: 1285
public class CopyLODValues : MonoBehaviour, IEditorComponent
{
	// Token: 0x0600283A RID: 10298 RVA: 0x000F5E9F File Offset: 0x000F409F
	public bool CanCopy()
	{
		return this.source != null && this.destination != null;
	}

	// Token: 0x0600283B RID: 10299 RVA: 0x000F5EC0 File Offset: 0x000F40C0
	public void Copy()
	{
		if (!this.CanCopy())
		{
			return;
		}
		LOD[] lods = this.source.GetLODs();
		if (this.scale)
		{
			float num = this.destination.size / this.source.size;
			for (int i = 0; i < lods.Length; i++)
			{
				LOD[] array = lods;
				int num2 = i;
				array[num2].screenRelativeTransitionHeight = array[num2].screenRelativeTransitionHeight * num;
			}
		}
		LOD[] lods2 = this.destination.GetLODs();
		int num3 = 0;
		while (num3 < lods2.Length && num3 < lods.Length)
		{
			int num4 = (num3 == lods2.Length - 1) ? (lods.Length - 1) : num3;
			lods2[num3].screenRelativeTransitionHeight = lods[num4].screenRelativeTransitionHeight;
			Debug.Log(string.Format("Set destination LOD {0} to {1}", num3, lods2[num3].screenRelativeTransitionHeight));
			num3++;
		}
		this.destination.SetLODs(lods2);
	}

	// Token: 0x04002096 RID: 8342
	[SerializeField]
	private LODGroup source;

	// Token: 0x04002097 RID: 8343
	[SerializeField]
	private LODGroup destination;

	// Token: 0x04002098 RID: 8344
	[Tooltip("Is false, exact values are copied. If true, values are scaled based on LODGroup size, so the changeover point will match.")]
	[SerializeField]
	private bool scale = true;
}
