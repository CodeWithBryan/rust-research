using System;
using UnityEngine;

// Token: 0x02000865 RID: 2149
public class ServerBrowserTagList : MonoBehaviour
{
	// Token: 0x06003533 RID: 13619 RVA: 0x00140160 File Offset: 0x0013E360
	private void Initialize()
	{
		if (this._groups == null)
		{
			this._groups = base.GetComponentsInChildren<ServerBrowserTagGroup>(true);
		}
	}

	// Token: 0x06003534 RID: 13620 RVA: 0x00140177 File Offset: 0x0013E377
	public void Awake()
	{
		this.Initialize();
	}

	// Token: 0x06003535 RID: 13621 RVA: 0x00140180 File Offset: 0x0013E380
	public bool Refresh(in ServerInfo server)
	{
		this.Initialize();
		int num = 0;
		ServerBrowserTagGroup[] groups = this._groups;
		for (int i = 0; i < groups.Length; i++)
		{
			groups[i].Refresh(server, ref num, this.maxTagsToShow);
		}
		return num > 0;
	}

	// Token: 0x04002FAC RID: 12204
	public int maxTagsToShow = 3;

	// Token: 0x04002FAD RID: 12205
	private ServerBrowserTagGroup[] _groups;
}
