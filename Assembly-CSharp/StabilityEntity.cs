using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x0200043C RID: 1084
public class StabilityEntity : global::DecayEntity
{
	// Token: 0x06002397 RID: 9111 RVA: 0x000E1558 File Offset: 0x000DF758
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.stabilityEntity = Facepunch.Pool.Get<ProtoBuf.StabilityEntity>();
		info.msg.stabilityEntity.stability = this.cachedStability;
		info.msg.stabilityEntity.distanceFromGround = this.cachedDistanceFromGround;
	}

	// Token: 0x06002398 RID: 9112 RVA: 0x000E15A8 File Offset: 0x000DF7A8
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.stabilityEntity != null)
		{
			this.cachedStability = info.msg.stabilityEntity.stability;
			this.cachedDistanceFromGround = info.msg.stabilityEntity.distanceFromGround;
			if (this.cachedStability <= 0f)
			{
				this.cachedStability = 0f;
			}
			if (this.cachedDistanceFromGround <= 0)
			{
				this.cachedDistanceFromGround = int.MaxValue;
			}
		}
	}

	// Token: 0x06002399 RID: 9113 RVA: 0x000E1621 File Offset: 0x000DF821
	public override void ResetState()
	{
		base.ResetState();
		this.cachedStability = 0f;
		this.cachedDistanceFromGround = int.MaxValue;
		if (base.isServer)
		{
			this.supports = null;
			this.stabilityStrikes = 0;
			this.dirty = false;
		}
	}

	// Token: 0x0600239A RID: 9114 RVA: 0x000E165C File Offset: 0x000DF85C
	public void InitializeSupports()
	{
		this.supports = new List<global::StabilityEntity.Support>();
		if (this.grounded)
		{
			return;
		}
		List<EntityLink> entityLinks = base.GetEntityLinks(true);
		for (int i = 0; i < entityLinks.Count; i++)
		{
			EntityLink entityLink = entityLinks[i];
			if (entityLink.IsMale())
			{
				if (entityLink.socket is StabilitySocket)
				{
					this.supports.Add(new global::StabilityEntity.Support(this, entityLink, (entityLink.socket as StabilitySocket).support));
				}
				if (entityLink.socket is ConstructionSocket)
				{
					this.supports.Add(new global::StabilityEntity.Support(this, entityLink, (entityLink.socket as ConstructionSocket).support));
				}
			}
		}
	}

	// Token: 0x0600239B RID: 9115 RVA: 0x000E1704 File Offset: 0x000DF904
	public int DistanceFromGround(global::StabilityEntity ignoreEntity = null)
	{
		if (this.grounded)
		{
			return 1;
		}
		if (this.supports == null)
		{
			return 1;
		}
		if (ignoreEntity == null)
		{
			ignoreEntity = this;
		}
		int num = int.MaxValue;
		for (int i = 0; i < this.supports.Count; i++)
		{
			global::StabilityEntity stabilityEntity = this.supports[i].SupportEntity(ignoreEntity);
			if (!(stabilityEntity == null))
			{
				int num2 = stabilityEntity.CachedDistanceFromGround(ignoreEntity);
				if (num2 != 2147483647)
				{
					num = Mathf.Min(num, num2 + 1);
				}
			}
		}
		return num;
	}

	// Token: 0x0600239C RID: 9116 RVA: 0x000E1784 File Offset: 0x000DF984
	public float SupportValue(global::StabilityEntity ignoreEntity = null)
	{
		if (this.grounded)
		{
			return 1f;
		}
		if (this.supports == null)
		{
			return 1f;
		}
		if (ignoreEntity == null)
		{
			ignoreEntity = this;
		}
		float num = 0f;
		for (int i = 0; i < this.supports.Count; i++)
		{
			global::StabilityEntity.Support support = this.supports[i];
			global::StabilityEntity stabilityEntity = support.SupportEntity(ignoreEntity);
			if (!(stabilityEntity == null))
			{
				float num2 = stabilityEntity.CachedSupportValue(ignoreEntity);
				if (num2 != 0f)
				{
					num += num2 * support.factor;
				}
			}
		}
		return Mathf.Clamp01(num);
	}

	// Token: 0x0600239D RID: 9117 RVA: 0x000E1818 File Offset: 0x000DFA18
	public int CachedDistanceFromGround(global::StabilityEntity ignoreEntity = null)
	{
		if (this.grounded)
		{
			return 1;
		}
		if (this.supports == null)
		{
			return 1;
		}
		if (ignoreEntity == null)
		{
			ignoreEntity = this;
		}
		int num = int.MaxValue;
		for (int i = 0; i < this.supports.Count; i++)
		{
			global::StabilityEntity stabilityEntity = this.supports[i].SupportEntity(ignoreEntity);
			if (!(stabilityEntity == null))
			{
				int num2 = stabilityEntity.cachedDistanceFromGround;
				if (num2 != 2147483647)
				{
					num = Mathf.Min(num, num2 + 1);
				}
			}
		}
		return num;
	}

	// Token: 0x0600239E RID: 9118 RVA: 0x000E1898 File Offset: 0x000DFA98
	public float CachedSupportValue(global::StabilityEntity ignoreEntity = null)
	{
		if (this.grounded)
		{
			return 1f;
		}
		if (this.supports == null)
		{
			return 1f;
		}
		if (ignoreEntity == null)
		{
			ignoreEntity = this;
		}
		float num = 0f;
		for (int i = 0; i < this.supports.Count; i++)
		{
			global::StabilityEntity.Support support = this.supports[i];
			global::StabilityEntity stabilityEntity = support.SupportEntity(ignoreEntity);
			if (!(stabilityEntity == null))
			{
				float num2 = stabilityEntity.cachedStability;
				if (num2 != 0f)
				{
					num += num2 * support.factor;
				}
			}
		}
		return Mathf.Clamp01(num);
	}

	// Token: 0x0600239F RID: 9119 RVA: 0x000E192C File Offset: 0x000DFB2C
	public void StabilityCheck()
	{
		if (base.IsDestroyed)
		{
			return;
		}
		if (this.supports == null)
		{
			this.InitializeSupports();
		}
		bool flag = false;
		int num = this.DistanceFromGround(null);
		if (num != this.cachedDistanceFromGround)
		{
			this.cachedDistanceFromGround = num;
			flag = true;
		}
		float num2 = this.SupportValue(null);
		if (Mathf.Abs(this.cachedStability - num2) > Stability.accuracy)
		{
			this.cachedStability = num2;
			flag = true;
		}
		if (flag)
		{
			this.dirty = true;
			this.UpdateConnectedEntities();
			this.UpdateStability();
		}
		else if (this.dirty)
		{
			this.dirty = false;
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
		if (num2 >= Stability.collapse)
		{
			this.stabilityStrikes = 0;
			return;
		}
		if (this.stabilityStrikes < Stability.strikes)
		{
			this.UpdateStability();
			this.stabilityStrikes++;
			return;
		}
		base.Kill(global::BaseNetworkable.DestroyMode.Gib);
	}

	// Token: 0x060023A0 RID: 9120 RVA: 0x000E19F8 File Offset: 0x000DFBF8
	public void UpdateStability()
	{
		global::StabilityEntity.stabilityCheckQueue.Add(this);
	}

	// Token: 0x060023A1 RID: 9121 RVA: 0x000E1A08 File Offset: 0x000DFC08
	public void UpdateSurroundingEntities()
	{
		global::StabilityEntity.updateSurroundingsQueue.Add(this.WorldSpaceBounds().ToBounds());
	}

	// Token: 0x060023A2 RID: 9122 RVA: 0x000E1A30 File Offset: 0x000DFC30
	public void UpdateConnectedEntities()
	{
		List<EntityLink> entityLinks = base.GetEntityLinks(true);
		for (int i = 0; i < entityLinks.Count; i++)
		{
			EntityLink entityLink = entityLinks[i];
			if (entityLink.IsFemale())
			{
				for (int j = 0; j < entityLink.connections.Count; j++)
				{
					global::StabilityEntity stabilityEntity = entityLink.connections[j].owner as global::StabilityEntity;
					if (!(stabilityEntity == null) && !stabilityEntity.isClient && !stabilityEntity.IsDestroyed)
					{
						stabilityEntity.UpdateStability();
					}
				}
			}
		}
	}

	// Token: 0x060023A3 RID: 9123 RVA: 0x000E1AB7 File Offset: 0x000DFCB7
	protected void OnPhysicsNeighbourChanged()
	{
		if (base.IsDestroyed)
		{
			return;
		}
		this.StabilityCheck();
	}

	// Token: 0x060023A4 RID: 9124 RVA: 0x000E1AC8 File Offset: 0x000DFCC8
	protected void DebugNudge()
	{
		this.StabilityCheck();
	}

	// Token: 0x060023A5 RID: 9125 RVA: 0x000E1AD0 File Offset: 0x000DFCD0
	public override void ServerInit()
	{
		base.ServerInit();
		if (!Rust.Application.isLoadingSave)
		{
			this.UpdateStability();
		}
	}

	// Token: 0x060023A6 RID: 9126 RVA: 0x000E1AE5 File Offset: 0x000DFCE5
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		this.UpdateSurroundingEntities();
	}

	// Token: 0x04001C49 RID: 7241
	public static global::StabilityEntity.StabilityCheckWorkQueue stabilityCheckQueue = new global::StabilityEntity.StabilityCheckWorkQueue();

	// Token: 0x04001C4A RID: 7242
	public static global::StabilityEntity.UpdateSurroundingsQueue updateSurroundingsQueue = new global::StabilityEntity.UpdateSurroundingsQueue();

	// Token: 0x04001C4B RID: 7243
	public bool grounded;

	// Token: 0x04001C4C RID: 7244
	[NonSerialized]
	public float cachedStability;

	// Token: 0x04001C4D RID: 7245
	[NonSerialized]
	public int cachedDistanceFromGround = int.MaxValue;

	// Token: 0x04001C4E RID: 7246
	private List<global::StabilityEntity.Support> supports;

	// Token: 0x04001C4F RID: 7247
	private int stabilityStrikes;

	// Token: 0x04001C50 RID: 7248
	private bool dirty;

	// Token: 0x02000C9A RID: 3226
	public class StabilityCheckWorkQueue : ObjectWorkQueue<global::StabilityEntity>
	{
		// Token: 0x06004D13 RID: 19731 RVA: 0x00196F01 File Offset: 0x00195101
		protected override void RunJob(global::StabilityEntity entity)
		{
			if (!this.ShouldAdd(entity))
			{
				return;
			}
			entity.StabilityCheck();
		}

		// Token: 0x06004D14 RID: 19732 RVA: 0x00196F13 File Offset: 0x00195113
		protected override bool ShouldAdd(global::StabilityEntity entity)
		{
			return ConVar.Server.stability && entity.IsValid() && entity.isServer;
		}
	}

	// Token: 0x02000C9B RID: 3227
	public class UpdateSurroundingsQueue : ObjectWorkQueue<Bounds>
	{
		// Token: 0x06004D16 RID: 19734 RVA: 0x00196F3C File Offset: 0x0019513C
		protected override void RunJob(Bounds bounds)
		{
			if (!ConVar.Server.stability)
			{
				return;
			}
			List<global::BaseEntity> list = Facepunch.Pool.GetList<global::BaseEntity>();
			global::Vis.Entities<global::BaseEntity>(bounds.center, bounds.extents.magnitude + 1f, list, 69372162, QueryTriggerInteraction.Collide);
			foreach (global::BaseEntity baseEntity in list)
			{
				if (!baseEntity.IsDestroyed && !baseEntity.isClient)
				{
					if (baseEntity is global::StabilityEntity)
					{
						(baseEntity as global::StabilityEntity).OnPhysicsNeighbourChanged();
					}
					else
					{
						baseEntity.BroadcastMessage("OnPhysicsNeighbourChanged", SendMessageOptions.DontRequireReceiver);
					}
				}
			}
			Facepunch.Pool.FreeList<global::BaseEntity>(ref list);
		}
	}

	// Token: 0x02000C9C RID: 3228
	private class Support
	{
		// Token: 0x06004D18 RID: 19736 RVA: 0x00196FFC File Offset: 0x001951FC
		public Support(global::StabilityEntity parent, EntityLink link, float factor)
		{
			this.parent = parent;
			this.link = link;
			this.factor = factor;
		}

		// Token: 0x06004D19 RID: 19737 RVA: 0x00197024 File Offset: 0x00195224
		public global::StabilityEntity SupportEntity(global::StabilityEntity ignoreEntity = null)
		{
			global::StabilityEntity stabilityEntity = null;
			for (int i = 0; i < this.link.connections.Count; i++)
			{
				global::StabilityEntity stabilityEntity2 = this.link.connections[i].owner as global::StabilityEntity;
				Socket_Base socket = this.link.connections[i].socket;
				ConstructionSocket constructionSocket;
				if (!(stabilityEntity2 == null) && !(stabilityEntity2 == this.parent) && !(stabilityEntity2 == ignoreEntity) && !stabilityEntity2.isClient && !stabilityEntity2.IsDestroyed && ((constructionSocket = (socket as ConstructionSocket)) == null || !constructionSocket.femaleNoStability) && (stabilityEntity == null || stabilityEntity2.cachedDistanceFromGround < stabilityEntity.cachedDistanceFromGround))
				{
					stabilityEntity = stabilityEntity2;
				}
			}
			return stabilityEntity;
		}

		// Token: 0x04004336 RID: 17206
		public global::StabilityEntity parent;

		// Token: 0x04004337 RID: 17207
		public EntityLink link;

		// Token: 0x04004338 RID: 17208
		public float factor = 1f;
	}
}
