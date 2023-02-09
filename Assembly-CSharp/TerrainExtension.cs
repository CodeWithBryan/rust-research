using System;
using Facepunch.Extend;
using UnityEngine;

// Token: 0x0200066B RID: 1643
[RequireComponent(typeof(TerrainMeta))]
public abstract class TerrainExtension : MonoBehaviour
{
	// Token: 0x06002E57 RID: 11863 RVA: 0x00115DCD File Offset: 0x00113FCD
	public void Init(Terrain terrain, TerrainConfig config)
	{
		this.terrain = terrain;
		this.config = config;
	}

	// Token: 0x06002E58 RID: 11864 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void Setup()
	{
	}

	// Token: 0x06002E59 RID: 11865 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void PostSetup()
	{
	}

	// Token: 0x06002E5A RID: 11866 RVA: 0x00115DDD File Offset: 0x00113FDD
	public void LogSize(object obj, ulong size)
	{
		Debug.Log(obj.GetType() + " allocated: " + size.FormatBytes(false));
	}

	// Token: 0x04002620 RID: 9760
	[NonSerialized]
	public bool isInitialized;

	// Token: 0x04002621 RID: 9761
	internal Terrain terrain;

	// Token: 0x04002622 RID: 9762
	internal TerrainConfig config;
}
