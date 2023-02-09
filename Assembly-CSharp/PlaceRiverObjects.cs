using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

// Token: 0x020006B4 RID: 1716
public class PlaceRiverObjects : ProceduralComponent
{
	// Token: 0x06003058 RID: 12376 RVA: 0x00129D28 File Offset: 0x00127F28
	public override void Process(uint seed)
	{
		List<PathList> rivers = TerrainMeta.Path.Rivers;
		if (World.Networked)
		{
			using (List<PathList>.Enumerator enumerator = rivers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PathList pathList = enumerator.Current;
					World.Spawn(pathList.Name, "assets/bundled/prefabs/autospawn/");
				}
				return;
			}
		}
		foreach (PathList pathList2 in rivers)
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
			foreach (PathList.PathObject obj4 in this.Path)
			{
				pathList2.SpawnAlong(ref seed, obj4);
			}
			foreach (PathList.SideObject obj5 in this.Side)
			{
				pathList2.SpawnSide(ref seed, obj5);
			}
			foreach (PathList.BasicObject obj6 in this.End)
			{
				pathList2.SpawnEnd(ref seed, obj6);
			}
			pathList2.ResetTrims();
		}
	}

	// Token: 0x0400275D RID: 10077
	public PathList.BasicObject[] Start;

	// Token: 0x0400275E RID: 10078
	public PathList.BasicObject[] End;

	// Token: 0x0400275F RID: 10079
	[FormerlySerializedAs("RiversideObjects")]
	public PathList.SideObject[] Side;

	// Token: 0x04002760 RID: 10080
	[FormerlySerializedAs("RiverObjects")]
	public PathList.PathObject[] Path;
}
