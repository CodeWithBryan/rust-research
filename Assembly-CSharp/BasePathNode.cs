using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000199 RID: 409
public class BasePathNode : MonoBehaviour
{
	// Token: 0x06001795 RID: 6037 RVA: 0x000059DD File Offset: 0x00003BDD
	public void OnDrawGizmosSelected()
	{
	}

	// Token: 0x04001086 RID: 4230
	public BasePath Path;

	// Token: 0x04001087 RID: 4231
	public List<BasePathNode> linked;

	// Token: 0x04001088 RID: 4232
	public float maxVelocityOnApproach = -1f;

	// Token: 0x04001089 RID: 4233
	public bool straightaway;
}
