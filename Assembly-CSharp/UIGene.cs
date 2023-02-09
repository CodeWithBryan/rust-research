using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200089B RID: 2203
public class UIGene : MonoBehaviour
{
	// Token: 0x060035B7 RID: 13751 RVA: 0x001425C0 File Offset: 0x001407C0
	public void Init(GrowableGene gene)
	{
		bool flag = gene.IsPositive();
		this.ImageBG.color = (flag ? this.PositiveColour : this.NegativeColour);
		this.TextGene.color = (flag ? this.PositiveTextColour : this.NegativeTextColour);
		this.TextGene.text = gene.GetDisplayCharacter();
		this.Show();
	}

	// Token: 0x060035B8 RID: 13752 RVA: 0x00142623 File Offset: 0x00140823
	public void InitPrevious(GrowableGene gene)
	{
		this.ImageBG.color = Color.black;
		this.TextGene.color = Color.grey;
		this.TextGene.text = GrowableGene.GetDisplayCharacter(gene.PreviousType);
		this.Show();
	}

	// Token: 0x060035B9 RID: 13753 RVA: 0x00142661 File Offset: 0x00140861
	public void Hide()
	{
		this.Child.gameObject.SetActive(false);
	}

	// Token: 0x060035BA RID: 13754 RVA: 0x00142674 File Offset: 0x00140874
	public void Show()
	{
		this.Child.gameObject.SetActive(true);
	}

	// Token: 0x040030C7 RID: 12487
	public GameObject Child;

	// Token: 0x040030C8 RID: 12488
	public Color PositiveColour;

	// Token: 0x040030C9 RID: 12489
	public Color NegativeColour;

	// Token: 0x040030CA RID: 12490
	public Color PositiveTextColour;

	// Token: 0x040030CB RID: 12491
	public Color NegativeTextColour;

	// Token: 0x040030CC RID: 12492
	public Image ImageBG;

	// Token: 0x040030CD RID: 12493
	public Text TextGene;
}
