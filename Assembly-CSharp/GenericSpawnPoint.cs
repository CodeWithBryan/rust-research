using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000543 RID: 1347
public class GenericSpawnPoint : BaseSpawnPoint
{
	// Token: 0x0600290A RID: 10506 RVA: 0x000F9D74 File Offset: 0x000F7F74
	public Quaternion GetRandomRotation()
	{
		if (!this.randomRot)
		{
			return Quaternion.identity;
		}
		int max = Mathf.FloorToInt(360f / this.randomRotSnapDegrees);
		int num = UnityEngine.Random.Range(0, max);
		return Quaternion.Euler(0f, (float)num * this.randomRotSnapDegrees, 0f);
	}

	// Token: 0x0600290B RID: 10507 RVA: 0x000F9DC4 File Offset: 0x000F7FC4
	public override void GetLocation(out Vector3 pos, out Quaternion rot)
	{
		pos = base.transform.position;
		if (this.randomRot)
		{
			rot = base.transform.rotation * this.GetRandomRotation();
		}
		else
		{
			rot = base.transform.rotation;
		}
		if (this.dropToGround)
		{
			base.DropToGround(ref pos, ref rot);
		}
	}

	// Token: 0x0600290C RID: 10508 RVA: 0x000F9E2C File Offset: 0x000F802C
	public override void ObjectSpawned(SpawnPointInstance instance)
	{
		if (this.spawnEffect.isValid)
		{
			Effect.server.Run(this.spawnEffect.resourcePath, instance.GetComponent<BaseEntity>(), 0U, Vector3.zero, Vector3.up, null, false);
		}
		this.OnObjectSpawnedEvent.Invoke();
		base.gameObject.SetActive(false);
	}

	// Token: 0x0600290D RID: 10509 RVA: 0x000F9E80 File Offset: 0x000F8080
	public override void ObjectRetired(SpawnPointInstance instance)
	{
		this.OnObjectRetiredEvent.Invoke();
		base.gameObject.SetActive(true);
	}

	// Token: 0x04002152 RID: 8530
	public bool dropToGround = true;

	// Token: 0x04002153 RID: 8531
	public bool randomRot;

	// Token: 0x04002154 RID: 8532
	[Range(1f, 180f)]
	public float randomRotSnapDegrees = 1f;

	// Token: 0x04002155 RID: 8533
	public GameObjectRef spawnEffect;

	// Token: 0x04002156 RID: 8534
	public UnityEvent OnObjectSpawnedEvent = new UnityEvent();

	// Token: 0x04002157 RID: 8535
	public UnityEvent OnObjectRetiredEvent = new UnityEvent();
}
