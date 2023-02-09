using System;
using System.Collections.Generic;
using UnityEngine;

namespace Network.Visibility
{
	// Token: 0x0200001B RID: 27
	public interface Provider
	{
		// Token: 0x06000112 RID: 274
		void OnGroupAdded(Group group);

		// Token: 0x06000113 RID: 275
		bool IsInside(Group group, Vector3 vPos);

		// Token: 0x06000114 RID: 276
		Group GetGroup(Vector3 vPos);

		// Token: 0x06000115 RID: 277
		void GetVisibleFromFar(Group group, List<Group> groups);

		// Token: 0x06000116 RID: 278
		void GetVisibleFromNear(Group group, List<Group> groups);
	}
}
