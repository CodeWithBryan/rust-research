using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000889 RID: 2185
public class TabToggle : MonoBehaviour
{
	// Token: 0x06003587 RID: 13703 RVA: 0x00141CF0 File Offset: 0x0013FEF0
	public void Awake()
	{
		if (this.TabHolder)
		{
			for (int i = 0; i < this.TabHolder.childCount; i++)
			{
				Button c = this.TabHolder.GetChild(i).GetComponent<Button>();
				if (c)
				{
					c.onClick.AddListener(delegate()
					{
						this.SwitchTo(c);
					});
				}
			}
		}
	}

	// Token: 0x06003588 RID: 13704 RVA: 0x00141D70 File Offset: 0x0013FF70
	public void SwitchTo(Button sourceTab)
	{
		string name = sourceTab.transform.name;
		if (this.TabHolder)
		{
			for (int i = 0; i < this.TabHolder.childCount; i++)
			{
				Button component = this.TabHolder.GetChild(i).GetComponent<Button>();
				if (component)
				{
					component.interactable = (component.name != name);
				}
			}
		}
		if (this.ContentHolder)
		{
			for (int j = 0; j < this.ContentHolder.childCount; j++)
			{
				Transform child = this.ContentHolder.GetChild(j);
				if (child.name == name)
				{
					this.Show(child.gameObject);
				}
				else
				{
					this.Hide(child.gameObject);
				}
			}
		}
	}

	// Token: 0x06003589 RID: 13705 RVA: 0x00141E34 File Offset: 0x00140034
	private void Hide(GameObject go)
	{
		if (!go.activeSelf)
		{
			return;
		}
		CanvasGroup component = go.GetComponent<CanvasGroup>();
		if (this.FadeOut && component)
		{
			LeanTween.alphaCanvas(component, 0f, 0.1f).setOnComplete(delegate()
			{
				go.SetActive(false);
			});
			return;
		}
		go.SetActive(false);
	}

	// Token: 0x0600358A RID: 13706 RVA: 0x00141EA8 File Offset: 0x001400A8
	private void Show(GameObject go)
	{
		if (go.activeSelf)
		{
			return;
		}
		CanvasGroup component = go.GetComponent<CanvasGroup>();
		if (this.FadeIn && component)
		{
			component.alpha = 0f;
			LeanTween.alphaCanvas(component, 1f, 0.1f);
		}
		go.SetActive(true);
	}

	// Token: 0x0400307F RID: 12415
	public Transform TabHolder;

	// Token: 0x04003080 RID: 12416
	public Transform ContentHolder;

	// Token: 0x04003081 RID: 12417
	public bool FadeIn;

	// Token: 0x04003082 RID: 12418
	public bool FadeOut;
}
