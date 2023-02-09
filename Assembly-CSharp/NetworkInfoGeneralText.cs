using System;
using Network;
using TMPro;
using UnityEngine;

// Token: 0x020002EF RID: 751
public class NetworkInfoGeneralText : MonoBehaviour
{
	// Token: 0x06001D66 RID: 7526 RVA: 0x000C9273 File Offset: 0x000C7473
	private void Update()
	{
		this.UpdateText();
	}

	// Token: 0x06001D67 RID: 7527 RVA: 0x000C927C File Offset: 0x000C747C
	private void UpdateText()
	{
		string str = "";
		if (Net.sv != null)
		{
			str += "Server\n";
			str += Net.sv.GetDebug(null);
			str += "\n";
		}
		this.text.text = str;
	}

	// Token: 0x06001D68 RID: 7528 RVA: 0x000C92CC File Offset: 0x000C74CC
	private static string ChannelStat(int window, int left)
	{
		return string.Format("{0}/{1}", left, window);
	}

	// Token: 0x040016CA RID: 5834
	public TextMeshProUGUI text;
}
