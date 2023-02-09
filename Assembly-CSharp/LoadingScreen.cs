using System;
using Rust.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000849 RID: 2121
public class LoadingScreen : SingletonComponent<LoadingScreen>
{
	// Token: 0x170003FD RID: 1021
	// (get) Token: 0x060034BD RID: 13501 RVA: 0x0013E8C9 File Offset: 0x0013CAC9
	public static bool isOpen
	{
		get
		{
			return SingletonComponent<LoadingScreen>.Instance && SingletonComponent<LoadingScreen>.Instance.panel && SingletonComponent<LoadingScreen>.Instance.panel.gameObject.activeSelf;
		}
	}

	// Token: 0x170003FE RID: 1022
	// (get) Token: 0x060034BE RID: 13502 RVA: 0x0013E8FE File Offset: 0x0013CAFE
	// (set) Token: 0x060034BF RID: 13503 RVA: 0x0013E905 File Offset: 0x0013CB05
	public static bool WantsSkip { get; private set; }

	// Token: 0x170003FF RID: 1023
	// (get) Token: 0x060034C1 RID: 13505 RVA: 0x0013E915 File Offset: 0x0013CB15
	// (set) Token: 0x060034C0 RID: 13504 RVA: 0x0013E90D File Offset: 0x0013CB0D
	public static string Text { get; private set; }

	// Token: 0x060034C2 RID: 13506 RVA: 0x000059DD File Offset: 0x00003BDD
	public static void Update(string strType)
	{
	}

	// Token: 0x060034C3 RID: 13507 RVA: 0x000059DD File Offset: 0x00003BDD
	public static void Update(string strType, string strSubtitle)
	{
	}

	// Token: 0x04002F25 RID: 12069
	public CanvasRenderer panel;

	// Token: 0x04002F26 RID: 12070
	public TextMeshProUGUI title;

	// Token: 0x04002F27 RID: 12071
	public TextMeshProUGUI subtitle;

	// Token: 0x04002F28 RID: 12072
	public Button skipButton;

	// Token: 0x04002F29 RID: 12073
	public Button cancelButton;

	// Token: 0x04002F2A RID: 12074
	public GameObject performanceWarning;

	// Token: 0x04002F2B RID: 12075
	public AudioSource music;

	// Token: 0x04002F2C RID: 12076
	public RectTransform serverInfo;

	// Token: 0x04002F2D RID: 12077
	public RustText serverName;

	// Token: 0x04002F2E RID: 12078
	public RustText serverPlayers;

	// Token: 0x04002F2F RID: 12079
	public RustLayout serverModeSection;

	// Token: 0x04002F30 RID: 12080
	public RustText serverMode;

	// Token: 0x04002F31 RID: 12081
	public RustText serverMap;

	// Token: 0x04002F32 RID: 12082
	public RustLayout serverTagsSection;

	// Token: 0x04002F33 RID: 12083
	public ServerBrowserTagList serverTags;

	// Token: 0x04002F34 RID: 12084
	public RectTransform demoInfo;

	// Token: 0x04002F35 RID: 12085
	public RustText demoName;

	// Token: 0x04002F36 RID: 12086
	public RustText demoLength;

	// Token: 0x04002F37 RID: 12087
	public RustText demoDate;

	// Token: 0x04002F38 RID: 12088
	public RustText demoMap;

	// Token: 0x04002F39 RID: 12089
	public RawImage backgroundImage;

	// Token: 0x04002F3A RID: 12090
	public Texture2D defaultBackground;

	// Token: 0x04002F3B RID: 12091
	public GameObject pingWarning;

	// Token: 0x04002F3C RID: 12092
	public RustText pingWarningText;

	// Token: 0x04002F3D RID: 12093
	[Tooltip("Ping must be at least this many ms higher than the server browser ping")]
	public int minPingDiffToShowWarning = 50;

	// Token: 0x04002F3E RID: 12094
	[Tooltip("Ping must be this many times higher than the server browser ping")]
	public float pingDiffFactorToShowWarning = 2f;

	// Token: 0x04002F3F RID: 12095
	[Tooltip("Number of ping samples required before showing the warning")]
	public int requiredPingSampleCount = 10;

	// Token: 0x04002F40 RID: 12096
	public GameObject blackout;
}
