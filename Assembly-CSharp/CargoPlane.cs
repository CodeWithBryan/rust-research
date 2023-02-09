using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x020004BB RID: 1211
public class CargoPlane : global::BaseEntity
{
	// Token: 0x060026F7 RID: 9975 RVA: 0x000F0943 File Offset: 0x000EEB43
	public override void ServerInit()
	{
		base.ServerInit();
		this.Initialize();
	}

	// Token: 0x060026F8 RID: 9976 RVA: 0x000F0951 File Offset: 0x000EEB51
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (this.dropPosition == Vector3.zero)
		{
			this.Initialize();
		}
	}

	// Token: 0x060026F9 RID: 9977 RVA: 0x000F0971 File Offset: 0x000EEB71
	private void Initialize()
	{
		if (this.dropPosition == Vector3.zero)
		{
			this.dropPosition = this.RandomDropPosition();
		}
		this.UpdateDropPosition(this.dropPosition);
	}

	// Token: 0x060026FA RID: 9978 RVA: 0x000F099D File Offset: 0x000EEB9D
	public void InitDropPosition(Vector3 newDropPosition)
	{
		this.dropPosition = newDropPosition;
		this.dropPosition.y = 0f;
	}

	// Token: 0x060026FB RID: 9979 RVA: 0x000F09B8 File Offset: 0x000EEBB8
	public Vector3 RandomDropPosition()
	{
		Vector3 vector = Vector3.zero;
		float num = 100f;
		float x = TerrainMeta.Size.x;
		do
		{
			vector = Vector3Ex.Range(-(x / 3f), x / 3f);
		}
		while (this.filter.GetFactor(vector, true) == 0f && (num -= 1f) > 0f);
		vector.y = 0f;
		return vector;
	}

	// Token: 0x060026FC RID: 9980 RVA: 0x000F0A24 File Offset: 0x000EEC24
	public void UpdateDropPosition(Vector3 newDropPosition)
	{
		float x = TerrainMeta.Size.x;
		float y = TerrainMeta.HighestPoint.y + 250f;
		this.startPos = Vector3Ex.Range(-1f, 1f);
		this.startPos.y = 0f;
		this.startPos.Normalize();
		this.startPos *= x * 2f;
		this.startPos.y = y;
		this.endPos = this.startPos * -1f;
		this.endPos.y = this.startPos.y;
		this.startPos += newDropPosition;
		this.endPos += newDropPosition;
		this.secondsToTake = Vector3.Distance(this.startPos, this.endPos) / 50f;
		this.secondsToTake *= UnityEngine.Random.Range(0.95f, 1.05f);
		base.transform.position = this.startPos;
		base.transform.rotation = Quaternion.LookRotation(this.endPos - this.startPos);
		this.dropPosition = newDropPosition;
	}

	// Token: 0x060026FD RID: 9981 RVA: 0x000F0B64 File Offset: 0x000EED64
	private void Update()
	{
		if (!base.isServer)
		{
			return;
		}
		this.secondsTaken += Time.deltaTime;
		float num = Mathf.InverseLerp(0f, this.secondsToTake, this.secondsTaken);
		if (!this.dropped && num >= 0.5f)
		{
			this.dropped = true;
			global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.prefabDrop.resourcePath, base.transform.position, default(Quaternion), true);
			if (baseEntity)
			{
				baseEntity.globalBroadcast = true;
				baseEntity.Spawn();
			}
		}
		base.transform.position = Vector3.Lerp(this.startPos, this.endPos, num);
		base.transform.hasChanged = true;
		if (num >= 1f)
		{
			base.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x060026FE RID: 9982 RVA: 0x000F0C34 File Offset: 0x000EEE34
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (base.isServer && info.forDisk)
		{
			info.msg.cargoPlane = Pool.Get<ProtoBuf.CargoPlane>();
			info.msg.cargoPlane.startPos = this.startPos;
			info.msg.cargoPlane.endPos = this.endPos;
			info.msg.cargoPlane.secondsToTake = this.secondsToTake;
			info.msg.cargoPlane.secondsTaken = this.secondsTaken;
			info.msg.cargoPlane.dropped = this.dropped;
			info.msg.cargoPlane.dropPosition = this.dropPosition;
		}
	}

	// Token: 0x060026FF RID: 9983 RVA: 0x000F0CF4 File Offset: 0x000EEEF4
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (base.isServer && info.fromDisk && info.msg.cargoPlane != null)
		{
			this.startPos = info.msg.cargoPlane.startPos;
			this.endPos = info.msg.cargoPlane.endPos;
			this.secondsToTake = info.msg.cargoPlane.secondsToTake;
			this.secondsTaken = info.msg.cargoPlane.secondsTaken;
			this.dropped = info.msg.cargoPlane.dropped;
			this.dropPosition = info.msg.cargoPlane.dropPosition;
		}
	}

	// Token: 0x04001F6B RID: 8043
	public GameObjectRef prefabDrop;

	// Token: 0x04001F6C RID: 8044
	public SpawnFilter filter;

	// Token: 0x04001F6D RID: 8045
	private Vector3 startPos;

	// Token: 0x04001F6E RID: 8046
	private Vector3 endPos;

	// Token: 0x04001F6F RID: 8047
	private float secondsToTake;

	// Token: 0x04001F70 RID: 8048
	private float secondsTaken;

	// Token: 0x04001F71 RID: 8049
	private bool dropped;

	// Token: 0x04001F72 RID: 8050
	private Vector3 dropPosition = Vector3.zero;
}
