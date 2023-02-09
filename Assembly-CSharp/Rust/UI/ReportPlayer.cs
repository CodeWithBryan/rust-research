using System;
using UnityEngine;

namespace Rust.UI
{
	// Token: 0x02000ACB RID: 2763
	public class ReportPlayer : UIDialog
	{
		// Token: 0x04003B0F RID: 15119
		public GameObject FindPlayer;

		// Token: 0x04003B10 RID: 15120
		public GameObject GetInformation;

		// Token: 0x04003B11 RID: 15121
		public GameObject Finished;

		// Token: 0x04003B12 RID: 15122
		public GameObject RecentlyReported;

		// Token: 0x04003B13 RID: 15123
		public Dropdown ReasonDropdown;

		// Token: 0x04003B14 RID: 15124
		public RustInput Subject;

		// Token: 0x04003B15 RID: 15125
		public RustInput Message;

		// Token: 0x04003B16 RID: 15126
		public RustButton ReportButton;

		// Token: 0x04003B17 RID: 15127
		public SteamUserButton SteamUserButton;

		// Token: 0x04003B18 RID: 15128
		public RustIcon ProgressIcon;

		// Token: 0x04003B19 RID: 15129
		public RustText ProgressText;

		// Token: 0x04003B1A RID: 15130
		public static Option[] ReportReasons = new Option[]
		{
			new Option(new Translate.Phrase("report.reason.none", "Select an option"), "none", false, Icons.Bars),
			new Option(new Translate.Phrase("report.reason.abuse", "Racism/Sexism/Abusive"), "abusive", false, Icons.Angry),
			new Option(new Translate.Phrase("report.reason.cheat", "Cheating"), "cheat", false, Icons.Crosshairs),
			new Option(new Translate.Phrase("report.reason.spam", "Spamming"), "spam", false, Icons.Bullhorn),
			new Option(new Translate.Phrase("report.reason.name", "Offensive Name"), "name", false, Icons.Radiation)
		};
	}
}
