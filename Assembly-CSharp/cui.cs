﻿using System;
using Network;

// Token: 0x02000954 RID: 2388
public class cui
{
	// Token: 0x0600385E RID: 14430 RVA: 0x0014CE3C File Offset: 0x0014B03C
	[ServerUserVar]
	public static void test(ConsoleSystem.Arg args)
	{
		CommunityEntity.ServerInstance.ClientRPCEx<string>(new SendInfo
		{
			connection = args.Connection
		}, null, "AddUI", "[\t\n\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\"name\": \"TestPanel7766\",\n\t\t\t\t\t\t\t\"parent\": \"Overlay\",\n\n\t\t\t\t\t\t\t\"components\":\n\t\t\t\t\t\t\t[\n\t\t\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\t\t\"type\":\"UnityEngine.UI.RawImage\",\n\t\t\t\t\t\t\t\t\t\"imagetype\": \"Tiled\",\n\t\t\t\t\t\t\t\t\t\"color\": \"1.0 1.0 1.0 1.0\",\n\t\t\t\t\t\t\t\t\t\"url\": \"http://files.facepunch.com/garry/2015/June/03/2015-06-03_12-19-17.jpg\",\n\t\t\t\t\t\t\t\t},\n\n\t\t\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\t\t\"type\":\"RectTransform\",\n\t\t\t\t\t\t\t\t\t\"anchormin\": \"0 0\",\n\t\t\t\t\t\t\t\t\t\"anchormax\": \"1 1\"\n\t\t\t\t\t\t\t\t},\n\n\t\t\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\t\t\"type\":\"NeedsCursor\"\n\t\t\t\t\t\t\t\t}\n\t\t\t\t\t\t\t]\n\t\t\t\t\t\t},\n\n\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\"parent\": \"TestPanel7766\",\n\n\t\t\t\t\t\t\t\"components\":\n\t\t\t\t\t\t\t[\n\t\t\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\t\t\"type\":\"UnityEngine.UI.Text\",\n\t\t\t\t\t\t\t\t\t\"text\":\"Do you want to press a button?\",\n\t\t\t\t\t\t\t\t\t\"fontSize\":32,\n\t\t\t\t\t\t\t\t\t\"align\": \"MiddleCenter\",\n\t\t\t\t\t\t\t\t},\n\n\t\t\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\t\t\"type\":\"RectTransform\",\n\t\t\t\t\t\t\t\t\t\"anchormin\": \"0 0.5\",\n\t\t\t\t\t\t\t\t\t\"anchormax\": \"1 0.9\"\n\t\t\t\t\t\t\t\t}\n\t\t\t\t\t\t\t]\n\t\t\t\t\t\t},\n\n\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\"name\": \"Button88\",\n\t\t\t\t\t\t\t\"parent\": \"TestPanel7766\",\n\n\t\t\t\t\t\t\t\"components\":\n\t\t\t\t\t\t\t[\n\t\t\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\t\t\"type\":\"UnityEngine.UI.Button\",\n\t\t\t\t\t\t\t\t\t\"close\":\"TestPanel7766\",\n\t\t\t\t\t\t\t\t\t\"command\":\"cui.endtest\",\n\t\t\t\t\t\t\t\t\t\"color\": \"0.9 0.8 0.3 0.8\",\n\t\t\t\t\t\t\t\t\t\"imagetype\": \"Tiled\"\n\t\t\t\t\t\t\t\t},\n\n\t\t\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\t\t\"type\":\"RectTransform\",\n\t\t\t\t\t\t\t\t\t\"anchormin\": \"0.3 0.15\",\n\t\t\t\t\t\t\t\t\t\"anchormax\": \"0.7 0.2\"\n\t\t\t\t\t\t\t\t}\n\t\t\t\t\t\t\t]\n\t\t\t\t\t\t},\n\n\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\"parent\": \"Button88\",\n\n\t\t\t\t\t\t\t\"components\":\n\t\t\t\t\t\t\t[\n\t\t\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\t\t\"type\":\"UnityEngine.UI.Text\",\n\t\t\t\t\t\t\t\t\t\"text\":\"YES\",\n\t\t\t\t\t\t\t\t\t\"fontSize\":20,\n\t\t\t\t\t\t\t\t\t\"align\": \"MiddleCenter\"\n\t\t\t\t\t\t\t\t}\n\t\t\t\t\t\t\t]\n\t\t\t\t\t\t}\n\n\t\t\t\t\t]\n\t\t\t\t\t");
	}

	// Token: 0x0600385F RID: 14431 RVA: 0x0014CE74 File Offset: 0x0014B074
	[ServerUserVar]
	public static void endtest(ConsoleSystem.Arg args)
	{
		args.ReplyWith("Ending Test!");
		CommunityEntity.ServerInstance.ClientRPCEx<string>(new SendInfo
		{
			connection = args.Connection
		}, null, "DestroyUI", "TestPanel7766");
	}
}
