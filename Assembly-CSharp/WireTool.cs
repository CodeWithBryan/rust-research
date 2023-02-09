using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000E9 RID: 233
public class WireTool : HeldEntity
{
	// Token: 0x06001437 RID: 5175 RVA: 0x0009F468 File Offset: 0x0009D668
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("WireTool.OnRpcMessage", 0))
		{
			if (rpc == 678101026U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - AddLine ");
				}
				using (TimeWarning.New("AddLine", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test(678101026U, "AddLine", this, player))
						{
							return true;
						}
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(678101026U, "AddLine", this, player))
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
							this.AddLine(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in AddLine");
					}
				}
				return true;
			}
			if (rpc == 40328523U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - MakeConnection ");
				}
				using (TimeWarning.New("MakeConnection", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test(40328523U, "MakeConnection", this, player))
						{
							return true;
						}
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(40328523U, "MakeConnection", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg3 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.MakeConnection(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in MakeConnection");
					}
				}
				return true;
			}
			if (rpc == 121409151U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RequestChangeColor ");
				}
				using (TimeWarning.New("RequestChangeColor", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test(121409151U, "RequestChangeColor", this, player))
						{
							return true;
						}
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(121409151U, "RequestChangeColor", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg4 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RequestChangeColor(msg4);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in RequestChangeColor");
					}
				}
				return true;
			}
			if (rpc == 2469840259U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RequestClear ");
				}
				using (TimeWarning.New("RequestClear", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test(2469840259U, "RequestClear", this, player))
						{
							return true;
						}
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(2469840259U, "RequestClear", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg5 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RequestClear(msg5);
						}
					}
					catch (Exception exception4)
					{
						Debug.LogException(exception4);
						player.Kick("RPC Error in RequestClear");
					}
				}
				return true;
			}
			if (rpc == 2596458392U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetPlugged ");
				}
				using (TimeWarning.New("SetPlugged", 0))
				{
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage plugged = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SetPlugged(plugged);
						}
					}
					catch (Exception exception5)
					{
						Debug.LogException(exception5);
						player.Kick("RPC Error in SetPlugged");
					}
				}
				return true;
			}
			if (rpc == 210386477U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - TryClear ");
				}
				using (TimeWarning.New("TryClear", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test(210386477U, "TryClear", this, player))
						{
							return true;
						}
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(210386477U, "TryClear", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg6 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.TryClear(msg6);
						}
					}
					catch (Exception exception6)
					{
						Debug.LogException(exception6);
						player.Kick("RPC Error in TryClear");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x170001A9 RID: 425
	// (get) Token: 0x06001438 RID: 5176 RVA: 0x0009FCEC File Offset: 0x0009DEEC
	public bool CanChangeColours
	{
		get
		{
			return this.wireType == IOEntity.IOType.Electric || this.wireType == IOEntity.IOType.Fluidic || this.wireType == IOEntity.IOType.Industrial;
		}
	}

	// Token: 0x06001439 RID: 5177 RVA: 0x0009FD0A File Offset: 0x0009DF0A
	public void ClearPendingPlug()
	{
		this.pending.ent = null;
		this.pending.index = -1;
	}

	// Token: 0x0600143A RID: 5178 RVA: 0x0009FD24 File Offset: 0x0009DF24
	public bool HasPendingPlug()
	{
		return this.pending.ent != null && this.pending.index != -1;
	}

	// Token: 0x0600143B RID: 5179 RVA: 0x0009FD4C File Offset: 0x0009DF4C
	public bool PendingPlugIsInput()
	{
		return this.pending.ent != null && this.pending.index != -1 && this.pending.input;
	}

	// Token: 0x0600143C RID: 5180 RVA: 0x0009FD7C File Offset: 0x0009DF7C
	public bool PendingPlugIsType(IOEntity.IOType type)
	{
		return this.pending.ent != null && this.pending.index != -1 && ((this.pending.input && this.pending.ent.inputs[this.pending.index].type == type) || (!this.pending.input && this.pending.ent.outputs[this.pending.index].type == type));
	}

	// Token: 0x0600143D RID: 5181 RVA: 0x0009FE12 File Offset: 0x0009E012
	public bool PendingPlugIsOutput()
	{
		return this.pending.ent != null && this.pending.index != -1 && !this.pending.input;
	}

	// Token: 0x0600143E RID: 5182 RVA: 0x0009FE48 File Offset: 0x0009E048
	public Vector3 PendingPlugWorldPos()
	{
		if (this.pending.ent == null || this.pending.index == -1)
		{
			return Vector3.zero;
		}
		if (this.pending.input)
		{
			return this.pending.ent.transform.TransformPoint(this.pending.ent.inputs[this.pending.index].handlePosition);
		}
		return this.pending.ent.transform.TransformPoint(this.pending.ent.outputs[this.pending.index].handlePosition);
	}

	// Token: 0x0600143F RID: 5183 RVA: 0x0009FEF8 File Offset: 0x0009E0F8
	public static bool CanPlayerUseWires(BasePlayer player)
	{
		if (!player.CanBuild())
		{
			return false;
		}
		List<Collider> list = Facepunch.Pool.GetList<Collider>();
		GamePhysics.OverlapSphere(player.eyes.position, 0.1f, list, 536870912, QueryTriggerInteraction.Collide);
		bool result = list.All((Collider collider) => collider.gameObject.CompareTag("IgnoreWireCheck"));
		Facepunch.Pool.FreeList<Collider>(ref list);
		return result;
	}

	// Token: 0x06001440 RID: 5184 RVA: 0x0009FF5D File Offset: 0x0009E15D
	public static bool CanModifyEntity(BasePlayer player, BaseEntity ent)
	{
		return player.CanBuild(ent.transform.position, ent.transform.rotation, ent.bounds);
	}

	// Token: 0x06001441 RID: 5185 RVA: 0x0009FF81 File Offset: 0x0009E181
	public bool PendingPlugRoot()
	{
		return this.pending.ent != null && this.pending.ent.IsRootEntity();
	}

	// Token: 0x06001442 RID: 5186 RVA: 0x0009FFA8 File Offset: 0x0009E1A8
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	[BaseEntity.RPC_Server.FromOwner]
	public void TryClear(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		uint uid = msg.read.UInt32();
		BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(uid);
		IOEntity ioentity = (baseNetworkable == null) ? null : baseNetworkable.GetComponent<IOEntity>();
		if (ioentity == null)
		{
			return;
		}
		if (!WireTool.CanPlayerUseWires(player))
		{
			return;
		}
		if (!WireTool.CanModifyEntity(player, ioentity))
		{
			return;
		}
		ioentity.ClearConnections();
		ioentity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001443 RID: 5187 RVA: 0x000A0014 File Offset: 0x0009E214
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	[BaseEntity.RPC_Server.FromOwner]
	public void MakeConnection(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!WireTool.CanPlayerUseWires(player))
		{
			return;
		}
		uint uid = msg.read.UInt32();
		int num = msg.read.Int32();
		uint uid2 = msg.read.UInt32();
		int num2 = msg.read.Int32();
		WireTool.WireColour wireColour = this.IntToColour(msg.read.Int32());
		BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(uid);
		IOEntity ioentity = (baseNetworkable == null) ? null : baseNetworkable.GetComponent<IOEntity>();
		if (ioentity == null)
		{
			return;
		}
		BaseNetworkable baseNetworkable2 = BaseNetworkable.serverEntities.Find(uid2);
		IOEntity ioentity2 = (baseNetworkable2 == null) ? null : baseNetworkable2.GetComponent<IOEntity>();
		if (ioentity2 == null)
		{
			return;
		}
		if (Vector3.Distance(baseNetworkable2.transform.position, baseNetworkable.transform.position) > WireTool.maxWireLength)
		{
			return;
		}
		if (num >= ioentity.inputs.Length)
		{
			return;
		}
		if (num2 >= ioentity2.outputs.Length)
		{
			return;
		}
		if (ioentity.inputs[num].connectedTo.Get(true) != null)
		{
			return;
		}
		if (ioentity2.outputs[num2].connectedTo.Get(true) != null)
		{
			return;
		}
		if (ioentity.inputs[num].rootConnectionsOnly && !ioentity2.IsRootEntity())
		{
			return;
		}
		if (!WireTool.CanModifyEntity(player, ioentity))
		{
			return;
		}
		if (!WireTool.CanModifyEntity(player, ioentity2))
		{
			return;
		}
		ioentity.inputs[num].connectedTo.Set(ioentity2);
		ioentity.inputs[num].connectedToSlot = num2;
		ioentity.inputs[num].wireColour = wireColour;
		ioentity.inputs[num].connectedTo.Init();
		ioentity2.outputs[num2].connectedTo.Set(ioentity);
		ioentity2.outputs[num2].connectedToSlot = num;
		ioentity2.outputs[num2].wireColour = wireColour;
		ioentity2.outputs[num2].connectedTo.Init();
		ioentity2.outputs[num2].worldSpaceLineEndRotation = ioentity.transform.TransformDirection(ioentity.inputs[num].handleDirection);
		ioentity2.MarkDirtyForceUpdateOutputs();
		ioentity2.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		ioentity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		ioentity2.SendChangedToRoot(true);
		ioentity2.RefreshIndustrialPreventBuilding();
	}

	// Token: 0x06001444 RID: 5188 RVA: 0x000059DD File Offset: 0x00003BDD
	[BaseEntity.RPC_Server]
	public void SetPlugged(BaseEntity.RPCMessage msg)
	{
	}

	// Token: 0x06001445 RID: 5189 RVA: 0x000A0260 File Offset: 0x0009E460
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	[BaseEntity.RPC_Server.FromOwner]
	public void RequestClear(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!WireTool.CanPlayerUseWires(player))
		{
			return;
		}
		uint uid = msg.read.UInt32();
		int clearIndex = msg.read.Int32();
		bool isInput = msg.read.Bit();
		WireTool.AttemptClearSlot(BaseNetworkable.serverEntities.Find(uid), player, clearIndex, isInput);
	}

	// Token: 0x06001446 RID: 5190 RVA: 0x000A02B4 File Offset: 0x0009E4B4
	public static void AttemptClearSlot(BaseNetworkable clearEnt, BasePlayer ply, int clearIndex, bool isInput)
	{
		IOEntity ioentity = (clearEnt == null) ? null : clearEnt.GetComponent<IOEntity>();
		if (ioentity == null)
		{
			return;
		}
		if (ply != null && !WireTool.CanModifyEntity(ply, ioentity))
		{
			return;
		}
		if (clearIndex >= (isInput ? ioentity.inputs.Length : ioentity.outputs.Length))
		{
			return;
		}
		IOEntity.IOSlot ioslot = isInput ? ioentity.inputs[clearIndex] : ioentity.outputs[clearIndex];
		if (ioslot.connectedTo.Get(true) == null)
		{
			return;
		}
		IOEntity ioentity2 = ioslot.connectedTo.Get(true);
		IOEntity.IOSlot ioslot2 = isInput ? ioentity2.outputs[ioslot.connectedToSlot] : ioentity2.inputs[ioslot.connectedToSlot];
		if (isInput)
		{
			ioentity.UpdateFromInput(0, clearIndex);
		}
		else if (ioentity2)
		{
			ioentity2.UpdateFromInput(0, ioslot.connectedToSlot);
		}
		ioslot.Clear();
		ioslot2.Clear();
		ioentity.MarkDirtyForceUpdateOutputs();
		ioentity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		ioentity.RefreshIndustrialPreventBuilding();
		if (ioentity2 != null)
		{
			ioentity2.RefreshIndustrialPreventBuilding();
		}
		if (isInput && ioentity2 != null)
		{
			ioentity2.SendChangedToRoot(true);
		}
		else if (!isInput)
		{
			foreach (IOEntity.IOSlot ioslot3 in ioentity.inputs)
			{
				if (ioslot3.mainPowerSlot && ioslot3.connectedTo.Get(true))
				{
					ioslot3.connectedTo.Get(true).SendChangedToRoot(true);
				}
			}
		}
		ioentity2.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001447 RID: 5191 RVA: 0x000A0420 File Offset: 0x0009E620
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	[BaseEntity.RPC_Server.FromOwner]
	public void RequestChangeColor(BaseEntity.RPCMessage msg)
	{
		if (!WireTool.CanPlayerUseWires(msg.player))
		{
			return;
		}
		uint uid = msg.read.UInt32();
		int index = msg.read.Int32();
		bool flag = msg.read.Bit();
		WireTool.WireColour wireColour = this.IntToColour(msg.read.Int32());
		IOEntity ioentity = BaseNetworkable.serverEntities.Find(uid) as IOEntity;
		if (ioentity == null)
		{
			return;
		}
		IOEntity.IOSlot ioslot = flag ? ioentity.inputs.ElementAtOrDefault(index) : ioentity.outputs.ElementAtOrDefault(index);
		if (ioslot == null)
		{
			return;
		}
		IOEntity ioentity2 = ioslot.connectedTo.Get(true);
		if (ioentity2 == null)
		{
			return;
		}
		IOEntity.IOSlot ioslot2 = (flag ? ioentity2.outputs : ioentity2.inputs)[ioslot.connectedToSlot];
		ioslot.wireColour = wireColour;
		ioentity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		ioslot2.wireColour = wireColour;
		ioentity2.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001448 RID: 5192 RVA: 0x000A0508 File Offset: 0x0009E708
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	[BaseEntity.RPC_Server.FromOwner]
	public void AddLine(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!WireTool.CanPlayerUseWires(player))
		{
			return;
		}
		int num = msg.read.Int32();
		if (num > 18)
		{
			return;
		}
		List<Vector3> list = new List<Vector3>();
		for (int i = 0; i < num; i++)
		{
			Vector3 item = msg.read.Vector3();
			list.Add(item);
		}
		uint uid = msg.read.UInt32();
		int num2 = msg.read.Int32();
		uint uid2 = msg.read.UInt32();
		int num3 = msg.read.Int32();
		WireTool.WireColour wireColour = this.IntToColour(msg.read.Int32());
		BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(uid);
		IOEntity ioentity = (baseNetworkable == null) ? null : baseNetworkable.GetComponent<IOEntity>();
		if (ioentity == null)
		{
			return;
		}
		BaseNetworkable baseNetworkable2 = BaseNetworkable.serverEntities.Find(uid2);
		IOEntity ioentity2 = (baseNetworkable2 == null) ? null : baseNetworkable2.GetComponent<IOEntity>();
		if (ioentity2 == null)
		{
			return;
		}
		if (!this.ValidateLine(list, ioentity, ioentity2, player, num3))
		{
			return;
		}
		if (num2 >= ioentity.inputs.Length)
		{
			return;
		}
		if (num3 >= ioentity2.outputs.Length)
		{
			return;
		}
		if (ioentity.inputs[num2].connectedTo.Get(true) != null)
		{
			return;
		}
		if (ioentity2.outputs[num3].connectedTo.Get(true) != null)
		{
			return;
		}
		if (ioentity.inputs[num2].rootConnectionsOnly && !ioentity2.IsRootEntity())
		{
			return;
		}
		if (!WireTool.CanModifyEntity(player, ioentity2))
		{
			return;
		}
		if (!WireTool.CanModifyEntity(player, ioentity))
		{
			return;
		}
		ioentity2.outputs[num3].linePoints = list.ToArray();
		ioentity2.outputs[num3].wireColour = wireColour;
		ioentity2.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		ioentity2.RefreshIndustrialPreventBuilding();
	}

	// Token: 0x06001449 RID: 5193 RVA: 0x000A06D4 File Offset: 0x0009E8D4
	private WireTool.WireColour IntToColour(int i)
	{
		if (i < 0)
		{
			i = 0;
		}
		if (i >= 10)
		{
			i = 9;
		}
		WireTool.WireColour wireColour = (WireTool.WireColour)i;
		if (this.wireType == IOEntity.IOType.Fluidic && wireColour == WireTool.WireColour.Green)
		{
			wireColour = WireTool.WireColour.Default;
		}
		return wireColour;
	}

	// Token: 0x0600144A RID: 5194 RVA: 0x000A0704 File Offset: 0x0009E904
	private bool ValidateLine(List<Vector3> lineList, IOEntity inputEntity, IOEntity outputEntity, BasePlayer byPlayer, int outputIndex)
	{
		if (lineList.Count < 2)
		{
			return false;
		}
		if (inputEntity == null || outputEntity == null)
		{
			return false;
		}
		Vector3 a = lineList[0];
		float num = 0f;
		int count = lineList.Count;
		for (int i = 1; i < count; i++)
		{
			Vector3 vector = lineList[i];
			num += Vector3.Distance(a, vector);
			if (num > WireTool.maxWireLength)
			{
				return false;
			}
			a = vector;
		}
		Vector3 point = lineList[count - 1];
		Bounds bounds = outputEntity.bounds;
		bounds.Expand(0.5f);
		if (!bounds.Contains(point))
		{
			return false;
		}
		Vector3 position = outputEntity.transform.TransformPoint(lineList[0]);
		point = inputEntity.transform.InverseTransformPoint(position);
		Bounds bounds2 = inputEntity.bounds;
		bounds2.Expand(0.5f);
		if (!bounds2.Contains(point))
		{
			return false;
		}
		if (byPlayer == null)
		{
			return false;
		}
		Vector3 position2 = outputEntity.transform.TransformPoint(lineList[lineList.Count - 1]);
		return (byPlayer.Distance(position2) <= 5f || byPlayer.Distance(position) <= 5f) && (outputIndex < 0 || outputIndex >= outputEntity.outputs.Length || outputEntity.outputs[outputIndex].type != IOEntity.IOType.Industrial || this.VerifyLineOfSight(lineList, outputEntity.transform.localToWorldMatrix));
	}

	// Token: 0x0600144B RID: 5195 RVA: 0x000A0868 File Offset: 0x0009EA68
	private bool VerifyLineOfSight(List<Vector3> positions, Matrix4x4 localToWorldSpace)
	{
		Vector3 worldSpaceA = localToWorldSpace.MultiplyPoint3x4(positions[0]);
		for (int i = 1; i < positions.Count; i++)
		{
			Vector3 vector = localToWorldSpace.MultiplyPoint3x4(positions[i]);
			if (!this.VerifyLineOfSight(worldSpaceA, vector))
			{
				return false;
			}
			worldSpaceA = vector;
		}
		return true;
	}

	// Token: 0x0600144C RID: 5196 RVA: 0x000A08B4 File Offset: 0x0009EAB4
	private bool VerifyLineOfSight(Vector3 worldSpaceA, Vector3 worldSpaceB)
	{
		float maxDistance = Vector3.Distance(worldSpaceA, worldSpaceB);
		Vector3 normalized = (worldSpaceA - worldSpaceB).normalized;
		List<RaycastHit> list = Facepunch.Pool.GetList<RaycastHit>();
		GamePhysics.TraceAll(new Ray(worldSpaceB, normalized), 0.01f, list, maxDistance, 2162944, QueryTriggerInteraction.UseGlobal, null);
		bool result = true;
		foreach (RaycastHit hit in list)
		{
			BaseEntity entity = hit.GetEntity();
			if (entity != null && hit.IsOnLayer(Rust.Layer.Deployed))
			{
				if (entity is VendingMachine)
				{
					result = false;
					break;
				}
			}
			else if (!(entity != null) || !(entity is Door))
			{
				result = false;
				break;
			}
		}
		Facepunch.Pool.FreeList<RaycastHit>(ref list);
		return result;
	}

	// Token: 0x04000CC8 RID: 3272
	public Sprite InputSprite;

	// Token: 0x04000CC9 RID: 3273
	public Sprite OutputSprite;

	// Token: 0x04000CCA RID: 3274
	public Sprite ClearSprite;

	// Token: 0x04000CCB RID: 3275
	public static float maxWireLength = 30f;

	// Token: 0x04000CCC RID: 3276
	private const int maxLineNodes = 16;

	// Token: 0x04000CCD RID: 3277
	public GameObjectRef plugEffect;

	// Token: 0x04000CCE RID: 3278
	public SoundDefinition clearStartSoundDef;

	// Token: 0x04000CCF RID: 3279
	public SoundDefinition clearSoundDef;

	// Token: 0x04000CD0 RID: 3280
	public GameObjectRef ioLine;

	// Token: 0x04000CD1 RID: 3281
	public IOEntity.IOType wireType;

	// Token: 0x04000CD2 RID: 3282
	public float RadialMenuHoldTime = 0.25f;

	// Token: 0x04000CD3 RID: 3283
	private const float IndustrialWallOffset = 0.02f;

	// Token: 0x04000CD4 RID: 3284
	public static Translate.Phrase Default = new Translate.Phrase("wiretoolcolour.default", "Default");

	// Token: 0x04000CD5 RID: 3285
	public static Translate.Phrase DefaultDesc = new Translate.Phrase("wiretoolcolour.default.desc", "Default connection color");

	// Token: 0x04000CD6 RID: 3286
	public static Translate.Phrase Red = new Translate.Phrase("wiretoolcolour.red", "Red");

	// Token: 0x04000CD7 RID: 3287
	public static Translate.Phrase RedDesc = new Translate.Phrase("wiretoolcolour.red.desc", "Red connection color");

	// Token: 0x04000CD8 RID: 3288
	public static Translate.Phrase Green = new Translate.Phrase("wiretoolcolour.green", "Green");

	// Token: 0x04000CD9 RID: 3289
	public static Translate.Phrase GreenDesc = new Translate.Phrase("wiretoolcolour.green.desc", "Green connection color");

	// Token: 0x04000CDA RID: 3290
	public static Translate.Phrase Blue = new Translate.Phrase("wiretoolcolour.blue", "Blue");

	// Token: 0x04000CDB RID: 3291
	public static Translate.Phrase BlueDesc = new Translate.Phrase("wiretoolcolour.blue.desc", "Blue connection color");

	// Token: 0x04000CDC RID: 3292
	public static Translate.Phrase Yellow = new Translate.Phrase("wiretoolcolour.yellow", "Yellow");

	// Token: 0x04000CDD RID: 3293
	public static Translate.Phrase YellowDesc = new Translate.Phrase("wiretoolcolour.yellow.desc", "Yellow connection color");

	// Token: 0x04000CDE RID: 3294
	public static Translate.Phrase LightBlue = new Translate.Phrase("wiretoolcolour.light_blue", "Light Blue");

	// Token: 0x04000CDF RID: 3295
	public static Translate.Phrase LightBlueDesc = new Translate.Phrase("wiretoolcolour.light_blue.desc", "Light Blue connection color");

	// Token: 0x04000CE0 RID: 3296
	public static Translate.Phrase Orange = new Translate.Phrase("wiretoolcolour.orange", "Orange");

	// Token: 0x04000CE1 RID: 3297
	public static Translate.Phrase OrangeDesc = new Translate.Phrase("wiretoolcolour.orange.desc", "Orange connection color");

	// Token: 0x04000CE2 RID: 3298
	public static Translate.Phrase Purple = new Translate.Phrase("wiretoolcolour.purple", "Purple");

	// Token: 0x04000CE3 RID: 3299
	public static Translate.Phrase PurpleDesc = new Translate.Phrase("wiretoolcolour.purple.desc", "Purple connection color");

	// Token: 0x04000CE4 RID: 3300
	public static Translate.Phrase White = new Translate.Phrase("wiretoolcolour.white", "White");

	// Token: 0x04000CE5 RID: 3301
	public static Translate.Phrase WhiteDesc = new Translate.Phrase("wiretoolcolour.white.desc", "White connection color");

	// Token: 0x04000CE6 RID: 3302
	public static Translate.Phrase Pink = new Translate.Phrase("wiretoolcolour.pink", "Pink");

	// Token: 0x04000CE7 RID: 3303
	public static Translate.Phrase PinkDesc = new Translate.Phrase("wiretoolcolour.pink.desc", "Pink connection color");

	// Token: 0x04000CE8 RID: 3304
	public WireTool.PendingPlug_t pending;

	// Token: 0x04000CE9 RID: 3305
	private const float IndustrialThickness = 0.01f;

	// Token: 0x02000BCC RID: 3020
	public enum WireColour
	{
		// Token: 0x04003FA8 RID: 16296
		Default,
		// Token: 0x04003FA9 RID: 16297
		Red,
		// Token: 0x04003FAA RID: 16298
		Green,
		// Token: 0x04003FAB RID: 16299
		Blue,
		// Token: 0x04003FAC RID: 16300
		Yellow,
		// Token: 0x04003FAD RID: 16301
		Pink,
		// Token: 0x04003FAE RID: 16302
		Purple,
		// Token: 0x04003FAF RID: 16303
		Orange,
		// Token: 0x04003FB0 RID: 16304
		White,
		// Token: 0x04003FB1 RID: 16305
		LightBlue,
		// Token: 0x04003FB2 RID: 16306
		Count
	}

	// Token: 0x02000BCD RID: 3021
	public struct PendingPlug_t
	{
		// Token: 0x04003FB3 RID: 16307
		public IOEntity ent;

		// Token: 0x04003FB4 RID: 16308
		public bool input;

		// Token: 0x04003FB5 RID: 16309
		public int index;

		// Token: 0x04003FB6 RID: 16310
		public GameObject tempLine;
	}
}
