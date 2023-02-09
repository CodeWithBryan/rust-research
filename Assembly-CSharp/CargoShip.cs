using System;
using System.Collections.Generic;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x0200001F RID: 31
public class CargoShip : global::BaseEntity
{
	// Token: 0x060000A7 RID: 167 RVA: 0x000058B6 File Offset: 0x00003AB6
	public override float GetNetworkTime()
	{
		return Time.fixedTime;
	}

	// Token: 0x060000A8 RID: 168 RVA: 0x000058BD File Offset: 0x00003ABD
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.simpleUID != null)
		{
			this.layoutChoice = info.msg.simpleUID.uid;
		}
	}

	// Token: 0x060000A9 RID: 169 RVA: 0x000058EC File Offset: 0x00003AEC
	public void RefreshActiveLayout()
	{
		for (int i = 0; i < this.layouts.Length; i++)
		{
			this.layouts[i].SetActive((ulong)this.layoutChoice == (ulong)((long)i));
		}
	}

	// Token: 0x060000AA RID: 170 RVA: 0x00005924 File Offset: 0x00003B24
	public void TriggeredEventSpawn()
	{
		Vector3 vector = TerrainMeta.RandomPointOffshore();
		vector.y = TerrainMeta.WaterMap.GetHeight(vector);
		base.transform.position = vector;
		if (!CargoShip.event_enabled || CargoShip.event_duration_minutes == 0f)
		{
			base.Invoke(new Action(this.DelayedDestroy), 1f);
		}
	}

	// Token: 0x060000AB RID: 171 RVA: 0x00005980 File Offset: 0x00003B80
	public void CreateMapMarker()
	{
		if (this.mapMarkerInstance)
		{
			this.mapMarkerInstance.Kill(global::BaseNetworkable.DestroyMode.None);
		}
		global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.mapMarkerEntityPrefab.resourcePath, Vector3.zero, Quaternion.identity, true);
		baseEntity.Spawn();
		baseEntity.SetParent(this, false, false);
		this.mapMarkerInstance = baseEntity;
	}

	// Token: 0x060000AC RID: 172 RVA: 0x000059DD File Offset: 0x00003BDD
	public void DisableCollisionTest()
	{
	}

	// Token: 0x060000AD RID: 173 RVA: 0x000059E0 File Offset: 0x00003BE0
	public void SpawnCrate(string resourcePath)
	{
		int index = UnityEngine.Random.Range(0, this.crateSpawns.Count);
		Vector3 position = this.crateSpawns[index].position;
		Quaternion rotation = this.crateSpawns[index].rotation;
		this.crateSpawns.Remove(this.crateSpawns[index]);
		global::BaseEntity baseEntity = GameManager.server.CreateEntity(resourcePath, position, rotation, true);
		if (baseEntity)
		{
			baseEntity.enableSaving = false;
			baseEntity.SendMessage("SetWasDropped", SendMessageOptions.DontRequireReceiver);
			baseEntity.Spawn();
			baseEntity.SetParent(this, true, false);
			Rigidbody component = baseEntity.GetComponent<Rigidbody>();
			if (component != null)
			{
				component.isKinematic = true;
			}
		}
	}

	// Token: 0x060000AE RID: 174 RVA: 0x00005A90 File Offset: 0x00003C90
	public void RespawnLoot()
	{
		base.InvokeRepeating(new Action(this.PlayHorn), 0f, 8f);
		this.SpawnCrate(this.lockedCratePrefab.resourcePath);
		this.SpawnCrate(this.eliteCratePrefab.resourcePath);
		for (int i = 0; i < 4; i++)
		{
			this.SpawnCrate(this.militaryCratePrefab.resourcePath);
		}
		for (int j = 0; j < 4; j++)
		{
			this.SpawnCrate(this.junkCratePrefab.resourcePath);
		}
		this.lootRoundsPassed++;
		if (this.lootRoundsPassed >= CargoShip.loot_rounds)
		{
			base.CancelInvoke(new Action(this.RespawnLoot));
		}
	}

	// Token: 0x060000AF RID: 175 RVA: 0x00005B44 File Offset: 0x00003D44
	public void SpawnSubEntities()
	{
		if (!Rust.Application.isLoadingSave)
		{
			global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.escapeBoatPrefab.resourcePath, this.escapeBoatPoint.position, this.escapeBoatPoint.rotation, true);
			if (baseEntity)
			{
				baseEntity.SetParent(this, true, false);
				baseEntity.Spawn();
				RHIB component = baseEntity.GetComponent<RHIB>();
				component.SetToKinematic();
				if (component)
				{
					component.AddFuel(50);
				}
			}
		}
		global::MicrophoneStand microphoneStand = GameManager.server.CreateEntity(this.microphonePrefab.resourcePath, this.microphonePoint.position, this.microphonePoint.rotation, true) as global::MicrophoneStand;
		if (microphoneStand)
		{
			microphoneStand.enableSaving = false;
			microphoneStand.SetParent(this, true, false);
			microphoneStand.Spawn();
			microphoneStand.SpawnChildEntity();
			global::IOEntity ioentity = microphoneStand.ioEntity.Get(true);
			foreach (Transform transform in this.speakerPoints)
			{
				global::IOEntity ioentity2 = GameManager.server.CreateEntity(this.speakerPrefab.resourcePath, transform.position, transform.rotation, true) as global::IOEntity;
				ioentity2.enableSaving = false;
				ioentity2.SetParent(this, true, false);
				ioentity2.Spawn();
				ioentity.outputs[0].connectedTo.Set(ioentity2);
				ioentity2.inputs[0].connectedTo.Set(ioentity);
				ioentity = ioentity2;
			}
			microphoneStand.ioEntity.Get(true).MarkDirtyForceUpdateOutputs();
		}
	}

	// Token: 0x060000B0 RID: 176 RVA: 0x00005CC4 File Offset: 0x00003EC4
	protected override void OnChildAdded(global::BaseEntity child)
	{
		base.OnChildAdded(child);
		RHIB rhib;
		if (base.isServer && Rust.Application.isLoadingSave && (rhib = (child as RHIB)) != null)
		{
			Vector3 localPosition = rhib.transform.localPosition;
			Vector3 b = base.transform.InverseTransformPoint(this.escapeBoatPoint.transform.position);
			if (Vector3.Distance(localPosition, b) < 1f)
			{
				rhib.SetToKinematic();
			}
		}
	}

	// Token: 0x060000B1 RID: 177 RVA: 0x00005D2B File Offset: 0x00003F2B
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.simpleUID = Pool.Get<SimpleUID>();
		info.msg.simpleUID.uid = this.layoutChoice;
	}

	// Token: 0x060000B2 RID: 178 RVA: 0x00005D5A File Offset: 0x00003F5A
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.RefreshActiveLayout();
	}

	// Token: 0x060000B3 RID: 179 RVA: 0x00005D68 File Offset: 0x00003F68
	public void PlayHorn()
	{
		base.ClientRPC(null, "DoHornSound");
		this.hornCount++;
		if (this.hornCount >= 3)
		{
			this.hornCount = 0;
			base.CancelInvoke(new Action(this.PlayHorn));
		}
	}

	// Token: 0x060000B4 RID: 180 RVA: 0x00005DA6 File Offset: 0x00003FA6
	public override void Spawn()
	{
		if (!Rust.Application.isLoadingSave)
		{
			this.layoutChoice = (uint)UnityEngine.Random.Range(0, this.layouts.Length);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			this.RefreshActiveLayout();
		}
		base.Spawn();
	}

	// Token: 0x060000B5 RID: 181 RVA: 0x00005DD8 File Offset: 0x00003FD8
	public override void ServerInit()
	{
		base.ServerInit();
		base.Invoke(new Action(this.FindInitialNode), 2f);
		base.InvokeRepeating(new Action(this.BuildingCheck), 1f, 5f);
		base.InvokeRepeating(new Action(this.RespawnLoot), 10f, 60f * CargoShip.loot_round_spacing_minutes);
		base.Invoke(new Action(this.DisableCollisionTest), 10f);
		float height = TerrainMeta.WaterMap.GetHeight(base.transform.position);
		Vector3 vector = base.transform.InverseTransformPoint(this.waterLine.transform.position);
		base.transform.position = new Vector3(base.transform.position.x, height - vector.y, base.transform.position.z);
		this.SpawnSubEntities();
		base.Invoke(new Action(this.StartEgress), 60f * CargoShip.event_duration_minutes);
		this.CreateMapMarker();
	}

	// Token: 0x060000B6 RID: 182 RVA: 0x00005EEC File Offset: 0x000040EC
	public void UpdateRadiation()
	{
		this.currentRadiation += 1f;
		TriggerRadiation[] componentsInChildren = this.radiation.GetComponentsInChildren<TriggerRadiation>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].RadiationAmountOverride = this.currentRadiation;
		}
	}

	// Token: 0x060000B7 RID: 183 RVA: 0x00005F34 File Offset: 0x00004134
	public void StartEgress()
	{
		if (this.egressing)
		{
			return;
		}
		this.egressing = true;
		base.CancelInvoke(new Action(this.PlayHorn));
		this.radiation.SetActive(true);
		base.SetFlag(global::BaseEntity.Flags.Reserved8, true, false, true);
		base.InvokeRepeating(new Action(this.UpdateRadiation), 10f, 1f);
		base.Invoke(new Action(this.DelayedDestroy), 60f * CargoShip.egress_duration_minutes);
	}

	// Token: 0x060000B8 RID: 184 RVA: 0x000029D4 File Offset: 0x00000BD4
	public void DelayedDestroy()
	{
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x060000B9 RID: 185 RVA: 0x00005FB6 File Offset: 0x000041B6
	public void FindInitialNode()
	{
		this.targetNodeIndex = this.GetClosestNodeToUs();
	}

	// Token: 0x060000BA RID: 186 RVA: 0x00005FC4 File Offset: 0x000041C4
	public void BuildingCheck()
	{
		List<global::DecayEntity> list = Pool.GetList<global::DecayEntity>();
		Vis.Entities<global::DecayEntity>(this.WorldSpaceBounds(), list, 2097152, QueryTriggerInteraction.Collide);
		foreach (global::DecayEntity decayEntity in list)
		{
			if (decayEntity.isServer && decayEntity.IsAlive())
			{
				decayEntity.Kill(global::BaseNetworkable.DestroyMode.Gib);
			}
		}
		Pool.FreeList<global::DecayEntity>(ref list);
	}

	// Token: 0x060000BB RID: 187 RVA: 0x00006044 File Offset: 0x00004244
	public void FixedUpdate()
	{
		if (base.isClient)
		{
			return;
		}
		this.UpdateMovement();
	}

	// Token: 0x060000BC RID: 188 RVA: 0x00006058 File Offset: 0x00004258
	public void UpdateMovement()
	{
		if (TerrainMeta.Path.OceanPatrolFar == null || TerrainMeta.Path.OceanPatrolFar.Count == 0)
		{
			return;
		}
		if (this.targetNodeIndex == -1)
		{
			return;
		}
		Vector3 vector = TerrainMeta.Path.OceanPatrolFar[this.targetNodeIndex];
		if (this.egressing)
		{
			vector = base.transform.position + (base.transform.position - Vector3.zero).normalized * 10000f;
		}
		Vector3 normalized = (vector - base.transform.position).normalized;
		float value = Vector3.Dot(base.transform.forward, normalized);
		float b = Mathf.InverseLerp(0f, 1f, value);
		float num = Vector3.Dot(base.transform.right, normalized);
		float num2 = 2.5f;
		float b2 = Mathf.InverseLerp(0.05f, 0.5f, Mathf.Abs(num));
		this.turnScale = Mathf.Lerp(this.turnScale, b2, Time.deltaTime * 0.2f);
		float num3 = (float)((num < 0f) ? -1 : 1);
		this.currentTurnSpeed = num2 * this.turnScale * num3;
		base.transform.Rotate(Vector3.up, Time.deltaTime * this.currentTurnSpeed, Space.World);
		this.currentThrottle = Mathf.Lerp(this.currentThrottle, b, Time.deltaTime * 0.2f);
		this.currentVelocity = base.transform.forward * (8f * this.currentThrottle);
		base.transform.position += this.currentVelocity * Time.deltaTime;
		if (Vector3.Distance(base.transform.position, vector) < 80f)
		{
			this.targetNodeIndex++;
			if (this.targetNodeIndex >= TerrainMeta.Path.OceanPatrolFar.Count)
			{
				this.targetNodeIndex = 0;
			}
		}
	}

	// Token: 0x060000BD RID: 189 RVA: 0x00006260 File Offset: 0x00004460
	public int GetClosestNodeToUs()
	{
		int result = 0;
		float num = float.PositiveInfinity;
		for (int i = 0; i < TerrainMeta.Path.OceanPatrolFar.Count; i++)
		{
			Vector3 b = TerrainMeta.Path.OceanPatrolFar[i];
			float num2 = Vector3.Distance(base.transform.position, b);
			if (num2 < num)
			{
				result = i;
				num = num2;
			}
		}
		return result;
	}

	// Token: 0x060000BE RID: 190 RVA: 0x000062BE File Offset: 0x000044BE
	public override Vector3 GetLocalVelocityServer()
	{
		return this.currentVelocity;
	}

	// Token: 0x060000BF RID: 191 RVA: 0x000062C6 File Offset: 0x000044C6
	public override Quaternion GetAngularVelocityServer()
	{
		return Quaternion.Euler(0f, this.currentTurnSpeed, 0f);
	}

	// Token: 0x060000C0 RID: 192 RVA: 0x000062DD File Offset: 0x000044DD
	public override float InheritedVelocityScale()
	{
		return 1f;
	}

	// Token: 0x060000C1 RID: 193 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool BlocksWaterFor(global::BasePlayer player)
	{
		return true;
	}

	// Token: 0x060000C2 RID: 194 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool SupportsChildDeployables()
	{
		return true;
	}

	// Token: 0x060000C3 RID: 195 RVA: 0x000062E4 File Offset: 0x000044E4
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CargoShip.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x040000B6 RID: 182
	public int targetNodeIndex = -1;

	// Token: 0x040000B7 RID: 183
	public GameObject wakeParent;

	// Token: 0x040000B8 RID: 184
	public GameObjectRef scientistTurretPrefab;

	// Token: 0x040000B9 RID: 185
	public Transform[] scientistSpawnPoints;

	// Token: 0x040000BA RID: 186
	public List<Transform> crateSpawns;

	// Token: 0x040000BB RID: 187
	public GameObjectRef lockedCratePrefab;

	// Token: 0x040000BC RID: 188
	public GameObjectRef militaryCratePrefab;

	// Token: 0x040000BD RID: 189
	public GameObjectRef eliteCratePrefab;

	// Token: 0x040000BE RID: 190
	public GameObjectRef junkCratePrefab;

	// Token: 0x040000BF RID: 191
	public Transform waterLine;

	// Token: 0x040000C0 RID: 192
	public Transform rudder;

	// Token: 0x040000C1 RID: 193
	public Transform propeller;

	// Token: 0x040000C2 RID: 194
	public GameObjectRef escapeBoatPrefab;

	// Token: 0x040000C3 RID: 195
	public Transform escapeBoatPoint;

	// Token: 0x040000C4 RID: 196
	public GameObjectRef microphonePrefab;

	// Token: 0x040000C5 RID: 197
	public Transform microphonePoint;

	// Token: 0x040000C6 RID: 198
	public GameObjectRef speakerPrefab;

	// Token: 0x040000C7 RID: 199
	public Transform[] speakerPoints;

	// Token: 0x040000C8 RID: 200
	public GameObject radiation;

	// Token: 0x040000C9 RID: 201
	public GameObjectRef mapMarkerEntityPrefab;

	// Token: 0x040000CA RID: 202
	public GameObject hornOrigin;

	// Token: 0x040000CB RID: 203
	public SoundDefinition hornDef;

	// Token: 0x040000CC RID: 204
	public CargoShipSounds cargoShipSounds;

	// Token: 0x040000CD RID: 205
	public GameObject[] layouts;

	// Token: 0x040000CE RID: 206
	public GameObjectRef playerTest;

	// Token: 0x040000CF RID: 207
	private uint layoutChoice;

	// Token: 0x040000D0 RID: 208
	[ServerVar]
	public static bool event_enabled = true;

	// Token: 0x040000D1 RID: 209
	[ServerVar]
	public static float event_duration_minutes = 50f;

	// Token: 0x040000D2 RID: 210
	[ServerVar]
	public static float egress_duration_minutes = 10f;

	// Token: 0x040000D3 RID: 211
	[ServerVar]
	public static int loot_rounds = 3;

	// Token: 0x040000D4 RID: 212
	[ServerVar]
	public static float loot_round_spacing_minutes = 10f;

	// Token: 0x040000D5 RID: 213
	private global::BaseEntity mapMarkerInstance;

	// Token: 0x040000D6 RID: 214
	private Vector3 currentVelocity = Vector3.zero;

	// Token: 0x040000D7 RID: 215
	private float currentThrottle;

	// Token: 0x040000D8 RID: 216
	private float currentTurnSpeed;

	// Token: 0x040000D9 RID: 217
	private float turnScale;

	// Token: 0x040000DA RID: 218
	private int lootRoundsPassed;

	// Token: 0x040000DB RID: 219
	private int hornCount;

	// Token: 0x040000DC RID: 220
	private float currentRadiation;

	// Token: 0x040000DD RID: 221
	private bool egressing;
}
