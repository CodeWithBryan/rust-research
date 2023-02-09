using System;
using UnityEngine;

// Token: 0x0200095D RID: 2397
public class ExplosionPlatformActivator : MonoBehaviour
{
	// Token: 0x06003893 RID: 14483 RVA: 0x0014DF93 File Offset: 0x0014C193
	private void Start()
	{
		this.currentRepeatTime = this.DefaultRepeatTime;
		base.Invoke("Init", this.TimeDelay);
	}

	// Token: 0x06003894 RID: 14484 RVA: 0x0014DFB2 File Offset: 0x0014C1B2
	private void Init()
	{
		this.canUpdate = true;
		this.Effect.SetActive(true);
	}

	// Token: 0x06003895 RID: 14485 RVA: 0x0014DFC8 File Offset: 0x0014C1C8
	private void Update()
	{
		if (!this.canUpdate || this.Effect == null)
		{
			return;
		}
		this.currentTime += Time.deltaTime;
		if (this.currentTime > this.currentRepeatTime)
		{
			this.currentTime = 0f;
			this.Effect.SetActive(false);
			this.Effect.SetActive(true);
		}
	}

	// Token: 0x06003896 RID: 14486 RVA: 0x0014E02F File Offset: 0x0014C22F
	private void OnTriggerEnter(Collider coll)
	{
		this.currentRepeatTime = this.NearRepeatTime;
	}

	// Token: 0x06003897 RID: 14487 RVA: 0x0014E03D File Offset: 0x0014C23D
	private void OnTriggerExit(Collider other)
	{
		this.currentRepeatTime = this.DefaultRepeatTime;
	}

	// Token: 0x040032DE RID: 13022
	public GameObject Effect;

	// Token: 0x040032DF RID: 13023
	public float TimeDelay;

	// Token: 0x040032E0 RID: 13024
	public float DefaultRepeatTime = 5f;

	// Token: 0x040032E1 RID: 13025
	public float NearRepeatTime = 3f;

	// Token: 0x040032E2 RID: 13026
	private float currentTime;

	// Token: 0x040032E3 RID: 13027
	private float currentRepeatTime;

	// Token: 0x040032E4 RID: 13028
	private bool canUpdate;
}
