using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

// Token: 0x020006B3 RID: 1715
public class PlacePowerlineObjects : ProceduralComponent
{
	// Token: 0x06003056 RID: 12374 RVA: 0x00129B6C File Offset: 0x00127D6C
	public override void Process(uint seed)
	{
		List<PathList> powerlines = TerrainMeta.Path.Powerlines;
		if (World.Networked)
		{
			using (List<PathList>.Enumerator enumerator = powerlines.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PathList pathList = enumerator.Current;
					World.Spawn(pathList.Name, "assets/bundled/prefabs/autospawn/");
				}
				return;
			}
		}
		foreach (PathList pathList2 in powerlines)
		{
			foreach (PathList.BasicObject obj in this.Start)
			{
				pathList2.TrimStart(obj);
			}
			foreach (PathList.BasicObject obj2 in this.End)
			{
				pathList2.TrimEnd(obj2);
			}
			foreach (PathList.BasicObject obj3 in this.Start)
			{
				pathList2.SpawnStart(ref seed, obj3);
			}
			foreach (PathList.BasicObject obj4 in this.End)
			{
				pathList2.SpawnEnd(ref seed, obj4);
			}
			foreach (PathList.PathObject obj5 in this.Path)
			{
				pathList2.SpawnAlong(ref seed, obj5);
			}
			foreach (PathList.SideObject obj6 in this.Side)
			{
				pathList2.SpawnSide(ref seed, obj6);
			}
			pathList2.ResetTrims();
		}
	}

	// Token: 0x04002759 RID: 10073
	public PathList.BasicObject[] Start;

	// Token: 0x0400275A RID: 10074
	public PathList.BasicObject[] End;

	// Token: 0x0400275B RID: 10075
	public PathList.SideObject[] Side;

	// Token: 0x0400275C RID: 10076
	[FormerlySerializedAs("PowerlineObjects")]
	public PathList.PathObject[] Path;
}
