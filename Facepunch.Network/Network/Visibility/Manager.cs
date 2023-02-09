using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

namespace Network.Visibility
{
	// Token: 0x0200001E RID: 30
	public class Manager : IDisposable
	{
		// Token: 0x06000129 RID: 297 RVA: 0x00004A94 File Offset: 0x00002C94
		public virtual void Dispose()
		{
			foreach (KeyValuePair<uint, Group> keyValuePair in this.groups)
			{
				keyValuePair.Value.Dispose();
			}
			this.groups.Clear();
			this.provider = null;
		}

		// Token: 0x0600012A RID: 298 RVA: 0x00004B00 File Offset: 0x00002D00
		public Manager(Provider p)
		{
			if (this.groups.Count > 0 && p != null)
			{
				Debug.LogWarning("SetProvider should be called before anything else! " + this.groups.Count + " groups have already been registered!");
			}
			this.provider = p;
		}

		// Token: 0x0600012B RID: 299 RVA: 0x00004B5C File Offset: 0x00002D5C
		public Group TryGet(uint ID)
		{
			Group result;
			if (this.groups.TryGetValue(ID, out result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x0600012C RID: 300 RVA: 0x00004B7C File Offset: 0x00002D7C
		public Group Get(uint ID)
		{
			Group group;
			if (this.groups.TryGetValue(ID, out group))
			{
				return group;
			}
			group = new Group(this, ID);
			this.groups.Add(ID, group);
			if (this.provider != null)
			{
				this.provider.OnGroupAdded(group);
			}
			return group;
		}

		// Token: 0x0600012D RID: 301 RVA: 0x00004BC5 File Offset: 0x00002DC5
		public Subscriber CreateSubscriber(Connection connection)
		{
			Subscriber subscriber = Pool.Get<Subscriber>();
			subscriber.manager = this;
			subscriber.connection = connection;
			return subscriber;
		}

		// Token: 0x0600012E RID: 302 RVA: 0x00004BDA File Offset: 0x00002DDA
		public void DestroySubscriber(ref Subscriber subscriber)
		{
			subscriber.Destroy();
			Pool.Free<Subscriber>(ref subscriber);
		}

		// Token: 0x0600012F RID: 303 RVA: 0x00004BE9 File Offset: 0x00002DE9
		public bool IsInside(Group group, Vector3 vPos)
		{
			return this.provider != null && group != null && this.provider.IsInside(group, vPos);
		}

		// Token: 0x06000130 RID: 304 RVA: 0x00004C07 File Offset: 0x00002E07
		public Group GetGroup(Vector3 vPos)
		{
			if (this.provider == null)
			{
				return this.Get(0U);
			}
			return this.provider.GetGroup(vPos);
		}

		// Token: 0x06000131 RID: 305 RVA: 0x00004C25 File Offset: 0x00002E25
		public void GetVisibleFromFar(Group center, List<Group> groups)
		{
			if (this.provider == null)
			{
				return;
			}
			if (center == null)
			{
				return;
			}
			this.provider.GetVisibleFromFar(center, groups);
		}

		// Token: 0x06000132 RID: 306 RVA: 0x00004C41 File Offset: 0x00002E41
		public void GetVisibleFromNear(Group center, List<Group> groups)
		{
			if (this.provider == null)
			{
				return;
			}
			if (center == null)
			{
				return;
			}
			this.provider.GetVisibleFromNear(center, groups);
		}

		// Token: 0x04000091 RID: 145
		private Dictionary<uint, Group> groups = new Dictionary<uint, Group>();

		// Token: 0x04000092 RID: 146
		internal Provider provider;
	}
}
