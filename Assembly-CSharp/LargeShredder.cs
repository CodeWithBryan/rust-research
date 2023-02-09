using System;
using Rust;
using UnityEngine;

// Token: 0x02000414 RID: 1044
public class LargeShredder : BaseEntity
{
	// Token: 0x060022F0 RID: 8944 RVA: 0x000DE774 File Offset: 0x000DC974
	public virtual void OnEntityEnteredTrigger(BaseEntity ent)
	{
		if (ent.IsDestroyed)
		{
			return;
		}
		Rigidbody component = ent.GetComponent<Rigidbody>();
		if (this.isShredding || this.currentlyShredding != null)
		{
			component.velocity = -component.velocity * 3f;
			return;
		}
		this.shredRail.transform.position = this.shredRailStartPos.position;
		this.shredRail.transform.rotation = Quaternion.LookRotation(this.shredRailStartRotation);
		this.entryRotation = ent.transform.rotation;
		Quaternion rotation = ent.transform.rotation;
		component.isKinematic = true;
		this.currentlyShredding = ent;
		ent.transform.rotation = rotation;
		this.isShredding = true;
		this.SetShredding(true);
		this.shredStartTime = Time.realtimeSinceStartup;
	}

	// Token: 0x060022F1 RID: 8945 RVA: 0x000DE84C File Offset: 0x000DCA4C
	public void CreateShredResources()
	{
		if (this.currentlyShredding == null)
		{
			return;
		}
		MagnetLiftable component = this.currentlyShredding.GetComponent<MagnetLiftable>();
		if (component == null)
		{
			return;
		}
		if (component.associatedPlayer != null && GameInfo.HasAchievements)
		{
			component.associatedPlayer.stats.Add("cars_shredded", 1, Stats.Steam);
			component.associatedPlayer.stats.Save(true);
		}
		foreach (ItemAmount itemAmount in component.shredResources)
		{
			Item item = ItemManager.Create(itemAmount.itemDef, (int)itemAmount.amount, 0UL);
			float num = 0.5f;
			if (item.CreateWorldObject(this.resourceSpawnPoint.transform.position + new Vector3(UnityEngine.Random.Range(-num, num), 1f, UnityEngine.Random.Range(-num, num)), default(Quaternion), null, 0U) == null)
			{
				item.Remove(0f);
			}
		}
		BaseModularVehicle component2 = this.currentlyShredding.GetComponent<BaseModularVehicle>();
		if (component2)
		{
			foreach (BaseVehicleModule baseVehicleModule in component2.AttachedModuleEntities)
			{
				if (baseVehicleModule.AssociatedItemDef && baseVehicleModule.AssociatedItemDef.Blueprint)
				{
					foreach (ItemAmount itemAmount2 in baseVehicleModule.AssociatedItemDef.Blueprint.ingredients)
					{
						int num2 = Mathf.FloorToInt(itemAmount2.amount * 0.5f);
						if (num2 != 0)
						{
							Item item2 = ItemManager.Create(itemAmount2.itemDef, num2, 0UL);
							float num3 = 0.5f;
							if (item2.CreateWorldObject(this.resourceSpawnPoint.transform.position + new Vector3(UnityEngine.Random.Range(-num3, num3), 1f, UnityEngine.Random.Range(-num3, num3)), default(Quaternion), null, 0U) == null)
							{
								item2.Remove(0f);
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060022F2 RID: 8946 RVA: 0x000DEACC File Offset: 0x000DCCCC
	public void UpdateBonePosition(float delta)
	{
		float t = delta / this.shredDurationPosition;
		float t2 = delta / this.shredDurationRotation;
		this.shredRail.transform.localPosition = Vector3.Lerp(this.shredRailStartPos.localPosition, this.shredRailEndPos.localPosition, t);
		this.shredRail.transform.rotation = Quaternion.LookRotation(Vector3.Lerp(this.shredRailStartRotation, this.shredRailEndRotation, t2));
	}

	// Token: 0x060022F3 RID: 8947 RVA: 0x000DEB3E File Offset: 0x000DCD3E
	public void SetShredding(bool isShredding)
	{
		if (isShredding)
		{
			base.InvokeRandomized(new Action(this.FireShredEffect), 0.25f, 0.75f, 0.25f);
			return;
		}
		base.CancelInvoke(new Action(this.FireShredEffect));
	}

	// Token: 0x060022F4 RID: 8948 RVA: 0x000DEB77 File Offset: 0x000DCD77
	public void FireShredEffect()
	{
		Effect.server.Run(this.shredSoundEffect.resourcePath, base.transform.position + Vector3.up * 3f, Vector3.up, null, false);
	}

	// Token: 0x060022F5 RID: 8949 RVA: 0x000DEBB0 File Offset: 0x000DCDB0
	public void ServerUpdate()
	{
		if (base.isClient)
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.Reserved10, this.isShredding, false, true);
		if (this.isShredding)
		{
			float num = Time.realtimeSinceStartup - this.shredStartTime;
			float t = num / this.shredDurationPosition;
			float t2 = num / this.shredDurationRotation;
			this.shredRail.transform.localPosition = Vector3.Lerp(this.shredRailStartPos.localPosition, this.shredRailEndPos.localPosition, t);
			this.shredRail.transform.rotation = Quaternion.LookRotation(Vector3.Lerp(this.shredRailStartRotation, this.shredRailEndRotation, t2));
			MagnetLiftable component = this.currentlyShredding.GetComponent<MagnetLiftable>();
			this.currentlyShredding.transform.position = this.shredRail.transform.position;
			Vector3 vector = base.transform.TransformDirection(component.shredDirection);
			if (Vector3.Dot(-vector, this.currentlyShredding.transform.forward) > Vector3.Dot(vector, this.currentlyShredding.transform.forward))
			{
				vector = base.transform.TransformDirection(-component.shredDirection);
			}
			bool flag = Vector3.Dot(component.transform.up, Vector3.up) >= -0.95f;
			Quaternion quaternion = QuaternionEx.LookRotationForcedUp(vector, flag ? (-base.transform.right) : base.transform.right);
			float num2 = Time.time * this.shredSwaySpeed;
			float num3 = Mathf.PerlinNoise(num2, 0f);
			float num4 = Mathf.PerlinNoise(0f, num2 + 150f);
			quaternion *= Quaternion.Euler(num3 * this.shredSwayAmount, 0f, num4 * this.shredSwayAmount);
			this.currentlyShredding.transform.rotation = Quaternion.Lerp(this.entryRotation, quaternion, t2);
			if (num > 5f)
			{
				this.CreateShredResources();
				this.currentlyShredding.Kill(BaseNetworkable.DestroyMode.None);
				this.currentlyShredding = null;
				this.isShredding = false;
				this.SetShredding(false);
			}
		}
	}

	// Token: 0x060022F6 RID: 8950 RVA: 0x000DEDC8 File Offset: 0x000DCFC8
	private void Update()
	{
		this.ServerUpdate();
	}

	// Token: 0x04001B4E RID: 6990
	public Transform shredRail;

	// Token: 0x04001B4F RID: 6991
	public Transform shredRailStartPos;

	// Token: 0x04001B50 RID: 6992
	public Transform shredRailEndPos;

	// Token: 0x04001B51 RID: 6993
	public Vector3 shredRailStartRotation;

	// Token: 0x04001B52 RID: 6994
	public Vector3 shredRailEndRotation;

	// Token: 0x04001B53 RID: 6995
	public LargeShredderTrigger trigger;

	// Token: 0x04001B54 RID: 6996
	public float shredDurationRotation = 2f;

	// Token: 0x04001B55 RID: 6997
	public float shredDurationPosition = 5f;

	// Token: 0x04001B56 RID: 6998
	public float shredSwayAmount = 1f;

	// Token: 0x04001B57 RID: 6999
	public float shredSwaySpeed = 3f;

	// Token: 0x04001B58 RID: 7000
	public BaseEntity currentlyShredding;

	// Token: 0x04001B59 RID: 7001
	public GameObject[] shreddingWheels;

	// Token: 0x04001B5A RID: 7002
	public float shredRotorSpeed = 1f;

	// Token: 0x04001B5B RID: 7003
	public GameObjectRef shredSoundEffect;

	// Token: 0x04001B5C RID: 7004
	public Transform resourceSpawnPoint;

	// Token: 0x04001B5D RID: 7005
	private Quaternion entryRotation;

	// Token: 0x04001B5E RID: 7006
	public const string SHRED_STAT = "cars_shredded";

	// Token: 0x04001B5F RID: 7007
	private bool isShredding;

	// Token: 0x04001B60 RID: 7008
	private float shredStartTime;
}
