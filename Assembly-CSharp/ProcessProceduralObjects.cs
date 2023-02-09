using System;
using System.Collections.Generic;

// Token: 0x020006B7 RID: 1719
public class ProcessProceduralObjects : ProceduralComponent
{
	// Token: 0x0600305E RID: 12382 RVA: 0x0012A0F0 File Offset: 0x001282F0
	public override void Process(uint seed)
	{
		List<ProceduralObject> proceduralObjects = SingletonComponent<WorldSetup>.Instance.ProceduralObjects;
		if (!World.Cached)
		{
			for (int i = 0; i < proceduralObjects.Count; i++)
			{
				ProceduralObject proceduralObject = proceduralObjects[i];
				if (proceduralObject)
				{
					proceduralObject.Process();
				}
			}
		}
		proceduralObjects.Clear();
	}

	// Token: 0x170003B9 RID: 953
	// (get) Token: 0x0600305F RID: 12383 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool RunOnCache
	{
		get
		{
			return true;
		}
	}
}
