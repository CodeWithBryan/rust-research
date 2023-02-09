using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200088D RID: 2189
public class TimeSlider : MonoBehaviour
{
	// Token: 0x06003593 RID: 13715 RVA: 0x00141F98 File Offset: 0x00140198
	private void Start()
	{
		this.slider = base.GetComponent<Slider>();
	}

	// Token: 0x06003594 RID: 13716 RVA: 0x00141FA6 File Offset: 0x001401A6
	private void Update()
	{
		if (TOD_Sky.Instance == null)
		{
			return;
		}
		this.slider.value = TOD_Sky.Instance.Cycle.Hour;
	}

	// Token: 0x06003595 RID: 13717 RVA: 0x00141FD0 File Offset: 0x001401D0
	public void OnValue(float f)
	{
		if (TOD_Sky.Instance == null)
		{
			return;
		}
		TOD_Sky.Instance.Cycle.Hour = f;
		TOD_Sky.Instance.UpdateAmbient();
		TOD_Sky.Instance.UpdateReflection();
		TOD_Sky.Instance.UpdateFog();
	}

	// Token: 0x0400309B RID: 12443
	private Slider slider;
}
