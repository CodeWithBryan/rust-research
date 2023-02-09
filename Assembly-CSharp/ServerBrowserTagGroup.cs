using System;
using Facepunch;
using UnityEngine;

// Token: 0x02000864 RID: 2148
public class ServerBrowserTagGroup : MonoBehaviour
{
	// Token: 0x0600352E RID: 13614 RVA: 0x00140099 File Offset: 0x0013E299
	private void Initialize()
	{
		if (this.tags == null)
		{
			this.tags = base.GetComponentsInChildren<ServerBrowserTag>(true);
		}
	}

	// Token: 0x0600352F RID: 13615 RVA: 0x001400B0 File Offset: 0x0013E2B0
	public void Awake()
	{
		this.Initialize();
	}

	// Token: 0x06003530 RID: 13616 RVA: 0x001400B8 File Offset: 0x0013E2B8
	public bool AnyActive()
	{
		ServerBrowserTag[] array = this.tags;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].IsActive)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06003531 RID: 13617 RVA: 0x001400E8 File Offset: 0x0013E2E8
	public void Refresh(in ServerInfo server, ref int tagsEnabled, int maxTags)
	{
		this.Initialize();
		bool flag = false;
		foreach (ServerBrowserTag serverBrowserTag in this.tags)
		{
			if ((!this.isExclusive || !flag) && tagsEnabled <= maxTags && server.Tags.Contains(serverBrowserTag.serverTag))
			{
				serverBrowserTag.SetActive(true);
				tagsEnabled++;
				flag = true;
			}
			else
			{
				serverBrowserTag.SetActive(false);
			}
		}
		base.gameObject.SetActive(flag);
	}

	// Token: 0x04002FAA RID: 12202
	[Tooltip("If set then queries will filter out servers matching unselected tags in the group")]
	public bool isExclusive;

	// Token: 0x04002FAB RID: 12203
	[NonSerialized]
	public ServerBrowserTag[] tags;
}
