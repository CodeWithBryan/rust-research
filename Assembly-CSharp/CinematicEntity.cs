using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002E5 RID: 741
public class CinematicEntity : BaseEntity
{
	// Token: 0x1700022D RID: 557
	// (get) Token: 0x06001D33 RID: 7475 RVA: 0x000C768C File Offset: 0x000C588C
	// (set) Token: 0x06001D34 RID: 7476 RVA: 0x000C7694 File Offset: 0x000C5894
	[ServerVar(Help = "Hides cinematic light source meshes (keeps lights visible)")]
	public static bool HideObjects
	{
		get
		{
			return CinematicEntity._hideObjects;
		}
		set
		{
			if (value != CinematicEntity._hideObjects)
			{
				CinematicEntity._hideObjects = value;
				foreach (CinematicEntity cinematicEntity in CinematicEntity.serverList)
				{
					cinematicEntity.SetFlag(BaseEntity.Flags.Reserved1, CinematicEntity._hideObjects, false, true);
				}
			}
		}
	}

	// Token: 0x06001D35 RID: 7477 RVA: 0x000C7700 File Offset: 0x000C5900
	public override void ServerInit()
	{
		base.ServerInit();
		if (!CinematicEntity.serverList.Contains(this))
		{
			CinematicEntity.serverList.Add(this);
		}
		base.SetFlag(BaseEntity.Flags.Reserved1, CinematicEntity.HideObjects, false, true);
	}

	// Token: 0x06001D36 RID: 7478 RVA: 0x000C7732 File Offset: 0x000C5932
	public override void DestroyShared()
	{
		base.DestroyShared();
		if (base.isServer && CinematicEntity.serverList.Contains(this))
		{
			CinematicEntity.serverList.Remove(this);
		}
	}

	// Token: 0x06001D37 RID: 7479 RVA: 0x000C775C File Offset: 0x000C595C
	public override void OnFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		bool state = !base.HasFlag(BaseEntity.Flags.Reserved1);
		this.ToggleObjects(state);
	}

	// Token: 0x06001D38 RID: 7480 RVA: 0x000C7788 File Offset: 0x000C5988
	private void ToggleObjects(bool state)
	{
		foreach (GameObject gameObject in this.DisableObjects)
		{
			if (gameObject != null)
			{
				gameObject.SetActive(state);
			}
		}
	}

	// Token: 0x040016AC RID: 5804
	private const BaseEntity.Flags HideMesh = BaseEntity.Flags.Reserved1;

	// Token: 0x040016AD RID: 5805
	public GameObject[] DisableObjects;

	// Token: 0x040016AE RID: 5806
	private static bool _hideObjects = false;

	// Token: 0x040016AF RID: 5807
	private static List<CinematicEntity> serverList = new List<CinematicEntity>();
}
