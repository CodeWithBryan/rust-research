using System;
using System.Collections.Generic;
using ConVar;
using Network;
using UnityEngine;

// Token: 0x020000EC RID: 236
public class XMasRefill : BaseEntity
{
	// Token: 0x06001472 RID: 5234 RVA: 0x000A1938 File Offset: 0x0009FB38
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("XMasRefill.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001473 RID: 5235 RVA: 0x000A1978 File Offset: 0x0009FB78
	public float GiftRadius()
	{
		return XMas.spawnRange;
	}

	// Token: 0x06001474 RID: 5236 RVA: 0x000A197F File Offset: 0x0009FB7F
	public int GiftsPerPlayer()
	{
		return XMas.giftsPerPlayer;
	}

	// Token: 0x06001475 RID: 5237 RVA: 0x000A1986 File Offset: 0x0009FB86
	public int GiftSpawnAttempts()
	{
		return XMas.giftsPerPlayer * XMas.spawnAttempts;
	}

	// Token: 0x06001476 RID: 5238 RVA: 0x000A1994 File Offset: 0x0009FB94
	public override void ServerInit()
	{
		base.ServerInit();
		if (!XMas.enabled)
		{
			base.Invoke(new Action(this.RemoveMe), 0.1f);
			return;
		}
		this.goodKids = ((BasePlayer.activePlayerList != null) ? new List<BasePlayer>(BasePlayer.activePlayerList) : new List<BasePlayer>());
		this.stockings = ((Stocking.stockings != null) ? new List<Stocking>(Stocking.stockings.Values) : new List<Stocking>());
		base.Invoke(new Action(this.RemoveMe), 60f);
		base.InvokeRepeating(new Action(this.DistributeLoot), 3f, 0.02f);
		base.Invoke(new Action(this.SendBells), 0.5f);
	}

	// Token: 0x06001477 RID: 5239 RVA: 0x000A1A51 File Offset: 0x0009FC51
	public void SendBells()
	{
		base.ClientRPC(null, "PlayBells");
	}

	// Token: 0x06001478 RID: 5240 RVA: 0x000A1A5F File Offset: 0x0009FC5F
	public void RemoveMe()
	{
		if (this.goodKids.Count == 0 && this.stockings.Count == 0)
		{
			base.Kill(BaseNetworkable.DestroyMode.None);
			return;
		}
		base.Invoke(new Action(this.RemoveMe), 60f);
	}

	// Token: 0x06001479 RID: 5241 RVA: 0x000A1A9C File Offset: 0x0009FC9C
	public void DistributeLoot()
	{
		if (this.goodKids.Count > 0)
		{
			BasePlayer basePlayer = null;
			foreach (BasePlayer basePlayer2 in this.goodKids)
			{
				if (!basePlayer2.IsSleeping() && !basePlayer2.IsWounded() && basePlayer2.IsAlive())
				{
					basePlayer = basePlayer2;
					break;
				}
			}
			if (basePlayer)
			{
				this.DistributeGiftsForPlayer(basePlayer);
				this.goodKids.Remove(basePlayer);
			}
		}
		if (this.stockings.Count > 0)
		{
			Stocking stocking = this.stockings[0];
			if (stocking != null)
			{
				stocking.SpawnLoot();
			}
			this.stockings.RemoveAt(0);
		}
	}

	// Token: 0x0600147A RID: 5242 RVA: 0x000A1B68 File Offset: 0x0009FD68
	protected bool DropToGround(ref Vector3 pos)
	{
		int intVal = 1235288065;
		int num = 8454144;
		if (TerrainMeta.TopologyMap && (TerrainMeta.TopologyMap.GetTopology(pos) & 82048) != 0)
		{
			return false;
		}
		if (TerrainMeta.HeightMap && TerrainMeta.Collision && !TerrainMeta.Collision.GetIgnore(pos, 0.01f))
		{
			float height = TerrainMeta.HeightMap.GetHeight(pos);
			pos.y = Mathf.Max(pos.y, height);
		}
		RaycastHit raycastHit;
		if (!TransformUtil.GetGroundInfo(pos, out raycastHit, 80f, intVal, null))
		{
			return false;
		}
		if ((1 << raycastHit.transform.gameObject.layer & num) == 0)
		{
			return false;
		}
		pos = raycastHit.point;
		return true;
	}

	// Token: 0x0600147B RID: 5243 RVA: 0x000A1C40 File Offset: 0x0009FE40
	public bool DistributeGiftsForPlayer(BasePlayer player)
	{
		int num = this.GiftsPerPlayer();
		int num2 = this.GiftSpawnAttempts();
		int num3 = 0;
		while (num3 < num2 && num > 0)
		{
			Vector2 vector = UnityEngine.Random.insideUnitCircle * this.GiftRadius();
			Vector3 pos = player.transform.position + new Vector3(vector.x, 10f, vector.y);
			Quaternion rot = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
			if (this.DropToGround(ref pos))
			{
				string resourcePath = this.giftPrefabs[UnityEngine.Random.Range(0, this.giftPrefabs.Length)].resourcePath;
				BaseEntity baseEntity = GameManager.server.CreateEntity(resourcePath, pos, rot, true);
				if (baseEntity)
				{
					baseEntity.Spawn();
					num--;
				}
			}
			num3++;
		}
		return true;
	}

	// Token: 0x04000CFC RID: 3324
	public GameObjectRef[] giftPrefabs;

	// Token: 0x04000CFD RID: 3325
	public List<BasePlayer> goodKids;

	// Token: 0x04000CFE RID: 3326
	public List<Stocking> stockings;

	// Token: 0x04000CFF RID: 3327
	public AudioSource bells;
}
