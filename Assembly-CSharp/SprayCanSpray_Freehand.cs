using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000D0 RID: 208
public class SprayCanSpray_Freehand : SprayCanSpray
{
	// Token: 0x06001234 RID: 4660 RVA: 0x0009253C File Offset: 0x0009073C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SprayCanSpray_Freehand.OnRpcMessage", 0))
		{
			if (rpc == 2020094435U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_AddPointMidSpray ");
				}
				using (TimeWarning.New("Server_AddPointMidSpray", 0))
				{
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_AddPointMidSpray(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in Server_AddPointMidSpray");
					}
				}
				return true;
			}
			if (rpc == 117883393U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_FinishEditing ");
				}
				using (TimeWarning.New("Server_FinishEditing", 0))
				{
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_FinishEditing(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in Server_FinishEditing");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000185 RID: 389
	// (get) Token: 0x06001235 RID: 4661 RVA: 0x0009279C File Offset: 0x0009099C
	private bool AcceptingChanges
	{
		get
		{
			return this.editingPlayer.IsValid(true);
		}
	}

	// Token: 0x06001236 RID: 4662 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool ShouldNetworkOwnerInfo()
	{
		return true;
	}

	// Token: 0x06001237 RID: 4663 RVA: 0x000927AA File Offset: 0x000909AA
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (this.LinePoints == null || this.LinePoints.Count == 0)
		{
			base.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x06001238 RID: 4664 RVA: 0x000927D0 File Offset: 0x000909D0
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.sprayLine == null)
		{
			info.msg.sprayLine = Facepunch.Pool.Get<SprayLine>();
		}
		if (info.msg.sprayLine.linePoints == null)
		{
			info.msg.sprayLine.linePoints = Facepunch.Pool.GetList<LinePoint>();
		}
		bool flag = this.AcceptingChanges && info.forDisk;
		if (this.LinePoints != null && !flag)
		{
			this.CopyPoints(this.LinePoints, info.msg.sprayLine.linePoints);
		}
		info.msg.sprayLine.width = this.width;
		info.msg.sprayLine.colour = new Vector3(this.colour.r, this.colour.g, this.colour.b);
		if (!info.forDisk)
		{
			info.msg.sprayLine.editingPlayer = this.editingPlayer.uid;
		}
	}

	// Token: 0x06001239 RID: 4665 RVA: 0x000928D0 File Offset: 0x00090AD0
	public void SetColour(Color newColour)
	{
		this.colour = newColour;
	}

	// Token: 0x0600123A RID: 4666 RVA: 0x000928D9 File Offset: 0x00090AD9
	public void SetWidth(float lineWidth)
	{
		this.width = lineWidth;
	}

	// Token: 0x0600123B RID: 4667 RVA: 0x000928E4 File Offset: 0x00090AE4
	[global::BaseEntity.RPC_Server]
	private void Server_AddPointMidSpray(global::BaseEntity.RPCMessage msg)
	{
		if (!this.AcceptingChanges || this.editingPlayer.Get(true) != msg.player)
		{
			return;
		}
		if (this.LinePoints.Count + 1 > 60)
		{
			return;
		}
		Vector3 vector = msg.read.Vector3();
		Vector3 worldNormal = msg.read.Vector3();
		if (Vector3.Distance(vector, this.LinePoints[0].LocalPosition) >= 10f)
		{
			return;
		}
		this.LinePoints.Add(new AlignedLineDrawer.LinePoint
		{
			LocalPosition = vector,
			WorldNormal = worldNormal
		});
		this.UpdateGroundWatch();
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600123C RID: 4668 RVA: 0x0009298D File Offset: 0x00090B8D
	public void EnableChanges(global::BasePlayer byPlayer)
	{
		base.OwnerID = byPlayer.userID;
		this.editingPlayer.Set(byPlayer);
		base.Invoke(new Action(this.TimeoutEditing), 30f);
	}

	// Token: 0x0600123D RID: 4669 RVA: 0x000929BE File Offset: 0x00090BBE
	private void TimeoutEditing()
	{
		if (this.editingPlayer.IsSet)
		{
			this.editingPlayer.Set(null);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			base.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x0600123E RID: 4670 RVA: 0x000929E8 File Offset: 0x00090BE8
	[global::BaseEntity.RPC_Server]
	private void Server_FinishEditing(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer basePlayer = this.editingPlayer.Get(true);
		if (msg.player != basePlayer)
		{
			return;
		}
		bool allowNewSprayImmediately = msg.read.Int32() == 1;
		SprayCan sprayCan;
		if (basePlayer != null && basePlayer.GetHeldEntity() != null && (sprayCan = (basePlayer.GetHeldEntity() as SprayCan)) != null)
		{
			sprayCan.ClearPaintingLine(allowNewSprayImmediately);
		}
		this.editingPlayer.Set(null);
		SprayList sprayList = SprayList.Deserialize(msg.read);
		int count = sprayList.linePoints.Count;
		if (count > 70)
		{
			base.Kill(global::BaseNetworkable.DestroyMode.None);
			Facepunch.Pool.FreeList<LinePoint>(ref sprayList.linePoints);
			Facepunch.Pool.Free<SprayList>(ref sprayList);
			return;
		}
		if (this.LinePoints.Count <= 1)
		{
			base.Kill(global::BaseNetworkable.DestroyMode.None);
			Facepunch.Pool.FreeList<LinePoint>(ref sprayList.linePoints);
			Facepunch.Pool.Free<SprayList>(ref sprayList);
			return;
		}
		base.CancelInvoke(new Action(this.TimeoutEditing));
		this.LinePoints.Clear();
		for (int i = 0; i < count; i++)
		{
			if (sprayList.linePoints[i].localPosition.sqrMagnitude < 100f)
			{
				this.LinePoints.Add(new AlignedLineDrawer.LinePoint
				{
					LocalPosition = sprayList.linePoints[i].localPosition,
					WorldNormal = sprayList.linePoints[i].worldNormal
				});
			}
		}
		this.OnDeployed(null, basePlayer, null);
		this.UpdateGroundWatch();
		Facepunch.Pool.FreeList<LinePoint>(ref sprayList.linePoints);
		Facepunch.Pool.Free<SprayList>(ref sprayList);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600123F RID: 4671 RVA: 0x00092B78 File Offset: 0x00090D78
	public void AddInitialPoint(Vector3 atNormal)
	{
		this.LinePoints = new List<AlignedLineDrawer.LinePoint>
		{
			new AlignedLineDrawer.LinePoint
			{
				LocalPosition = Vector3.zero,
				WorldNormal = atNormal
			}
		};
	}

	// Token: 0x06001240 RID: 4672 RVA: 0x00092BB4 File Offset: 0x00090DB4
	private void UpdateGroundWatch()
	{
		if (base.isServer && this.LinePoints.Count > 1)
		{
			Vector3 groundPosition = Vector3.Lerp(this.LinePoints[0].LocalPosition, this.LinePoints[this.LinePoints.Count - 1].LocalPosition, 0.5f);
			if (this.groundWatch != null)
			{
				this.groundWatch.groundPosition = groundPosition;
			}
		}
	}

	// Token: 0x06001241 RID: 4673 RVA: 0x00092C2C File Offset: 0x00090E2C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.sprayLine != null)
		{
			if (info.msg.sprayLine.linePoints != null)
			{
				this.LinePoints.Clear();
				this.CopyPoints(info.msg.sprayLine.linePoints, this.LinePoints);
			}
			this.colour = new Color(info.msg.sprayLine.colour.x, info.msg.sprayLine.colour.y, info.msg.sprayLine.colour.z);
			this.width = info.msg.sprayLine.width;
			this.editingPlayer.uid = info.msg.sprayLine.editingPlayer;
			this.UpdateGroundWatch();
		}
	}

	// Token: 0x06001242 RID: 4674 RVA: 0x00092D0C File Offset: 0x00090F0C
	private void CopyPoints(List<AlignedLineDrawer.LinePoint> from, List<LinePoint> to)
	{
		to.Clear();
		foreach (AlignedLineDrawer.LinePoint linePoint in from)
		{
			LinePoint linePoint2 = Facepunch.Pool.Get<LinePoint>();
			linePoint2.localPosition = linePoint.LocalPosition;
			linePoint2.worldNormal = linePoint.WorldNormal;
			to.Add(linePoint2);
		}
	}

	// Token: 0x06001243 RID: 4675 RVA: 0x00092D80 File Offset: 0x00090F80
	private void CopyPoints(List<AlignedLineDrawer.LinePoint> from, List<Vector3> to)
	{
		to.Clear();
		foreach (AlignedLineDrawer.LinePoint linePoint in from)
		{
			to.Add(linePoint.LocalPosition);
			to.Add(linePoint.WorldNormal);
		}
	}

	// Token: 0x06001244 RID: 4676 RVA: 0x00092DE8 File Offset: 0x00090FE8
	private void CopyPoints(List<LinePoint> from, List<AlignedLineDrawer.LinePoint> to)
	{
		to.Clear();
		foreach (LinePoint linePoint in from)
		{
			to.Add(new AlignedLineDrawer.LinePoint
			{
				LocalPosition = linePoint.localPosition,
				WorldNormal = linePoint.worldNormal
			});
		}
	}

	// Token: 0x06001245 RID: 4677 RVA: 0x00092E60 File Offset: 0x00091060
	public static void CopyPoints(List<AlignedLineDrawer.LinePoint> from, List<AlignedLineDrawer.LinePoint> to)
	{
		to.Clear();
		foreach (AlignedLineDrawer.LinePoint item in from)
		{
			to.Add(item);
		}
	}

	// Token: 0x06001246 RID: 4678 RVA: 0x00092EB4 File Offset: 0x000910B4
	public override void ResetState()
	{
		base.ResetState();
		this.editingPlayer.Set(null);
	}

	// Token: 0x04000B6F RID: 2927
	public AlignedLineDrawer LineDrawer;

	// Token: 0x04000B70 RID: 2928
	public List<AlignedLineDrawer.LinePoint> LinePoints = new List<AlignedLineDrawer.LinePoint>();

	// Token: 0x04000B71 RID: 2929
	private Color colour = Color.white;

	// Token: 0x04000B72 RID: 2930
	private float width;

	// Token: 0x04000B73 RID: 2931
	private EntityRef<global::BasePlayer> editingPlayer;

	// Token: 0x04000B74 RID: 2932
	public GroundWatch groundWatch;

	// Token: 0x04000B75 RID: 2933
	public MeshCollider meshCollider;

	// Token: 0x04000B76 RID: 2934
	public const int MaxLinePointLength = 60;

	// Token: 0x04000B77 RID: 2935
	public const float SimplifyTolerance = 0.008f;
}
