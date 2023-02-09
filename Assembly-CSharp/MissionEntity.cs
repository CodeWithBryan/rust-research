using System;
using UnityEngine;

// Token: 0x020005E3 RID: 1507
public class MissionEntity : BaseMonoBehaviour, IOnParentDestroying
{
	// Token: 0x06002C4A RID: 11338 RVA: 0x00109B88 File Offset: 0x00107D88
	public void OnParentDestroying()
	{
		UnityEngine.Object.Destroy(this);
	}

	// Token: 0x06002C4B RID: 11339 RVA: 0x00109B90 File Offset: 0x00107D90
	public virtual void Setup(BasePlayer assignee, BaseMission.MissionInstance instance, bool wantsSuccessCleanup, bool wantsFailedCleanup)
	{
		this.cleanupOnMissionFailed = wantsFailedCleanup;
		this.cleanupOnMissionSuccess = wantsSuccessCleanup;
		BaseEntity entity = this.GetEntity();
		if (entity)
		{
			entity.SendMessage("MissionSetupPlayer", assignee, SendMessageOptions.DontRequireReceiver);
		}
	}

	// Token: 0x06002C4C RID: 11340 RVA: 0x00109BC8 File Offset: 0x00107DC8
	public virtual void MissionStarted(BasePlayer assignee, BaseMission.MissionInstance instance)
	{
		IMissionEntityListener[] componentsInChildren = base.GetComponentsInChildren<IMissionEntityListener>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].MissionStarted(assignee, instance);
		}
	}

	// Token: 0x06002C4D RID: 11341 RVA: 0x00109BF4 File Offset: 0x00107DF4
	public virtual void MissionEnded(BasePlayer assignee, BaseMission.MissionInstance instance)
	{
		IMissionEntityListener[] componentsInChildren = base.GetComponentsInChildren<IMissionEntityListener>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].MissionEnded(assignee, instance);
		}
		if (instance.createdEntities.Contains(this))
		{
			instance.createdEntities.Remove(this);
		}
		if ((this.cleanupOnMissionSuccess && (instance.status == BaseMission.MissionStatus.Completed || instance.status == BaseMission.MissionStatus.Accomplished)) || (this.cleanupOnMissionFailed && instance.status == BaseMission.MissionStatus.Failed))
		{
			BaseEntity entity = this.GetEntity();
			if (entity)
			{
				entity.Kill(BaseNetworkable.DestroyMode.None);
			}
		}
	}

	// Token: 0x06002C4E RID: 11342 RVA: 0x00109C84 File Offset: 0x00107E84
	public BaseEntity GetEntity()
	{
		return base.GetComponent<BaseEntity>();
	}

	// Token: 0x04002421 RID: 9249
	public bool cleanupOnMissionSuccess = true;

	// Token: 0x04002422 RID: 9250
	public bool cleanupOnMissionFailed = true;
}
