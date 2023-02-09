using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000408 RID: 1032
public class WaterBall : BaseEntity
{
	// Token: 0x060022A7 RID: 8871 RVA: 0x000DDB08 File Offset: 0x000DBD08
	public override void ServerInit()
	{
		base.ServerInit();
		base.Invoke(new Action(this.Extinguish), 10f);
	}

	// Token: 0x060022A8 RID: 8872 RVA: 0x000DDB27 File Offset: 0x000DBD27
	public void Extinguish()
	{
		base.CancelInvoke(new Action(this.Extinguish));
		if (!base.IsDestroyed)
		{
			base.Kill(BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x060022A9 RID: 8873 RVA: 0x000DDB4A File Offset: 0x000DBD4A
	public void FixedUpdate()
	{
		if (base.isServer)
		{
			base.GetComponent<Rigidbody>().AddForce(Physics.gravity, ForceMode.Acceleration);
		}
	}

	// Token: 0x060022AA RID: 8874 RVA: 0x000DDB68 File Offset: 0x000DBD68
	public static bool DoSplash(Vector3 position, float radius, ItemDefinition liquidDef, int amount)
	{
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities<BaseEntity>(position, radius, list, 1219701523, QueryTriggerInteraction.Collide);
		int num = 0;
		int num2 = amount;
		while (amount > 0 && num < 3)
		{
			List<ISplashable> list2 = Pool.GetList<ISplashable>();
			foreach (BaseEntity baseEntity in list)
			{
				if (!baseEntity.isClient)
				{
					ISplashable splashable = baseEntity as ISplashable;
					if (splashable != null && !list2.Contains(splashable) && splashable.WantsSplash(liquidDef, amount))
					{
						list2.Add(splashable);
					}
				}
			}
			if (list2.Count == 0)
			{
				break;
			}
			int b = Mathf.CeilToInt((float)(amount / list2.Count));
			foreach (ISplashable splashable2 in list2)
			{
				int num3 = splashable2.DoSplash(liquidDef, Mathf.Min(amount, b));
				amount -= num3;
				if (amount <= 0)
				{
					break;
				}
			}
			Pool.FreeList<ISplashable>(ref list2);
			num++;
		}
		Pool.FreeList<BaseEntity>(ref list);
		return amount < num2;
	}

	// Token: 0x060022AB RID: 8875 RVA: 0x000DDC94 File Offset: 0x000DBE94
	private void OnCollisionEnter(Collision collision)
	{
		if (base.isClient)
		{
			return;
		}
		if (this.myRigidBody.isKinematic)
		{
			return;
		}
		float num = 2.5f;
		WaterBall.DoSplash(base.transform.position + new Vector3(0f, num * 0.75f, 0f), num, this.liquidType, this.waterAmount);
		Effect.server.Run(this.waterExplosion.resourcePath, base.transform.position + new Vector3(0f, 0f, 0f), Vector3.up, null, false);
		this.myRigidBody.isKinematic = true;
		base.Invoke(new Action(this.Extinguish), 2f);
	}

	// Token: 0x04001B18 RID: 6936
	public ItemDefinition liquidType;

	// Token: 0x04001B19 RID: 6937
	public int waterAmount;

	// Token: 0x04001B1A RID: 6938
	public GameObjectRef waterExplosion;

	// Token: 0x04001B1B RID: 6939
	public Rigidbody myRigidBody;
}
