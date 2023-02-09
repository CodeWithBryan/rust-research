using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200076A RID: 1898
public class ChatEntry : MonoBehaviour
{
	// Token: 0x040029C5 RID: 10693
	public TextMeshProUGUI text;

	// Token: 0x040029C6 RID: 10694
	public RawImage avatar;

	// Token: 0x040029C7 RID: 10695
	public CanvasGroup canvasGroup;

	// Token: 0x040029C8 RID: 10696
	public float lifeStarted;

	// Token: 0x040029C9 RID: 10697
	public ulong steamid;

	// Token: 0x040029CA RID: 10698
	public Translate.Phrase LocalPhrase = new Translate.Phrase("local", "local");

	// Token: 0x040029CB RID: 10699
	public Translate.Phrase CardsPhrase = new Translate.Phrase("cards", "cards");

	// Token: 0x040029CC RID: 10700
	public Translate.Phrase TeamPhrase = new Translate.Phrase("team", "team");
}
