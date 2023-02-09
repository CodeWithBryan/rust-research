using System;
using System.Collections.Generic;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;

// Token: 0x020000D1 RID: 209
public class StagedResourceEntity : ResourceEntity
{
	// Token: 0x06001248 RID: 4680 RVA: 0x00092EE8 File Offset: 0x000910E8
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("StagedResourceEntity.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001249 RID: 4681 RVA: 0x00092F28 File Offset: 0x00091128
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.resource == null)
		{
			return;
		}
		int num = info.msg.resource.stage;
		if (info.fromDisk && base.isServer)
		{
			this.health = this.startHealth;
			num = 0;
		}
		if (num != this.stage)
		{
			this.stage = num;
			this.UpdateStage();
		}
	}

	// Token: 0x0600124A RID: 4682 RVA: 0x00092F94 File Offset: 0x00091194
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.resource == null)
		{
			info.msg.resource = Pool.Get<BaseResource>();
		}
		info.msg.resource.health = this.Health();
		info.msg.resource.stage = this.stage;
	}

	// Token: 0x0600124B RID: 4683 RVA: 0x00092FF1 File Offset: 0x000911F1
	protected override void OnHealthChanged()
	{
		base.Invoke(new Action(this.UpdateNetworkStage), 0.1f);
	}

	// Token: 0x0600124C RID: 4684 RVA: 0x0009300B File Offset: 0x0009120B
	protected virtual void UpdateNetworkStage()
	{
		if (this.FindBestStage() != this.stage)
		{
			this.stage = this.FindBestStage();
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			this.UpdateStage();
		}
	}

	// Token: 0x0600124D RID: 4685 RVA: 0x00093034 File Offset: 0x00091234
	private int FindBestStage()
	{
		float num = Mathf.InverseLerp(0f, this.MaxHealth(), this.Health());
		for (int i = 0; i < this.stages.Count; i++)
		{
			if (num >= this.stages[i].health)
			{
				return i;
			}
		}
		return this.stages.Count - 1;
	}

	// Token: 0x0600124E RID: 4686 RVA: 0x00093091 File Offset: 0x00091291
	public T GetStageComponent<T>() where T : Component
	{
		return this.stages[this.stage].instance.GetComponentInChildren<T>();
	}

	// Token: 0x0600124F RID: 4687 RVA: 0x000930B0 File Offset: 0x000912B0
	private void UpdateStage()
	{
		if (this.stages.Count == 0)
		{
			return;
		}
		for (int i = 0; i < this.stages.Count; i++)
		{
			this.stages[i].instance.SetActive(i == this.stage);
		}
		GroundWatch.PhysicsChanged(base.gameObject);
	}

	// Token: 0x04000B78 RID: 2936
	public List<StagedResourceEntity.ResourceStage> stages = new List<StagedResourceEntity.ResourceStage>();

	// Token: 0x04000B79 RID: 2937
	public int stage;

	// Token: 0x04000B7A RID: 2938
	public GameObjectRef changeStageEffect;

	// Token: 0x04000B7B RID: 2939
	public GameObject gibSourceTest;

	// Token: 0x02000BBB RID: 3003
	[Serializable]
	public class ResourceStage
	{
		// Token: 0x04003F56 RID: 16214
		public float health;

		// Token: 0x04003F57 RID: 16215
		public GameObject instance;
	}
}
