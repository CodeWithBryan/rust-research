using System;
using System.Collections.Generic;
using Facepunch.Math;
using UnityEngine;

namespace Facepunch
{
	// Token: 0x02000AA9 RID: 2729
	public static class Output
	{
		// Token: 0x14000007 RID: 7
		// (add) Token: 0x0600414E RID: 16718 RVA: 0x0017F664 File Offset: 0x0017D864
		// (remove) Token: 0x0600414F RID: 16719 RVA: 0x0017F698 File Offset: 0x0017D898
		public static event Action<string, string, LogType> OnMessage;

		// Token: 0x06004150 RID: 16720 RVA: 0x0017F6CB File Offset: 0x0017D8CB
		public static void Install()
		{
			if (Output.installed)
			{
				return;
			}
			Application.logMessageReceived += Output.LogHandler;
			Output.installed = true;
		}

		// Token: 0x06004151 RID: 16721 RVA: 0x0017F6EC File Offset: 0x0017D8EC
		internal static void LogHandler(string log, string stacktrace, LogType type)
		{
			if (Output.OnMessage == null)
			{
				return;
			}
			if (log.StartsWith("Kinematic body only supports Speculative Continuous collision detection"))
			{
				return;
			}
			if (log.StartsWith("Skipped frame because GfxDevice"))
			{
				return;
			}
			if (log.StartsWith("Your current multi-scene setup has inconsistent Lighting"))
			{
				return;
			}
			if (log.Contains("HandleD3DDeviceLost"))
			{
				return;
			}
			if (log.Contains("ResetD3DDevice"))
			{
				return;
			}
			if (log.Contains("dev->Reset"))
			{
				return;
			}
			if (log.Contains("D3Dwindow device not lost anymore"))
			{
				return;
			}
			if (log.Contains("D3D device reset"))
			{
				return;
			}
			if (log.Contains("group < 0xfff"))
			{
				return;
			}
			if (log.Contains("Mesh can not have more than 65000 vert"))
			{
				return;
			}
			if (log.Contains("Trying to add (Layout Rebuilder for)"))
			{
				return;
			}
			if (log.Contains("Coroutine continue failure"))
			{
				return;
			}
			if (log.Contains("No texture data available to upload"))
			{
				return;
			}
			if (log.Contains("Trying to reload asset from disk that is not"))
			{
				return;
			}
			if (log.Contains("Unable to find shaders used for the terrain engine."))
			{
				return;
			}
			if (log.Contains("Canvas element contains more than 65535 vertices"))
			{
				return;
			}
			if (log.Contains("RectTransform.set_anchorMin"))
			{
				return;
			}
			if (log.Contains("FMOD failed to initialize the output device"))
			{
				return;
			}
			if (log.Contains("Cannot create FMOD::Sound"))
			{
				return;
			}
			if (log.Contains("invalid utf-16 sequence"))
			{
				return;
			}
			if (log.Contains("missing surrogate tail"))
			{
				return;
			}
			if (log.Contains("Failed to create agent because it is not close enough to the Nav"))
			{
				return;
			}
			if (log.Contains("user-provided triangle mesh descriptor is invalid"))
			{
				return;
			}
			if (log.Contains("Releasing render texture that is set as"))
			{
				return;
			}
			using (TimeWarning.New("Facepunch.Output.LogHandler", 0))
			{
				try
				{
					Action<string, string, LogType> onMessage = Output.OnMessage;
					if (onMessage != null)
					{
						onMessage(log, stacktrace, type);
					}
				}
				catch (Exception)
				{
				}
			}
			Output.HistoryOutput.Add(new Output.Entry
			{
				Message = log,
				Stacktrace = stacktrace,
				Type = type.ToString(),
				Time = Epoch.Current
			});
			while (Output.HistoryOutput.Count > 65536)
			{
				Output.HistoryOutput.RemoveAt(0);
			}
		}

		// Token: 0x04003A3B RID: 14907
		public static bool installed = false;

		// Token: 0x04003A3C RID: 14908
		public static List<Output.Entry> HistoryOutput = new List<Output.Entry>();

		// Token: 0x02000F03 RID: 3843
		public struct Entry
		{
			// Token: 0x04004CD8 RID: 19672
			public string Message;

			// Token: 0x04004CD9 RID: 19673
			public string Stacktrace;

			// Token: 0x04004CDA RID: 19674
			public string Type;

			// Token: 0x04004CDB RID: 19675
			public int Time;
		}
	}
}
