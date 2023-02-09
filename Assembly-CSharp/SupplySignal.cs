using System;
using UnityEngine;

// Token: 0x02000403 RID: 1027
public class SupplySignal : TimedExplosive
{
	// Token: 0x06002291 RID: 8849 RVA: 0x000DD2E8 File Offset: 0x000DB4E8
	public override void Explode()
	{
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.EntityToCreate.resourcePath, default(Vector3), default(Quaternion), true);
		if (baseEntity)
		{
			Vector3 b = new Vector3(UnityEngine.Random.Range(-20f, 20f), 0f, UnityEngine.Random.Range(-20f, 20f));
			baseEntity.SendMessage("InitDropPosition", base.transform.position + b, SendMessageOptions.DontRequireReceiver);
			baseEntity.Spawn();
		}
		base.Invoke(new Action(this.FinishUp), 210f);
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06002292 RID: 8850 RVA: 0x000029D4 File Offset: 0x00000BD4
	public void FinishUp()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x04001B08 RID: 6920
	public GameObjectRef smokeEffectPrefab;

	// Token: 0x04001B09 RID: 6921
	public GameObjectRef EntityToCreate;

	// Token: 0x04001B0A RID: 6922
	[NonSerialized]
	public GameObject smokeEffect;
}
