using System;

namespace CompanionServer
{
	// Token: 0x020009B5 RID: 2485
	public struct PlayerTarget : IEquatable<PlayerTarget>
	{
		// Token: 0x1700048D RID: 1165
		// (get) Token: 0x06003AE0 RID: 15072 RVA: 0x001596CD File Offset: 0x001578CD
		public ulong SteamId { get; }

		// Token: 0x06003AE1 RID: 15073 RVA: 0x001596D5 File Offset: 0x001578D5
		public PlayerTarget(ulong steamId)
		{
			this.SteamId = steamId;
		}

		// Token: 0x06003AE2 RID: 15074 RVA: 0x001596DE File Offset: 0x001578DE
		public bool Equals(PlayerTarget other)
		{
			return this.SteamId == other.SteamId;
		}

		// Token: 0x06003AE3 RID: 15075 RVA: 0x001596F0 File Offset: 0x001578F0
		public override bool Equals(object obj)
		{
			if (obj is PlayerTarget)
			{
				PlayerTarget other = (PlayerTarget)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x06003AE4 RID: 15076 RVA: 0x00159718 File Offset: 0x00157918
		public override int GetHashCode()
		{
			return this.SteamId.GetHashCode();
		}

		// Token: 0x06003AE5 RID: 15077 RVA: 0x00159733 File Offset: 0x00157933
		public static bool operator ==(PlayerTarget left, PlayerTarget right)
		{
			return left.Equals(right);
		}

		// Token: 0x06003AE6 RID: 15078 RVA: 0x0015973D File Offset: 0x0015793D
		public static bool operator !=(PlayerTarget left, PlayerTarget right)
		{
			return !left.Equals(right);
		}
	}
}
