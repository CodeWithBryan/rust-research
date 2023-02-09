using System;
using UnityEngine;

namespace Facepunch.GUI
{
	// Token: 0x02000AB9 RID: 2745
	public static class Controls
	{
		// Token: 0x06004274 RID: 17012 RVA: 0x001841E4 File Offset: 0x001823E4
		public static float FloatSlider(string strLabel, float value, float low, float high, string format = "0.00")
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(strLabel, new GUILayoutOption[]
			{
				GUILayout.Width(Controls.labelWidth)
			});
			float value2 = float.Parse(GUILayout.TextField(value.ToString(format), new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true)
			}));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			float result = GUILayout.HorizontalSlider(value2, low, high, Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			return result;
		}

		// Token: 0x06004275 RID: 17013 RVA: 0x00184258 File Offset: 0x00182458
		public static int IntSlider(string strLabel, int value, int low, int high, string format = "0")
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(strLabel, new GUILayoutOption[]
			{
				GUILayout.Width(Controls.labelWidth)
			});
			float num = (float)int.Parse(GUILayout.TextField(value.ToString(format), new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true)
			}));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			int result = (int)GUILayout.HorizontalSlider(num, (float)low, (float)high, Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			return result;
		}

		// Token: 0x06004276 RID: 17014 RVA: 0x001842CE File Offset: 0x001824CE
		public static string TextArea(string strName, string value)
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(strName, new GUILayoutOption[]
			{
				GUILayout.Width(Controls.labelWidth)
			});
			string result = GUILayout.TextArea(value, Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			return result;
		}

		// Token: 0x06004277 RID: 17015 RVA: 0x00184303 File Offset: 0x00182503
		public static bool Checkbox(string strName, bool value)
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(strName, new GUILayoutOption[]
			{
				GUILayout.Width(Controls.labelWidth)
			});
			bool result = GUILayout.Toggle(value, "", Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			return result;
		}

		// Token: 0x06004278 RID: 17016 RVA: 0x0018433D File Offset: 0x0018253D
		public static bool Button(string strName)
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			bool result = GUILayout.Button(strName, Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			return result;
		}

		// Token: 0x04003AB9 RID: 15033
		public static float labelWidth = 100f;
	}
}
