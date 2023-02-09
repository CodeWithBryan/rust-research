using System;
using UnityEngine;

namespace JesseStiller.TerrainFormerExtension
{
	// Token: 0x020009A8 RID: 2472
	public class TerrainSetNeighbours : MonoBehaviour
	{
		// Token: 0x06003A9C RID: 15004 RVA: 0x001582B2 File Offset: 0x001564B2
		private void Awake()
		{
			base.GetComponent<Terrain>().SetNeighbors(this.leftTerrain, this.topTerrain, this.rightTerrain, this.bottomTerrain);
			UnityEngine.Object.Destroy(this);
		}

		// Token: 0x06003A9D RID: 15005 RVA: 0x001582DD File Offset: 0x001564DD
		public void SetNeighbours(Terrain leftTerrain, Terrain topTerrain, Terrain rightTerrain, Terrain bottomTerrain)
		{
			this.leftTerrain = leftTerrain;
			this.topTerrain = topTerrain;
			this.rightTerrain = rightTerrain;
			this.bottomTerrain = bottomTerrain;
		}

		// Token: 0x040034D4 RID: 13524
		[SerializeField]
		private Terrain leftTerrain;

		// Token: 0x040034D5 RID: 13525
		[SerializeField]
		private Terrain topTerrain;

		// Token: 0x040034D6 RID: 13526
		[SerializeField]
		private Terrain rightTerrain;

		// Token: 0x040034D7 RID: 13527
		[SerializeField]
		private Terrain bottomTerrain;
	}
}
