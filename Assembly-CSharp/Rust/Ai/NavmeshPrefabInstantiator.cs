using System;
using UnityEngine;

namespace Rust.Ai
{
	// Token: 0x02000AFA RID: 2810
	public class NavmeshPrefabInstantiator : MonoBehaviour
	{
		// Token: 0x06004397 RID: 17303 RVA: 0x0018825D File Offset: 0x0018645D
		private void Start()
		{
			if (this.NavmeshPrefab != null)
			{
				this.NavmeshPrefab.Instantiate(base.transform).SetActive(true);
				UnityEngine.Object.Destroy(this);
			}
		}

		// Token: 0x04003C12 RID: 15378
		public GameObjectRef NavmeshPrefab;
	}
}
