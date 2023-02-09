using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

// Token: 0x020006B5 RID: 1717
public class PlaceRoadObjects : ProceduralComponent
{
	// Token: 0x0600305A RID: 12378 RVA: 0x00129EE4 File Offset: 0x001280E4
	public override void Process(uint seed)
	{
		List<PathList> roads = TerrainMeta.Path.Roads;
		if (World.Networked)
		{
			using (List<PathList>.Enumerator enumerator = roads.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PathList pathList = enumerator.Current;
					World.Spawn(pathList.Name, "assets/bundled/prefabs/autospawn/");
				}
				return;
			}
		}
		foreach (PathList pathList2 in roads)
		{
			if (pathList2.Hierarchy < 2)
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
	}

	// Token: 0x04002761 RID: 10081
	public PathList.BasicObject[] Start;

	// Token: 0x04002762 RID: 10082
	public PathList.BasicObject[] End;

	// Token: 0x04002763 RID: 10083
	[FormerlySerializedAs("RoadsideObjects")]
	public PathList.SideObject[] Side;

	// Token: 0x04002764 RID: 10084
	[FormerlySerializedAs("RoadObjects")]
	public PathList.PathObject[] Path;
}
