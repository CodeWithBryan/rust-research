using System;
using UnityEngine;

namespace AmplifyOcclusion
{
	// Token: 0x02000992 RID: 2450
	[Serializable]
	public class VersionInfo
	{
		// Token: 0x060039FE RID: 14846 RVA: 0x00155FF3 File Offset: 0x001541F3
		public static string StaticToString()
		{
			return string.Format("{0}.{1}.{2}", 2, 0, 0) + VersionInfo.StageSuffix;
		}

		// Token: 0x060039FF RID: 14847 RVA: 0x0015601B File Offset: 0x0015421B
		public override string ToString()
		{
			return string.Format("{0}.{1}.{2}", this.m_major, this.m_minor, this.m_release) + VersionInfo.StageSuffix;
		}

		// Token: 0x17000476 RID: 1142
		// (get) Token: 0x06003A00 RID: 14848 RVA: 0x00156052 File Offset: 0x00154252
		public int Number
		{
			get
			{
				return this.m_major * 100 + this.m_minor * 10 + this.m_release;
			}
		}

		// Token: 0x06003A01 RID: 14849 RVA: 0x0015606E File Offset: 0x0015426E
		private VersionInfo()
		{
			this.m_major = 2;
			this.m_minor = 0;
			this.m_release = 0;
		}

		// Token: 0x06003A02 RID: 14850 RVA: 0x0015608B File Offset: 0x0015428B
		private VersionInfo(byte major, byte minor, byte release)
		{
			this.m_major = (int)major;
			this.m_minor = (int)minor;
			this.m_release = (int)release;
		}

		// Token: 0x06003A03 RID: 14851 RVA: 0x001560A8 File Offset: 0x001542A8
		public static VersionInfo Current()
		{
			return new VersionInfo(2, 0, 0);
		}

		// Token: 0x06003A04 RID: 14852 RVA: 0x001560B2 File Offset: 0x001542B2
		public static bool Matches(VersionInfo version)
		{
			return 2 == version.m_major && version.m_minor == 0 && version.m_release == 0;
		}

		// Token: 0x040034A9 RID: 13481
		public const byte Major = 2;

		// Token: 0x040034AA RID: 13482
		public const byte Minor = 0;

		// Token: 0x040034AB RID: 13483
		public const byte Release = 0;

		// Token: 0x040034AC RID: 13484
		private static string StageSuffix = "_dev002";

		// Token: 0x040034AD RID: 13485
		[SerializeField]
		private int m_major;

		// Token: 0x040034AE RID: 13486
		[SerializeField]
		private int m_minor;

		// Token: 0x040034AF RID: 13487
		[SerializeField]
		private int m_release;
	}
}
