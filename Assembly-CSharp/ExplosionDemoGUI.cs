using System;
using UnityEngine;

// Token: 0x0200095B RID: 2395
public class ExplosionDemoGUI : MonoBehaviour
{
	// Token: 0x0600388C RID: 14476 RVA: 0x0014DBE0 File Offset: 0x0014BDE0
	private void Start()
	{
		if (Screen.dpi < 1f)
		{
			this.dpiScale = 1f;
		}
		if (Screen.dpi < 200f)
		{
			this.dpiScale = 1f;
		}
		else
		{
			this.dpiScale = Screen.dpi / 200f;
		}
		this.guiStyleHeader.fontSize = (int)(15f * this.dpiScale);
		this.guiStyleHeader.normal.textColor = new Color(0.15f, 0.15f, 0.15f);
		this.currentInstance = UnityEngine.Object.Instantiate<GameObject>(this.Prefabs[this.currentNomber], base.transform.position, default(Quaternion));
		this.currentInstance.AddComponent<ExplosionDemoReactivator>().TimeDelayToReactivate = this.reactivateTime;
		this.sunIntensity = this.Sun.intensity;
	}

	// Token: 0x0600388D RID: 14477 RVA: 0x0014DCC0 File Offset: 0x0014BEC0
	private void OnGUI()
	{
		if (GUI.Button(new Rect(10f * this.dpiScale, 15f * this.dpiScale, 135f * this.dpiScale, 37f * this.dpiScale), "PREVIOUS EFFECT"))
		{
			this.ChangeCurrent(-1);
		}
		if (GUI.Button(new Rect(160f * this.dpiScale, 15f * this.dpiScale, 135f * this.dpiScale, 37f * this.dpiScale), "NEXT EFFECT"))
		{
			this.ChangeCurrent(1);
		}
		this.sunIntensity = GUI.HorizontalSlider(new Rect(10f * this.dpiScale, 70f * this.dpiScale, 285f * this.dpiScale, 15f * this.dpiScale), this.sunIntensity, 0f, 0.6f);
		this.Sun.intensity = this.sunIntensity;
		GUI.Label(new Rect(300f * this.dpiScale, 70f * this.dpiScale, 30f * this.dpiScale, 30f * this.dpiScale), "SUN INTENSITY", this.guiStyleHeader);
		GUI.Label(new Rect(400f * this.dpiScale, 15f * this.dpiScale, 100f * this.dpiScale, 20f * this.dpiScale), "Prefab name is \"" + this.Prefabs[this.currentNomber].name + "\"  \r\nHold any mouse button that would move the camera", this.guiStyleHeader);
	}

	// Token: 0x0600388E RID: 14478 RVA: 0x0014DE64 File Offset: 0x0014C064
	private void ChangeCurrent(int delta)
	{
		this.currentNomber += delta;
		if (this.currentNomber > this.Prefabs.Length - 1)
		{
			this.currentNomber = 0;
		}
		else if (this.currentNomber < 0)
		{
			this.currentNomber = this.Prefabs.Length - 1;
		}
		if (this.currentInstance != null)
		{
			UnityEngine.Object.Destroy(this.currentInstance);
		}
		this.currentInstance = UnityEngine.Object.Instantiate<GameObject>(this.Prefabs[this.currentNomber], base.transform.position, default(Quaternion));
		this.currentInstance.AddComponent<ExplosionDemoReactivator>().TimeDelayToReactivate = this.reactivateTime;
	}

	// Token: 0x040032D5 RID: 13013
	public GameObject[] Prefabs;

	// Token: 0x040032D6 RID: 13014
	public float reactivateTime = 4f;

	// Token: 0x040032D7 RID: 13015
	public Light Sun;

	// Token: 0x040032D8 RID: 13016
	private int currentNomber;

	// Token: 0x040032D9 RID: 13017
	private GameObject currentInstance;

	// Token: 0x040032DA RID: 13018
	private GUIStyle guiStyleHeader = new GUIStyle();

	// Token: 0x040032DB RID: 13019
	private float sunIntensity;

	// Token: 0x040032DC RID: 13020
	private float dpiScale;
}
