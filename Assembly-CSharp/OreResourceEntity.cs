using System;
using Facepunch.Rust;
using Network;
using UnityEngine;

// Token: 0x020000A2 RID: 162
public class OreResourceEntity : StagedResourceEntity
{
	// Token: 0x06000EDE RID: 3806 RVA: 0x0007C350 File Offset: 0x0007A550
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("OreResourceEntity.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000EDF RID: 3807 RVA: 0x0007C390 File Offset: 0x0007A590
	protected override void UpdateNetworkStage()
	{
		int stage = this.stage;
		base.UpdateNetworkStage();
		if (this.stage != stage && this._hotSpot)
		{
			this.DelayedBonusSpawn();
		}
	}

	// Token: 0x06000EE0 RID: 3808 RVA: 0x0007C3C6 File Offset: 0x0007A5C6
	public void CleanupBonus()
	{
		if (this._hotSpot)
		{
			this._hotSpot.Kill(BaseNetworkable.DestroyMode.None);
		}
		this._hotSpot = null;
	}

	// Token: 0x06000EE1 RID: 3809 RVA: 0x0007C3E8 File Offset: 0x0007A5E8
	public override void DestroyShared()
	{
		base.DestroyShared();
		this.CleanupBonus();
	}

	// Token: 0x06000EE2 RID: 3810 RVA: 0x0007C3F6 File Offset: 0x0007A5F6
	public override void OnKilled(HitInfo info)
	{
		this.CleanupBonus();
		Analytics.Server.OreKilled(this, info);
		base.OnKilled(info);
	}

	// Token: 0x06000EE3 RID: 3811 RVA: 0x0007C40C File Offset: 0x0007A60C
	public override void ServerInit()
	{
		base.ServerInit();
		base.Invoke(new Action(this.InitialSpawnBonusSpot), 0f);
	}

	// Token: 0x06000EE4 RID: 3812 RVA: 0x0007C42B File Offset: 0x0007A62B
	private void InitialSpawnBonusSpot()
	{
		if (base.IsDestroyed)
		{
			return;
		}
		this._hotSpot = this.SpawnBonusSpot(Vector3.zero);
	}

	// Token: 0x06000EE5 RID: 3813 RVA: 0x0007C447 File Offset: 0x0007A647
	public void FinishBonusAssigned()
	{
		Effect.server.Run(this.finishEffect.resourcePath, base.transform.position, base.transform.up, null, false);
	}

	// Token: 0x06000EE6 RID: 3814 RVA: 0x0007C474 File Offset: 0x0007A674
	public override void OnAttacked(HitInfo info)
	{
		if (base.isClient)
		{
			base.OnAttacked(info);
			return;
		}
		if (!info.DidGather && info.gatherScale > 0f)
		{
			Jackhammer jackhammer = info.Weapon as Jackhammer;
			if (this._hotSpot || jackhammer)
			{
				if (this._hotSpot == null)
				{
					this._hotSpot = this.SpawnBonusSpot(this.lastNodeDir);
				}
				if (Vector3.Distance(info.HitPositionWorld, this._hotSpot.transform.position) <= this._hotSpot.GetComponent<SphereCollider>().radius * 1.5f || jackhammer != null)
				{
					float num = (jackhammer == null) ? 1f : jackhammer.HotspotBonusScale;
					this.bonusesKilled++;
					info.gatherScale = 1f + Mathf.Clamp((float)this.bonusesKilled * 0.5f, 0f, 2f * num);
					this._hotSpot.FireFinishEffect();
					base.ClientRPC<int, Vector3>(null, "PlayBonusLevelSound", this.bonusesKilled, this._hotSpot.transform.position);
				}
				else if (this.bonusesKilled > 0)
				{
					this.bonusesKilled = 0;
					Effect.server.Run(this.bonusFailEffect.resourcePath, base.transform.position, base.transform.up, null, false);
				}
				if (this.bonusesKilled > 0)
				{
					this.CleanupBonus();
				}
			}
		}
		if (this._hotSpot == null)
		{
			this.DelayedBonusSpawn();
		}
		base.OnAttacked(info);
	}

	// Token: 0x06000EE7 RID: 3815 RVA: 0x0007C60C File Offset: 0x0007A80C
	public void DelayedBonusSpawn()
	{
		base.CancelInvoke(new Action(this.RespawnBonus));
		base.Invoke(new Action(this.RespawnBonus), 0.25f);
	}

	// Token: 0x06000EE8 RID: 3816 RVA: 0x0007C637 File Offset: 0x0007A837
	public void RespawnBonus()
	{
		this.CleanupBonus();
		this._hotSpot = this.SpawnBonusSpot(this.lastNodeDir);
	}

	// Token: 0x06000EE9 RID: 3817 RVA: 0x0007C654 File Offset: 0x0007A854
	public OreHotSpot SpawnBonusSpot(Vector3 lastDirection)
	{
		if (base.isClient)
		{
			return null;
		}
		if (!this.bonusPrefab.isValid)
		{
			return null;
		}
		Vector2 normalized = UnityEngine.Random.insideUnitCircle.normalized;
		Vector3 vector = Vector3.zero;
		MeshCollider stageComponent = base.GetStageComponent<MeshCollider>();
		Vector3 vector2 = base.transform.InverseTransformPoint(stageComponent.bounds.center);
		if (lastDirection == Vector3.zero)
		{
			Vector3 vector3 = this.RandomCircle(1f, false);
			this.lastNodeDir = vector3.normalized;
			Vector3 vector4 = base.transform.TransformDirection(vector3.normalized);
			vector3 = base.transform.position + base.transform.up * (vector2.y + 0.5f) + vector4.normalized * 2.5f;
			vector = vector3;
		}
		else
		{
			Vector3 a = Vector3.Cross(this.lastNodeDir, Vector3.up);
			float d = UnityEngine.Random.Range(0.25f, 0.5f);
			float d2 = (UnityEngine.Random.Range(0, 2) == 0) ? -1f : 1f;
			Vector3 normalized2 = (this.lastNodeDir + a * d * d2).normalized;
			this.lastNodeDir = normalized2;
			vector = base.transform.position + base.transform.TransformDirection(normalized2) * 2f;
			float num = UnityEngine.Random.Range(1f, 1.5f);
			vector += base.transform.up * (vector2.y + num);
		}
		this.bonusesSpawned++;
		Vector3 normalized3 = (stageComponent.bounds.center - vector).normalized;
		RaycastHit raycastHit;
		if (stageComponent.Raycast(new Ray(vector, normalized3), out raycastHit, 10f))
		{
			OreHotSpot oreHotSpot = GameManager.server.CreateEntity(this.bonusPrefab.resourcePath, raycastHit.point - normalized3 * 0.025f, Quaternion.LookRotation(raycastHit.normal, Vector3.up), true) as OreHotSpot;
			oreHotSpot.Spawn();
			oreHotSpot.SendMessage("OreOwner", this);
			return oreHotSpot;
		}
		return null;
	}

	// Token: 0x06000EEA RID: 3818 RVA: 0x0007C894 File Offset: 0x0007AA94
	public Vector3 RandomCircle(float distance = 1f, bool allowInside = false)
	{
		Vector2 vector = allowInside ? UnityEngine.Random.insideUnitCircle : UnityEngine.Random.insideUnitCircle.normalized;
		return new Vector3(vector.x, 0f, vector.y);
	}

	// Token: 0x06000EEB RID: 3819 RVA: 0x0007C8D0 File Offset: 0x0007AAD0
	public Vector3 RandomHemisphereDirection(Vector3 input, float degreesOffset, bool allowInside = true, bool changeHeight = true)
	{
		degreesOffset = Mathf.Clamp(degreesOffset / 180f, -180f, 180f);
		Vector2 vector = allowInside ? UnityEngine.Random.insideUnitCircle : UnityEngine.Random.insideUnitCircle.normalized;
		Vector3 b = new Vector3(vector.x * degreesOffset, changeHeight ? (UnityEngine.Random.Range(-1f, 1f) * degreesOffset) : 0f, vector.y * degreesOffset);
		return (input + b).normalized;
	}

	// Token: 0x06000EEC RID: 3820 RVA: 0x0007C950 File Offset: 0x0007AB50
	public Vector3 ClampToHemisphere(Vector3 hemiInput, float degreesOffset, Vector3 inputVec)
	{
		degreesOffset = Mathf.Clamp(degreesOffset / 180f, -180f, 180f);
		Vector3 normalized = (hemiInput + Vector3.one * degreesOffset).normalized;
		Vector3 normalized2 = (hemiInput + Vector3.one * -degreesOffset).normalized;
		for (int i = 0; i < 3; i++)
		{
			inputVec[i] = Mathf.Clamp(inputVec[i], normalized2[i], normalized[i]);
		}
		return inputVec;
	}

	// Token: 0x06000EED RID: 3821 RVA: 0x0007C9DC File Offset: 0x0007ABDC
	public static Vector3 RandomCylinderPointAroundVector(Vector3 input, float distance, float minHeight = 0f, float maxHeight = 0f, bool allowInside = false)
	{
		Vector2 vector = allowInside ? UnityEngine.Random.insideUnitCircle : UnityEngine.Random.insideUnitCircle.normalized;
		Vector3 result = new Vector3(vector.x, 0f, vector.y).normalized * distance;
		result.y = UnityEngine.Random.Range(minHeight, maxHeight);
		return result;
	}

	// Token: 0x06000EEE RID: 3822 RVA: 0x00029180 File Offset: 0x00027380
	public Vector3 ClampToCylinder(Vector3 localPos, Vector3 cylinderAxis, float cylinderDistance, float minHeight = 0f, float maxHeight = 0f)
	{
		return Vector3.zero;
	}

	// Token: 0x040009BE RID: 2494
	public GameObjectRef bonusPrefab;

	// Token: 0x040009BF RID: 2495
	public GameObjectRef finishEffect;

	// Token: 0x040009C0 RID: 2496
	public GameObjectRef bonusFailEffect;

	// Token: 0x040009C1 RID: 2497
	public OreHotSpot _hotSpot;

	// Token: 0x040009C2 RID: 2498
	public SoundPlayer bonusSound;

	// Token: 0x040009C3 RID: 2499
	private int bonusesKilled;

	// Token: 0x040009C4 RID: 2500
	private int bonusesSpawned;

	// Token: 0x040009C5 RID: 2501
	private Vector3 lastNodeDir = Vector3.zero;
}
