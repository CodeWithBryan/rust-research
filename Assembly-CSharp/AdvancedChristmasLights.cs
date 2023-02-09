using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200002C RID: 44
public class AdvancedChristmasLights : global::IOEntity
{
	// Token: 0x06000115 RID: 277 RVA: 0x0001EFE4 File Offset: 0x0001D1E4
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("AdvancedChristmasLights.OnRpcMessage", 0))
		{
			if (rpc == 1435781224U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetAnimationStyle ");
				}
				using (TimeWarning.New("SetAnimationStyle", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(1435781224U, "SetAnimationStyle", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SetAnimationStyle(rpcmessage);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in SetAnimationStyle");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000116 RID: 278 RVA: 0x0001F14C File Offset: 0x0001D34C
	public void ClearPoints()
	{
		this.points.Clear();
	}

	// Token: 0x06000117 RID: 279 RVA: 0x0001F159 File Offset: 0x0001D359
	public void FinishEditing()
	{
		this.finalized = true;
	}

	// Token: 0x06000118 RID: 280 RVA: 0x0001F162 File Offset: 0x0001D362
	public bool IsFinalized()
	{
		return this.finalized;
	}

	// Token: 0x06000119 RID: 281 RVA: 0x0001F16C File Offset: 0x0001D36C
	public void AddPoint(Vector3 newPoint, Vector3 newNormal)
	{
		if (base.isServer && this.points.Count == 0)
		{
			newPoint = this.wireEmission.position;
		}
		AdvancedChristmasLights.pointEntry item = default(AdvancedChristmasLights.pointEntry);
		item.point = newPoint;
		item.normal = newNormal;
		this.points.Add(item);
		if (base.isServer)
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x0600011A RID: 282 RVA: 0x0001F1CE File Offset: 0x0001D3CE
	public override int ConsumptionAmount()
	{
		return 5;
	}

	// Token: 0x0600011B RID: 283 RVA: 0x0001F1D1 File Offset: 0x0001D3D1
	protected override int GetPickupCount()
	{
		return Mathf.Max(this.lengthUsed, 1);
	}

	// Token: 0x0600011C RID: 284 RVA: 0x0001F1DF File Offset: 0x0001D3DF
	public void AddLengthUsed(int addLength)
	{
		this.lengthUsed += addLength;
	}

	// Token: 0x0600011D RID: 285 RVA: 0x0001F1EF File Offset: 0x0001D3EF
	public override void ServerInit()
	{
		base.ServerInit();
	}

	// Token: 0x0600011E RID: 286 RVA: 0x0001F1F8 File Offset: 0x0001D3F8
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.lightString = Facepunch.Pool.Get<LightString>();
		info.msg.lightString.points = Facepunch.Pool.GetList<LightString.StringPoint>();
		info.msg.lightString.lengthUsed = this.lengthUsed;
		info.msg.lightString.animationStyle = (int)this.animationStyle;
		foreach (AdvancedChristmasLights.pointEntry pointEntry in this.points)
		{
			LightString.StringPoint stringPoint = Facepunch.Pool.Get<LightString.StringPoint>();
			stringPoint.point = pointEntry.point;
			stringPoint.normal = pointEntry.normal;
			info.msg.lightString.points.Add(stringPoint);
		}
	}

	// Token: 0x0600011F RID: 287 RVA: 0x0001F2D0 File Offset: 0x0001D4D0
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.lightString != null)
		{
			this.ClearPoints();
			foreach (LightString.StringPoint stringPoint in info.msg.lightString.points)
			{
				this.AddPoint(stringPoint.point, stringPoint.normal);
			}
			this.lengthUsed = info.msg.lightString.lengthUsed;
			this.animationStyle = (AdvancedChristmasLights.AnimationType)info.msg.lightString.animationStyle;
			if (info.fromDisk)
			{
				this.FinishEditing();
			}
		}
	}

	// Token: 0x06000120 RID: 288 RVA: 0x0001F390 File Offset: 0x0001D590
	public bool IsStyle(AdvancedChristmasLights.AnimationType testType)
	{
		return testType == this.animationStyle;
	}

	// Token: 0x06000121 RID: 289 RVA: 0x00003A54 File Offset: 0x00001C54
	public bool CanPlayerManipulate(global::BasePlayer player)
	{
		return true;
	}

	// Token: 0x06000122 RID: 290 RVA: 0x0001F39C File Offset: 0x0001D59C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void SetAnimationStyle(global::BaseEntity.RPCMessage msg)
	{
		int num = msg.read.Int32();
		num = Mathf.Clamp(num, 1, 7);
		if (Global.developer > 0)
		{
			Debug.Log(string.Concat(new object[]
			{
				"Set animation style to :",
				num,
				" old was : ",
				(int)this.animationStyle
			}));
		}
		AdvancedChristmasLights.AnimationType animationType = (AdvancedChristmasLights.AnimationType)num;
		if (animationType == this.animationStyle)
		{
			return;
		}
		this.animationStyle = animationType;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x04000177 RID: 375
	public GameObjectRef bulbPrefab;

	// Token: 0x04000178 RID: 376
	public LineRenderer lineRenderer;

	// Token: 0x04000179 RID: 377
	public List<AdvancedChristmasLights.pointEntry> points = new List<AdvancedChristmasLights.pointEntry>();

	// Token: 0x0400017A RID: 378
	public List<BaseBulb> bulbs = new List<BaseBulb>();

	// Token: 0x0400017B RID: 379
	public float bulbSpacing = 0.25f;

	// Token: 0x0400017C RID: 380
	public float wireThickness = 0.02f;

	// Token: 0x0400017D RID: 381
	public Transform wireEmission;

	// Token: 0x0400017E RID: 382
	public AdvancedChristmasLights.AnimationType animationStyle = AdvancedChristmasLights.AnimationType.ON;

	// Token: 0x0400017F RID: 383
	public RendererLOD _lod;

	// Token: 0x04000180 RID: 384
	[Tooltip("This many units used will result in +1 power usage")]
	public float lengthToPowerRatio = 5f;

	// Token: 0x04000181 RID: 385
	private bool finalized;

	// Token: 0x04000182 RID: 386
	private int lengthUsed;

	// Token: 0x02000B0C RID: 2828
	public struct pointEntry
	{
		// Token: 0x04003C7C RID: 15484
		public Vector3 point;

		// Token: 0x04003C7D RID: 15485
		public Vector3 normal;
	}

	// Token: 0x02000B0D RID: 2829
	public enum AnimationType
	{
		// Token: 0x04003C7F RID: 15487
		ON = 1,
		// Token: 0x04003C80 RID: 15488
		FLASHING,
		// Token: 0x04003C81 RID: 15489
		CHASING,
		// Token: 0x04003C82 RID: 15490
		FADE,
		// Token: 0x04003C83 RID: 15491
		SLOWGLOW = 6
	}
}
