using System;
using ConVar;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

// Token: 0x02000079 RID: 121
public class HackableLockedCrate : LootContainer
{
	// Token: 0x06000B84 RID: 2948 RVA: 0x00064BBC File Offset: 0x00062DBC
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("HackableLockedCrate.OnRpcMessage", 0))
		{
			if (rpc == 888500940U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Hack ");
				}
				using (TimeWarning.New("RPC_Hack", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(888500940U, "RPC_Hack", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_Hack(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_Hack");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000B85 RID: 2949 RVA: 0x00020A80 File Offset: 0x0001EC80
	public bool IsBeingHacked()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	// Token: 0x06000B86 RID: 2950 RVA: 0x00004C84 File Offset: 0x00002E84
	public bool IsFullyHacked()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved2);
	}

	// Token: 0x06000B87 RID: 2951 RVA: 0x00064D24 File Offset: 0x00062F24
	public override void DestroyShared()
	{
		if (base.isServer && this.mapMarkerInstance)
		{
			this.mapMarkerInstance.Kill(BaseNetworkable.DestroyMode.None);
		}
		base.DestroyShared();
	}

	// Token: 0x06000B88 RID: 2952 RVA: 0x00064D50 File Offset: 0x00062F50
	public void CreateMapMarker(float durationMinutes)
	{
		if (this.mapMarkerInstance)
		{
			this.mapMarkerInstance.Kill(BaseNetworkable.DestroyMode.None);
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.mapMarkerEntityPrefab.resourcePath, base.transform.position, Quaternion.identity, true);
		baseEntity.Spawn();
		baseEntity.SetParent(this, false, false);
		baseEntity.transform.localPosition = Vector3.zero;
		baseEntity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		this.mapMarkerInstance = baseEntity;
	}

	// Token: 0x06000B89 RID: 2953 RVA: 0x00064DCA File Offset: 0x00062FCA
	public void RefreshDecay()
	{
		base.CancelInvoke(new Action(this.DelayedDestroy));
		if (this.shouldDecay)
		{
			base.Invoke(new Action(this.DelayedDestroy), HackableLockedCrate.decaySeconds);
		}
	}

	// Token: 0x06000B8A RID: 2954 RVA: 0x000029D4 File Offset: 0x00000BD4
	public void DelayedDestroy()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06000B8B RID: 2955 RVA: 0x00064E00 File Offset: 0x00063000
	public override void OnAttacked(HitInfo info)
	{
		if (base.isServer)
		{
			if (StringPool.Get(info.HitBone) == "laptopcollision")
			{
				Effect.server.Run(this.shockEffect.resourcePath, info.HitPositionWorld, Vector3.up, null, false);
				this.hackSeconds -= 8f * (info.damageTypes.Total() / 50f);
				if (this.hackSeconds < 0f)
				{
					this.hackSeconds = 0f;
				}
			}
			this.RefreshDecay();
		}
		base.OnAttacked(info);
	}

	// Token: 0x06000B8C RID: 2956 RVA: 0x00064E92 File Offset: 0x00063092
	public void SetWasDropped()
	{
		this.wasDropped = true;
	}

	// Token: 0x06000B8D RID: 2957 RVA: 0x00064E9C File Offset: 0x0006309C
	public override void ServerInit()
	{
		base.ServerInit();
		if (base.isClient)
		{
			return;
		}
		if (!Rust.Application.isLoadingSave)
		{
			base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
			base.SetFlag(BaseEntity.Flags.Reserved2, false, false, true);
			if (this.wasDropped)
			{
				base.InvokeRepeating(new Action(this.LandCheck), 0f, 0.015f);
			}
		}
		this.RefreshDecay();
		this.isLootable = this.IsFullyHacked();
		this.CreateMapMarker(120f);
	}

	// Token: 0x06000B8E RID: 2958 RVA: 0x00064F1C File Offset: 0x0006311C
	public void LandCheck()
	{
		if (this.hasLanded)
		{
			return;
		}
		RaycastHit raycastHit;
		if (UnityEngine.Physics.Raycast(new Ray(base.transform.position + Vector3.up * 0.5f, Vector3.down), out raycastHit, 1f, 1218511105))
		{
			Effect.server.Run(this.landEffect.resourcePath, raycastHit.point, Vector3.up, null, false);
			this.hasLanded = true;
			base.CancelInvoke(new Action(this.LandCheck));
		}
	}

	// Token: 0x06000B8F RID: 2959 RVA: 0x00064FA5 File Offset: 0x000631A5
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
	}

	// Token: 0x06000B90 RID: 2960 RVA: 0x00064FBB File Offset: 0x000631BB
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_Hack(BaseEntity.RPCMessage msg)
	{
		if (this.IsBeingHacked())
		{
			return;
		}
		this.StartHacking();
	}

	// Token: 0x06000B91 RID: 2961 RVA: 0x00064FCC File Offset: 0x000631CC
	public void StartHacking()
	{
		base.BroadcastEntityMessage("HackingStarted", 20f, 256);
		base.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
		base.InvokeRepeating(new Action(this.HackProgress), 1f, 1f);
		base.ClientRPC<int, int>(null, "UpdateHackProgress", 0, (int)HackableLockedCrate.requiredHackSeconds);
		this.RefreshDecay();
	}

	// Token: 0x06000B92 RID: 2962 RVA: 0x00065034 File Offset: 0x00063234
	public void HackProgress()
	{
		this.hackSeconds += 1f;
		if (this.hackSeconds > HackableLockedCrate.requiredHackSeconds)
		{
			this.RefreshDecay();
			base.SetFlag(BaseEntity.Flags.Reserved2, true, false, true);
			this.isLootable = true;
			base.CancelInvoke(new Action(this.HackProgress));
		}
		base.ClientRPC<int, int>(null, "UpdateHackProgress", (int)this.hackSeconds, (int)HackableLockedCrate.requiredHackSeconds);
	}

	// Token: 0x0400075C RID: 1884
	public const BaseEntity.Flags Flag_Hacking = BaseEntity.Flags.Reserved1;

	// Token: 0x0400075D RID: 1885
	public const BaseEntity.Flags Flag_FullyHacked = BaseEntity.Flags.Reserved2;

	// Token: 0x0400075E RID: 1886
	public Text timerText;

	// Token: 0x0400075F RID: 1887
	[ServerVar(Help = "How many seconds for the crate to unlock")]
	public static float requiredHackSeconds = 900f;

	// Token: 0x04000760 RID: 1888
	[ServerVar(Help = "How many seconds until the crate is destroyed without any hack attempts")]
	public static float decaySeconds = 7200f;

	// Token: 0x04000761 RID: 1889
	public SoundPlayer hackProgressBeep;

	// Token: 0x04000762 RID: 1890
	private float hackSeconds;

	// Token: 0x04000763 RID: 1891
	public GameObjectRef shockEffect;

	// Token: 0x04000764 RID: 1892
	public GameObjectRef mapMarkerEntityPrefab;

	// Token: 0x04000765 RID: 1893
	public GameObjectRef landEffect;

	// Token: 0x04000766 RID: 1894
	public bool shouldDecay = true;

	// Token: 0x04000767 RID: 1895
	private BaseEntity mapMarkerInstance;

	// Token: 0x04000768 RID: 1896
	private bool hasLanded;

	// Token: 0x04000769 RID: 1897
	private bool wasDropped;
}
