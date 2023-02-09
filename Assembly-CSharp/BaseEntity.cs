using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ConVar;
using Facepunch;
using Facepunch.Extend;
using Network;
using ProtoBuf;
using Rust;
using Rust.Ai;
using Rust.Workshop;
using Spatial;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

// Token: 0x02000035 RID: 53
public class BaseEntity : global::BaseNetworkable, IOnParentSpawning, IPrefabPreProcess
{
	// Token: 0x06000264 RID: 612 RVA: 0x00027394 File Offset: 0x00025594
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseEntity.OnRpcMessage", 0))
		{
			if (rpc == 1552640099U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - BroadcastSignalFromClient ");
				}
				using (TimeWarning.New("BroadcastSignalFromClient", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(1552640099U, "BroadcastSignalFromClient", this, player))
						{
							return true;
						}
					}
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
							this.BroadcastSignalFromClient(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in BroadcastSignalFromClient");
					}
				}
				return true;
			}
			if (rpc == 3645147041U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SV_RequestFile ");
				}
				using (TimeWarning.New("SV_RequestFile", 0))
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
							this.SV_RequestFile(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in SV_RequestFile");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x1700003B RID: 59
	// (get) Token: 0x06000265 RID: 613 RVA: 0x00027640 File Offset: 0x00025840
	public virtual float RealisticMass
	{
		get
		{
			return 100f;
		}
	}

	// Token: 0x06000266 RID: 614 RVA: 0x00027647 File Offset: 0x00025847
	public virtual void OnCollision(Collision collision, global::BaseEntity hitEntity)
	{
		throw new NotImplementedException();
	}

	// Token: 0x06000267 RID: 615 RVA: 0x0002764E File Offset: 0x0002584E
	protected void ReceiveCollisionMessages(bool b)
	{
		if (b)
		{
			base.gameObject.transform.GetOrAddComponent<EntityCollisionMessage>();
			return;
		}
		base.gameObject.transform.RemoveComponent<EntityCollisionMessage>();
	}

	// Token: 0x06000268 RID: 616 RVA: 0x00027678 File Offset: 0x00025878
	public virtual void DebugServer(int rep, float time)
	{
		this.DebugText(base.transform.position + Vector3.up * 1f, string.Format("{0}: {1}\n{2}", (this.net != null) ? this.net.ID : 0U, base.name, this.DebugText()), Color.white, time);
	}

	// Token: 0x06000269 RID: 617 RVA: 0x000276E1 File Offset: 0x000258E1
	public virtual string DebugText()
	{
		return "";
	}

	// Token: 0x0600026A RID: 618 RVA: 0x000276E8 File Offset: 0x000258E8
	public void OnDebugStart()
	{
		EntityDebug entityDebug = base.gameObject.GetComponent<EntityDebug>();
		if (entityDebug == null)
		{
			entityDebug = base.gameObject.AddComponent<EntityDebug>();
		}
		entityDebug.enabled = true;
	}

	// Token: 0x0600026B RID: 619 RVA: 0x0002771D File Offset: 0x0002591D
	protected void DebugText(Vector3 pos, string str, Color color, float time)
	{
		if (base.isServer)
		{
			ConsoleNetwork.BroadcastToAllClients("ddraw.text", new object[]
			{
				time,
				color,
				pos,
				str
			});
		}
	}

	// Token: 0x0600026C RID: 620 RVA: 0x00027757 File Offset: 0x00025957
	public bool HasFlag(global::BaseEntity.Flags f)
	{
		return (this.flags & f) == f;
	}

	// Token: 0x0600026D RID: 621 RVA: 0x00027764 File Offset: 0x00025964
	public bool ParentHasFlag(global::BaseEntity.Flags f)
	{
		global::BaseEntity parentEntity = base.GetParentEntity();
		return !(parentEntity == null) && parentEntity.HasFlag(f);
	}

	// Token: 0x0600026E RID: 622 RVA: 0x0002778C File Offset: 0x0002598C
	public void SetFlag(global::BaseEntity.Flags f, bool b, bool recursive = false, bool networkupdate = true)
	{
		global::BaseEntity.Flags old = this.flags;
		if (b)
		{
			if (this.HasFlag(f))
			{
				return;
			}
			this.flags |= f;
		}
		else
		{
			if (!this.HasFlag(f))
			{
				return;
			}
			this.flags &= ~f;
		}
		this.OnFlagsChanged(old, this.flags);
		if (networkupdate)
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
		else
		{
			base.InvalidateNetworkCache();
		}
		if (recursive && this.children != null)
		{
			for (int i = 0; i < this.children.Count; i++)
			{
				this.children[i].SetFlag(f, b, true, true);
			}
		}
	}

	// Token: 0x0600026F RID: 623 RVA: 0x0002782C File Offset: 0x00025A2C
	public bool IsOn()
	{
		return this.HasFlag(global::BaseEntity.Flags.On);
	}

	// Token: 0x06000270 RID: 624 RVA: 0x00027835 File Offset: 0x00025A35
	public bool IsOpen()
	{
		return this.HasFlag(global::BaseEntity.Flags.Open);
	}

	// Token: 0x06000271 RID: 625 RVA: 0x000028BF File Offset: 0x00000ABF
	public bool IsOnFire()
	{
		return this.HasFlag(global::BaseEntity.Flags.OnFire);
	}

	// Token: 0x06000272 RID: 626 RVA: 0x0002783E File Offset: 0x00025A3E
	public bool IsLocked()
	{
		return this.HasFlag(global::BaseEntity.Flags.Locked);
	}

	// Token: 0x06000273 RID: 627 RVA: 0x00027848 File Offset: 0x00025A48
	public override bool IsDebugging()
	{
		return this.HasFlag(global::BaseEntity.Flags.Debugging);
	}

	// Token: 0x06000274 RID: 628 RVA: 0x00027852 File Offset: 0x00025A52
	public bool IsDisabled()
	{
		return this.HasFlag(global::BaseEntity.Flags.Disabled) || this.ParentHasFlag(global::BaseEntity.Flags.Disabled);
	}

	// Token: 0x06000275 RID: 629 RVA: 0x00027868 File Offset: 0x00025A68
	public bool IsBroken()
	{
		return this.HasFlag(global::BaseEntity.Flags.Broken);
	}

	// Token: 0x06000276 RID: 630 RVA: 0x00027875 File Offset: 0x00025A75
	public bool IsBusy()
	{
		return this.HasFlag(global::BaseEntity.Flags.Busy);
	}

	// Token: 0x06000277 RID: 631 RVA: 0x00027882 File Offset: 0x00025A82
	public override string GetLogColor()
	{
		if (base.isServer)
		{
			return "cyan";
		}
		return "yellow";
	}

	// Token: 0x06000278 RID: 632 RVA: 0x00027897 File Offset: 0x00025A97
	public virtual void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		if (this.IsDebugging() && (old & global::BaseEntity.Flags.Debugging) != (next & global::BaseEntity.Flags.Debugging))
		{
			this.OnDebugStart();
		}
	}

	// Token: 0x06000279 RID: 633 RVA: 0x000278B4 File Offset: 0x00025AB4
	protected void SendNetworkUpdate_Flags()
	{
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (Rust.Application.isLoadingSave)
		{
			return;
		}
		if (base.IsDestroyed)
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (!this.isSpawned)
		{
			return;
		}
		using (TimeWarning.New("SendNetworkUpdate_Flags", 0))
		{
			base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "SendNetworkUpdate_Flags");
			List<Connection> subscribers = base.GetSubscribers();
			if (subscribers != null && subscribers.Count > 0)
			{
				NetWrite netWrite = Network.Net.sv.StartWrite();
				netWrite.PacketID(Message.Type.EntityFlags);
				netWrite.EntityID(this.net.ID);
				netWrite.Int32((int)this.flags);
				SendInfo info = new SendInfo(subscribers);
				netWrite.Send(info);
			}
			base.gameObject.SendOnSendNetworkUpdate(this);
		}
	}

	// Token: 0x0600027A RID: 634 RVA: 0x00027980 File Offset: 0x00025B80
	public bool IsOccupied(Socket_Base socket)
	{
		EntityLink entityLink = this.FindLink(socket);
		return entityLink != null && entityLink.IsOccupied();
	}

	// Token: 0x0600027B RID: 635 RVA: 0x000279A0 File Offset: 0x00025BA0
	public bool IsOccupied(string socketName)
	{
		EntityLink entityLink = this.FindLink(socketName);
		return entityLink != null && entityLink.IsOccupied();
	}

	// Token: 0x0600027C RID: 636 RVA: 0x000279C0 File Offset: 0x00025BC0
	public EntityLink FindLink(Socket_Base socket)
	{
		List<EntityLink> entityLinks = this.GetEntityLinks(true);
		for (int i = 0; i < entityLinks.Count; i++)
		{
			if (entityLinks[i].socket == socket)
			{
				return entityLinks[i];
			}
		}
		return null;
	}

	// Token: 0x0600027D RID: 637 RVA: 0x00027A04 File Offset: 0x00025C04
	public EntityLink FindLink(string socketName)
	{
		List<EntityLink> entityLinks = this.GetEntityLinks(true);
		for (int i = 0; i < entityLinks.Count; i++)
		{
			if (entityLinks[i].socket.socketName == socketName)
			{
				return entityLinks[i];
			}
		}
		return null;
	}

	// Token: 0x0600027E RID: 638 RVA: 0x00027A4C File Offset: 0x00025C4C
	public EntityLink FindLink(string[] socketNames)
	{
		List<EntityLink> entityLinks = this.GetEntityLinks(true);
		for (int i = 0; i < entityLinks.Count; i++)
		{
			for (int j = 0; j < socketNames.Length; j++)
			{
				if (entityLinks[i].socket.socketName == socketNames[j])
				{
					return entityLinks[i];
				}
			}
		}
		return null;
	}

	// Token: 0x0600027F RID: 639 RVA: 0x00027AA4 File Offset: 0x00025CA4
	public T FindLinkedEntity<T>() where T : global::BaseEntity
	{
		List<EntityLink> entityLinks = this.GetEntityLinks(true);
		for (int i = 0; i < entityLinks.Count; i++)
		{
			EntityLink entityLink = entityLinks[i];
			for (int j = 0; j < entityLink.connections.Count; j++)
			{
				EntityLink entityLink2 = entityLink.connections[j];
				if (entityLink2.owner is T)
				{
					return entityLink2.owner as T;
				}
			}
		}
		return default(T);
	}

	// Token: 0x06000280 RID: 640 RVA: 0x00027B20 File Offset: 0x00025D20
	public void EntityLinkMessage<T>(Action<T> action) where T : global::BaseEntity
	{
		List<EntityLink> entityLinks = this.GetEntityLinks(true);
		for (int i = 0; i < entityLinks.Count; i++)
		{
			EntityLink entityLink = entityLinks[i];
			for (int j = 0; j < entityLink.connections.Count; j++)
			{
				EntityLink entityLink2 = entityLink.connections[j];
				if (entityLink2.owner is T)
				{
					action(entityLink2.owner as T);
				}
			}
		}
	}

	// Token: 0x06000281 RID: 641 RVA: 0x00027B98 File Offset: 0x00025D98
	public void EntityLinkBroadcast<T, S>(Action<T> action, Func<S, bool> canTraverseSocket) where T : global::BaseEntity where S : Socket_Base
	{
		global::BaseEntity.globalBroadcastProtocol += 1U;
		global::BaseEntity.globalBroadcastQueue.Clear();
		this.broadcastProtocol = global::BaseEntity.globalBroadcastProtocol;
		global::BaseEntity.globalBroadcastQueue.Enqueue(this);
		if (this is T)
		{
			action(this as T);
		}
		while (global::BaseEntity.globalBroadcastQueue.Count > 0)
		{
			List<EntityLink> entityLinks = global::BaseEntity.globalBroadcastQueue.Dequeue().GetEntityLinks(true);
			for (int i = 0; i < entityLinks.Count; i++)
			{
				EntityLink entityLink = entityLinks[i];
				if (entityLink.socket is S && canTraverseSocket(entityLink.socket as S))
				{
					for (int j = 0; j < entityLink.connections.Count; j++)
					{
						global::BaseEntity owner = entityLink.connections[j].owner;
						if (owner.broadcastProtocol != global::BaseEntity.globalBroadcastProtocol)
						{
							owner.broadcastProtocol = global::BaseEntity.globalBroadcastProtocol;
							global::BaseEntity.globalBroadcastQueue.Enqueue(owner);
							if (owner is T)
							{
								action(owner as T);
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06000282 RID: 642 RVA: 0x00027CC4 File Offset: 0x00025EC4
	public void EntityLinkBroadcast<T>(Action<T> action) where T : global::BaseEntity
	{
		global::BaseEntity.globalBroadcastProtocol += 1U;
		global::BaseEntity.globalBroadcastQueue.Clear();
		this.broadcastProtocol = global::BaseEntity.globalBroadcastProtocol;
		global::BaseEntity.globalBroadcastQueue.Enqueue(this);
		if (this is T)
		{
			action(this as T);
		}
		while (global::BaseEntity.globalBroadcastQueue.Count > 0)
		{
			List<EntityLink> entityLinks = global::BaseEntity.globalBroadcastQueue.Dequeue().GetEntityLinks(true);
			for (int i = 0; i < entityLinks.Count; i++)
			{
				EntityLink entityLink = entityLinks[i];
				for (int j = 0; j < entityLink.connections.Count; j++)
				{
					global::BaseEntity owner = entityLink.connections[j].owner;
					if (owner.broadcastProtocol != global::BaseEntity.globalBroadcastProtocol)
					{
						owner.broadcastProtocol = global::BaseEntity.globalBroadcastProtocol;
						global::BaseEntity.globalBroadcastQueue.Enqueue(owner);
						if (owner is T)
						{
							action(owner as T);
						}
					}
				}
			}
		}
	}

	// Token: 0x06000283 RID: 643 RVA: 0x00027DC4 File Offset: 0x00025FC4
	public void EntityLinkBroadcast()
	{
		global::BaseEntity.globalBroadcastProtocol += 1U;
		global::BaseEntity.globalBroadcastQueue.Clear();
		this.broadcastProtocol = global::BaseEntity.globalBroadcastProtocol;
		global::BaseEntity.globalBroadcastQueue.Enqueue(this);
		while (global::BaseEntity.globalBroadcastQueue.Count > 0)
		{
			List<EntityLink> entityLinks = global::BaseEntity.globalBroadcastQueue.Dequeue().GetEntityLinks(true);
			for (int i = 0; i < entityLinks.Count; i++)
			{
				EntityLink entityLink = entityLinks[i];
				for (int j = 0; j < entityLink.connections.Count; j++)
				{
					global::BaseEntity owner = entityLink.connections[j].owner;
					if (owner.broadcastProtocol != global::BaseEntity.globalBroadcastProtocol)
					{
						owner.broadcastProtocol = global::BaseEntity.globalBroadcastProtocol;
						global::BaseEntity.globalBroadcastQueue.Enqueue(owner);
					}
				}
			}
		}
	}

	// Token: 0x06000284 RID: 644 RVA: 0x00027E88 File Offset: 0x00026088
	public bool ReceivedEntityLinkBroadcast()
	{
		return this.broadcastProtocol == global::BaseEntity.globalBroadcastProtocol;
	}

	// Token: 0x06000285 RID: 645 RVA: 0x00027E97 File Offset: 0x00026097
	public List<EntityLink> GetEntityLinks(bool linkToNeighbours = true)
	{
		if (Rust.Application.isLoadingSave)
		{
			return this.links;
		}
		if (!this.linkedToNeighbours && linkToNeighbours)
		{
			this.LinkToNeighbours();
		}
		return this.links;
	}

	// Token: 0x06000286 RID: 646 RVA: 0x00027EC0 File Offset: 0x000260C0
	private void LinkToEntity(global::BaseEntity other)
	{
		if (this == other)
		{
			return;
		}
		if (this.links.Count == 0 || other.links.Count == 0)
		{
			return;
		}
		using (TimeWarning.New("LinkToEntity", 0))
		{
			for (int i = 0; i < this.links.Count; i++)
			{
				EntityLink entityLink = this.links[i];
				for (int j = 0; j < other.links.Count; j++)
				{
					EntityLink entityLink2 = other.links[j];
					if (entityLink.CanConnect(entityLink2))
					{
						if (!entityLink.Contains(entityLink2))
						{
							entityLink.Add(entityLink2);
						}
						if (!entityLink2.Contains(entityLink))
						{
							entityLink2.Add(entityLink);
						}
					}
				}
			}
		}
	}

	// Token: 0x06000287 RID: 647 RVA: 0x00027F90 File Offset: 0x00026190
	private void LinkToNeighbours()
	{
		if (this.links.Count == 0)
		{
			return;
		}
		this.linkedToNeighbours = true;
		using (TimeWarning.New("LinkToNeighbours", 0))
		{
			List<global::BaseEntity> list = Facepunch.Pool.GetList<global::BaseEntity>();
			OBB obb = this.WorldSpaceBounds();
			global::Vis.Entities<global::BaseEntity>(obb.position, obb.extents.magnitude + 1f, list, -1, QueryTriggerInteraction.Collide);
			for (int i = 0; i < list.Count; i++)
			{
				global::BaseEntity baseEntity = list[i];
				if (baseEntity.isServer == base.isServer)
				{
					this.LinkToEntity(baseEntity);
				}
			}
			Facepunch.Pool.FreeList<global::BaseEntity>(ref list);
		}
	}

	// Token: 0x06000288 RID: 648 RVA: 0x00028040 File Offset: 0x00026240
	private void InitEntityLinks()
	{
		using (TimeWarning.New("InitEntityLinks", 0))
		{
			if (base.isServer)
			{
				this.links.AddLinks(this, PrefabAttribute.server.FindAll<Socket_Base>(this.prefabID));
			}
		}
	}

	// Token: 0x06000289 RID: 649 RVA: 0x0002809C File Offset: 0x0002629C
	private void FreeEntityLinks()
	{
		using (TimeWarning.New("FreeEntityLinks", 0))
		{
			this.links.FreeLinks();
			this.linkedToNeighbours = false;
		}
	}

	// Token: 0x0600028A RID: 650 RVA: 0x000280E4 File Offset: 0x000262E4
	public void RefreshEntityLinks()
	{
		using (TimeWarning.New("RefreshEntityLinks", 0))
		{
			this.links.ClearLinks();
			this.LinkToNeighbours();
		}
	}

	// Token: 0x0600028B RID: 651 RVA: 0x0002812C File Offset: 0x0002632C
	[global::BaseEntity.RPC_Server]
	public void SV_RequestFile(global::BaseEntity.RPCMessage msg)
	{
		uint num = msg.read.UInt32();
		FileStorage.Type type = (FileStorage.Type)msg.read.UInt8();
		string funcName = StringPool.Get(msg.read.UInt32());
		uint num2 = (msg.read.Unread > 0) ? msg.read.UInt32() : 0U;
		bool flag = msg.read.Unread > 0 && msg.read.Bit();
		byte[] array = FileStorage.server.Get(num, type, this.net.ID, num2);
		if (array == null)
		{
			if (!flag)
			{
				return;
			}
			array = Array.Empty<byte>();
		}
		SendInfo sendInfo = new SendInfo(msg.connection)
		{
			channel = 2,
			method = SendMethod.Reliable
		};
		this.ClientRPCEx<uint, uint, byte[], uint, byte>(sendInfo, null, funcName, num, (uint)array.Length, array, num2, (byte)type);
	}

	// Token: 0x0600028C RID: 652 RVA: 0x000281FC File Offset: 0x000263FC
	public void SetParent(global::BaseEntity entity, bool worldPositionStays = false, bool sendImmediate = false)
	{
		this.SetParent(entity, 0U, worldPositionStays, sendImmediate);
	}

	// Token: 0x0600028D RID: 653 RVA: 0x00028208 File Offset: 0x00026408
	public void SetParent(global::BaseEntity entity, string strBone, bool worldPositionStays = false, bool sendImmediate = false)
	{
		this.SetParent(entity, string.IsNullOrEmpty(strBone) ? 0U : StringPool.Get(strBone), worldPositionStays, sendImmediate);
	}

	// Token: 0x0600028E RID: 654 RVA: 0x00028228 File Offset: 0x00026428
	public bool HasChild(global::BaseEntity c)
	{
		if (c == this)
		{
			return true;
		}
		global::BaseEntity parentEntity = c.GetParentEntity();
		return parentEntity != null && this.HasChild(parentEntity);
	}

	// Token: 0x0600028F RID: 655 RVA: 0x0002825C File Offset: 0x0002645C
	public void SetParent(global::BaseEntity entity, uint boneID, bool worldPositionStays = false, bool sendImmediate = false)
	{
		if (entity != null)
		{
			if (entity == this)
			{
				Debug.LogError("Trying to parent to self " + this, base.gameObject);
				return;
			}
			if (this.HasChild(entity))
			{
				Debug.LogError("Trying to parent to child " + this, base.gameObject);
				return;
			}
		}
		base.LogEntry(BaseMonoBehaviour.LogEntryType.Hierarchy, 2, "SetParent {0} {1}", entity, boneID);
		global::BaseEntity parentEntity = base.GetParentEntity();
		if (parentEntity)
		{
			parentEntity.RemoveChild(this);
		}
		if (base.limitNetworking && parentEntity != null && parentEntity != entity)
		{
			global::BasePlayer basePlayer = parentEntity as global::BasePlayer;
			if (basePlayer.IsValid())
			{
				this.DestroyOnClient(basePlayer.net.connection);
			}
		}
		if (entity == null)
		{
			this.OnParentChanging(parentEntity, null);
			this.parentEntity.Set(null);
			base.transform.SetParent(null, worldPositionStays);
			this.parentBone = 0U;
			this.UpdateNetworkGroup();
			if (sendImmediate)
			{
				base.SendNetworkUpdateImmediate(false);
				this.SendChildrenNetworkUpdateImmediate();
				return;
			}
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			this.SendChildrenNetworkUpdate();
			return;
		}
		else
		{
			Debug.Assert(entity.isServer, "SetParent - child should be a SERVER entity");
			Debug.Assert(entity.net != null, "Setting parent to entity that hasn't spawned yet! (net is null)");
			Debug.Assert(entity.net.ID > 0U, "Setting parent to entity that hasn't spawned yet! (id = 0)");
			entity.AddChild(this);
			this.OnParentChanging(parentEntity, entity);
			this.parentEntity.Set(entity);
			if (boneID != 0U && boneID != StringPool.closest)
			{
				base.transform.SetParent(entity.FindBone(StringPool.Get(boneID)), worldPositionStays);
			}
			else
			{
				base.transform.SetParent(entity.transform, worldPositionStays);
			}
			this.parentBone = boneID;
			this.UpdateNetworkGroup();
			if (sendImmediate)
			{
				base.SendNetworkUpdateImmediate(false);
				this.SendChildrenNetworkUpdateImmediate();
				return;
			}
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			this.SendChildrenNetworkUpdate();
			return;
		}
	}

	// Token: 0x06000290 RID: 656 RVA: 0x00028428 File Offset: 0x00026628
	private void DestroyOnClient(Connection connection)
	{
		if (this.children != null)
		{
			foreach (global::BaseEntity baseEntity in this.children)
			{
				baseEntity.DestroyOnClient(connection);
			}
		}
		if (Network.Net.sv.IsConnected())
		{
			NetWrite netWrite = Network.Net.sv.StartWrite();
			netWrite.PacketID(Message.Type.EntityDestroy);
			netWrite.EntityID(this.net.ID);
			netWrite.UInt8(0);
			netWrite.Send(new SendInfo(connection));
			base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "EntityDestroy");
		}
	}

	// Token: 0x06000291 RID: 657 RVA: 0x000284D0 File Offset: 0x000266D0
	private void SendChildrenNetworkUpdate()
	{
		if (this.children == null)
		{
			return;
		}
		foreach (global::BaseEntity baseEntity in this.children)
		{
			baseEntity.UpdateNetworkGroup();
			baseEntity.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06000292 RID: 658 RVA: 0x00028530 File Offset: 0x00026730
	private void SendChildrenNetworkUpdateImmediate()
	{
		if (this.children == null)
		{
			return;
		}
		foreach (global::BaseEntity baseEntity in this.children)
		{
			baseEntity.UpdateNetworkGroup();
			baseEntity.SendNetworkUpdateImmediate(false);
		}
	}

	// Token: 0x06000293 RID: 659 RVA: 0x00028590 File Offset: 0x00026790
	public virtual void SwitchParent(global::BaseEntity ent)
	{
		this.Log("SwitchParent Missed " + ent);
	}

	// Token: 0x06000294 RID: 660 RVA: 0x000285A4 File Offset: 0x000267A4
	public virtual void OnParentChanging(global::BaseEntity oldParent, global::BaseEntity newParent)
	{
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component)
		{
			if (oldParent != null && oldParent.GetComponent<Rigidbody>() == null)
			{
				component.velocity += oldParent.GetWorldVelocity();
			}
			if (newParent != null && newParent.GetComponent<Rigidbody>() == null)
			{
				component.velocity -= newParent.GetWorldVelocity();
			}
		}
	}

	// Token: 0x06000295 RID: 661 RVA: 0x0002861C File Offset: 0x0002681C
	public virtual BuildingPrivlidge GetBuildingPrivilege()
	{
		return this.GetBuildingPrivilege(this.WorldSpaceBounds());
	}

	// Token: 0x06000296 RID: 662 RVA: 0x0002862C File Offset: 0x0002682C
	public BuildingPrivlidge GetBuildingPrivilege(OBB obb)
	{
		global::BuildingBlock other = null;
		BuildingPrivlidge result = null;
		List<global::BuildingBlock> list = Facepunch.Pool.GetList<global::BuildingBlock>();
		global::Vis.Entities<global::BuildingBlock>(obb.position, 16f + obb.extents.magnitude, list, 2097152, QueryTriggerInteraction.Collide);
		for (int i = 0; i < list.Count; i++)
		{
			global::BuildingBlock buildingBlock = list[i];
			if (buildingBlock.isServer == base.isServer && buildingBlock.IsOlderThan(other) && obb.Distance(buildingBlock.WorldSpaceBounds()) <= 16f)
			{
				BuildingManager.Building building = buildingBlock.GetBuilding();
				if (building != null)
				{
					BuildingPrivlidge dominatingBuildingPrivilege = building.GetDominatingBuildingPrivilege();
					if (!(dominatingBuildingPrivilege == null))
					{
						other = buildingBlock;
						result = dominatingBuildingPrivilege;
					}
				}
			}
		}
		Facepunch.Pool.FreeList<global::BuildingBlock>(ref list);
		return result;
	}

	// Token: 0x06000297 RID: 663 RVA: 0x000286E0 File Offset: 0x000268E0
	public void SV_RPCMessage(uint nameID, Message message)
	{
		Assert.IsTrue(base.isServer, "Should be server!");
		global::BasePlayer basePlayer = message.Player();
		if (!basePlayer.IsValid())
		{
			if (ConVar.Global.developer > 0)
			{
				Debug.Log("SV_RPCMessage: From invalid player " + basePlayer);
			}
			return;
		}
		if (basePlayer.isStalled)
		{
			if (ConVar.Global.developer > 0)
			{
				Debug.Log("SV_RPCMessage: player is stalled " + basePlayer);
			}
			return;
		}
		if (this.OnRpcMessage(basePlayer, nameID, message))
		{
			return;
		}
		for (int i = 0; i < this.Components.Length; i++)
		{
			if (this.Components[i].OnRpcMessage(basePlayer, nameID, message))
			{
				return;
			}
		}
	}

	// Token: 0x06000298 RID: 664 RVA: 0x00028778 File Offset: 0x00026978
	public void ClientRPCPlayer<T1, T2, T3, T4, T5>(Connection sourceConnection, global::BasePlayer player, string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (player.net.connection == null)
		{
			return;
		}
		this.ClientRPCEx<T1, T2, T3, T4, T5>(new SendInfo(player.net.connection), sourceConnection, funcName, arg1, arg2, arg3, arg4, arg5);
	}

	// Token: 0x06000299 RID: 665 RVA: 0x000287CC File Offset: 0x000269CC
	public void ClientRPCPlayer<T1, T2, T3, T4>(Connection sourceConnection, global::BasePlayer player, string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (player.net.connection == null)
		{
			return;
		}
		this.ClientRPCEx<T1, T2, T3, T4>(new SendInfo(player.net.connection), sourceConnection, funcName, arg1, arg2, arg3, arg4);
	}

	// Token: 0x0600029A RID: 666 RVA: 0x00028820 File Offset: 0x00026A20
	public void ClientRPCPlayer<T1, T2, T3>(Connection sourceConnection, global::BasePlayer player, string funcName, T1 arg1, T2 arg2, T3 arg3)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (player.net.connection == null)
		{
			return;
		}
		this.ClientRPCEx<T1, T2, T3>(new SendInfo(player.net.connection), sourceConnection, funcName, arg1, arg2, arg3);
	}

	// Token: 0x0600029B RID: 667 RVA: 0x00028870 File Offset: 0x00026A70
	public void ClientRPCPlayer<T1, T2>(Connection sourceConnection, global::BasePlayer player, string funcName, T1 arg1, T2 arg2)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (player.net.connection == null)
		{
			return;
		}
		this.ClientRPCEx<T1, T2>(new SendInfo(player.net.connection), sourceConnection, funcName, arg1, arg2);
	}

	// Token: 0x0600029C RID: 668 RVA: 0x000288BD File Offset: 0x00026ABD
	public void ClientRPCPlayer<T1>(Connection sourceConnection, global::BasePlayer player, string funcName, T1 arg1)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (player.net.connection == null)
		{
			return;
		}
		this.ClientRPCEx<T1>(new SendInfo(player.net.connection), sourceConnection, funcName, arg1);
	}

	// Token: 0x0600029D RID: 669 RVA: 0x000288FD File Offset: 0x00026AFD
	public void ClientRPCPlayer(Connection sourceConnection, global::BasePlayer player, string funcName)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (player.net.connection == null)
		{
			return;
		}
		this.ClientRPCEx(new SendInfo(player.net.connection), sourceConnection, funcName);
	}

	// Token: 0x0600029E RID: 670 RVA: 0x0002893C File Offset: 0x00026B3C
	public void ClientRPC<T1, T2, T3, T4, T5>(Connection sourceConnection, string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (this.net.group == null)
		{
			return;
		}
		this.ClientRPCEx<T1, T2, T3, T4, T5>(new SendInfo(this.net.group.subscribers), sourceConnection, funcName, arg1, arg2, arg3, arg4, arg5);
	}

	// Token: 0x0600029F RID: 671 RVA: 0x00028994 File Offset: 0x00026B94
	public void ClientRPC<T1, T2, T3, T4>(Connection sourceConnection, string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (this.net.group == null)
		{
			return;
		}
		this.ClientRPCEx<T1, T2, T3, T4>(new SendInfo(this.net.group.subscribers), sourceConnection, funcName, arg1, arg2, arg3, arg4);
	}

	// Token: 0x060002A0 RID: 672 RVA: 0x000289EC File Offset: 0x00026BEC
	public void ClientRPC<T1, T2, T3>(Connection sourceConnection, string funcName, T1 arg1, T2 arg2, T3 arg3)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (this.net.group == null)
		{
			return;
		}
		this.ClientRPCEx<T1, T2, T3>(new SendInfo(this.net.group.subscribers), sourceConnection, funcName, arg1, arg2, arg3);
	}

	// Token: 0x060002A1 RID: 673 RVA: 0x00028A40 File Offset: 0x00026C40
	public void ClientRPC<T1, T2>(Connection sourceConnection, string funcName, T1 arg1, T2 arg2)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (this.net.group == null)
		{
			return;
		}
		this.ClientRPCEx<T1, T2>(new SendInfo(this.net.group.subscribers), sourceConnection, funcName, arg1, arg2);
	}

	// Token: 0x060002A2 RID: 674 RVA: 0x00028A94 File Offset: 0x00026C94
	public void ClientRPC<T1>(Connection sourceConnection, string funcName, T1 arg1)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (this.net.group == null)
		{
			return;
		}
		this.ClientRPCEx<T1>(new SendInfo(this.net.group.subscribers), sourceConnection, funcName, arg1);
	}

	// Token: 0x060002A3 RID: 675 RVA: 0x00028AE4 File Offset: 0x00026CE4
	public void ClientRPC(Connection sourceConnection, string funcName)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (this.net.group == null)
		{
			return;
		}
		this.ClientRPCEx(new SendInfo(this.net.group.subscribers), sourceConnection, funcName);
	}

	// Token: 0x060002A4 RID: 676 RVA: 0x00028B34 File Offset: 0x00026D34
	public void ClientRPCEx<T1, T2, T3, T4, T5>(SendInfo sendInfo, Connection sourceConnection, string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		NetWrite write = this.ClientRPCStart(sourceConnection, funcName);
		this.ClientRPCWrite<T1>(write, arg1);
		this.ClientRPCWrite<T2>(write, arg2);
		this.ClientRPCWrite<T3>(write, arg3);
		this.ClientRPCWrite<T4>(write, arg4);
		this.ClientRPCWrite<T5>(write, arg5);
		this.ClientRPCSend(write, sendInfo);
	}

	// Token: 0x060002A5 RID: 677 RVA: 0x00028B98 File Offset: 0x00026D98
	public void ClientRPCEx<T1, T2, T3, T4>(SendInfo sendInfo, Connection sourceConnection, string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		NetWrite write = this.ClientRPCStart(sourceConnection, funcName);
		this.ClientRPCWrite<T1>(write, arg1);
		this.ClientRPCWrite<T2>(write, arg2);
		this.ClientRPCWrite<T3>(write, arg3);
		this.ClientRPCWrite<T4>(write, arg4);
		this.ClientRPCSend(write, sendInfo);
	}

	// Token: 0x060002A6 RID: 678 RVA: 0x00028BF0 File Offset: 0x00026DF0
	public void ClientRPCEx<T1, T2, T3>(SendInfo sendInfo, Connection sourceConnection, string funcName, T1 arg1, T2 arg2, T3 arg3)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		NetWrite write = this.ClientRPCStart(sourceConnection, funcName);
		this.ClientRPCWrite<T1>(write, arg1);
		this.ClientRPCWrite<T2>(write, arg2);
		this.ClientRPCWrite<T3>(write, arg3);
		this.ClientRPCSend(write, sendInfo);
	}

	// Token: 0x060002A7 RID: 679 RVA: 0x00028C40 File Offset: 0x00026E40
	public void ClientRPCEx<T1, T2>(SendInfo sendInfo, Connection sourceConnection, string funcName, T1 arg1, T2 arg2)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		NetWrite write = this.ClientRPCStart(sourceConnection, funcName);
		this.ClientRPCWrite<T1>(write, arg1);
		this.ClientRPCWrite<T2>(write, arg2);
		this.ClientRPCSend(write, sendInfo);
	}

	// Token: 0x060002A8 RID: 680 RVA: 0x00028C88 File Offset: 0x00026E88
	public void ClientRPCEx<T1>(SendInfo sendInfo, Connection sourceConnection, string funcName, T1 arg1)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		NetWrite write = this.ClientRPCStart(sourceConnection, funcName);
		this.ClientRPCWrite<T1>(write, arg1);
		this.ClientRPCSend(write, sendInfo);
	}

	// Token: 0x060002A9 RID: 681 RVA: 0x00028CC8 File Offset: 0x00026EC8
	public void ClientRPCEx(SendInfo sendInfo, Connection sourceConnection, string funcName)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		NetWrite write = this.ClientRPCStart(sourceConnection, funcName);
		this.ClientRPCSend(write, sendInfo);
	}

	// Token: 0x060002AA RID: 682 RVA: 0x00028CFC File Offset: 0x00026EFC
	public void ClientRPCPlayerAndSpectators(Connection sourceConnection, global::BasePlayer player, string funcName)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (player.net == null)
		{
			return;
		}
		if (player.net.connection == null)
		{
			return;
		}
		this.ClientRPCEx(new SendInfo(player.net.connection), sourceConnection, funcName);
		if (player.IsBeingSpectated && player.children != null)
		{
			using (List<global::BaseEntity>.Enumerator enumerator = player.children.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					global::BasePlayer player2;
					if ((player2 = (enumerator.Current as global::BasePlayer)) != null)
					{
						this.ClientRPCPlayer(sourceConnection, player2, funcName);
					}
				}
			}
		}
	}

	// Token: 0x060002AB RID: 683 RVA: 0x00028DA8 File Offset: 0x00026FA8
	public void ClientRPCPlayerAndSpectators<T1>(Connection sourceConnection, global::BasePlayer player, string funcName, T1 arg1)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (player.net == null)
		{
			return;
		}
		if (player.net.connection == null)
		{
			return;
		}
		this.ClientRPCEx<T1>(new SendInfo(player.net.connection), sourceConnection, funcName, arg1);
		if (player.IsBeingSpectated && player.children != null)
		{
			using (List<global::BaseEntity>.Enumerator enumerator = player.children.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					global::BasePlayer player2;
					if ((player2 = (enumerator.Current as global::BasePlayer)) != null)
					{
						this.ClientRPCPlayer<T1>(sourceConnection, player2, funcName, arg1);
					}
				}
			}
		}
	}

	// Token: 0x060002AC RID: 684 RVA: 0x00028E58 File Offset: 0x00027058
	public void ClientRPCPlayerAndSpectators<T1, T2>(Connection sourceConnection, global::BasePlayer player, string funcName, T1 arg1, T2 arg2)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (player.net == null)
		{
			return;
		}
		if (player.net.connection == null)
		{
			return;
		}
		this.ClientRPCPlayer<T1, T2>(sourceConnection, player, funcName, arg1, arg2);
		if (player.IsBeingSpectated && player.children != null)
		{
			using (List<global::BaseEntity>.Enumerator enumerator = this.children.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					global::BasePlayer player2;
					if ((player2 = (enumerator.Current as global::BasePlayer)) != null)
					{
						this.ClientRPCPlayer<T1, T2>(sourceConnection, player2, funcName, arg1, arg2);
					}
				}
			}
		}
	}

	// Token: 0x060002AD RID: 685 RVA: 0x00028EFC File Offset: 0x000270FC
	public void ClientRPCPlayerAndSpectators<T1, T2, T3>(Connection sourceConnection, global::BasePlayer player, string funcName, T1 arg1, T2 arg2, T3 arg3)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (player.net == null)
		{
			return;
		}
		if (player.net.connection == null)
		{
			return;
		}
		this.ClientRPCPlayer<T1, T2, T3>(sourceConnection, player, funcName, arg1, arg2, arg3);
		if (player.IsBeingSpectated && player.children != null)
		{
			using (List<global::BaseEntity>.Enumerator enumerator = player.children.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					global::BasePlayer player2;
					if ((player2 = (enumerator.Current as global::BasePlayer)) != null)
					{
						this.ClientRPCPlayer<T1, T2, T3>(sourceConnection, player2, funcName, arg1, arg2, arg3);
					}
				}
			}
		}
	}

	// Token: 0x060002AE RID: 686 RVA: 0x00028FA4 File Offset: 0x000271A4
	private NetWrite ClientRPCStart(Connection sourceConnection, string funcName)
	{
		NetWrite netWrite = Network.Net.sv.StartWrite();
		netWrite.PacketID(Message.Type.RPCMessage);
		netWrite.UInt32(this.net.ID);
		netWrite.UInt32(StringPool.Get(funcName));
		netWrite.UInt64((sourceConnection == null) ? 0UL : sourceConnection.userid);
		return netWrite;
	}

	// Token: 0x060002AF RID: 687 RVA: 0x00028FF3 File Offset: 0x000271F3
	private void ClientRPCWrite<T>(NetWrite write, T arg)
	{
		write.WriteObject(arg);
	}

	// Token: 0x060002B0 RID: 688 RVA: 0x00028FFC File Offset: 0x000271FC
	private void ClientRPCSend(NetWrite write, SendInfo sendInfo)
	{
		write.Send(sendInfo);
	}

	// Token: 0x1700003C RID: 60
	// (get) Token: 0x060002B1 RID: 689 RVA: 0x00029008 File Offset: 0x00027208
	public float radiationLevel
	{
		get
		{
			if (this.triggers == null)
			{
				return 0f;
			}
			float num = 0f;
			for (int i = 0; i < this.triggers.Count; i++)
			{
				TriggerRadiation triggerRadiation = this.triggers[i] as TriggerRadiation;
				if (!(triggerRadiation == null))
				{
					Vector3 position = this.GetNetworkPosition();
					global::BaseEntity parentEntity = base.GetParentEntity();
					if (parentEntity != null)
					{
						position = parentEntity.transform.TransformPoint(position);
					}
					num = Mathf.Max(num, triggerRadiation.GetRadiation(position, this.RadiationProtection()));
				}
			}
			return num;
		}
	}

	// Token: 0x060002B2 RID: 690 RVA: 0x00026FFC File Offset: 0x000251FC
	public virtual float RadiationProtection()
	{
		return 0f;
	}

	// Token: 0x060002B3 RID: 691 RVA: 0x000062DD File Offset: 0x000044DD
	public virtual float RadiationExposureFraction()
	{
		return 1f;
	}

	// Token: 0x1700003D RID: 61
	// (get) Token: 0x060002B4 RID: 692 RVA: 0x00029098 File Offset: 0x00027298
	public float currentTemperature
	{
		get
		{
			float num = Climate.GetTemperature(base.transform.position);
			if (this.triggers == null)
			{
				return num;
			}
			for (int i = 0; i < this.triggers.Count; i++)
			{
				TriggerTemperature triggerTemperature = this.triggers[i] as TriggerTemperature;
				if (!(triggerTemperature == null))
				{
					num = triggerTemperature.WorkoutTemperature(this.GetNetworkPosition(), num);
				}
			}
			return num;
		}
	}

	// Token: 0x1700003E RID: 62
	// (get) Token: 0x060002B5 RID: 693 RVA: 0x00029100 File Offset: 0x00027300
	public float currentEnvironmentalWetness
	{
		get
		{
			if (this.triggers == null)
			{
				return 0f;
			}
			float num = 0f;
			Vector3 networkPosition = this.GetNetworkPosition();
			using (List<TriggerBase>.Enumerator enumerator = this.triggers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TriggerWetness triggerWetness;
					if ((triggerWetness = (enumerator.Current as TriggerWetness)) != null)
					{
						num += triggerWetness.WorkoutWetness(networkPosition);
					}
				}
			}
			return Mathf.Clamp01(num);
		}
	}

	// Token: 0x060002B6 RID: 694 RVA: 0x00029180 File Offset: 0x00027380
	public virtual Vector3 GetLocalVelocityServer()
	{
		return Vector3.zero;
	}

	// Token: 0x060002B7 RID: 695 RVA: 0x00029187 File Offset: 0x00027387
	public virtual Quaternion GetAngularVelocityServer()
	{
		return Quaternion.identity;
	}

	// Token: 0x060002B8 RID: 696 RVA: 0x0002918E File Offset: 0x0002738E
	public void EnableGlobalBroadcast(bool wants)
	{
		if (this.globalBroadcast == wants)
		{
			return;
		}
		this.globalBroadcast = wants;
		this.UpdateNetworkGroup();
	}

	// Token: 0x060002B9 RID: 697 RVA: 0x000291A8 File Offset: 0x000273A8
	public void EnableSaving(bool wants)
	{
		if (this.enableSaving == wants)
		{
			return;
		}
		this.enableSaving = wants;
		if (this.enableSaving)
		{
			if (!global::BaseEntity.saveList.Contains(this))
			{
				global::BaseEntity.saveList.Add(this);
				return;
			}
		}
		else
		{
			global::BaseEntity.saveList.Remove(this);
		}
	}

	// Token: 0x060002BA RID: 698 RVA: 0x000291F4 File Offset: 0x000273F4
	public override void ServerInit()
	{
		this._spawnable = base.GetComponent<global::Spawnable>();
		base.ServerInit();
		if (this.enableSaving && !global::BaseEntity.saveList.Contains(this))
		{
			global::BaseEntity.saveList.Add(this);
		}
		if (this.flags != (global::BaseEntity.Flags)0)
		{
			this.OnFlagsChanged((global::BaseEntity.Flags)0, this.flags);
		}
		if (this.syncPosition && this.PositionTickRate >= 0f)
		{
			if (this.PositionTickFixedTime)
			{
				base.InvokeRepeatingFixedTime(new Action(this.NetworkPositionTick));
			}
			else
			{
				base.InvokeRandomized(new Action(this.NetworkPositionTick), this.PositionTickRate, this.PositionTickRate - this.PositionTickRate * 0.05f, this.PositionTickRate * 0.05f);
			}
		}
		global::BaseEntity.Query.Server.Add(this);
	}

	// Token: 0x060002BB RID: 699 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnSensation(Sensation sensation)
	{
	}

	// Token: 0x1700003F RID: 63
	// (get) Token: 0x060002BC RID: 700 RVA: 0x000292BC File Offset: 0x000274BC
	protected virtual float PositionTickRate
	{
		get
		{
			return 0.1f;
		}
	}

	// Token: 0x17000040 RID: 64
	// (get) Token: 0x060002BD RID: 701 RVA: 0x00007074 File Offset: 0x00005274
	protected virtual bool PositionTickFixedTime
	{
		get
		{
			return false;
		}
	}

	// Token: 0x060002BE RID: 702 RVA: 0x000292C4 File Offset: 0x000274C4
	protected void NetworkPositionTick()
	{
		if (!base.transform.hasChanged)
		{
			if (this.ticksSinceStopped >= 6)
			{
				return;
			}
			this.ticksSinceStopped++;
		}
		else
		{
			this.ticksSinceStopped = 0;
		}
		this.TransformChanged();
		base.transform.hasChanged = false;
	}

	// Token: 0x060002BF RID: 703 RVA: 0x00029314 File Offset: 0x00027514
	private void TransformChanged()
	{
		if (global::BaseEntity.Query.Server != null)
		{
			global::BaseEntity.Query.Server.Move(this);
		}
		if (this.net == null)
		{
			return;
		}
		base.InvalidateNetworkCache();
		if (!this.globalBroadcast && !ValidBounds.Test(base.transform.position))
		{
			this.OnInvalidPosition();
			return;
		}
		if (this.syncPosition)
		{
			if (!this.isCallingUpdateNetworkGroup)
			{
				base.Invoke(new Action(this.UpdateNetworkGroup), 5f);
				this.isCallingUpdateNetworkGroup = true;
			}
			base.SendNetworkUpdate_Position();
			this.OnPositionalNetworkUpdate();
		}
	}

	// Token: 0x060002C0 RID: 704 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnPositionalNetworkUpdate()
	{
	}

	// Token: 0x060002C1 RID: 705 RVA: 0x000293A0 File Offset: 0x000275A0
	public void DoMovingWithoutARigidBodyCheck()
	{
		if (this.doneMovingWithoutARigidBodyCheck > 10)
		{
			return;
		}
		this.doneMovingWithoutARigidBodyCheck++;
		if (this.doneMovingWithoutARigidBodyCheck < 10)
		{
			return;
		}
		if (base.GetComponent<Collider>() == null)
		{
			return;
		}
		if (base.GetComponent<Rigidbody>() == null)
		{
			Debug.LogWarning("Entity moving without a rigid body! (" + base.gameObject + ")", this);
		}
	}

	// Token: 0x060002C2 RID: 706 RVA: 0x00029409 File Offset: 0x00027609
	public override void Spawn()
	{
		base.Spawn();
		if (base.isServer)
		{
			base.gameObject.BroadcastOnParentSpawning();
		}
	}

	// Token: 0x060002C3 RID: 707 RVA: 0x00029424 File Offset: 0x00027624
	public void OnParentSpawning()
	{
		if (this.net != null)
		{
			return;
		}
		if (base.IsDestroyed)
		{
			return;
		}
		if (Rust.Application.isLoadingSave)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		if (GameManager.server.preProcessed.NeedsProcessing(base.gameObject))
		{
			GameManager.server.preProcessed.ProcessObject(null, base.gameObject, false);
		}
		global::BaseEntity baseEntity = (base.transform.parent != null) ? base.transform.parent.GetComponentInParent<global::BaseEntity>() : null;
		this.Spawn();
		if (baseEntity != null)
		{
			this.SetParent(baseEntity, true, false);
		}
	}

	// Token: 0x060002C4 RID: 708 RVA: 0x000294C4 File Offset: 0x000276C4
	public void SpawnAsMapEntity()
	{
		if (this.net != null)
		{
			return;
		}
		if (base.IsDestroyed)
		{
			return;
		}
		if (((base.transform.parent != null) ? base.transform.parent.GetComponentInParent<global::BaseEntity>() : null) == null)
		{
			if (GameManager.server.preProcessed.NeedsProcessing(base.gameObject))
			{
				GameManager.server.preProcessed.ProcessObject(null, base.gameObject, false);
			}
			base.transform.parent = null;
			SceneManager.MoveGameObjectToScene(base.gameObject, Rust.Server.EntityScene);
			base.gameObject.SetActive(true);
			this.Spawn();
		}
	}

	// Token: 0x060002C5 RID: 709 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void PostMapEntitySpawn()
	{
	}

	// Token: 0x060002C6 RID: 710 RVA: 0x00029570 File Offset: 0x00027770
	internal override void DoServerDestroy()
	{
		base.CancelInvoke(new Action(this.NetworkPositionTick));
		global::BaseEntity.saveList.Remove(this);
		this.RemoveFromTriggers();
		if (this.children != null)
		{
			global::BaseEntity[] array = this.children.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnParentRemoved();
			}
		}
		this.SetParent(null, true, false);
		global::BaseEntity.Query.Server.Remove(this, false);
		base.DoServerDestroy();
	}

	// Token: 0x060002C7 RID: 711 RVA: 0x000029D4 File Offset: 0x00000BD4
	internal virtual void OnParentRemoved()
	{
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x060002C8 RID: 712 RVA: 0x000295E8 File Offset: 0x000277E8
	public virtual void OnInvalidPosition()
	{
		Debug.Log(string.Concat(new object[]
		{
			"Invalid Position: ",
			this,
			" ",
			base.transform.position,
			" (destroying)"
		}));
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x060002C9 RID: 713 RVA: 0x0002963C File Offset: 0x0002783C
	public BaseCorpse DropCorpse(string strCorpsePrefab)
	{
		Assert.IsTrue(base.isServer, "DropCorpse called on client!");
		if (!ConVar.Server.corpses)
		{
			return null;
		}
		if (string.IsNullOrEmpty(strCorpsePrefab))
		{
			return null;
		}
		BaseCorpse baseCorpse = GameManager.server.CreateEntity(strCorpsePrefab, default(Vector3), default(Quaternion), true) as BaseCorpse;
		if (baseCorpse == null)
		{
			Debug.LogWarning(string.Concat(new object[]
			{
				"Error creating corpse: ",
				base.gameObject,
				" - ",
				strCorpsePrefab
			}));
			return null;
		}
		baseCorpse.InitCorpse(this);
		return baseCorpse;
	}

	// Token: 0x060002CA RID: 714 RVA: 0x000296D0 File Offset: 0x000278D0
	public override void UpdateNetworkGroup()
	{
		Assert.IsTrue(base.isServer, "UpdateNetworkGroup called on clientside entity!");
		this.isCallingUpdateNetworkGroup = false;
		if (this.net == null)
		{
			return;
		}
		if (Network.Net.sv == null)
		{
			return;
		}
		if (Network.Net.sv.visibility == null)
		{
			return;
		}
		using (TimeWarning.New("UpdateNetworkGroup", 0))
		{
			if (this.globalBroadcast)
			{
				if (this.net.SwitchGroup(global::BaseNetworkable.GlobalNetworkGroup))
				{
					base.SendNetworkGroupChange();
				}
			}
			else if (this.ShouldInheritNetworkGroup() && this.parentEntity.IsSet())
			{
				global::BaseEntity parentEntity = base.GetParentEntity();
				if (!parentEntity.IsValid())
				{
					if (!Rust.Application.isLoadingSave)
					{
						Debug.LogWarning("UpdateNetworkGroup: Missing parent entity " + this.parentEntity.uid);
						base.Invoke(new Action(this.UpdateNetworkGroup), 2f);
						this.isCallingUpdateNetworkGroup = true;
					}
				}
				else if (parentEntity != null)
				{
					if (this.net.SwitchGroup(parentEntity.net.group))
					{
						base.SendNetworkGroupChange();
					}
				}
				else
				{
					Debug.LogWarning(base.gameObject + ": has parent id - but couldn't find parent! " + this.parentEntity);
				}
			}
			else if (base.limitNetworking)
			{
				if (this.net.SwitchGroup(global::BaseNetworkable.LimboNetworkGroup))
				{
					base.SendNetworkGroupChange();
				}
			}
			else
			{
				base.UpdateNetworkGroup();
			}
		}
	}

	// Token: 0x060002CB RID: 715 RVA: 0x00029854 File Offset: 0x00027A54
	public virtual void Eat(BaseNpc baseNpc, float timeSpent)
	{
		baseNpc.AddCalories(100f);
	}

	// Token: 0x060002CC RID: 716 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnDeployed(global::BaseEntity parent, global::BasePlayer deployedBy, global::Item fromItem)
	{
	}

	// Token: 0x060002CD RID: 717 RVA: 0x00029864 File Offset: 0x00027A64
	public override bool ShouldNetworkTo(global::BasePlayer player)
	{
		if (player == this)
		{
			return true;
		}
		global::BaseEntity parentEntity = base.GetParentEntity();
		if (base.limitNetworking)
		{
			if (parentEntity == null)
			{
				return false;
			}
			if (parentEntity != player)
			{
				return false;
			}
		}
		if (parentEntity != null)
		{
			return parentEntity.ShouldNetworkTo(player);
		}
		return base.ShouldNetworkTo(player);
	}

	// Token: 0x060002CE RID: 718 RVA: 0x000298B9 File Offset: 0x00027AB9
	public virtual void AttackerInfo(PlayerLifeStory.DeathInfo info)
	{
		info.attackerName = base.ShortPrefabName;
		info.attackerSteamID = 0UL;
		info.inflictorName = "";
	}

	// Token: 0x060002CF RID: 719 RVA: 0x000298DA File Offset: 0x00027ADA
	public virtual void Push(Vector3 velocity)
	{
		this.SetVelocity(velocity);
	}

	// Token: 0x060002D0 RID: 720 RVA: 0x000298E4 File Offset: 0x00027AE4
	public virtual void ApplyInheritedVelocity(Vector3 velocity)
	{
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component)
		{
			component.velocity = Vector3.Lerp(component.velocity, velocity, 10f * UnityEngine.Time.fixedDeltaTime);
			component.angularVelocity *= Mathf.Clamp01(1f - 10f * UnityEngine.Time.fixedDeltaTime);
			component.AddForce(-UnityEngine.Physics.gravity * Mathf.Clamp01(0.9f), ForceMode.Acceleration);
		}
	}

	// Token: 0x060002D1 RID: 721 RVA: 0x00029964 File Offset: 0x00027B64
	public virtual void SetVelocity(Vector3 velocity)
	{
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component)
		{
			component.velocity = velocity;
		}
	}

	// Token: 0x060002D2 RID: 722 RVA: 0x00029988 File Offset: 0x00027B88
	public virtual void SetAngularVelocity(Vector3 velocity)
	{
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component)
		{
			component.angularVelocity = velocity;
		}
	}

	// Token: 0x060002D3 RID: 723 RVA: 0x000299AB File Offset: 0x00027BAB
	public virtual Vector3 GetDropPosition()
	{
		return base.transform.position;
	}

	// Token: 0x060002D4 RID: 724 RVA: 0x000299B8 File Offset: 0x00027BB8
	public virtual Vector3 GetDropVelocity()
	{
		return this.GetInheritedDropVelocity() + Vector3.up;
	}

	// Token: 0x060002D5 RID: 725 RVA: 0x00003A54 File Offset: 0x00001C54
	public virtual bool OnStartBeingLooted(global::BasePlayer baseEntity)
	{
		return true;
	}

	// Token: 0x17000041 RID: 65
	// (get) Token: 0x060002D6 RID: 726 RVA: 0x000299CA File Offset: 0x00027BCA
	// (set) Token: 0x060002D7 RID: 727 RVA: 0x000299D7 File Offset: 0x00027BD7
	public virtual Vector3 ServerPosition
	{
		get
		{
			return base.transform.localPosition;
		}
		set
		{
			if (base.transform.localPosition == value)
			{
				return;
			}
			base.transform.localPosition = value;
			base.transform.hasChanged = true;
		}
	}

	// Token: 0x17000042 RID: 66
	// (get) Token: 0x060002D8 RID: 728 RVA: 0x00029A05 File Offset: 0x00027C05
	// (set) Token: 0x060002D9 RID: 729 RVA: 0x00029A12 File Offset: 0x00027C12
	public virtual Quaternion ServerRotation
	{
		get
		{
			return base.transform.localRotation;
		}
		set
		{
			if (base.transform.localRotation == value)
			{
				return;
			}
			base.transform.localRotation = value;
			base.transform.hasChanged = true;
		}
	}

	// Token: 0x060002DA RID: 730 RVA: 0x00029A40 File Offset: 0x00027C40
	public virtual string Admin_Who()
	{
		return string.Format("Owner ID: {0}", this.OwnerID);
	}

	// Token: 0x060002DB RID: 731 RVA: 0x00029A58 File Offset: 0x00027C58
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	private void BroadcastSignalFromClient(global::BaseEntity.RPCMessage msg)
	{
		uint num = StringPool.Get("BroadcastSignalFromClient");
		if (num == 0U)
		{
			return;
		}
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		if (!player.rpcHistory.TryIncrement(num, (ulong)((long)ConVar.Server.maxpacketspersecond_rpc_signal)))
		{
			return;
		}
		global::BaseEntity.Signal signal = (global::BaseEntity.Signal)msg.read.Int32();
		string arg = msg.read.String(256);
		this.SignalBroadcast(signal, arg, msg.connection);
	}

	// Token: 0x060002DC RID: 732 RVA: 0x00029AC8 File Offset: 0x00027CC8
	public void SignalBroadcast(global::BaseEntity.Signal signal, string arg, Connection sourceConnection = null)
	{
		if (this.net == null)
		{
			return;
		}
		if (this.net.group == null)
		{
			return;
		}
		this.ClientRPCEx<int, string>(new SendInfo(this.net.group.subscribers)
		{
			method = SendMethod.Unreliable,
			priority = Priority.Immediate
		}, sourceConnection, "SignalFromServerEx", (int)signal, arg);
	}

	// Token: 0x060002DD RID: 733 RVA: 0x00029B24 File Offset: 0x00027D24
	public void SignalBroadcast(global::BaseEntity.Signal signal, Connection sourceConnection = null)
	{
		if (this.net == null)
		{
			return;
		}
		if (this.net.group == null)
		{
			return;
		}
		this.ClientRPCEx<int>(new SendInfo(this.net.group.subscribers)
		{
			method = SendMethod.Unreliable,
			priority = Priority.Immediate
		}, sourceConnection, "SignalFromServer", (int)signal);
	}

	// Token: 0x060002DE RID: 734 RVA: 0x00029B7D File Offset: 0x00027D7D
	private void OnSkinChanged(ulong oldSkinID, ulong newSkinID)
	{
		if (oldSkinID == newSkinID)
		{
			return;
		}
		this.skinID = newSkinID;
	}

	// Token: 0x060002DF RID: 735 RVA: 0x00029B8B File Offset: 0x00027D8B
	public virtual void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (clientside && Skinnable.All != null && Skinnable.FindForEntity(name) != null)
		{
			Rust.Workshop.WorkshopSkin.Prepare(rootObj);
			MaterialReplacement.Prepare(rootObj);
		}
	}

	// Token: 0x060002E0 RID: 736 RVA: 0x00029BB4 File Offset: 0x00027DB4
	public bool HasAnySlot()
	{
		for (int i = 0; i < this.entitySlots.Length; i++)
		{
			if (this.entitySlots[i].IsValid(base.isServer))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060002E1 RID: 737 RVA: 0x00029BF0 File Offset: 0x00027DF0
	public global::BaseEntity GetSlot(global::BaseEntity.Slot slot)
	{
		return this.entitySlots[(int)slot].Get(base.isServer);
	}

	// Token: 0x060002E2 RID: 738 RVA: 0x00029C09 File Offset: 0x00027E09
	public string GetSlotAnchorName(global::BaseEntity.Slot slot)
	{
		return slot.ToString().ToLower();
	}

	// Token: 0x060002E3 RID: 739 RVA: 0x00029C1D File Offset: 0x00027E1D
	public void SetSlot(global::BaseEntity.Slot slot, global::BaseEntity ent)
	{
		this.entitySlots[(int)slot].Set(ent);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060002E4 RID: 740 RVA: 0x00029C38 File Offset: 0x00027E38
	public EntityRef[] GetSlots()
	{
		return this.entitySlots;
	}

	// Token: 0x060002E5 RID: 741 RVA: 0x00029C40 File Offset: 0x00027E40
	public void SetSlots(EntityRef[] newSlots)
	{
		this.entitySlots = newSlots;
	}

	// Token: 0x060002E6 RID: 742 RVA: 0x00007074 File Offset: 0x00005274
	public virtual bool HasSlot(global::BaseEntity.Slot slot)
	{
		return false;
	}

	// Token: 0x17000043 RID: 67
	// (get) Token: 0x060002E7 RID: 743 RVA: 0x00007074 File Offset: 0x00005274
	public virtual global::BaseEntity.TraitFlag Traits
	{
		get
		{
			return global::BaseEntity.TraitFlag.None;
		}
	}

	// Token: 0x060002E8 RID: 744 RVA: 0x00029C49 File Offset: 0x00027E49
	public bool HasTrait(global::BaseEntity.TraitFlag f)
	{
		return (this.Traits & f) == f;
	}

	// Token: 0x060002E9 RID: 745 RVA: 0x00029C56 File Offset: 0x00027E56
	public bool HasAnyTrait(global::BaseEntity.TraitFlag f)
	{
		return (this.Traits & f) > global::BaseEntity.TraitFlag.None;
	}

	// Token: 0x060002EA RID: 746 RVA: 0x00029C63 File Offset: 0x00027E63
	public virtual bool EnterTrigger(TriggerBase trigger)
	{
		if (this.triggers == null)
		{
			this.triggers = Facepunch.Pool.Get<List<TriggerBase>>();
		}
		this.triggers.Add(trigger);
		return true;
	}

	// Token: 0x060002EB RID: 747 RVA: 0x00029C85 File Offset: 0x00027E85
	public virtual void LeaveTrigger(TriggerBase trigger)
	{
		if (this.triggers == null)
		{
			return;
		}
		this.triggers.Remove(trigger);
		if (this.triggers.Count == 0)
		{
			Facepunch.Pool.FreeList<TriggerBase>(ref this.triggers);
		}
	}

	// Token: 0x060002EC RID: 748 RVA: 0x00029CB8 File Offset: 0x00027EB8
	public void RemoveFromTriggers()
	{
		if (this.triggers == null)
		{
			return;
		}
		using (TimeWarning.New("RemoveFromTriggers", 0))
		{
			foreach (TriggerBase triggerBase in this.triggers.ToArray())
			{
				if (triggerBase)
				{
					triggerBase.RemoveEntity(this);
				}
			}
			if (this.triggers != null && this.triggers.Count == 0)
			{
				Facepunch.Pool.FreeList<TriggerBase>(ref this.triggers);
			}
		}
	}

	// Token: 0x060002ED RID: 749 RVA: 0x00029D44 File Offset: 0x00027F44
	public T FindTrigger<T>() where T : TriggerBase
	{
		if (this.triggers == null)
		{
			return default(T);
		}
		foreach (TriggerBase triggerBase in this.triggers)
		{
			if (!(triggerBase as T == null))
			{
				return triggerBase as T;
			}
		}
		return default(T);
	}

	// Token: 0x060002EE RID: 750 RVA: 0x00029DD4 File Offset: 0x00027FD4
	public bool FindTrigger<T>(out T result) where T : TriggerBase
	{
		result = this.FindTrigger<T>();
		return result != null;
	}

	// Token: 0x060002EF RID: 751 RVA: 0x00029DF3 File Offset: 0x00027FF3
	private void ForceUpdateTriggersAction()
	{
		if (!base.IsDestroyed)
		{
			this.ForceUpdateTriggers(false, true, false);
		}
	}

	// Token: 0x060002F0 RID: 752 RVA: 0x00029E08 File Offset: 0x00028008
	public void ForceUpdateTriggers(bool enter = true, bool exit = true, bool invoke = true)
	{
		List<TriggerBase> list = Facepunch.Pool.GetList<TriggerBase>();
		List<TriggerBase> list2 = Facepunch.Pool.GetList<TriggerBase>();
		if (this.triggers != null)
		{
			list.AddRange(this.triggers);
		}
		Collider componentInChildren = base.GetComponentInChildren<Collider>();
		if (componentInChildren is CapsuleCollider)
		{
			CapsuleCollider capsuleCollider = componentInChildren as CapsuleCollider;
			Vector3 point = base.transform.position + new Vector3(0f, capsuleCollider.radius, 0f);
			Vector3 point2 = base.transform.position + new Vector3(0f, capsuleCollider.height - capsuleCollider.radius, 0f);
			GamePhysics.OverlapCapsule<TriggerBase>(point, point2, capsuleCollider.radius, list2, 262144, QueryTriggerInteraction.Collide);
		}
		else if (componentInChildren is BoxCollider)
		{
			BoxCollider boxCollider = componentInChildren as BoxCollider;
			GamePhysics.OverlapOBB<TriggerBase>(new OBB(base.transform.position, base.transform.lossyScale, base.transform.rotation, new Bounds(boxCollider.center, boxCollider.size)), list2, 262144, QueryTriggerInteraction.Collide);
		}
		else if (componentInChildren is SphereCollider)
		{
			SphereCollider sphereCollider = componentInChildren as SphereCollider;
			GamePhysics.OverlapSphere<TriggerBase>(base.transform.TransformPoint(sphereCollider.center), sphereCollider.radius, list2, 262144, QueryTriggerInteraction.Collide);
		}
		else
		{
			list2.AddRange(list);
		}
		if (exit)
		{
			foreach (TriggerBase triggerBase in list)
			{
				if (!list2.Contains(triggerBase))
				{
					triggerBase.OnTriggerExit(componentInChildren);
				}
			}
		}
		if (enter)
		{
			foreach (TriggerBase triggerBase2 in list2)
			{
				if (!list.Contains(triggerBase2))
				{
					triggerBase2.OnTriggerEnter(componentInChildren);
				}
			}
		}
		Facepunch.Pool.FreeList<TriggerBase>(ref list);
		Facepunch.Pool.FreeList<TriggerBase>(ref list2);
		if (invoke)
		{
			base.Invoke(new Action(this.ForceUpdateTriggersAction), UnityEngine.Time.time - UnityEngine.Time.fixedTime + UnityEngine.Time.fixedDeltaTime * 1.5f);
		}
	}

	// Token: 0x060002F1 RID: 753 RVA: 0x0002A024 File Offset: 0x00028224
	public TriggerParent FindSuitableParent()
	{
		if (this.triggers == null)
		{
			return null;
		}
		using (List<TriggerBase>.Enumerator enumerator = this.triggers.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				TriggerParent triggerParent;
				if ((triggerParent = (enumerator.Current as TriggerParent)) != null && triggerParent.ShouldParent(this, true))
				{
					return triggerParent;
				}
			}
		}
		return null;
	}

	// Token: 0x17000044 RID: 68
	// (get) Token: 0x060002F2 RID: 754 RVA: 0x0002A094 File Offset: 0x00028294
	// (set) Token: 0x060002F3 RID: 755 RVA: 0x0002A09C File Offset: 0x0002829C
	public float Weight { get; protected set; }

	// Token: 0x17000045 RID: 69
	// (get) Token: 0x060002F4 RID: 756 RVA: 0x0002A0A8 File Offset: 0x000282A8
	public EntityComponentBase[] Components
	{
		get
		{
			EntityComponentBase[] result;
			if ((result = this._components) == null)
			{
				result = (this._components = base.GetComponentsInChildren<EntityComponentBase>(true));
			}
			return result;
		}
	}

	// Token: 0x060002F5 RID: 757 RVA: 0x0002A0CF File Offset: 0x000282CF
	public virtual global::BasePlayer ToPlayer()
	{
		return null;
	}

	// Token: 0x17000046 RID: 70
	// (get) Token: 0x060002F6 RID: 758 RVA: 0x00007074 File Offset: 0x00005274
	public virtual bool IsNpc
	{
		get
		{
			return false;
		}
	}

	// Token: 0x060002F7 RID: 759 RVA: 0x0002A0D2 File Offset: 0x000282D2
	public override void InitShared()
	{
		base.InitShared();
		this.InitEntityLinks();
	}

	// Token: 0x060002F8 RID: 760 RVA: 0x0002A0E0 File Offset: 0x000282E0
	public override void DestroyShared()
	{
		base.DestroyShared();
		this.FreeEntityLinks();
	}

	// Token: 0x060002F9 RID: 761 RVA: 0x0002A0EE File Offset: 0x000282EE
	public override void ResetState()
	{
		base.ResetState();
		this.parentBone = 0U;
		this.OwnerID = 0UL;
		this.flags = (global::BaseEntity.Flags)0;
		this.parentEntity = default(EntityRef);
		if (base.isServer)
		{
			this._spawnable = null;
		}
	}

	// Token: 0x060002FA RID: 762 RVA: 0x00026FFC File Offset: 0x000251FC
	public virtual float InheritedVelocityScale()
	{
		return 0f;
	}

	// Token: 0x060002FB RID: 763 RVA: 0x00003A54 File Offset: 0x00001C54
	public virtual bool InheritedVelocityDirection()
	{
		return true;
	}

	// Token: 0x060002FC RID: 764 RVA: 0x0002A128 File Offset: 0x00028328
	public virtual Vector3 GetInheritedProjectileVelocity(Vector3 direction)
	{
		global::BaseEntity baseEntity = this.parentEntity.Get(base.isServer);
		if (baseEntity == null)
		{
			return Vector3.zero;
		}
		if (baseEntity.InheritedVelocityDirection())
		{
			return this.GetParentVelocity() * baseEntity.InheritedVelocityScale();
		}
		return Mathf.Max(Vector3.Dot(this.GetParentVelocity() * baseEntity.InheritedVelocityScale(), direction), 0f) * direction;
	}

	// Token: 0x060002FD RID: 765 RVA: 0x0002A197 File Offset: 0x00028397
	public virtual Vector3 GetInheritedThrowVelocity(Vector3 direction)
	{
		return this.GetParentVelocity();
	}

	// Token: 0x060002FE RID: 766 RVA: 0x0002A1A0 File Offset: 0x000283A0
	public virtual Vector3 GetInheritedDropVelocity()
	{
		global::BaseEntity baseEntity = this.parentEntity.Get(base.isServer);
		if (!(baseEntity != null))
		{
			return Vector3.zero;
		}
		return baseEntity.GetWorldVelocity();
	}

	// Token: 0x060002FF RID: 767 RVA: 0x0002A1D4 File Offset: 0x000283D4
	public Vector3 GetParentVelocity()
	{
		global::BaseEntity baseEntity = this.parentEntity.Get(base.isServer);
		if (!(baseEntity != null))
		{
			return Vector3.zero;
		}
		return baseEntity.GetWorldVelocity() + (baseEntity.GetAngularVelocity() * base.transform.localPosition - base.transform.localPosition);
	}

	// Token: 0x06000300 RID: 768 RVA: 0x0002A234 File Offset: 0x00028434
	public Vector3 GetWorldVelocity()
	{
		global::BaseEntity baseEntity = this.parentEntity.Get(base.isServer);
		if (!(baseEntity != null))
		{
			return this.GetLocalVelocity();
		}
		return baseEntity.GetWorldVelocity() + (baseEntity.GetAngularVelocity() * base.transform.localPosition - base.transform.localPosition) + baseEntity.transform.TransformDirection(this.GetLocalVelocity());
	}

	// Token: 0x06000301 RID: 769 RVA: 0x0002A2AA File Offset: 0x000284AA
	public Vector3 GetLocalVelocity()
	{
		if (base.isServer)
		{
			return this.GetLocalVelocityServer();
		}
		return Vector3.zero;
	}

	// Token: 0x06000302 RID: 770 RVA: 0x0002A2C0 File Offset: 0x000284C0
	public Quaternion GetAngularVelocity()
	{
		if (base.isServer)
		{
			return this.GetAngularVelocityServer();
		}
		return Quaternion.identity;
	}

	// Token: 0x06000303 RID: 771 RVA: 0x0002A2D6 File Offset: 0x000284D6
	public virtual OBB WorldSpaceBounds()
	{
		return new OBB(base.transform.position, base.transform.lossyScale, base.transform.rotation, this.bounds);
	}

	// Token: 0x06000304 RID: 772 RVA: 0x000299AB File Offset: 0x00027BAB
	public Vector3 PivotPoint()
	{
		return base.transform.position;
	}

	// Token: 0x06000305 RID: 773 RVA: 0x0002A304 File Offset: 0x00028504
	public Vector3 CenterPoint()
	{
		return this.WorldSpaceBounds().position;
	}

	// Token: 0x06000306 RID: 774 RVA: 0x0002A314 File Offset: 0x00028514
	public Vector3 ClosestPoint(Vector3 position)
	{
		return this.WorldSpaceBounds().ClosestPoint(position);
	}

	// Token: 0x06000307 RID: 775 RVA: 0x0002A330 File Offset: 0x00028530
	public virtual Vector3 TriggerPoint()
	{
		return this.CenterPoint();
	}

	// Token: 0x06000308 RID: 776 RVA: 0x0002A338 File Offset: 0x00028538
	public float Distance(Vector3 position)
	{
		return (this.ClosestPoint(position) - position).magnitude;
	}

	// Token: 0x06000309 RID: 777 RVA: 0x0002A35C File Offset: 0x0002855C
	public float SqrDistance(Vector3 position)
	{
		return (this.ClosestPoint(position) - position).sqrMagnitude;
	}

	// Token: 0x0600030A RID: 778 RVA: 0x0002A37E File Offset: 0x0002857E
	public float Distance(global::BaseEntity other)
	{
		return this.Distance(other.transform.position);
	}

	// Token: 0x0600030B RID: 779 RVA: 0x0002A391 File Offset: 0x00028591
	public float SqrDistance(global::BaseEntity other)
	{
		return this.SqrDistance(other.transform.position);
	}

	// Token: 0x0600030C RID: 780 RVA: 0x0002A3A4 File Offset: 0x000285A4
	public float Distance2D(Vector3 position)
	{
		return (this.ClosestPoint(position) - position).Magnitude2D();
	}

	// Token: 0x0600030D RID: 781 RVA: 0x0002A3B8 File Offset: 0x000285B8
	public float SqrDistance2D(Vector3 position)
	{
		return (this.ClosestPoint(position) - position).SqrMagnitude2D();
	}

	// Token: 0x0600030E RID: 782 RVA: 0x0002A37E File Offset: 0x0002857E
	public float Distance2D(global::BaseEntity other)
	{
		return this.Distance(other.transform.position);
	}

	// Token: 0x0600030F RID: 783 RVA: 0x0002A391 File Offset: 0x00028591
	public float SqrDistance2D(global::BaseEntity other)
	{
		return this.SqrDistance(other.transform.position);
	}

	// Token: 0x06000310 RID: 784 RVA: 0x0002A3CC File Offset: 0x000285CC
	public bool IsVisible(Ray ray, int layerMask, float maxDistance)
	{
		if (ray.origin.IsNaNOrInfinity())
		{
			return false;
		}
		if (ray.direction.IsNaNOrInfinity())
		{
			return false;
		}
		if (ray.direction == Vector3.zero)
		{
			return false;
		}
		RaycastHit raycastHit;
		if (!this.WorldSpaceBounds().Trace(ray, out raycastHit, maxDistance))
		{
			return false;
		}
		RaycastHit hit;
		if (GamePhysics.Trace(ray, 0f, out hit, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal, null))
		{
			global::BaseEntity entity = hit.GetEntity();
			if (entity == this)
			{
				return true;
			}
			if (entity != null && base.GetParentEntity() && base.GetParentEntity().EqualNetID(entity) && hit.IsOnLayer(Rust.Layer.Vehicle_Detailed))
			{
				return true;
			}
			if (hit.distance <= raycastHit.distance)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000311 RID: 785 RVA: 0x0002A48C File Offset: 0x0002868C
	public bool IsVisibleSpecificLayers(Vector3 position, Vector3 target, int layerMask, float maxDistance = float.PositiveInfinity)
	{
		Vector3 a = target - position;
		float magnitude = a.magnitude;
		if (magnitude < Mathf.Epsilon)
		{
			return true;
		}
		Vector3 vector = a / magnitude;
		Vector3 b = vector * Mathf.Min(magnitude, 0.01f);
		return this.IsVisible(new Ray(position + b, vector), layerMask, maxDistance);
	}

	// Token: 0x06000312 RID: 786 RVA: 0x0002A4E4 File Offset: 0x000286E4
	public bool IsVisible(Vector3 position, Vector3 target, float maxDistance = float.PositiveInfinity)
	{
		Vector3 a = target - position;
		float magnitude = a.magnitude;
		if (magnitude < Mathf.Epsilon)
		{
			return true;
		}
		Vector3 vector = a / magnitude;
		Vector3 b = vector * Mathf.Min(magnitude, 0.01f);
		return this.IsVisible(new Ray(position + b, vector), 1218519041, maxDistance);
	}

	// Token: 0x06000313 RID: 787 RVA: 0x0002A540 File Offset: 0x00028740
	public bool IsVisible(Vector3 position, float maxDistance = float.PositiveInfinity)
	{
		Vector3 target = this.CenterPoint();
		if (this.IsVisible(position, target, maxDistance))
		{
			return true;
		}
		Vector3 target2 = this.ClosestPoint(position);
		return this.IsVisible(position, target2, maxDistance);
	}

	// Token: 0x06000314 RID: 788 RVA: 0x0002A578 File Offset: 0x00028778
	public bool IsVisibleAndCanSee(Vector3 position, float maxDistance = float.PositiveInfinity)
	{
		Vector3 vector = this.CenterPoint();
		if (this.IsVisible(position, vector, maxDistance) && this.IsVisible(vector, position, maxDistance))
		{
			return true;
		}
		Vector3 vector2 = this.ClosestPoint(position);
		return this.IsVisible(position, vector2, maxDistance) && this.IsVisible(vector2, position, maxDistance);
	}

	// Token: 0x06000315 RID: 789 RVA: 0x0002A5C8 File Offset: 0x000287C8
	public bool IsOlderThan(global::BaseEntity other)
	{
		if (other == null)
		{
			return true;
		}
		uint num = (this.net == null) ? 0U : this.net.ID;
		uint num2 = (other.net == null) ? 0U : other.net.ID;
		return num < num2;
	}

	// Token: 0x06000316 RID: 790 RVA: 0x0002A610 File Offset: 0x00028810
	public virtual bool IsOutside()
	{
		OBB obb = this.WorldSpaceBounds();
		return this.IsOutside(obb.position);
	}

	// Token: 0x06000317 RID: 791 RVA: 0x0002A630 File Offset: 0x00028830
	public bool IsOutside(Vector3 position)
	{
		bool result = true;
		bool flag;
		do
		{
			flag = false;
			RaycastHit raycastHit;
			if (UnityEngine.Physics.Raycast(position, Vector3.up, out raycastHit, 100f, 161546513))
			{
				global::BaseEntity baseEntity = raycastHit.collider.ToBaseEntity();
				if (baseEntity != null && baseEntity.HasEntityInParents(this))
				{
					if (raycastHit.point.y > position.y + 0.2f)
					{
						position = raycastHit.point + Vector3.up * 0.05f;
					}
					else
					{
						position.y += 0.2f;
					}
					flag = true;
				}
				else
				{
					result = false;
				}
			}
		}
		while (flag);
		return result;
	}

	// Token: 0x06000318 RID: 792 RVA: 0x0002A6D0 File Offset: 0x000288D0
	public virtual float WaterFactor()
	{
		return WaterLevel.Factor(this.WorldSpaceBounds().ToBounds(), this);
	}

	// Token: 0x06000319 RID: 793 RVA: 0x0002A6F1 File Offset: 0x000288F1
	public virtual float AirFactor()
	{
		if (this.WaterFactor() <= 0.85f)
		{
			return 1f;
		}
		return 0f;
	}

	// Token: 0x0600031A RID: 794 RVA: 0x0002A70C File Offset: 0x0002890C
	public bool WaterTestFromVolumes(Vector3 pos, out WaterLevel.WaterInfo info)
	{
		if (this.triggers == null)
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		for (int i = 0; i < this.triggers.Count; i++)
		{
			WaterVolume waterVolume;
			if ((waterVolume = (this.triggers[i] as WaterVolume)) != null && waterVolume.Test(pos, out info))
			{
				return true;
			}
		}
		info = default(WaterLevel.WaterInfo);
		return false;
	}

	// Token: 0x0600031B RID: 795 RVA: 0x0002A76C File Offset: 0x0002896C
	public bool IsInWaterVolume(Vector3 pos)
	{
		if (this.triggers == null)
		{
			return false;
		}
		WaterLevel.WaterInfo waterInfo = default(WaterLevel.WaterInfo);
		for (int i = 0; i < this.triggers.Count; i++)
		{
			WaterVolume waterVolume;
			if ((waterVolume = (this.triggers[i] as WaterVolume)) != null && waterVolume.Test(pos, out waterInfo))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600031C RID: 796 RVA: 0x0002A7C4 File Offset: 0x000289C4
	public bool WaterTestFromVolumes(Bounds bounds, out WaterLevel.WaterInfo info)
	{
		if (this.triggers == null)
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		for (int i = 0; i < this.triggers.Count; i++)
		{
			WaterVolume waterVolume;
			if ((waterVolume = (this.triggers[i] as WaterVolume)) != null && waterVolume.Test(bounds, out info))
			{
				return true;
			}
		}
		info = default(WaterLevel.WaterInfo);
		return false;
	}

	// Token: 0x0600031D RID: 797 RVA: 0x0002A824 File Offset: 0x00028A24
	public bool WaterTestFromVolumes(Vector3 start, Vector3 end, float radius, out WaterLevel.WaterInfo info)
	{
		if (this.triggers == null)
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		for (int i = 0; i < this.triggers.Count; i++)
		{
			WaterVolume waterVolume;
			if ((waterVolume = (this.triggers[i] as WaterVolume)) != null && waterVolume.Test(start, end, radius, out info))
			{
				return true;
			}
		}
		info = default(WaterLevel.WaterInfo);
		return false;
	}

	// Token: 0x0600031E RID: 798 RVA: 0x00007074 File Offset: 0x00005274
	public virtual bool BlocksWaterFor(global::BasePlayer player)
	{
		return false;
	}

	// Token: 0x0600031F RID: 799 RVA: 0x00026FFC File Offset: 0x000251FC
	public virtual float Health()
	{
		return 0f;
	}

	// Token: 0x06000320 RID: 800 RVA: 0x00026FFC File Offset: 0x000251FC
	public virtual float MaxHealth()
	{
		return 0f;
	}

	// Token: 0x06000321 RID: 801 RVA: 0x00026FFC File Offset: 0x000251FC
	public virtual float MaxVelocity()
	{
		return 0f;
	}

	// Token: 0x06000322 RID: 802 RVA: 0x000292BC File Offset: 0x000274BC
	public virtual float BoundsPadding()
	{
		return 0.1f;
	}

	// Token: 0x06000323 RID: 803 RVA: 0x00027640 File Offset: 0x00025840
	public virtual float PenetrationResistance(HitInfo info)
	{
		return 100f;
	}

	// Token: 0x06000324 RID: 804 RVA: 0x0002A886 File Offset: 0x00028A86
	public virtual GameObjectRef GetImpactEffect(HitInfo info)
	{
		return this.impactEffect;
	}

	// Token: 0x06000325 RID: 805 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnAttacked(HitInfo info)
	{
	}

	// Token: 0x06000326 RID: 806 RVA: 0x0002A0CF File Offset: 0x000282CF
	public virtual global::Item GetItem()
	{
		return null;
	}

	// Token: 0x06000327 RID: 807 RVA: 0x0002A0CF File Offset: 0x000282CF
	public virtual global::Item GetItem(uint itemId)
	{
		return null;
	}

	// Token: 0x06000328 RID: 808 RVA: 0x0002A88E File Offset: 0x00028A8E
	public virtual void GiveItem(global::Item item, global::BaseEntity.GiveItemReason reason = global::BaseEntity.GiveItemReason.Generic)
	{
		item.Remove(0f);
	}

	// Token: 0x06000329 RID: 809 RVA: 0x00003A54 File Offset: 0x00001C54
	public virtual bool CanBeLooted(global::BasePlayer player)
	{
		return true;
	}

	// Token: 0x0600032A RID: 810 RVA: 0x00002E37 File Offset: 0x00001037
	public virtual global::BaseEntity GetEntity()
	{
		return this;
	}

	// Token: 0x0600032B RID: 811 RVA: 0x0002A89C File Offset: 0x00028A9C
	public override string ToString()
	{
		if (this._name == null)
		{
			if (base.isServer)
			{
				this._name = string.Format("{1}[{0}]", (this.net != null) ? this.net.ID : 0U, base.ShortPrefabName);
			}
			else
			{
				this._name = base.ShortPrefabName;
			}
		}
		return this._name;
	}

	// Token: 0x0600032C RID: 812 RVA: 0x0002A8FE File Offset: 0x00028AFE
	public virtual string Categorize()
	{
		return "entity";
	}

	// Token: 0x0600032D RID: 813 RVA: 0x0002A908 File Offset: 0x00028B08
	public void Log(string str)
	{
		if (base.isClient)
		{
			Debug.Log(string.Concat(new string[]
			{
				"<color=#ffa>[",
				this.ToString(),
				"] ",
				str,
				"</color>"
			}), base.gameObject);
			return;
		}
		Debug.Log(string.Concat(new string[]
		{
			"<color=#aff>[",
			this.ToString(),
			"] ",
			str,
			"</color>"
		}), base.gameObject);
	}

	// Token: 0x0600032E RID: 814 RVA: 0x0002A994 File Offset: 0x00028B94
	public void SetModel(Model mdl)
	{
		if (this.model == mdl)
		{
			return;
		}
		this.model = mdl;
	}

	// Token: 0x0600032F RID: 815 RVA: 0x0002A9AC File Offset: 0x00028BAC
	public Model GetModel()
	{
		return this.model;
	}

	// Token: 0x06000330 RID: 816 RVA: 0x0002A9B4 File Offset: 0x00028BB4
	public virtual Transform[] GetBones()
	{
		if (this.model)
		{
			return this.model.GetBones();
		}
		return null;
	}

	// Token: 0x06000331 RID: 817 RVA: 0x0002A9D0 File Offset: 0x00028BD0
	public virtual Transform FindBone(string strName)
	{
		if (this.model)
		{
			return this.model.FindBone(strName);
		}
		return base.transform;
	}

	// Token: 0x06000332 RID: 818 RVA: 0x0002A9F2 File Offset: 0x00028BF2
	public virtual uint FindBoneID(Transform boneTransform)
	{
		if (this.model)
		{
			return this.model.FindBoneID(boneTransform);
		}
		return StringPool.closest;
	}

	// Token: 0x06000333 RID: 819 RVA: 0x0002AA13 File Offset: 0x00028C13
	public virtual Transform FindClosestBone(Vector3 worldPos)
	{
		if (this.model)
		{
			return this.model.FindClosestBone(worldPos);
		}
		return base.transform;
	}

	// Token: 0x17000047 RID: 71
	// (get) Token: 0x06000334 RID: 820 RVA: 0x0002AA35 File Offset: 0x00028C35
	// (set) Token: 0x06000335 RID: 821 RVA: 0x0002AA3D File Offset: 0x00028C3D
	public ulong OwnerID { get; set; }

	// Token: 0x06000336 RID: 822 RVA: 0x00003A54 File Offset: 0x00001C54
	public virtual bool ShouldBlockProjectiles()
	{
		return true;
	}

	// Token: 0x06000337 RID: 823 RVA: 0x00003A54 File Offset: 0x00001C54
	public virtual bool ShouldInheritNetworkGroup()
	{
		return true;
	}

	// Token: 0x06000338 RID: 824 RVA: 0x00007074 File Offset: 0x00005274
	public virtual bool SupportsChildDeployables()
	{
		return false;
	}

	// Token: 0x06000339 RID: 825 RVA: 0x0002AA48 File Offset: 0x00028C48
	public void BroadcastEntityMessage(string msg, float radius = 20f, int layerMask = 1218652417)
	{
		if (base.isClient)
		{
			return;
		}
		List<global::BaseEntity> list = Facepunch.Pool.GetList<global::BaseEntity>();
		global::Vis.Entities<global::BaseEntity>(base.transform.position, radius, list, layerMask, QueryTriggerInteraction.Collide);
		foreach (global::BaseEntity baseEntity in list)
		{
			if (baseEntity.isServer)
			{
				baseEntity.OnEntityMessage(this, msg);
			}
		}
		Facepunch.Pool.FreeList<global::BaseEntity>(ref list);
	}

	// Token: 0x0600033A RID: 826 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnEntityMessage(global::BaseEntity from, string msg)
	{
	}

	// Token: 0x0600033B RID: 827 RVA: 0x0002AACC File Offset: 0x00028CCC
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		global::BaseEntity baseEntity = this.parentEntity.Get(base.isServer);
		info.msg.baseEntity = Facepunch.Pool.Get<ProtoBuf.BaseEntity>();
		if (info.forDisk)
		{
			if (this is global::BasePlayer)
			{
				if (baseEntity == null || baseEntity.enableSaving)
				{
					info.msg.baseEntity.pos = base.transform.localPosition;
					info.msg.baseEntity.rot = base.transform.localRotation.eulerAngles;
				}
				else
				{
					info.msg.baseEntity.pos = base.transform.position;
					info.msg.baseEntity.rot = base.transform.rotation.eulerAngles;
				}
			}
			else
			{
				info.msg.baseEntity.pos = base.transform.localPosition;
				info.msg.baseEntity.rot = base.transform.localRotation.eulerAngles;
			}
		}
		else
		{
			info.msg.baseEntity.pos = this.GetNetworkPosition();
			info.msg.baseEntity.rot = this.GetNetworkRotation().eulerAngles;
			info.msg.baseEntity.time = this.GetNetworkTime();
		}
		info.msg.baseEntity.flags = (int)this.flags;
		info.msg.baseEntity.skinid = this.skinID;
		if (info.forDisk && this is global::BasePlayer)
		{
			if (baseEntity != null && baseEntity.enableSaving)
			{
				info.msg.parent = Facepunch.Pool.Get<ParentInfo>();
				info.msg.parent.uid = this.parentEntity.uid;
				info.msg.parent.bone = this.parentBone;
			}
		}
		else if (baseEntity != null)
		{
			info.msg.parent = Facepunch.Pool.Get<ParentInfo>();
			info.msg.parent.uid = this.parentEntity.uid;
			info.msg.parent.bone = this.parentBone;
		}
		if (this.HasAnySlot())
		{
			info.msg.entitySlots = Facepunch.Pool.Get<EntitySlots>();
			info.msg.entitySlots.slotLock = this.entitySlots[0].uid;
			info.msg.entitySlots.slotFireMod = this.entitySlots[1].uid;
			info.msg.entitySlots.slotUpperModification = this.entitySlots[2].uid;
			info.msg.entitySlots.centerDecoration = this.entitySlots[5].uid;
			info.msg.entitySlots.lowerCenterDecoration = this.entitySlots[6].uid;
			info.msg.entitySlots.storageMonitor = this.entitySlots[7].uid;
		}
		if (info.forDisk && this._spawnable)
		{
			this._spawnable.Save(info);
		}
		if (this.OwnerID != 0UL && (info.forDisk || this.ShouldNetworkOwnerInfo()))
		{
			info.msg.ownerInfo = Facepunch.Pool.Get<OwnerInfo>();
			info.msg.ownerInfo.steamid = this.OwnerID;
		}
		if (this.Components != null)
		{
			for (int i = 0; i < this.Components.Length; i++)
			{
				if (!(this.Components[i] == null))
				{
					this.Components[i].SaveComponent(info);
				}
			}
		}
	}

	// Token: 0x0600033C RID: 828 RVA: 0x00007074 File Offset: 0x00005274
	public virtual bool ShouldNetworkOwnerInfo()
	{
		return false;
	}

	// Token: 0x0600033D RID: 829 RVA: 0x0002AE94 File Offset: 0x00029094
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.baseEntity != null)
		{
			ProtoBuf.BaseEntity baseEntity = info.msg.baseEntity;
			global::BaseEntity.Flags old = this.flags;
			this.flags = (global::BaseEntity.Flags)baseEntity.flags;
			this.OnFlagsChanged(old, this.flags);
			this.OnSkinChanged(this.skinID, info.msg.baseEntity.skinid);
			if (info.fromDisk)
			{
				if (baseEntity.pos.IsNaNOrInfinity())
				{
					Debug.LogWarning(this.ToString() + " has broken position - " + baseEntity.pos);
					baseEntity.pos = Vector3.zero;
				}
				base.transform.localPosition = baseEntity.pos;
				base.transform.localRotation = Quaternion.Euler(baseEntity.rot);
			}
		}
		if (info.msg.entitySlots != null)
		{
			this.entitySlots[0].uid = info.msg.entitySlots.slotLock;
			this.entitySlots[1].uid = info.msg.entitySlots.slotFireMod;
			this.entitySlots[2].uid = info.msg.entitySlots.slotUpperModification;
			this.entitySlots[5].uid = info.msg.entitySlots.centerDecoration;
			this.entitySlots[6].uid = info.msg.entitySlots.lowerCenterDecoration;
			this.entitySlots[7].uid = info.msg.entitySlots.storageMonitor;
		}
		if (info.msg.parent != null)
		{
			if (base.isServer)
			{
				global::BaseEntity entity = global::BaseNetworkable.serverEntities.Find(info.msg.parent.uid) as global::BaseEntity;
				this.SetParent(entity, info.msg.parent.bone, false, false);
			}
			this.parentEntity.uid = info.msg.parent.uid;
			this.parentBone = info.msg.parent.bone;
		}
		else
		{
			this.parentEntity.uid = 0U;
			this.parentBone = 0U;
		}
		if (info.msg.ownerInfo != null)
		{
			this.OwnerID = info.msg.ownerInfo.steamid;
		}
		if (this._spawnable)
		{
			this._spawnable.Load(info);
		}
		if (this.Components != null)
		{
			for (int i = 0; i < this.Components.Length; i++)
			{
				if (!(this.Components[i] == null))
				{
					this.Components[i].LoadComponent(info);
				}
			}
		}
	}

	// Token: 0x04000233 RID: 563
	private static Queue<global::BaseEntity> globalBroadcastQueue = new Queue<global::BaseEntity>();

	// Token: 0x04000234 RID: 564
	private static uint globalBroadcastProtocol = 0U;

	// Token: 0x04000235 RID: 565
	private uint broadcastProtocol;

	// Token: 0x04000236 RID: 566
	private List<EntityLink> links = new List<EntityLink>();

	// Token: 0x04000237 RID: 567
	private bool linkedToNeighbours;

	// Token: 0x04000238 RID: 568
	[NonSerialized]
	public global::BaseEntity creatorEntity;

	// Token: 0x04000239 RID: 569
	private int ticksSinceStopped;

	// Token: 0x0400023A RID: 570
	private int doneMovingWithoutARigidBodyCheck = 1;

	// Token: 0x0400023B RID: 571
	private bool isCallingUpdateNetworkGroup;

	// Token: 0x0400023C RID: 572
	private EntityRef[] entitySlots = new EntityRef[8];

	// Token: 0x0400023D RID: 573
	protected List<TriggerBase> triggers;

	// Token: 0x0400023E RID: 574
	protected bool isVisible = true;

	// Token: 0x0400023F RID: 575
	protected bool isAnimatorVisible = true;

	// Token: 0x04000240 RID: 576
	protected bool isShadowVisible = true;

	// Token: 0x04000241 RID: 577
	protected OccludeeSphere localOccludee = new OccludeeSphere(-1);

	// Token: 0x04000243 RID: 579
	[Header("BaseEntity")]
	public Bounds bounds;

	// Token: 0x04000244 RID: 580
	public GameObjectRef impactEffect;

	// Token: 0x04000245 RID: 581
	public bool enableSaving = true;

	// Token: 0x04000246 RID: 582
	public bool syncPosition;

	// Token: 0x04000247 RID: 583
	public Model model;

	// Token: 0x04000248 RID: 584
	[global::InspectorFlags]
	public global::BaseEntity.Flags flags;

	// Token: 0x04000249 RID: 585
	[NonSerialized]
	public uint parentBone;

	// Token: 0x0400024A RID: 586
	[NonSerialized]
	public ulong skinID;

	// Token: 0x0400024B RID: 587
	private EntityComponentBase[] _components;

	// Token: 0x0400024C RID: 588
	[HideInInspector]
	public bool HasBrain;

	// Token: 0x0400024D RID: 589
	[NonSerialized]
	protected string _name;

	// Token: 0x0400024F RID: 591
	private global::Spawnable _spawnable;

	// Token: 0x04000250 RID: 592
	public static HashSet<global::BaseEntity> saveList = new HashSet<global::BaseEntity>();

	// Token: 0x02000B32 RID: 2866
	public class Menu : Attribute
	{
		// Token: 0x06004A3D RID: 19005 RVA: 0x00002809 File Offset: 0x00000A09
		public Menu()
		{
		}

		// Token: 0x06004A3E RID: 19006 RVA: 0x0018F8B0 File Offset: 0x0018DAB0
		public Menu(string menuTitleToken, string menuTitleEnglish)
		{
			this.TitleToken = menuTitleToken;
			this.TitleEnglish = menuTitleEnglish;
		}

		// Token: 0x04003CD7 RID: 15575
		public string TitleToken;

		// Token: 0x04003CD8 RID: 15576
		public string TitleEnglish;

		// Token: 0x04003CD9 RID: 15577
		public string UseVariable;

		// Token: 0x04003CDA RID: 15578
		public int Order;

		// Token: 0x04003CDB RID: 15579
		public string ProxyFunction;

		// Token: 0x04003CDC RID: 15580
		public float Time;

		// Token: 0x04003CDD RID: 15581
		public string OnStart;

		// Token: 0x04003CDE RID: 15582
		public string OnProgress;

		// Token: 0x04003CDF RID: 15583
		public bool LongUseOnly;

		// Token: 0x04003CE0 RID: 15584
		public bool PrioritizeIfNotWhitelisted;

		// Token: 0x04003CE1 RID: 15585
		public bool PrioritizeIfUnlocked;

		// Token: 0x02000F49 RID: 3913
		[Serializable]
		public struct Option
		{
			// Token: 0x04004DD3 RID: 19923
			public Translate.Phrase name;

			// Token: 0x04004DD4 RID: 19924
			public Translate.Phrase description;

			// Token: 0x04004DD5 RID: 19925
			public Sprite icon;

			// Token: 0x04004DD6 RID: 19926
			public int order;

			// Token: 0x04004DD7 RID: 19927
			public bool usableWhileWounded;
		}

		// Token: 0x02000F4A RID: 3914
		public class Description : Attribute
		{
			// Token: 0x0600522D RID: 21037 RVA: 0x001A6A28 File Offset: 0x001A4C28
			public Description(string t, string e)
			{
				this.token = t;
				this.english = e;
			}

			// Token: 0x04004DD8 RID: 19928
			public string token;

			// Token: 0x04004DD9 RID: 19929
			public string english;
		}

		// Token: 0x02000F4B RID: 3915
		public class Icon : Attribute
		{
			// Token: 0x0600522E RID: 21038 RVA: 0x001A6A3E File Offset: 0x001A4C3E
			public Icon(string i)
			{
				this.icon = i;
			}

			// Token: 0x04004DDA RID: 19930
			public string icon;
		}

		// Token: 0x02000F4C RID: 3916
		public class ShowIf : Attribute
		{
			// Token: 0x0600522F RID: 21039 RVA: 0x001A6A4D File Offset: 0x001A4C4D
			public ShowIf(string testFunc)
			{
				this.functionName = testFunc;
			}

			// Token: 0x04004DDB RID: 19931
			public string functionName;
		}

		// Token: 0x02000F4D RID: 3917
		public class Priority : Attribute
		{
			// Token: 0x06005230 RID: 21040 RVA: 0x001A6A5C File Offset: 0x001A4C5C
			public Priority(string priorityFunc)
			{
				this.functionName = priorityFunc;
			}

			// Token: 0x04004DDC RID: 19932
			public string functionName;
		}

		// Token: 0x02000F4E RID: 3918
		public class UsableWhileWounded : Attribute
		{
		}
	}

	// Token: 0x02000B33 RID: 2867
	[Serializable]
	public struct MovementModify
	{
		// Token: 0x04003CE2 RID: 15586
		public float drag;
	}

	// Token: 0x02000B34 RID: 2868
	[Flags]
	public enum Flags
	{
		// Token: 0x04003CE4 RID: 15588
		Placeholder = 1,
		// Token: 0x04003CE5 RID: 15589
		On = 2,
		// Token: 0x04003CE6 RID: 15590
		OnFire = 4,
		// Token: 0x04003CE7 RID: 15591
		Open = 8,
		// Token: 0x04003CE8 RID: 15592
		Locked = 16,
		// Token: 0x04003CE9 RID: 15593
		Debugging = 32,
		// Token: 0x04003CEA RID: 15594
		Disabled = 64,
		// Token: 0x04003CEB RID: 15595
		Reserved1 = 128,
		// Token: 0x04003CEC RID: 15596
		Reserved2 = 256,
		// Token: 0x04003CED RID: 15597
		Reserved3 = 512,
		// Token: 0x04003CEE RID: 15598
		Reserved4 = 1024,
		// Token: 0x04003CEF RID: 15599
		Reserved5 = 2048,
		// Token: 0x04003CF0 RID: 15600
		Broken = 4096,
		// Token: 0x04003CF1 RID: 15601
		Busy = 8192,
		// Token: 0x04003CF2 RID: 15602
		Reserved6 = 16384,
		// Token: 0x04003CF3 RID: 15603
		Reserved7 = 32768,
		// Token: 0x04003CF4 RID: 15604
		Reserved8 = 65536,
		// Token: 0x04003CF5 RID: 15605
		Reserved9 = 131072,
		// Token: 0x04003CF6 RID: 15606
		Reserved10 = 262144,
		// Token: 0x04003CF7 RID: 15607
		Reserved11 = 524288,
		// Token: 0x04003CF8 RID: 15608
		InUse = 1048576
	}

	// Token: 0x02000B35 RID: 2869
	private readonly struct QueuedFileRequest : IEquatable<global::BaseEntity.QueuedFileRequest>
	{
		// Token: 0x06004A3F RID: 19007 RVA: 0x0018F8C6 File Offset: 0x0018DAC6
		public QueuedFileRequest(global::BaseEntity entity, FileStorage.Type type, uint part, uint crc, uint responseFunction, bool? respondIfNotFound)
		{
			this.Entity = entity;
			this.Type = type;
			this.Part = part;
			this.Crc = crc;
			this.ResponseFunction = responseFunction;
			this.RespondIfNotFound = respondIfNotFound;
		}

		// Token: 0x06004A40 RID: 19008 RVA: 0x0018F8F8 File Offset: 0x0018DAF8
		public bool Equals(global::BaseEntity.QueuedFileRequest other)
		{
			if (object.Equals(this.Entity, other.Entity) && this.Type == other.Type && this.Part == other.Part && this.Crc == other.Crc && this.ResponseFunction == other.ResponseFunction)
			{
				bool? respondIfNotFound = this.RespondIfNotFound;
				bool? respondIfNotFound2 = other.RespondIfNotFound;
				return respondIfNotFound.GetValueOrDefault() == respondIfNotFound2.GetValueOrDefault() & respondIfNotFound != null == (respondIfNotFound2 != null);
			}
			return false;
		}

		// Token: 0x06004A41 RID: 19009 RVA: 0x0018F984 File Offset: 0x0018DB84
		public override bool Equals(object obj)
		{
			if (obj is global::BaseEntity.QueuedFileRequest)
			{
				global::BaseEntity.QueuedFileRequest other = (global::BaseEntity.QueuedFileRequest)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x06004A42 RID: 19010 RVA: 0x0018F9AC File Offset: 0x0018DBAC
		public override int GetHashCode()
		{
			return ((((((this.Entity != null) ? this.Entity.GetHashCode() : 0) * 397 ^ (int)this.Type) * 397 ^ (int)this.Part) * 397 ^ (int)this.Crc) * 397 ^ (int)this.ResponseFunction) * 397 ^ this.RespondIfNotFound.GetHashCode();
		}

		// Token: 0x04003CF9 RID: 15609
		public readonly global::BaseEntity Entity;

		// Token: 0x04003CFA RID: 15610
		public readonly FileStorage.Type Type;

		// Token: 0x04003CFB RID: 15611
		public readonly uint Part;

		// Token: 0x04003CFC RID: 15612
		public readonly uint Crc;

		// Token: 0x04003CFD RID: 15613
		public readonly uint ResponseFunction;

		// Token: 0x04003CFE RID: 15614
		public readonly bool? RespondIfNotFound;
	}

	// Token: 0x02000B36 RID: 2870
	private readonly struct PendingFileRequest : IEquatable<global::BaseEntity.PendingFileRequest>
	{
		// Token: 0x06004A43 RID: 19011 RVA: 0x0018FA24 File Offset: 0x0018DC24
		public PendingFileRequest(FileStorage.Type type, uint numId, uint crc, IServerFileReceiver receiver)
		{
			this.Type = type;
			this.NumId = numId;
			this.Crc = crc;
			this.Receiver = receiver;
			this.Time = UnityEngine.Time.realtimeSinceStartup;
		}

		// Token: 0x06004A44 RID: 19012 RVA: 0x0018FA4E File Offset: 0x0018DC4E
		public bool Equals(global::BaseEntity.PendingFileRequest other)
		{
			return this.Type == other.Type && this.NumId == other.NumId && this.Crc == other.Crc && object.Equals(this.Receiver, other.Receiver);
		}

		// Token: 0x06004A45 RID: 19013 RVA: 0x0018FA90 File Offset: 0x0018DC90
		public override bool Equals(object obj)
		{
			if (obj is global::BaseEntity.PendingFileRequest)
			{
				global::BaseEntity.PendingFileRequest other = (global::BaseEntity.PendingFileRequest)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x06004A46 RID: 19014 RVA: 0x0018FAB7 File Offset: 0x0018DCB7
		public override int GetHashCode()
		{
			return (int)(((this.Type * (FileStorage.Type)397 ^ (FileStorage.Type)this.NumId) * (FileStorage.Type)397 ^ (FileStorage.Type)this.Crc) * (FileStorage.Type)397 ^ (FileStorage.Type)((this.Receiver != null) ? this.Receiver.GetHashCode() : 0));
		}

		// Token: 0x04003CFF RID: 15615
		public readonly FileStorage.Type Type;

		// Token: 0x04003D00 RID: 15616
		public readonly uint NumId;

		// Token: 0x04003D01 RID: 15617
		public readonly uint Crc;

		// Token: 0x04003D02 RID: 15618
		public readonly IServerFileReceiver Receiver;

		// Token: 0x04003D03 RID: 15619
		public readonly float Time;
	}

	// Token: 0x02000B37 RID: 2871
	public static class Query
	{
		// Token: 0x04003D04 RID: 15620
		public static global::BaseEntity.Query.EntityTree Server;

		// Token: 0x02000F4F RID: 3919
		public class EntityTree
		{
			// Token: 0x06005232 RID: 21042 RVA: 0x001A6A6B File Offset: 0x001A4C6B
			public EntityTree(float worldSize)
			{
				this.Grid = new Grid<global::BaseEntity>(32, worldSize);
				this.PlayerGrid = new Grid<global::BasePlayer>(32, worldSize);
				this.BrainGrid = new Grid<global::BaseEntity>(32, worldSize);
			}

			// Token: 0x06005233 RID: 21043 RVA: 0x001A6AA0 File Offset: 0x001A4CA0
			public void Add(global::BaseEntity ent)
			{
				Vector3 position = ent.transform.position;
				this.Grid.Add(ent, position.x, position.z);
			}

			// Token: 0x06005234 RID: 21044 RVA: 0x001A6AD4 File Offset: 0x001A4CD4
			public void AddPlayer(global::BasePlayer player)
			{
				Vector3 position = player.transform.position;
				this.PlayerGrid.Add(player, position.x, position.z);
			}

			// Token: 0x06005235 RID: 21045 RVA: 0x001A6B08 File Offset: 0x001A4D08
			public void AddBrain(global::BaseEntity entity)
			{
				Vector3 position = entity.transform.position;
				this.BrainGrid.Add(entity, position.x, position.z);
			}

			// Token: 0x06005236 RID: 21046 RVA: 0x001A6B3C File Offset: 0x001A4D3C
			public void Remove(global::BaseEntity ent, bool isPlayer = false)
			{
				this.Grid.Remove(ent);
				if (isPlayer)
				{
					global::BasePlayer basePlayer = ent as global::BasePlayer;
					if (basePlayer != null)
					{
						this.PlayerGrid.Remove(basePlayer);
					}
				}
			}

			// Token: 0x06005237 RID: 21047 RVA: 0x001A6B76 File Offset: 0x001A4D76
			public void RemovePlayer(global::BasePlayer player)
			{
				this.PlayerGrid.Remove(player);
			}

			// Token: 0x06005238 RID: 21048 RVA: 0x001A6B85 File Offset: 0x001A4D85
			public void RemoveBrain(global::BaseEntity entity)
			{
				if (entity == null)
				{
					return;
				}
				this.BrainGrid.Remove(entity);
			}

			// Token: 0x06005239 RID: 21049 RVA: 0x001A6BA0 File Offset: 0x001A4DA0
			public void Move(global::BaseEntity ent)
			{
				Vector3 position = ent.transform.position;
				this.Grid.Move(ent, position.x, position.z);
				global::BasePlayer basePlayer = ent as global::BasePlayer;
				if (basePlayer != null)
				{
					this.MovePlayer(basePlayer);
				}
				if (ent.HasBrain)
				{
					this.MoveBrain(ent);
				}
			}

			// Token: 0x0600523A RID: 21050 RVA: 0x001A6BF8 File Offset: 0x001A4DF8
			public void MovePlayer(global::BasePlayer player)
			{
				Vector3 position = player.transform.position;
				this.PlayerGrid.Move(player, position.x, position.z);
			}

			// Token: 0x0600523B RID: 21051 RVA: 0x001A6C2C File Offset: 0x001A4E2C
			public void MoveBrain(global::BaseEntity entity)
			{
				Vector3 position = entity.transform.position;
				this.BrainGrid.Move(entity, position.x, position.z);
			}

			// Token: 0x0600523C RID: 21052 RVA: 0x001A6C5D File Offset: 0x001A4E5D
			public int GetInSphere(Vector3 position, float distance, global::BaseEntity[] results, Func<global::BaseEntity, bool> filter = null)
			{
				return this.Grid.Query(position.x, position.z, distance, results, filter);
			}

			// Token: 0x0600523D RID: 21053 RVA: 0x001A6C7A File Offset: 0x001A4E7A
			public int GetPlayersInSphere(Vector3 position, float distance, global::BasePlayer[] results, Func<global::BasePlayer, bool> filter = null)
			{
				return this.PlayerGrid.Query(position.x, position.z, distance, results, filter);
			}

			// Token: 0x0600523E RID: 21054 RVA: 0x001A6C97 File Offset: 0x001A4E97
			public int GetBrainsInSphere(Vector3 position, float distance, global::BaseEntity[] results, Func<global::BaseEntity, bool> filter = null)
			{
				return this.BrainGrid.Query(position.x, position.z, distance, results, filter);
			}

			// Token: 0x04004DDD RID: 19933
			private Grid<global::BaseEntity> Grid;

			// Token: 0x04004DDE RID: 19934
			private Grid<global::BasePlayer> PlayerGrid;

			// Token: 0x04004DDF RID: 19935
			private Grid<global::BaseEntity> BrainGrid;
		}
	}

	// Token: 0x02000B38 RID: 2872
	public class RPC_Shared : Attribute
	{
	}

	// Token: 0x02000B39 RID: 2873
	public struct RPCMessage
	{
		// Token: 0x04003D05 RID: 15621
		public Connection connection;

		// Token: 0x04003D06 RID: 15622
		public global::BasePlayer player;

		// Token: 0x04003D07 RID: 15623
		public NetRead read;
	}

	// Token: 0x02000B3A RID: 2874
	public class RPC_Server : global::BaseEntity.RPC_Shared
	{
		// Token: 0x02000F50 RID: 3920
		public abstract class Conditional : Attribute
		{
			// Token: 0x0600523F RID: 21055 RVA: 0x0002A0CF File Offset: 0x000282CF
			public virtual string GetArgs()
			{
				return null;
			}
		}

		// Token: 0x02000F51 RID: 3921
		public class MaxDistance : global::BaseEntity.RPC_Server.Conditional
		{
			// Token: 0x06005241 RID: 21057 RVA: 0x001A6CB4 File Offset: 0x001A4EB4
			public MaxDistance(float maxDist)
			{
				this.maximumDistance = maxDist;
			}

			// Token: 0x06005242 RID: 21058 RVA: 0x001A6CC3 File Offset: 0x001A4EC3
			public override string GetArgs()
			{
				return this.maximumDistance.ToString("0.00f");
			}

			// Token: 0x06005243 RID: 21059 RVA: 0x001A6CD5 File Offset: 0x001A4ED5
			public static bool Test(string debugName, global::BaseEntity ent, global::BasePlayer player, float maximumDistance)
			{
				return global::BaseEntity.RPC_Server.MaxDistance.Test(0U, debugName, ent, player, maximumDistance);
			}

			// Token: 0x06005244 RID: 21060 RVA: 0x001A6CE1 File Offset: 0x001A4EE1
			public static bool Test(uint id, string debugName, global::BaseEntity ent, global::BasePlayer player, float maximumDistance)
			{
				return !(ent == null) && !(player == null) && ent.Distance(player.eyes.position) <= maximumDistance;
			}

			// Token: 0x04004DE0 RID: 19936
			private float maximumDistance;
		}

		// Token: 0x02000F52 RID: 3922
		public class IsVisible : global::BaseEntity.RPC_Server.Conditional
		{
			// Token: 0x06005245 RID: 21061 RVA: 0x001A6D0F File Offset: 0x001A4F0F
			public IsVisible(float maxDist)
			{
				this.maximumDistance = maxDist;
			}

			// Token: 0x06005246 RID: 21062 RVA: 0x001A6D1E File Offset: 0x001A4F1E
			public override string GetArgs()
			{
				return this.maximumDistance.ToString("0.00f");
			}

			// Token: 0x06005247 RID: 21063 RVA: 0x001A6D30 File Offset: 0x001A4F30
			public static bool Test(string debugName, global::BaseEntity ent, global::BasePlayer player, float maximumDistance)
			{
				return global::BaseEntity.RPC_Server.IsVisible.Test(0U, debugName, ent, player, maximumDistance);
			}

			// Token: 0x06005248 RID: 21064 RVA: 0x001A6D3C File Offset: 0x001A4F3C
			public static bool Test(uint id, string debugName, global::BaseEntity ent, global::BasePlayer player, float maximumDistance)
			{
				return !(ent == null) && !(player == null) && GamePhysics.LineOfSight(player.eyes.center, player.eyes.position, 2162688, null) && (ent.IsVisible(player.eyes.HeadRay(), 1218519041, maximumDistance) || ent.IsVisible(player.eyes.position, maximumDistance));
			}

			// Token: 0x04004DE1 RID: 19937
			private float maximumDistance;
		}

		// Token: 0x02000F53 RID: 3923
		public class FromOwner : global::BaseEntity.RPC_Server.Conditional
		{
			// Token: 0x06005249 RID: 21065 RVA: 0x001A6DB1 File Offset: 0x001A4FB1
			public static bool Test(string debugName, global::BaseEntity ent, global::BasePlayer player)
			{
				return global::BaseEntity.RPC_Server.FromOwner.Test(0U, debugName, ent, player);
			}

			// Token: 0x0600524A RID: 21066 RVA: 0x001A6DBC File Offset: 0x001A4FBC
			public static bool Test(uint id, string debugName, global::BaseEntity ent, global::BasePlayer player)
			{
				return !(ent == null) && !(player == null) && ent.net != null && player.net != null && (ent.net.ID == player.net.ID || ent.parentEntity.uid == player.net.ID);
			}
		}

		// Token: 0x02000F54 RID: 3924
		public class IsActiveItem : global::BaseEntity.RPC_Server.Conditional
		{
			// Token: 0x0600524C RID: 21068 RVA: 0x001A6E2C File Offset: 0x001A502C
			public static bool Test(string debugName, global::BaseEntity ent, global::BasePlayer player)
			{
				return global::BaseEntity.RPC_Server.IsActiveItem.Test(0U, debugName, ent, player);
			}

			// Token: 0x0600524D RID: 21069 RVA: 0x001A6E38 File Offset: 0x001A5038
			public static bool Test(uint id, string debugName, global::BaseEntity ent, global::BasePlayer player)
			{
				if (ent == null || player == null)
				{
					return false;
				}
				if (ent.net == null || player.net == null)
				{
					return false;
				}
				if (ent.net.ID == player.net.ID)
				{
					return true;
				}
				if (ent.parentEntity.uid != player.net.ID)
				{
					return false;
				}
				global::Item activeItem = player.GetActiveItem();
				return activeItem != null && !(activeItem.GetHeldEntity() != ent);
			}
		}

		// Token: 0x02000F55 RID: 3925
		public class CallsPerSecond : global::BaseEntity.RPC_Server.Conditional
		{
			// Token: 0x0600524F RID: 21071 RVA: 0x001A6EBC File Offset: 0x001A50BC
			public CallsPerSecond(ulong limit)
			{
				this.callsPerSecond = limit;
			}

			// Token: 0x06005250 RID: 21072 RVA: 0x001A6ECB File Offset: 0x001A50CB
			public override string GetArgs()
			{
				return this.callsPerSecond.ToString();
			}

			// Token: 0x06005251 RID: 21073 RVA: 0x001A6ED8 File Offset: 0x001A50D8
			public static bool Test(uint id, string debugName, global::BaseEntity ent, global::BasePlayer player, ulong callsPerSecond)
			{
				return !(ent == null) && !(player == null) && player.rpcHistory.TryIncrement(id, callsPerSecond);
			}

			// Token: 0x04004DE2 RID: 19938
			private ulong callsPerSecond;
		}
	}

	// Token: 0x02000B3B RID: 2875
	public enum Signal
	{
		// Token: 0x04003D09 RID: 15625
		Attack,
		// Token: 0x04003D0A RID: 15626
		Alt_Attack,
		// Token: 0x04003D0B RID: 15627
		DryFire,
		// Token: 0x04003D0C RID: 15628
		Reload,
		// Token: 0x04003D0D RID: 15629
		Deploy,
		// Token: 0x04003D0E RID: 15630
		Flinch_Head,
		// Token: 0x04003D0F RID: 15631
		Flinch_Chest,
		// Token: 0x04003D10 RID: 15632
		Flinch_Stomach,
		// Token: 0x04003D11 RID: 15633
		Flinch_RearHead,
		// Token: 0x04003D12 RID: 15634
		Flinch_RearTorso,
		// Token: 0x04003D13 RID: 15635
		Throw,
		// Token: 0x04003D14 RID: 15636
		Relax,
		// Token: 0x04003D15 RID: 15637
		Gesture,
		// Token: 0x04003D16 RID: 15638
		PhysImpact,
		// Token: 0x04003D17 RID: 15639
		Eat,
		// Token: 0x04003D18 RID: 15640
		Startled,
		// Token: 0x04003D19 RID: 15641
		Admire
	}

	// Token: 0x02000B3C RID: 2876
	public enum Slot
	{
		// Token: 0x04003D1B RID: 15643
		Lock,
		// Token: 0x04003D1C RID: 15644
		FireMod,
		// Token: 0x04003D1D RID: 15645
		UpperModifier,
		// Token: 0x04003D1E RID: 15646
		MiddleModifier,
		// Token: 0x04003D1F RID: 15647
		LowerModifier,
		// Token: 0x04003D20 RID: 15648
		CenterDecoration,
		// Token: 0x04003D21 RID: 15649
		LowerCenterDecoration,
		// Token: 0x04003D22 RID: 15650
		StorageMonitor,
		// Token: 0x04003D23 RID: 15651
		Count
	}

	// Token: 0x02000B3D RID: 2877
	[Flags]
	public enum TraitFlag
	{
		// Token: 0x04003D25 RID: 15653
		None = 0,
		// Token: 0x04003D26 RID: 15654
		Alive = 1,
		// Token: 0x04003D27 RID: 15655
		Animal = 2,
		// Token: 0x04003D28 RID: 15656
		Human = 4,
		// Token: 0x04003D29 RID: 15657
		Interesting = 8,
		// Token: 0x04003D2A RID: 15658
		Food = 16,
		// Token: 0x04003D2B RID: 15659
		Meat = 32,
		// Token: 0x04003D2C RID: 15660
		Water = 32
	}

	// Token: 0x02000B3E RID: 2878
	public static class Util
	{
		// Token: 0x06004A49 RID: 19017 RVA: 0x0018FB00 File Offset: 0x0018DD00
		public static global::BaseEntity[] FindTargets(string strFilter, bool onlyPlayers)
		{
			return (from x in global::BaseNetworkable.serverEntities.Where(delegate(global::BaseNetworkable x)
			{
				if (x is global::BasePlayer)
				{
					global::BasePlayer basePlayer = x as global::BasePlayer;
					return string.IsNullOrEmpty(strFilter) || (strFilter == "!alive" && basePlayer.IsAlive()) || (strFilter == "!sleeping" && basePlayer.IsSleeping()) || strFilter[0] == '!' || basePlayer.displayName.Contains(strFilter, CompareOptions.IgnoreCase) || basePlayer.UserIDString.Contains(strFilter);
				}
				return !onlyPlayers && !string.IsNullOrEmpty(strFilter) && x.ShortPrefabName.Contains(strFilter);
			})
			select x as global::BaseEntity).ToArray<global::BaseEntity>();
		}

		// Token: 0x06004A4A RID: 19018 RVA: 0x0018FB60 File Offset: 0x0018DD60
		public static global::BaseEntity[] FindTargetsOwnedBy(ulong ownedBy, string strFilter)
		{
			bool hasFilter = !string.IsNullOrEmpty(strFilter);
			return (from x in global::BaseNetworkable.serverEntities.Where(delegate(global::BaseNetworkable x)
			{
				global::BaseEntity baseEntity;
				if ((baseEntity = (x as global::BaseEntity)) != null)
				{
					if (baseEntity.OwnerID != ownedBy)
					{
						return false;
					}
					if (!hasFilter || baseEntity.ShortPrefabName.Contains(strFilter))
					{
						return true;
					}
				}
				return false;
			})
			select x as global::BaseEntity).ToArray<global::BaseEntity>();
		}

		// Token: 0x06004A4B RID: 19019 RVA: 0x0018FBD4 File Offset: 0x0018DDD4
		public static global::BaseEntity[] FindTargetsAuthedTo(ulong authId, string strFilter)
		{
			bool hasFilter = !string.IsNullOrEmpty(strFilter);
			return (from x in global::BaseNetworkable.serverEntities.Where(delegate(global::BaseNetworkable x)
			{
				BuildingPrivlidge buildingPrivlidge;
				global::AutoTurret autoTurret;
				global::CodeLock codeLock;
				if ((buildingPrivlidge = (x as BuildingPrivlidge)) != null)
				{
					if (!buildingPrivlidge.IsAuthed(authId))
					{
						return false;
					}
					if (!hasFilter || x.ShortPrefabName.Contains(strFilter))
					{
						return true;
					}
				}
				else if ((autoTurret = (x as global::AutoTurret)) != null)
				{
					if (!autoTurret.IsAuthed(authId))
					{
						return false;
					}
					if (!hasFilter || x.ShortPrefabName.Contains(strFilter))
					{
						return true;
					}
				}
				else if ((codeLock = (x as global::CodeLock)) != null)
				{
					if (!codeLock.whitelistPlayers.Contains(authId))
					{
						return false;
					}
					if (!hasFilter || x.ShortPrefabName.Contains(strFilter))
					{
						return true;
					}
				}
				return false;
			})
			select x as global::BaseEntity).ToArray<global::BaseEntity>();
		}

		// Token: 0x06004A4C RID: 19020 RVA: 0x0018FC48 File Offset: 0x0018DE48
		public static T[] FindAll<T>() where T : global::BaseEntity
		{
			return (from x in global::BaseNetworkable.serverEntities
			where x is T
			select x as T).ToArray<T>();
		}
	}

	// Token: 0x02000B3F RID: 2879
	public enum GiveItemReason
	{
		// Token: 0x04003D2E RID: 15662
		Generic,
		// Token: 0x04003D2F RID: 15663
		ResourceHarvested,
		// Token: 0x04003D30 RID: 15664
		PickedUp,
		// Token: 0x04003D31 RID: 15665
		Crafted
	}
}
