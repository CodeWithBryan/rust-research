using System;
using System.Collections.Generic;
using UnityEngine;

namespace Facepunch.GUI
{
	// Token: 0x02000ABA RID: 2746
	internal class TabbedPanel
	{
		// Token: 0x170005BA RID: 1466
		// (get) Token: 0x0600427A RID: 17018 RVA: 0x00184365 File Offset: 0x00182565
		public TabbedPanel.Tab selectedTab
		{
			get
			{
				return this.tabs[this.selectedTabID];
			}
		}

		// Token: 0x0600427B RID: 17019 RVA: 0x00184378 File Offset: 0x00182578
		public void Add(TabbedPanel.Tab tab)
		{
			this.tabs.Add(tab);
		}

		// Token: 0x0600427C RID: 17020 RVA: 0x00184388 File Offset: 0x00182588
		internal void DrawVertical(float width)
		{
			GUILayout.BeginVertical(new GUILayoutOption[]
			{
				GUILayout.Width(width),
				GUILayout.ExpandHeight(true)
			});
			for (int i = 0; i < this.tabs.Count; i++)
			{
				if (GUILayout.Toggle(this.selectedTabID == i, this.tabs[i].name, new GUIStyle("devtab"), Array.Empty<GUILayoutOption>()))
				{
					this.selectedTabID = i;
				}
			}
			if (GUILayout.Toggle(false, "", new GUIStyle("devtab"), new GUILayoutOption[]
			{
				GUILayout.ExpandHeight(true)
			}))
			{
				this.selectedTabID = -1;
			}
			GUILayout.EndVertical();
		}

		// Token: 0x0600427D RID: 17021 RVA: 0x0018443C File Offset: 0x0018263C
		internal void DrawContents()
		{
			if (this.selectedTabID < 0)
			{
				return;
			}
			TabbedPanel.Tab selectedTab = this.selectedTab;
			GUILayout.BeginVertical(new GUIStyle("devtabcontents"), new GUILayoutOption[]
			{
				GUILayout.ExpandHeight(true),
				GUILayout.ExpandWidth(true)
			});
			if (selectedTab.drawFunc != null)
			{
				selectedTab.drawFunc();
			}
			GUILayout.EndVertical();
		}

		// Token: 0x04003ABA RID: 15034
		private int selectedTabID;

		// Token: 0x04003ABB RID: 15035
		private List<TabbedPanel.Tab> tabs = new List<TabbedPanel.Tab>();

		// Token: 0x02000F1D RID: 3869
		public struct Tab
		{
			// Token: 0x04004D40 RID: 19776
			public string name;

			// Token: 0x04004D41 RID: 19777
			public Action drawFunc;
		}
	}
}
