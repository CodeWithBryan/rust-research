using System;
using UnityEngine;

// Token: 0x02000303 RID: 771
public class DevWeatherAdjust : MonoBehaviour
{
	// Token: 0x06001DA1 RID: 7585 RVA: 0x000CA824 File Offset: 0x000C8A24
	protected void Awake()
	{
		SingletonComponent<Climate>.Instance.Overrides.Clouds = 0f;
		SingletonComponent<Climate>.Instance.Overrides.Fog = 0f;
		SingletonComponent<Climate>.Instance.Overrides.Wind = 0f;
		SingletonComponent<Climate>.Instance.Overrides.Rain = 0f;
	}

	// Token: 0x06001DA2 RID: 7586 RVA: 0x000CA884 File Offset: 0x000C8A84
	protected void OnGUI()
	{
		float num = (float)Screen.width * 0.2f;
		GUILayout.BeginArea(new Rect((float)Screen.width - num - 20f, 20f, num, 400f), "", "box");
		GUILayout.Box("Weather", Array.Empty<GUILayoutOption>());
		GUILayout.FlexibleSpace();
		GUILayout.Label("Clouds", Array.Empty<GUILayoutOption>());
		SingletonComponent<Climate>.Instance.Overrides.Clouds = GUILayout.HorizontalSlider(SingletonComponent<Climate>.Instance.Overrides.Clouds, 0f, 1f, Array.Empty<GUILayoutOption>());
		GUILayout.Label("Fog", Array.Empty<GUILayoutOption>());
		SingletonComponent<Climate>.Instance.Overrides.Fog = GUILayout.HorizontalSlider(SingletonComponent<Climate>.Instance.Overrides.Fog, 0f, 1f, Array.Empty<GUILayoutOption>());
		GUILayout.Label("Wind", Array.Empty<GUILayoutOption>());
		SingletonComponent<Climate>.Instance.Overrides.Wind = GUILayout.HorizontalSlider(SingletonComponent<Climate>.Instance.Overrides.Wind, 0f, 1f, Array.Empty<GUILayoutOption>());
		GUILayout.Label("Rain", Array.Empty<GUILayoutOption>());
		SingletonComponent<Climate>.Instance.Overrides.Rain = GUILayout.HorizontalSlider(SingletonComponent<Climate>.Instance.Overrides.Rain, 0f, 1f, Array.Empty<GUILayoutOption>());
		GUILayout.FlexibleSpace();
		GUILayout.EndArea();
	}
}
