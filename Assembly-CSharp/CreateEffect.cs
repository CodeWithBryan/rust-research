using System;
using UnityEngine;

// Token: 0x020004C7 RID: 1223
public class CreateEffect : MonoBehaviour
{
	// Token: 0x06002765 RID: 10085 RVA: 0x000F28DF File Offset: 0x000F0ADF
	public void OnEnable()
	{
		Effect.client.Run(this.EffectToCreate.resourcePath, base.transform.position, base.transform.up, base.transform.forward, Effect.Type.Generic);
	}

	// Token: 0x04001FAD RID: 8109
	public GameObjectRef EffectToCreate;
}
