using System;
using UnityEngine;
using UnityEngine.Events;

namespace Rust.Modular
{
	// Token: 0x02000AE1 RID: 2785
	public class EnableDisableEvent : MonoBehaviour
	{
		// Token: 0x060042E7 RID: 17127 RVA: 0x001856DD File Offset: 0x001838DD
		protected void OnEnable()
		{
			if (this.enableEvent != null)
			{
				this.enableEvent.Invoke();
			}
		}

		// Token: 0x060042E8 RID: 17128 RVA: 0x001856F2 File Offset: 0x001838F2
		protected void OnDisable()
		{
			if (this.disableEvent != null)
			{
				this.disableEvent.Invoke();
			}
		}

		// Token: 0x04003B8E RID: 15246
		[SerializeField]
		private UnityEvent enableEvent;

		// Token: 0x04003B8F RID: 15247
		[SerializeField]
		private UnityEvent disableEvent;
	}
}
