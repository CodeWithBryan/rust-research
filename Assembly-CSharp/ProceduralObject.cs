using System;
using UnityEngine;

// Token: 0x020006B9 RID: 1721
public abstract class ProceduralObject : MonoBehaviour
{
	// Token: 0x06003063 RID: 12387 RVA: 0x0012A148 File Offset: 0x00128348
	protected void Awake()
	{
		if (!(SingletonComponent<WorldSetup>.Instance == null))
		{
			if (SingletonComponent<WorldSetup>.Instance.ProceduralObjects == null)
			{
				Debug.LogError("WorldSetup.Instance.ProceduralObjects is null.", this);
				return;
			}
			SingletonComponent<WorldSetup>.Instance.ProceduralObjects.Add(this);
		}
	}

	// Token: 0x06003064 RID: 12388
	public abstract void Process();
}
