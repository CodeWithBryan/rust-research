using System;
using System.Text;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000A9E RID: 2718
	[ConsoleSystem.Factory("texture")]
	public class Texture : ConsoleSystem
	{
		// Token: 0x1700056A RID: 1386
		// (get) Token: 0x060040EC RID: 16620 RVA: 0x0017E3AE File Offset: 0x0017C5AE
		// (set) Token: 0x060040ED RID: 16621 RVA: 0x0017E3B5 File Offset: 0x0017C5B5
		[ClientVar(Saved = true, Help = "Enable/Disable texture streaming")]
		public static bool streaming
		{
			get
			{
				return QualitySettings.streamingMipmapsActive;
			}
			set
			{
				QualitySettings.streamingMipmapsActive = value;
			}
		}

		// Token: 0x060040EE RID: 16622 RVA: 0x0017E3C0 File Offset: 0x0017C5C0
		[ClientVar]
		public static void stats(ConsoleSystem.Arg arg)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(string.Format("Supports streaming:               {0}", SystemInfo.supportsMipStreaming));
			stringBuilder.AppendLine(string.Format("Streaming enabled:                {0}", QualitySettings.streamingMipmapsActive));
			stringBuilder.AppendLine(string.Format("Immediately discard unused mips:  {0}", Texture.streamingTextureDiscardUnusedMips));
			stringBuilder.AppendLine(string.Format("Max level of reduction:           {0}", QualitySettings.streamingMipmapsMaxLevelReduction));
			stringBuilder.AppendLine();
			stringBuilder.AppendLine(string.Format("currentTextureMemory:             {0}MB (current estimated usage)", Texture.currentTextureMemory / 1048576UL));
			stringBuilder.AppendLine(string.Format("desiredTextureMemory:             {0}MB", Texture.desiredTextureMemory / 1048576UL));
			stringBuilder.AppendLine(string.Format("nonStreamingTextureCount:         {0}", Texture.nonStreamingTextureCount));
			stringBuilder.AppendLine(string.Format("nonStreamingTextureMemory:        {0}MB", Texture.nonStreamingTextureMemory / 1048576UL));
			stringBuilder.AppendLine(string.Format("streamingTextureCount:            {0}", Texture.streamingTextureCount));
			stringBuilder.AppendLine(string.Format("targetTextureMemory:              {0}MB", Texture.targetTextureMemory / 1048576UL));
			stringBuilder.AppendLine(string.Format("totalTextureMemory:               {0}MB (if everything was loaded at highest quality)", Texture.totalTextureMemory / 1048576UL));
			stringBuilder.AppendLine();
			stringBuilder.AppendLine(string.Format("streamingMipmapUploadCount:       {0}", Texture.streamingMipmapUploadCount));
			stringBuilder.AppendLine(string.Format("streamingTextureLoadingCount:     {0}", Texture.streamingTextureLoadingCount));
			stringBuilder.AppendLine(string.Format("streamingTexturePendingLoadCount: {0}", Texture.streamingTexturePendingLoadCount));
			stringBuilder.AppendLine();
			stringBuilder.AppendLine(string.Format("TargetBudget:                     {0}MB", QualitySettings.streamingMipmapsMemoryBudget));
			arg.ReplyWith(stringBuilder.ToString());
		}

		// Token: 0x04003A19 RID: 14873
		[ClientVar]
		public static int streamingBudgetOverride;
	}
}
