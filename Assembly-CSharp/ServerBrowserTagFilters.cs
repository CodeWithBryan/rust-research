using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000863 RID: 2147
public class ServerBrowserTagFilters : MonoBehaviour
{
	// Token: 0x06003529 RID: 13609 RVA: 0x0013FE88 File Offset: 0x0013E088
	public void Start()
	{
		this._groups = base.gameObject.GetComponentsInChildren<ServerBrowserTagGroup>();
		UnityAction call = delegate()
		{
			UnityEvent tagFiltersChanged = this.TagFiltersChanged;
			if (tagFiltersChanged == null)
			{
				return;
			}
			tagFiltersChanged.Invoke();
		};
		ServerBrowserTagGroup[] groups = this._groups;
		for (int i = 0; i < groups.Length; i++)
		{
			foreach (ServerBrowserTag serverBrowserTag in groups[i].tags)
			{
				serverBrowserTag.button.OnPressed.AddListener(call);
				serverBrowserTag.button.OnReleased.AddListener(call);
			}
		}
	}

	// Token: 0x0600352A RID: 13610 RVA: 0x0013FF08 File Offset: 0x0013E108
	public void DeselectAll()
	{
		if (this._groups == null)
		{
			return;
		}
		foreach (ServerBrowserTagGroup serverBrowserTagGroup in this._groups)
		{
			if (serverBrowserTagGroup.tags != null)
			{
				ServerBrowserTag[] tags = serverBrowserTagGroup.tags;
				for (int j = 0; j < tags.Length; j++)
				{
					tags[j].button.SetToggleFalse();
				}
			}
		}
	}

	// Token: 0x0600352B RID: 13611 RVA: 0x0013FF68 File Offset: 0x0013E168
	public void GetTags(out List<HashSet<string>> searchTagGroups, out HashSet<string> excludeTags)
	{
		searchTagGroups = new List<HashSet<string>>();
		excludeTags = new HashSet<string>();
		foreach (ServerBrowserTagGroup serverBrowserTagGroup in this._groups)
		{
			if (serverBrowserTagGroup.AnyActive())
			{
				if (serverBrowserTagGroup.isExclusive)
				{
					HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
					foreach (ServerBrowserTag serverBrowserTag in serverBrowserTagGroup.tags)
					{
						if (serverBrowserTag.IsActive)
						{
							hashSet.Add(serverBrowserTag.serverTag);
						}
						else if (serverBrowserTagGroup.isExclusive)
						{
							excludeTags.Add(serverBrowserTag.serverTag);
						}
					}
					if (hashSet.Count > 0)
					{
						searchTagGroups.Add(hashSet);
					}
				}
				else
				{
					foreach (ServerBrowserTag serverBrowserTag2 in serverBrowserTagGroup.tags)
					{
						if (serverBrowserTag2.IsActive)
						{
							HashSet<string> hashSet2 = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
							hashSet2.Add(serverBrowserTag2.serverTag);
							searchTagGroups.Add(hashSet2);
						}
					}
				}
			}
		}
	}

	// Token: 0x04002FA7 RID: 12199
	public UnityEvent TagFiltersChanged = new UnityEvent();

	// Token: 0x04002FA8 RID: 12200
	private ServerBrowserTagGroup[] _groups;

	// Token: 0x04002FA9 RID: 12201
	private List<bool> _previousState;
}
