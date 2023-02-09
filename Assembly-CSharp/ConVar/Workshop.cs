using System;

namespace ConVar
{
	// Token: 0x02000AA6 RID: 2726
	[ConsoleSystem.Factory("workshop")]
	public class Workshop : ConsoleSystem
	{
		// Token: 0x06004142 RID: 16706 RVA: 0x0017F230 File Offset: 0x0017D430
		[ServerVar]
		public static void print_approved_skins(ConsoleSystem.Arg arg)
		{
			if (!PlatformService.Instance.IsValid)
			{
				return;
			}
			if (PlatformService.Instance.ItemDefinitions == null)
			{
				return;
			}
			TextTable textTable = new TextTable();
			textTable.AddColumn("name");
			textTable.AddColumn("itemshortname");
			textTable.AddColumn("workshopid");
			textTable.AddColumn("workshopdownload");
			foreach (IPlayerItemDefinition playerItemDefinition in PlatformService.Instance.ItemDefinitions)
			{
				string name = playerItemDefinition.Name;
				string itemShortName = playerItemDefinition.ItemShortName;
				string text = playerItemDefinition.WorkshopId.ToString();
				string text2 = playerItemDefinition.WorkshopDownload.ToString();
				textTable.AddRow(new string[]
				{
					name,
					itemShortName,
					text,
					text2
				});
			}
			arg.ReplyWith(arg.HasArg("--json") ? textTable.ToJson() : textTable.ToString());
		}
	}
}
