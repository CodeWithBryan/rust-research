using System;
using UnityEngine;
using UnityEngine.UI;

namespace Rust.UI.Utility
{
	// Token: 0x02000ADE RID: 2782
	[RequireComponent(typeof(Toggle))]
	internal class ForceWeather : MonoBehaviour
	{
		// Token: 0x060042DD RID: 17117 RVA: 0x001854CD File Offset: 0x001836CD
		public void OnEnable()
		{
			this.component = base.GetComponent<Toggle>();
		}

		// Token: 0x060042DE RID: 17118 RVA: 0x001854DC File Offset: 0x001836DC
		public void Update()
		{
			if (SingletonComponent<Climate>.Instance == null)
			{
				return;
			}
			if (this.Rain)
			{
				SingletonComponent<Climate>.Instance.Overrides.Rain = Mathf.MoveTowards(SingletonComponent<Climate>.Instance.Overrides.Rain, (float)(this.component.isOn ? 1 : 0), Time.deltaTime / 2f);
			}
			if (this.Fog)
			{
				SingletonComponent<Climate>.Instance.Overrides.Fog = Mathf.MoveTowards(SingletonComponent<Climate>.Instance.Overrides.Fog, (float)(this.component.isOn ? 1 : 0), Time.deltaTime / 2f);
			}
			if (this.Wind)
			{
				SingletonComponent<Climate>.Instance.Overrides.Wind = Mathf.MoveTowards(SingletonComponent<Climate>.Instance.Overrides.Wind, (float)(this.component.isOn ? 1 : 0), Time.deltaTime / 2f);
			}
			if (this.Clouds)
			{
				SingletonComponent<Climate>.Instance.Overrides.Clouds = Mathf.MoveTowards(SingletonComponent<Climate>.Instance.Overrides.Clouds, (float)(this.component.isOn ? 1 : 0), Time.deltaTime / 2f);
			}
		}

		// Token: 0x04003B79 RID: 15225
		private Toggle component;

		// Token: 0x04003B7A RID: 15226
		public bool Rain;

		// Token: 0x04003B7B RID: 15227
		public bool Fog;

		// Token: 0x04003B7C RID: 15228
		public bool Wind;

		// Token: 0x04003B7D RID: 15229
		public bool Clouds;
	}
}
