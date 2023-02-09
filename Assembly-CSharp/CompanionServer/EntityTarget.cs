using System;

namespace CompanionServer
{
	// Token: 0x020009AF RID: 2479
	public struct EntityTarget : IEquatable<EntityTarget>
	{
		// Token: 0x1700048C RID: 1164
		// (get) Token: 0x06003ABD RID: 15037 RVA: 0x00158B28 File Offset: 0x00156D28
		public uint EntityId { get; }

		// Token: 0x06003ABE RID: 15038 RVA: 0x00158B30 File Offset: 0x00156D30
		public EntityTarget(uint entityId)
		{
			this.EntityId = entityId;
		}

		// Token: 0x06003ABF RID: 15039 RVA: 0x00158B39 File Offset: 0x00156D39
		public bool Equals(EntityTarget other)
		{
			return this.EntityId == other.EntityId;
		}

		// Token: 0x06003AC0 RID: 15040 RVA: 0x00158B4C File Offset: 0x00156D4C
		public override bool Equals(object obj)
		{
			if (obj is EntityTarget)
			{
				EntityTarget other = (EntityTarget)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x06003AC1 RID: 15041 RVA: 0x00158B73 File Offset: 0x00156D73
		public override int GetHashCode()
		{
			return (int)this.EntityId;
		}

		// Token: 0x06003AC2 RID: 15042 RVA: 0x00158B7B File Offset: 0x00156D7B
		public static bool operator ==(EntityTarget left, EntityTarget right)
		{
			return left.Equals(right);
		}

		// Token: 0x06003AC3 RID: 15043 RVA: 0x00158B85 File Offset: 0x00156D85
		public static bool operator !=(EntityTarget left, EntityTarget right)
		{
			return !left.Equals(right);
		}
	}
}
