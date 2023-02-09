using System;
using TMPro;
using UnityEngine;

namespace Facepunch.UI
{
	// Token: 0x02000AAE RID: 2734
	public class ESPPlayerInfo : MonoBehaviour
	{
		// Token: 0x1700058B RID: 1419
		// (get) Token: 0x06004178 RID: 16760 RVA: 0x001804EE File Offset: 0x0017E6EE
		// (set) Token: 0x06004179 RID: 16761 RVA: 0x001804F6 File Offset: 0x0017E6F6
		public BasePlayer Entity { get; set; }

		// Token: 0x04003A64 RID: 14948
		public Vector3 WorldOffset;

		// Token: 0x04003A65 RID: 14949
		public TextMeshProUGUI Text;

		// Token: 0x04003A66 RID: 14950
		public TextMeshProUGUI Image;

		// Token: 0x04003A67 RID: 14951
		public CanvasGroup group;

		// Token: 0x04003A68 RID: 14952
		public Gradient gradientNormal;

		// Token: 0x04003A69 RID: 14953
		public Gradient gradientTeam;

		// Token: 0x04003A6A RID: 14954
		public Color TeamColor;

		// Token: 0x04003A6B RID: 14955
		public Color AllyColor = Color.blue;

		// Token: 0x04003A6C RID: 14956
		public Color EnemyColor;

		// Token: 0x04003A6D RID: 14957
		public QueryVis visCheck;
	}
}
