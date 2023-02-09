using System;
using UnityEngine;

// Token: 0x020002F0 RID: 752
public class ObjectSpam : MonoBehaviour
{
	// Token: 0x06001D6A RID: 7530 RVA: 0x000C92E4 File Offset: 0x000C74E4
	private void Start()
	{
		for (int i = 0; i < this.amount; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.source);
			gameObject.transform.position = base.transform.position + Vector3Ex.Range(-this.radius, this.radius);
			gameObject.hideFlags = (HideFlags.HideInHierarchy | HideFlags.HideInInspector);
		}
	}

	// Token: 0x040016CB RID: 5835
	public GameObject source;

	// Token: 0x040016CC RID: 5836
	public int amount = 1000;

	// Token: 0x040016CD RID: 5837
	public float radius;
}
