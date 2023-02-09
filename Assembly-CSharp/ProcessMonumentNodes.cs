using System;
using System.Collections.Generic;

// Token: 0x020006B6 RID: 1718
public class ProcessMonumentNodes : ProceduralComponent
{
	// Token: 0x0600305C RID: 12380 RVA: 0x0012A0AC File Offset: 0x001282AC
	public override void Process(uint seed)
	{
		List<MonumentNode> monumentNodes = SingletonComponent<WorldSetup>.Instance.MonumentNodes;
		if (!World.Cached)
		{
			for (int i = 0; i < monumentNodes.Count; i++)
			{
				monumentNodes[i].Process(ref seed);
			}
		}
		monumentNodes.Clear();
	}
}
