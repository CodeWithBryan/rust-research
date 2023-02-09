using System;
using UnityEngine;

// Token: 0x02000302 RID: 770
public class DevTimeAdjust : MonoBehaviour
{
	// Token: 0x06001D9E RID: 7582 RVA: 0x000CA73D File Offset: 0x000C893D
	private void Start()
	{
		if (!TOD_Sky.Instance)
		{
			return;
		}
		TOD_Sky.Instance.Cycle.Hour = PlayerPrefs.GetFloat("DevTime");
	}

	// Token: 0x06001D9F RID: 7583 RVA: 0x000CA768 File Offset: 0x000C8968
	private void OnGUI()
	{
		if (!TOD_Sky.Instance)
		{
			return;
		}
		float num = (float)Screen.width * 0.2f;
		Rect position = new Rect((float)Screen.width - (num + 20f), (float)Screen.height - 30f, num, 20f);
		float num2 = TOD_Sky.Instance.Cycle.Hour;
		num2 = GUI.HorizontalSlider(position, num2, 0f, 24f);
		position.y -= 20f;
		GUI.Label(position, "Time Of Day");
		if (num2 != TOD_Sky.Instance.Cycle.Hour)
		{
			TOD_Sky.Instance.Cycle.Hour = num2;
			PlayerPrefs.SetFloat("DevTime", num2);
		}
	}
}
