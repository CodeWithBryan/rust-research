using System;
using UnityEngine;

// Token: 0x02000649 RID: 1609
public class DungeonVolume : MonoBehaviour
{
	// Token: 0x06002E05 RID: 11781 RVA: 0x001143D8 File Offset: 0x001125D8
	public OBB GetBounds(Vector3 position, Quaternion rotation)
	{
		position += rotation * (base.transform.localRotation * this.bounds.center + base.transform.localPosition);
		return new OBB(position, this.bounds.size, rotation * base.transform.localRotation);
	}

	// Token: 0x06002E06 RID: 11782 RVA: 0x00114440 File Offset: 0x00112640
	public OBB GetBounds(Vector3 position, Quaternion rotation, Vector3 extrude)
	{
		position += rotation * (base.transform.localRotation * this.bounds.center + base.transform.localPosition);
		return new OBB(position, this.bounds.size + extrude, rotation * base.transform.localRotation);
	}

	// Token: 0x040025AE RID: 9646
	public Bounds bounds = new Bounds(Vector3.zero, Vector3.one);
}
