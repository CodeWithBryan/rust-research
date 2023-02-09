using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x020000EE RID: 238
public class ZiplineArrivalPoint : global::BaseEntity
{
	// Token: 0x0600148E RID: 5262 RVA: 0x000A2B80 File Offset: 0x000A0D80
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.ZiplineArrival == null)
		{
			info.msg.ZiplineArrival = Pool.Get<ProtoBuf.ZiplineArrivalPoint>();
		}
		info.msg.ZiplineArrival.linePoints = Pool.GetList<VectorData>();
		foreach (Vector3 v in this.linePositions)
		{
			info.msg.ZiplineArrival.linePoints.Add(v);
		}
	}

	// Token: 0x0600148F RID: 5263 RVA: 0x000A2C00 File Offset: 0x000A0E00
	public void SetPositions(List<Vector3> points)
	{
		this.linePositions = new Vector3[points.Count];
		for (int i = 0; i < points.Count; i++)
		{
			this.linePositions[i] = points[i];
		}
	}

	// Token: 0x06001490 RID: 5264 RVA: 0x000A2C44 File Offset: 0x000A0E44
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ZiplineArrival != null && this.linePositions == null)
		{
			this.linePositions = new Vector3[info.msg.ZiplineArrival.linePoints.Count];
			for (int i = 0; i < info.msg.ZiplineArrival.linePoints.Count; i++)
			{
				this.linePositions[i] = info.msg.ZiplineArrival.linePoints[i];
			}
		}
	}

	// Token: 0x06001491 RID: 5265 RVA: 0x000A2CD4 File Offset: 0x000A0ED4
	public override void ResetState()
	{
		base.ResetState();
		this.linePositions = null;
	}

	// Token: 0x04000D0C RID: 3340
	public LineRenderer Line;

	// Token: 0x04000D0D RID: 3341
	private Vector3[] linePositions;
}
