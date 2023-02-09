using System;
using System.Collections.Generic;
using Facepunch;
using Facepunch.Extend;

namespace Network
{
	// Token: 0x0200001A RID: 26
	public class Stats
	{
		// Token: 0x0600010E RID: 270 RVA: 0x00004754 File Offset: 0x00002954
		public Stats()
		{
			this.Building.Add("", 0L);
			this.Building.Clear();
			this.Previous.Add("", 0L);
			this.Previous.Clear();
		}

		// Token: 0x0600010F RID: 271 RVA: 0x000047BC File Offset: 0x000029BC
		public void Add(string Category, string Object, long Bytes)
		{
			if (!this.Enabled)
			{
				return;
			}
			this.Building.Bytes += Bytes;
			this.Building.Count += 1L;
			this.Building.Add(Category, Bytes).Add(Object, Bytes);
		}

		// Token: 0x06000110 RID: 272 RVA: 0x0000480E File Offset: 0x00002A0E
		public void Add(string Category, long Bytes)
		{
			if (!this.Enabled)
			{
				return;
			}
			this.Building.Bytes += Bytes;
			this.Building.Count += 1L;
			this.Building.Add(Category, Bytes);
		}

		// Token: 0x06000111 RID: 273 RVA: 0x00004850 File Offset: 0x00002A50
		public void Flip()
		{
			if (!this.Enabled)
			{
				return;
			}
			Stats.Node building = this.Building;
			this.Building = this.Previous;
			this.Previous = building;
			this.Building.Clear();
		}

		// Token: 0x04000086 RID: 134
		public bool Enabled;

		// Token: 0x04000087 RID: 135
		public Stats.Node Building = new Stats.Node();

		// Token: 0x04000088 RID: 136
		public Stats.Node Previous = new Stats.Node();

		// Token: 0x02000025 RID: 37
		public class Node : Pool.IPooled
		{
			// Token: 0x06000139 RID: 313 RVA: 0x00004D1C File Offset: 0x00002F1C
			internal Stats.Node Add(string category, long bytes)
			{
				if (this.Children == null)
				{
					this.Children = Pool.Get<Dictionary<string, Stats.Node>>();
				}
				Stats.Node orCreatePooled = this.Children.GetOrCreatePooled(category);
				orCreatePooled.Bytes += bytes;
				orCreatePooled.Count += 1L;
				return orCreatePooled;
			}

			// Token: 0x0600013A RID: 314 RVA: 0x00004D5C File Offset: 0x00002F5C
			internal void Clear()
			{
				this.Bytes = 0L;
				this.Count = 0L;
				if (this.Children == null)
				{
					return;
				}
				foreach (KeyValuePair<string, Stats.Node> keyValuePair in this.Children)
				{
					Stats.Node value = keyValuePair.Value;
					Pool.Free<Stats.Node>(ref value);
				}
				this.Children.Clear();
			}

			// Token: 0x0600013B RID: 315 RVA: 0x00004DDC File Offset: 0x00002FDC
			public void EnterPool()
			{
				this.Clear();
			}

			// Token: 0x0600013C RID: 316 RVA: 0x00004DDC File Offset: 0x00002FDC
			public void LeavePool()
			{
				this.Clear();
			}

			// Token: 0x040000C5 RID: 197
			public Dictionary<string, Stats.Node> Children;

			// Token: 0x040000C6 RID: 198
			public long Bytes;

			// Token: 0x040000C7 RID: 199
			public long Count;
		}
	}
}
