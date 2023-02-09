using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ConVar;
using Facepunch;
using Network;
using Network.Visibility;
using ProtoBuf;
using Rust;
using Rust.Registry;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000399 RID: 921
public abstract class BaseNetworkable : BaseMonoBehaviour, IPrefabPostProcess, IEntity, NetworkHandler
{
	// Token: 0x06001FDE RID: 8158 RVA: 0x000D19B4 File Offset: 0x000CFBB4
	public void BroadcastOnPostNetworkUpdate(global::BaseEntity entity)
	{
		foreach (Component component in this.postNetworkUpdateComponents)
		{
			IOnPostNetworkUpdate onPostNetworkUpdate = component as IOnPostNetworkUpdate;
			if (onPostNetworkUpdate != null)
			{
				onPostNetworkUpdate.OnPostNetworkUpdate(entity);
			}
		}
		foreach (global::BaseEntity baseEntity in this.children)
		{
			baseEntity.BroadcastOnPostNetworkUpdate(entity);
		}
	}

	// Token: 0x06001FDF RID: 8159 RVA: 0x000D1A50 File Offset: 0x000CFC50
	public virtual void PostProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (serverside)
		{
			return;
		}
		this.postNetworkUpdateComponents = base.GetComponentsInChildren<IOnPostNetworkUpdate>(true).Cast<Component>().ToList<Component>();
	}

	// Token: 0x17000270 RID: 624
	// (get) Token: 0x06001FE0 RID: 8160 RVA: 0x000D1A6E File Offset: 0x000CFC6E
	// (set) Token: 0x06001FE1 RID: 8161 RVA: 0x000D1A76 File Offset: 0x000CFC76
	public bool limitNetworking
	{
		get
		{
			return this._limitedNetworking;
		}
		set
		{
			if (value == this._limitedNetworking)
			{
				return;
			}
			this._limitedNetworking = value;
			if (this._limitedNetworking)
			{
				this.OnNetworkLimitStart();
			}
			else
			{
				this.OnNetworkLimitEnd();
			}
			this.UpdateNetworkGroup();
		}
	}

	// Token: 0x06001FE2 RID: 8162 RVA: 0x000D1AA8 File Offset: 0x000CFCA8
	private void OnNetworkLimitStart()
	{
		base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "OnNetworkLimitStart");
		List<Connection> list = this.GetSubscribers();
		if (list == null)
		{
			return;
		}
		list = list.ToList<Connection>();
		list.RemoveAll((Connection x) => this.ShouldNetworkTo(x.player as global::BasePlayer));
		this.OnNetworkSubscribersLeave(list);
		if (this.children != null)
		{
			foreach (global::BaseEntity baseEntity in this.children)
			{
				baseEntity.OnNetworkLimitStart();
			}
		}
	}

	// Token: 0x06001FE3 RID: 8163 RVA: 0x000D1B3C File Offset: 0x000CFD3C
	private void OnNetworkLimitEnd()
	{
		base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "OnNetworkLimitEnd");
		List<Connection> subscribers = this.GetSubscribers();
		if (subscribers == null)
		{
			return;
		}
		this.OnNetworkSubscribersEnter(subscribers);
		if (this.children != null)
		{
			foreach (global::BaseEntity baseEntity in this.children)
			{
				baseEntity.OnNetworkLimitEnd();
			}
		}
	}

	// Token: 0x06001FE4 RID: 8164 RVA: 0x000D1BB4 File Offset: 0x000CFDB4
	public global::BaseEntity GetParentEntity()
	{
		return this.parentEntity.Get(this.isServer);
	}

	// Token: 0x06001FE5 RID: 8165 RVA: 0x000D1BC7 File Offset: 0x000CFDC7
	public bool HasParent()
	{
		return this.parentEntity.IsValid(this.isServer);
	}

	// Token: 0x06001FE6 RID: 8166 RVA: 0x000D1BDA File Offset: 0x000CFDDA
	public void AddChild(global::BaseEntity child)
	{
		if (this.children.Contains(child))
		{
			return;
		}
		this.children.Add(child);
		this.OnChildAdded(child);
	}

	// Token: 0x06001FE7 RID: 8167 RVA: 0x000059DD File Offset: 0x00003BDD
	protected virtual void OnChildAdded(global::BaseEntity child)
	{
	}

	// Token: 0x06001FE8 RID: 8168 RVA: 0x000D1BFE File Offset: 0x000CFDFE
	public void RemoveChild(global::BaseEntity child)
	{
		this.children.Remove(child);
		this.OnChildRemoved(child);
	}

	// Token: 0x06001FE9 RID: 8169 RVA: 0x000059DD File Offset: 0x00003BDD
	protected virtual void OnChildRemoved(global::BaseEntity child)
	{
	}

	// Token: 0x17000271 RID: 625
	// (get) Token: 0x06001FEA RID: 8170 RVA: 0x000D1C14 File Offset: 0x000CFE14
	public GameManager gameManager
	{
		get
		{
			if (this.isServer)
			{
				return GameManager.server;
			}
			throw new NotImplementedException("Missing gameManager path");
		}
	}

	// Token: 0x17000272 RID: 626
	// (get) Token: 0x06001FEB RID: 8171 RVA: 0x000D1C2E File Offset: 0x000CFE2E
	public PrefabAttribute.Library prefabAttribute
	{
		get
		{
			if (this.isServer)
			{
				return PrefabAttribute.server;
			}
			throw new NotImplementedException("Missing prefabAttribute path");
		}
	}

	// Token: 0x17000273 RID: 627
	// (get) Token: 0x06001FEC RID: 8172 RVA: 0x000D1C48 File Offset: 0x000CFE48
	public static Group GlobalNetworkGroup
	{
		get
		{
			return Network.Net.sv.visibility.Get(0U);
		}
	}

	// Token: 0x17000274 RID: 628
	// (get) Token: 0x06001FED RID: 8173 RVA: 0x000D1C5A File Offset: 0x000CFE5A
	public static Group LimboNetworkGroup
	{
		get
		{
			return Network.Net.sv.visibility.Get(1U);
		}
	}

	// Token: 0x06001FEE RID: 8174 RVA: 0x000C40E3 File Offset: 0x000C22E3
	public virtual float GetNetworkTime()
	{
		return UnityEngine.Time.time;
	}

	// Token: 0x06001FEF RID: 8175 RVA: 0x000D1C6C File Offset: 0x000CFE6C
	public virtual void Spawn()
	{
		this.SpawnShared();
		if (this.net == null)
		{
			this.net = Network.Net.sv.CreateNetworkable();
		}
		this.creationFrame = UnityEngine.Time.frameCount;
		this.PreInitShared();
		this.InitShared();
		this.ServerInit();
		this.PostInitShared();
		this.UpdateNetworkGroup();
		this.isSpawned = true;
		this.SendNetworkUpdateImmediate(true);
		if (Rust.Application.isLoading && !Rust.Application.isLoadingSave)
		{
			base.gameObject.SendOnSendNetworkUpdate(this as global::BaseEntity);
		}
	}

	// Token: 0x06001FF0 RID: 8176 RVA: 0x000D1CED File Offset: 0x000CFEED
	public bool IsFullySpawned()
	{
		return this.isSpawned;
	}

	// Token: 0x06001FF1 RID: 8177 RVA: 0x000D1CF5 File Offset: 0x000CFEF5
	public virtual void ServerInit()
	{
		global::BaseNetworkable.serverEntities.RegisterID(this);
		if (this.net != null)
		{
			this.net.handler = this;
		}
	}

	// Token: 0x06001FF2 RID: 8178 RVA: 0x000D1D16 File Offset: 0x000CFF16
	protected List<Connection> GetSubscribers()
	{
		if (this.net == null)
		{
			return null;
		}
		if (this.net.group == null)
		{
			return null;
		}
		return this.net.group.subscribers;
	}

	// Token: 0x06001FF3 RID: 8179 RVA: 0x000029D4 File Offset: 0x00000BD4
	public void KillMessage()
	{
		this.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06001FF4 RID: 8180 RVA: 0x00026D90 File Offset: 0x00024F90
	public virtual void AdminKill()
	{
		this.Kill(global::BaseNetworkable.DestroyMode.Gib);
	}

	// Token: 0x06001FF5 RID: 8181 RVA: 0x000D1D41 File Offset: 0x000CFF41
	public void Kill(global::BaseNetworkable.DestroyMode mode = global::BaseNetworkable.DestroyMode.None)
	{
		if (this.IsDestroyed)
		{
			Debug.LogWarning("Calling kill - but already IsDestroyed!? " + this);
			return;
		}
		base.gameObject.BroadcastOnParentDestroying();
		this.DoEntityDestroy();
		this.TerminateOnClient(mode);
		this.TerminateOnServer();
		this.EntityDestroy();
	}

	// Token: 0x06001FF6 RID: 8182 RVA: 0x000D1D80 File Offset: 0x000CFF80
	private void TerminateOnClient(global::BaseNetworkable.DestroyMode mode)
	{
		if (this.net == null)
		{
			return;
		}
		if (this.net.group == null)
		{
			return;
		}
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "Term {0}", mode);
		NetWrite netWrite = Network.Net.sv.StartWrite();
		netWrite.PacketID(Message.Type.EntityDestroy);
		netWrite.EntityID(this.net.ID);
		netWrite.UInt8((byte)mode);
		netWrite.Send(new SendInfo(this.net.group.subscribers));
	}

	// Token: 0x06001FF7 RID: 8183 RVA: 0x000D1E07 File Offset: 0x000D0007
	private void TerminateOnServer()
	{
		if (this.net == null)
		{
			return;
		}
		this.InvalidateNetworkCache();
		global::BaseNetworkable.serverEntities.UnregisterID(this);
		Network.Net.sv.DestroyNetworkable(ref this.net);
		base.StopAllCoroutines();
		base.gameObject.SetActive(false);
	}

	// Token: 0x06001FF8 RID: 8184 RVA: 0x000D1E45 File Offset: 0x000D0045
	internal virtual void DoServerDestroy()
	{
		this.isSpawned = false;
	}

	// Token: 0x06001FF9 RID: 8185 RVA: 0x000D1E4E File Offset: 0x000D004E
	public virtual bool ShouldNetworkTo(global::BasePlayer player)
	{
		return this.net.group == null || player.net.subscriber.IsSubscribed(this.net.group);
	}

	// Token: 0x06001FFA RID: 8186 RVA: 0x000D1E7C File Offset: 0x000D007C
	protected void SendNetworkGroupChange()
	{
		if (!this.isSpawned)
		{
			return;
		}
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net.group == null)
		{
			Debug.LogWarning(this.ToString() + " changed its network group to null");
			return;
		}
		NetWrite netWrite = Network.Net.sv.StartWrite();
		netWrite.PacketID(Message.Type.GroupChange);
		netWrite.EntityID(this.net.ID);
		netWrite.GroupID(this.net.group.ID);
		netWrite.Send(new SendInfo(this.net.group.subscribers));
	}

	// Token: 0x06001FFB RID: 8187 RVA: 0x000D1F14 File Offset: 0x000D0114
	protected void SendAsSnapshot(Connection connection, bool justCreated = false)
	{
		NetWrite netWrite = Network.Net.sv.StartWrite();
		connection.validate.entityUpdates = connection.validate.entityUpdates + 1U;
		global::BaseNetworkable.SaveInfo saveInfo = new global::BaseNetworkable.SaveInfo
		{
			forConnection = connection,
			forDisk = false
		};
		netWrite.PacketID(Message.Type.Entities);
		netWrite.UInt32(connection.validate.entityUpdates);
		this.ToStreamForNetwork(netWrite, saveInfo);
		netWrite.Send(new SendInfo(connection));
	}

	// Token: 0x06001FFC RID: 8188 RVA: 0x000D1F84 File Offset: 0x000D0184
	public void SendNetworkUpdate(global::BasePlayer.NetworkQueue queue = global::BasePlayer.NetworkQueue.Update)
	{
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (Rust.Application.isLoadingSave)
		{
			return;
		}
		if (this.IsDestroyed)
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
		using (TimeWarning.New("SendNetworkUpdate", 0))
		{
			base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "SendNetworkUpdate");
			this.InvalidateNetworkCache();
			List<Connection> subscribers = this.GetSubscribers();
			if (subscribers != null && subscribers.Count > 0)
			{
				for (int i = 0; i < subscribers.Count; i++)
				{
					global::BasePlayer basePlayer = subscribers[i].player as global::BasePlayer;
					if (!(basePlayer == null) && this.ShouldNetworkTo(basePlayer))
					{
						basePlayer.QueueUpdate(queue, this);
					}
				}
			}
		}
		base.gameObject.SendOnSendNetworkUpdate(this as global::BaseEntity);
	}

	// Token: 0x06001FFD RID: 8189 RVA: 0x000D2058 File Offset: 0x000D0258
	public void SendNetworkUpdateImmediate(bool justCreated = false)
	{
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (Rust.Application.isLoadingSave)
		{
			return;
		}
		if (this.IsDestroyed)
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
		using (TimeWarning.New("SendNetworkUpdateImmediate", 0))
		{
			base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "SendNetworkUpdateImmediate");
			this.InvalidateNetworkCache();
			List<Connection> subscribers = this.GetSubscribers();
			if (subscribers != null && subscribers.Count > 0)
			{
				for (int i = 0; i < subscribers.Count; i++)
				{
					Connection connection = subscribers[i];
					global::BasePlayer basePlayer = connection.player as global::BasePlayer;
					if (!(basePlayer == null) && this.ShouldNetworkTo(basePlayer))
					{
						this.SendAsSnapshot(connection, justCreated);
					}
				}
			}
		}
		base.gameObject.SendOnSendNetworkUpdate(this as global::BaseEntity);
	}

	// Token: 0x06001FFE RID: 8190 RVA: 0x000D2134 File Offset: 0x000D0334
	protected void SendNetworkUpdate_Position()
	{
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (Rust.Application.isLoadingSave)
		{
			return;
		}
		if (this.IsDestroyed)
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
		using (TimeWarning.New("SendNetworkUpdate_Position", 0))
		{
			base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "SendNetworkUpdate_Position");
			List<Connection> subscribers = this.GetSubscribers();
			if (subscribers != null && subscribers.Count > 0)
			{
				NetWrite netWrite = Network.Net.sv.StartWrite();
				netWrite.PacketID(Message.Type.EntityPosition);
				netWrite.EntityID(this.net.ID);
				NetWrite netWrite2 = netWrite;
				Vector3 vector = this.GetNetworkPosition();
				netWrite2.Vector3(vector);
				NetWrite netWrite3 = netWrite;
				vector = this.GetNetworkRotation().eulerAngles;
				netWrite3.Vector3(vector);
				netWrite.Float(this.GetNetworkTime());
				uint uid = this.parentEntity.uid;
				if (uid > 0U)
				{
					netWrite.EntityID(uid);
				}
				SendInfo info = new SendInfo(subscribers)
				{
					method = SendMethod.ReliableUnordered,
					priority = Priority.Immediate
				};
				netWrite.Send(info);
			}
		}
	}

	// Token: 0x06001FFF RID: 8191 RVA: 0x000D2250 File Offset: 0x000D0450
	private void ToStream(Stream stream, global::BaseNetworkable.SaveInfo saveInfo)
	{
		using (saveInfo.msg = Facepunch.Pool.Get<ProtoBuf.Entity>())
		{
			this.Save(saveInfo);
			if (saveInfo.msg.baseEntity == null)
			{
				Debug.LogError(this + ": ToStream - no BaseEntity!?");
			}
			if (saveInfo.msg.baseNetworkable == null)
			{
				Debug.LogError(this + ": ToStream - no baseNetworkable!?");
			}
			saveInfo.msg.ToProto(stream);
			this.PostSave(saveInfo);
		}
	}

	// Token: 0x06002000 RID: 8192 RVA: 0x000D22E0 File Offset: 0x000D04E0
	public virtual bool CanUseNetworkCache(Connection connection)
	{
		return ConVar.Server.netcache;
	}

	// Token: 0x06002001 RID: 8193 RVA: 0x000D22E8 File Offset: 0x000D04E8
	public void ToStreamForNetwork(Stream stream, global::BaseNetworkable.SaveInfo saveInfo)
	{
		if (!this.CanUseNetworkCache(saveInfo.forConnection))
		{
			this.ToStream(stream, saveInfo);
			return;
		}
		if (this._NetworkCache == null)
		{
			this._NetworkCache = ((global::BaseNetworkable.EntityMemoryStreamPool.Count > 0) ? (this._NetworkCache = global::BaseNetworkable.EntityMemoryStreamPool.Dequeue()) : new MemoryStream(8));
			this.ToStream(this._NetworkCache, saveInfo);
			ConVar.Server.netcachesize += (int)this._NetworkCache.Length;
		}
		this._NetworkCache.WriteTo(stream);
	}

	// Token: 0x06002002 RID: 8194 RVA: 0x000D2374 File Offset: 0x000D0574
	public void InvalidateNetworkCache()
	{
		using (TimeWarning.New("InvalidateNetworkCache", 0))
		{
			if (this._SaveCache != null)
			{
				ConVar.Server.savecachesize -= (int)this._SaveCache.Length;
				this._SaveCache.SetLength(0L);
				this._SaveCache.Position = 0L;
				global::BaseNetworkable.EntityMemoryStreamPool.Enqueue(this._SaveCache);
				this._SaveCache = null;
			}
			if (this._NetworkCache != null)
			{
				ConVar.Server.netcachesize -= (int)this._NetworkCache.Length;
				this._NetworkCache.SetLength(0L);
				this._NetworkCache.Position = 0L;
				global::BaseNetworkable.EntityMemoryStreamPool.Enqueue(this._NetworkCache);
				this._NetworkCache = null;
			}
			base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 3, "InvalidateNetworkCache");
		}
	}

	// Token: 0x06002003 RID: 8195 RVA: 0x000D2458 File Offset: 0x000D0658
	public MemoryStream GetSaveCache()
	{
		if (this._SaveCache == null)
		{
			if (global::BaseNetworkable.EntityMemoryStreamPool.Count > 0)
			{
				this._SaveCache = global::BaseNetworkable.EntityMemoryStreamPool.Dequeue();
			}
			else
			{
				this._SaveCache = new MemoryStream(8);
			}
			global::BaseNetworkable.SaveInfo saveInfo = new global::BaseNetworkable.SaveInfo
			{
				forDisk = true
			};
			this.ToStream(this._SaveCache, saveInfo);
			ConVar.Server.savecachesize += (int)this._SaveCache.Length;
		}
		return this._SaveCache;
	}

	// Token: 0x06002004 RID: 8196 RVA: 0x000D24D4 File Offset: 0x000D06D4
	public virtual void UpdateNetworkGroup()
	{
		Assert.IsTrue(this.isServer, "UpdateNetworkGroup called on clientside entity!");
		if (this.net == null)
		{
			return;
		}
		using (TimeWarning.New("UpdateGroups", 0))
		{
			if (this.net.UpdateGroups(base.transform.position))
			{
				this.SendNetworkGroupChange();
			}
		}
	}

	// Token: 0x17000275 RID: 629
	// (get) Token: 0x06002005 RID: 8197 RVA: 0x000D2540 File Offset: 0x000D0740
	// (set) Token: 0x06002006 RID: 8198 RVA: 0x000D2548 File Offset: 0x000D0748
	public bool IsDestroyed { get; private set; }

	// Token: 0x17000276 RID: 630
	// (get) Token: 0x06002007 RID: 8199 RVA: 0x000D2551 File Offset: 0x000D0751
	public string PrefabName
	{
		get
		{
			if (this._prefabName == null)
			{
				this._prefabName = StringPool.Get(this.prefabID);
			}
			return this._prefabName;
		}
	}

	// Token: 0x17000277 RID: 631
	// (get) Token: 0x06002008 RID: 8200 RVA: 0x000D2572 File Offset: 0x000D0772
	public string ShortPrefabName
	{
		get
		{
			if (this._prefabNameWithoutExtension == null)
			{
				this._prefabNameWithoutExtension = Path.GetFileNameWithoutExtension(this.PrefabName);
			}
			return this._prefabNameWithoutExtension;
		}
	}

	// Token: 0x06002009 RID: 8201 RVA: 0x000299CA File Offset: 0x00027BCA
	public virtual Vector3 GetNetworkPosition()
	{
		return base.transform.localPosition;
	}

	// Token: 0x0600200A RID: 8202 RVA: 0x00029A05 File Offset: 0x00027C05
	public virtual Quaternion GetNetworkRotation()
	{
		return base.transform.localRotation;
	}

	// Token: 0x0600200B RID: 8203 RVA: 0x000D2594 File Offset: 0x000D0794
	public string InvokeString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		List<InvokeAction> list = Facepunch.Pool.GetList<InvokeAction>();
		InvokeHandler.FindInvokes(this, list);
		foreach (InvokeAction invokeAction in list)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append(", ");
			}
			stringBuilder.Append(invokeAction.action.Method.Name);
		}
		Facepunch.Pool.FreeList<InvokeAction>(ref list);
		return stringBuilder.ToString();
	}

	// Token: 0x0600200C RID: 8204 RVA: 0x000D2628 File Offset: 0x000D0828
	public global::BaseEntity LookupPrefab()
	{
		return this.gameManager.FindPrefab(this.PrefabName).ToBaseEntity();
	}

	// Token: 0x0600200D RID: 8205 RVA: 0x000D2640 File Offset: 0x000D0840
	public bool EqualNetID(global::BaseNetworkable other)
	{
		return !other.IsRealNull() && other.net != null && this.net != null && other.net.ID == this.net.ID;
	}

	// Token: 0x0600200E RID: 8206 RVA: 0x000D2674 File Offset: 0x000D0874
	public bool EqualNetID(uint otherID)
	{
		return this.net != null && otherID == this.net.ID;
	}

	// Token: 0x0600200F RID: 8207 RVA: 0x000D268E File Offset: 0x000D088E
	public virtual void ResetState()
	{
		if (this.children.Count > 0)
		{
			this.children.Clear();
		}
	}

	// Token: 0x06002010 RID: 8208 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void InitShared()
	{
	}

	// Token: 0x06002011 RID: 8209 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void PreInitShared()
	{
	}

	// Token: 0x06002012 RID: 8210 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void PostInitShared()
	{
	}

	// Token: 0x06002013 RID: 8211 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void DestroyShared()
	{
	}

	// Token: 0x06002014 RID: 8212 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnNetworkGroupEnter(Group group)
	{
	}

	// Token: 0x06002015 RID: 8213 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnNetworkGroupLeave(Group group)
	{
	}

	// Token: 0x06002016 RID: 8214 RVA: 0x000D26AC File Offset: 0x000D08AC
	public void OnNetworkGroupChange()
	{
		if (this.children != null)
		{
			foreach (global::BaseEntity baseEntity in this.children)
			{
				if (baseEntity.ShouldInheritNetworkGroup())
				{
					baseEntity.net.SwitchGroup(this.net.group);
				}
				else if (this.isServer)
				{
					baseEntity.UpdateNetworkGroup();
				}
			}
		}
	}

	// Token: 0x06002017 RID: 8215 RVA: 0x000D2730 File Offset: 0x000D0930
	public void OnNetworkSubscribersEnter(List<Connection> connections)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		foreach (Connection connection in connections)
		{
			global::BasePlayer basePlayer = connection.player as global::BasePlayer;
			if (!(basePlayer == null))
			{
				basePlayer.QueueUpdate(global::BasePlayer.NetworkQueue.Update, this as global::BaseEntity);
			}
		}
	}

	// Token: 0x06002018 RID: 8216 RVA: 0x000D27A4 File Offset: 0x000D09A4
	public void OnNetworkSubscribersLeave(List<Connection> connections)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "LeaveVisibility");
		NetWrite netWrite = Network.Net.sv.StartWrite();
		netWrite.PacketID(Message.Type.EntityDestroy);
		netWrite.EntityID(this.net.ID);
		netWrite.UInt8(0);
		netWrite.Send(new SendInfo(connections));
	}

	// Token: 0x06002019 RID: 8217 RVA: 0x000D27FF File Offset: 0x000D09FF
	private void EntityDestroy()
	{
		if (base.gameObject)
		{
			this.ResetState();
			this.gameManager.Retire(base.gameObject);
		}
	}

	// Token: 0x0600201A RID: 8218 RVA: 0x000D2828 File Offset: 0x000D0A28
	private void DoEntityDestroy()
	{
		if (this.IsDestroyed)
		{
			return;
		}
		this.IsDestroyed = true;
		if (Rust.Application.isQuitting)
		{
			return;
		}
		this.DestroyShared();
		if (this.isServer)
		{
			this.DoServerDestroy();
		}
		using (TimeWarning.New("Registry.Entity.Unregister", 0))
		{
			Rust.Registry.Entity.Unregister(base.gameObject);
		}
	}

	// Token: 0x0600201B RID: 8219 RVA: 0x000D2894 File Offset: 0x000D0A94
	private void SpawnShared()
	{
		this.IsDestroyed = false;
		using (TimeWarning.New("Registry.Entity.Register", 0))
		{
			Rust.Registry.Entity.Register(base.gameObject, this);
		}
	}

	// Token: 0x0600201C RID: 8220 RVA: 0x000D28DC File Offset: 0x000D0ADC
	public virtual void Save(global::BaseNetworkable.SaveInfo info)
	{
		if (this.prefabID == 0U)
		{
			Debug.LogError("PrefabID is 0! " + base.transform.GetRecursiveName(""), base.gameObject);
		}
		info.msg.baseNetworkable = Facepunch.Pool.Get<ProtoBuf.BaseNetworkable>();
		info.msg.baseNetworkable.uid = this.net.ID;
		info.msg.baseNetworkable.prefabID = this.prefabID;
		if (this.net.group != null)
		{
			info.msg.baseNetworkable.group = this.net.group.ID;
		}
		if (!info.forDisk)
		{
			info.msg.createdThisFrame = (this.creationFrame == UnityEngine.Time.frameCount);
		}
	}

	// Token: 0x0600201D RID: 8221 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void PostSave(global::BaseNetworkable.SaveInfo info)
	{
	}

	// Token: 0x0600201E RID: 8222 RVA: 0x000D29A4 File Offset: 0x000D0BA4
	public void InitLoad(uint entityID)
	{
		this.net = Network.Net.sv.CreateNetworkable(entityID);
		global::BaseNetworkable.serverEntities.RegisterID(this);
		this.PreServerLoad();
	}

	// Token: 0x0600201F RID: 8223 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void PreServerLoad()
	{
	}

	// Token: 0x06002020 RID: 8224 RVA: 0x000D29C8 File Offset: 0x000D0BC8
	public virtual void Load(global::BaseNetworkable.LoadInfo info)
	{
		if (info.msg.baseNetworkable == null)
		{
			return;
		}
		ProtoBuf.BaseNetworkable baseNetworkable = info.msg.baseNetworkable;
		if (this.prefabID != baseNetworkable.prefabID)
		{
			Debug.LogError(string.Concat(new object[]
			{
				"Prefab IDs don't match! ",
				this.prefabID,
				"/",
				baseNetworkable.prefabID,
				" -> ",
				base.gameObject
			}), base.gameObject);
		}
	}

	// Token: 0x06002021 RID: 8225 RVA: 0x000D2A50 File Offset: 0x000D0C50
	public virtual void PostServerLoad()
	{
		base.gameObject.SendOnSendNetworkUpdate(this as global::BaseEntity);
	}

	// Token: 0x17000278 RID: 632
	// (get) Token: 0x06002022 RID: 8226 RVA: 0x00003A54 File Offset: 0x00001C54
	public bool isServer
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000279 RID: 633
	// (get) Token: 0x06002023 RID: 8227 RVA: 0x00007074 File Offset: 0x00005274
	public bool isClient
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06002024 RID: 8228 RVA: 0x000D2A64 File Offset: 0x000D0C64
	public T ToServer<T>() where T : global::BaseNetworkable
	{
		if (this.isServer)
		{
			return this as T;
		}
		return default(T);
	}

	// Token: 0x06002025 RID: 8229 RVA: 0x00007074 File Offset: 0x00005274
	public virtual bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		return false;
	}

	// Token: 0x06002026 RID: 8230 RVA: 0x000D2A90 File Offset: 0x000D0C90
	public static List<Connection> GetConnectionsWithin(Vector3 position, float distance)
	{
		global::BaseNetworkable.connectionsInSphereList.Clear();
		float num = distance * distance;
		List<Connection> subscribers = global::BaseNetworkable.GlobalNetworkGroup.subscribers;
		for (int i = 0; i < subscribers.Count; i++)
		{
			Connection connection = subscribers[i];
			if (connection.active)
			{
				global::BasePlayer basePlayer = connection.player as global::BasePlayer;
				if (!(basePlayer == null) && basePlayer.SqrDistance(position) <= num)
				{
					global::BaseNetworkable.connectionsInSphereList.Add(connection);
				}
			}
		}
		return global::BaseNetworkable.connectionsInSphereList;
	}

	// Token: 0x06002027 RID: 8231 RVA: 0x000D2B0C File Offset: 0x000D0D0C
	public static void GetCloseConnections(Vector3 position, float distance, List<global::BasePlayer> players)
	{
		if (Network.Net.sv == null)
		{
			return;
		}
		if (Network.Net.sv.visibility == null)
		{
			return;
		}
		float num = distance * distance;
		Group group = Network.Net.sv.visibility.GetGroup(position);
		if (group == null)
		{
			return;
		}
		List<Connection> subscribers = group.subscribers;
		for (int i = 0; i < subscribers.Count; i++)
		{
			Connection connection = subscribers[i];
			if (connection.active)
			{
				global::BasePlayer basePlayer = connection.player as global::BasePlayer;
				if (!(basePlayer == null) && basePlayer.SqrDistance(position) <= num)
				{
					players.Add(basePlayer);
				}
			}
		}
	}

	// Token: 0x06002028 RID: 8232 RVA: 0x000D2B9C File Offset: 0x000D0D9C
	public static bool HasCloseConnections(Vector3 position, float distance)
	{
		if (Network.Net.sv == null)
		{
			return false;
		}
		if (Network.Net.sv.visibility == null)
		{
			return false;
		}
		float num = distance * distance;
		Group group = Network.Net.sv.visibility.GetGroup(position);
		if (group == null)
		{
			return false;
		}
		List<Connection> subscribers = group.subscribers;
		for (int i = 0; i < subscribers.Count; i++)
		{
			Connection connection = subscribers[i];
			if (connection.active)
			{
				global::BasePlayer basePlayer = connection.player as global::BasePlayer;
				if (!(basePlayer == null) && basePlayer.SqrDistance(position) <= num)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x04001909 RID: 6409
	public List<Component> postNetworkUpdateComponents = new List<Component>();

	// Token: 0x0400190A RID: 6410
	private bool _limitedNetworking;

	// Token: 0x0400190B RID: 6411
	[NonSerialized]
	public EntityRef parentEntity;

	// Token: 0x0400190C RID: 6412
	[NonSerialized]
	public readonly List<global::BaseEntity> children = new List<global::BaseEntity>();

	// Token: 0x0400190D RID: 6413
	[NonSerialized]
	public bool canTriggerParent = true;

	// Token: 0x0400190E RID: 6414
	private int creationFrame;

	// Token: 0x0400190F RID: 6415
	protected bool isSpawned;

	// Token: 0x04001910 RID: 6416
	private MemoryStream _NetworkCache;

	// Token: 0x04001911 RID: 6417
	public static Queue<MemoryStream> EntityMemoryStreamPool = new Queue<MemoryStream>();

	// Token: 0x04001912 RID: 6418
	private MemoryStream _SaveCache;

	// Token: 0x04001913 RID: 6419
	[Header("BaseNetworkable")]
	[ReadOnly]
	public uint prefabID;

	// Token: 0x04001914 RID: 6420
	[Tooltip("If enabled the entity will send to everyone on the server - regardless of position")]
	public bool globalBroadcast;

	// Token: 0x04001915 RID: 6421
	[NonSerialized]
	public Networkable net;

	// Token: 0x04001917 RID: 6423
	private string _prefabName;

	// Token: 0x04001918 RID: 6424
	private string _prefabNameWithoutExtension;

	// Token: 0x04001919 RID: 6425
	public static global::BaseNetworkable.EntityRealm serverEntities = new global::BaseNetworkable.EntityRealmServer();

	// Token: 0x0400191A RID: 6426
	private const bool isServersideEntity = true;

	// Token: 0x0400191B RID: 6427
	private static List<Connection> connectionsInSphereList = new List<Connection>();

	// Token: 0x02000C66 RID: 3174
	public struct SaveInfo
	{
		// Token: 0x06004CDA RID: 19674 RVA: 0x0019688F File Offset: 0x00194A8F
		internal bool SendingTo(Connection ownerConnection)
		{
			return ownerConnection != null && this.forConnection != null && this.forConnection == ownerConnection;
		}

		// Token: 0x04004232 RID: 16946
		public ProtoBuf.Entity msg;

		// Token: 0x04004233 RID: 16947
		public bool forDisk;

		// Token: 0x04004234 RID: 16948
		public Connection forConnection;
	}

	// Token: 0x02000C67 RID: 3175
	public struct LoadInfo
	{
		// Token: 0x04004235 RID: 16949
		public ProtoBuf.Entity msg;

		// Token: 0x04004236 RID: 16950
		public bool fromDisk;
	}

	// Token: 0x02000C68 RID: 3176
	public class EntityRealmServer : global::BaseNetworkable.EntityRealm
	{
		// Token: 0x17000627 RID: 1575
		// (get) Token: 0x06004CDB RID: 19675 RVA: 0x001968A9 File Offset: 0x00194AA9
		protected override Manager visibilityManager
		{
			get
			{
				if (Network.Net.sv == null)
				{
					return null;
				}
				return Network.Net.sv.visibility;
			}
		}
	}

	// Token: 0x02000C69 RID: 3177
	public abstract class EntityRealm : IEnumerable<global::BaseNetworkable>, IEnumerable
	{
		// Token: 0x17000628 RID: 1576
		// (get) Token: 0x06004CDD RID: 19677 RVA: 0x001968C6 File Offset: 0x00194AC6
		public int Count
		{
			get
			{
				return this.entityList.Count;
			}
		}

		// Token: 0x17000629 RID: 1577
		// (get) Token: 0x06004CDE RID: 19678
		protected abstract Manager visibilityManager { get; }

		// Token: 0x06004CDF RID: 19679 RVA: 0x001968D3 File Offset: 0x00194AD3
		public bool Contains(uint uid)
		{
			return this.entityList.Contains(uid);
		}

		// Token: 0x06004CE0 RID: 19680 RVA: 0x001968E4 File Offset: 0x00194AE4
		public global::BaseNetworkable Find(uint uid)
		{
			global::BaseNetworkable result = null;
			if (!this.entityList.TryGetValue(uid, out result))
			{
				return null;
			}
			return result;
		}

		// Token: 0x06004CE1 RID: 19681 RVA: 0x00196908 File Offset: 0x00194B08
		public void RegisterID(global::BaseNetworkable ent)
		{
			if (ent.net != null)
			{
				if (this.entityList.Contains(ent.net.ID))
				{
					this.entityList[ent.net.ID] = ent;
					return;
				}
				this.entityList.Add(ent.net.ID, ent);
			}
		}

		// Token: 0x06004CE2 RID: 19682 RVA: 0x00196964 File Offset: 0x00194B64
		public void UnregisterID(global::BaseNetworkable ent)
		{
			if (ent.net != null)
			{
				this.entityList.Remove(ent.net.ID);
			}
		}

		// Token: 0x06004CE3 RID: 19683 RVA: 0x00196988 File Offset: 0x00194B88
		public Group FindGroup(uint uid)
		{
			Manager visibilityManager = this.visibilityManager;
			if (visibilityManager == null)
			{
				return null;
			}
			return visibilityManager.Get(uid);
		}

		// Token: 0x06004CE4 RID: 19684 RVA: 0x001969A8 File Offset: 0x00194BA8
		public Group TryFindGroup(uint uid)
		{
			Manager visibilityManager = this.visibilityManager;
			if (visibilityManager == null)
			{
				return null;
			}
			return visibilityManager.TryGet(uid);
		}

		// Token: 0x06004CE5 RID: 19685 RVA: 0x001969C8 File Offset: 0x00194BC8
		public void FindInGroup(uint uid, List<global::BaseNetworkable> list)
		{
			Group group = this.TryFindGroup(uid);
			if (group == null)
			{
				return;
			}
			int count = group.networkables.Values.Count;
			Networkable[] buffer = group.networkables.Values.Buffer;
			for (int i = 0; i < count; i++)
			{
				Networkable networkable = buffer[i];
				global::BaseNetworkable baseNetworkable = this.Find(networkable.ID);
				if (!(baseNetworkable == null) && baseNetworkable.net != null && baseNetworkable.net.group != null)
				{
					if (baseNetworkable.net.group.ID != uid)
					{
						Debug.LogWarning("Group ID mismatch: " + baseNetworkable.ToString());
					}
					else
					{
						list.Add(baseNetworkable);
					}
				}
			}
		}

		// Token: 0x06004CE6 RID: 19686 RVA: 0x00196A78 File Offset: 0x00194C78
		public IEnumerator<global::BaseNetworkable> GetEnumerator()
		{
			return this.entityList.Values.GetEnumerator();
		}

		// Token: 0x06004CE7 RID: 19687 RVA: 0x00196A8F File Offset: 0x00194C8F
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x06004CE8 RID: 19688 RVA: 0x00196A97 File Offset: 0x00194C97
		public void Clear()
		{
			this.entityList.Clear();
		}

		// Token: 0x04004237 RID: 16951
		private ListDictionary<uint, global::BaseNetworkable> entityList = new ListDictionary<uint, global::BaseNetworkable>();
	}

	// Token: 0x02000C6A RID: 3178
	public enum DestroyMode : byte
	{
		// Token: 0x04004239 RID: 16953
		None,
		// Token: 0x0400423A RID: 16954
		Gib
	}
}
