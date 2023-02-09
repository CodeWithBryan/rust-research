using System;
using System.Collections.Generic;
using Facepunch;
using Facepunch.Extend;
using Network.Visibility;
using UnityEngine;

namespace Network
{
	// Token: 0x02000016 RID: 22
	public class Networkable : Pool.IPooled
	{
		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000DD RID: 221 RVA: 0x00003D23 File Offset: 0x00001F23
		// (set) Token: 0x060000DE RID: 222 RVA: 0x00003D2B File Offset: 0x00001F2B
		public Connection connection { get; private set; }

		// Token: 0x060000DF RID: 223 RVA: 0x00003D34 File Offset: 0x00001F34
		public void Destroy()
		{
			this.CloseSubscriber();
			if (this.ID > 0U)
			{
				this.SwitchGroup(null);
				if (this.sv != null)
				{
					this.sv.ReturnUID(this.ID);
				}
			}
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x00003D66 File Offset: 0x00001F66
		public void EnterPool()
		{
			this.ID = 0U;
			this.connection = null;
			this.group = null;
			this.secondaryGroup = null;
			this.sv = null;
			this.cl = null;
			this.handler = null;
			this.updateSubscriptions = false;
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x00002209 File Offset: 0x00000409
		public void LeavePool()
		{
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x00003DA0 File Offset: 0x00001FA0
		public void StartSubscriber()
		{
			if (this.subscriber != null)
			{
				Debug.Log("BecomeSubscriber called twice!");
				return;
			}
			this.subscriber = this.sv.visibility.CreateSubscriber(this.connection);
			this.OnSubscriptionChange();
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x00003DD7 File Offset: 0x00001FD7
		public void OnConnected(Connection c)
		{
			this.connection = c;
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x00003DE0 File Offset: 0x00001FE0
		public void OnDisconnected()
		{
			this.connection = null;
			this.CloseSubscriber();
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x00003DEF File Offset: 0x00001FEF
		public void CloseSubscriber()
		{
			if (this.subscriber != null)
			{
				this.sv.visibility.DestroySubscriber(ref this.subscriber);
			}
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x00003E10 File Offset: 0x00002010
		public bool UpdateGroups(Vector3 position)
		{
			Debug.Assert(this.sv != null, "SV IS NULL");
			Debug.Assert(this.sv.visibility != null, "sv.visibility IS NULL");
			Group newGroup = this.sv.visibility.GetGroup(position);
			return this.SwitchGroup(newGroup);
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x00003E64 File Offset: 0x00002064
		public bool SwitchGroup(Group newGroup)
		{
			if (newGroup == this.group)
			{
				return false;
			}
			using (TimeWarning.New("SwitchGroup", 0))
			{
				if (this.group != null)
				{
					using (TimeWarning.New("group.Leave", 0))
					{
						this.group.Leave(this);
					}
				}
				Group oldGroup = this.group;
				this.group = newGroup;
				if (this.group != null)
				{
					using (TimeWarning.New("group.Join", 0))
					{
						this.group.Join(this);
					}
				}
				if (this.handler != null && this.group != null)
				{
					using (TimeWarning.New("OnNetworkGroupChange", 0))
					{
						this.handler.OnNetworkGroupChange();
					}
				}
				using (TimeWarning.New("OnSubscriptionChange", 0))
				{
					this.OnSubscriptionChange();
				}
				using (TimeWarning.New("OnGroupTransition", 0))
				{
					this.OnGroupTransition(oldGroup);
				}
			}
			return true;
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x00003FB8 File Offset: 0x000021B8
		internal void OnGroupTransition(Group oldGroup)
		{
			if (oldGroup == null)
			{
				if (this.group != null && this.handler != null)
				{
					this.handler.OnNetworkSubscribersEnter(this.group.subscribers);
				}
				return;
			}
			if (this.group == null)
			{
				if (oldGroup != null && this.handler != null)
				{
					this.handler.OnNetworkSubscribersLeave(oldGroup.subscribers);
				}
				return;
			}
			List<Connection> list = Pool.GetList<Connection>();
			List<Connection> list2 = Pool.GetList<Connection>();
			oldGroup.subscribers.Compare(this.group.subscribers, list, list2, null);
			if (this.handler != null)
			{
				this.handler.OnNetworkSubscribersEnter(list);
			}
			if (this.handler != null)
			{
				this.handler.OnNetworkSubscribersLeave(list2);
			}
			Pool.FreeList<Connection>(ref list);
			Pool.FreeList<Connection>(ref list2);
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x00004070 File Offset: 0x00002270
		internal void OnSubscriptionChange()
		{
			if (this.subscriber == null)
			{
				return;
			}
			if (this.group != null && !this.subscriber.IsSubscribed(this.group))
			{
				this.subscriber.Subscribe(this.group);
				if (this.handler != null)
				{
					this.handler.OnNetworkGroupEnter(this.group);
				}
			}
			this.updateSubscriptions = true;
			this.UpdateHighPrioritySubscriptions();
		}

		// Token: 0x060000EA RID: 234 RVA: 0x000040DC File Offset: 0x000022DC
		public bool SwitchSecondaryGroup(Group newGroup)
		{
			if (newGroup == this.secondaryGroup)
			{
				return false;
			}
			using (TimeWarning.New("SwitchSecondaryGroup", 0))
			{
				this.secondaryGroup = newGroup;
				using (TimeWarning.New("OnSubscriptionChange", 0))
				{
					this.OnSubscriptionChange();
				}
			}
			return true;
		}

		// Token: 0x060000EB RID: 235 RVA: 0x0000414C File Offset: 0x0000234C
		private void AddVisibleFromNear(Group additionalGroup, List<Group> groupsVisible)
		{
			if (additionalGroup == null)
			{
				return;
			}
			List<Group> list = Pool.GetList<Group>();
			this.sv.visibility.GetVisibleFromNear(additionalGroup, list);
			for (int i = 0; i < list.Count; i++)
			{
				Group item = list[i];
				if (!groupsVisible.Contains(item))
				{
					groupsVisible.Add(item);
				}
			}
			Pool.FreeList<Group>(ref list);
		}

		// Token: 0x060000EC RID: 236 RVA: 0x000041A8 File Offset: 0x000023A8
		public bool UpdateSubscriptions(int removeLimit, int addLimit)
		{
			if (!this.updateSubscriptions)
			{
				return false;
			}
			if (this.subscriber == null)
			{
				return false;
			}
			using (TimeWarning.New("UpdateSubscriptions", 0))
			{
				this.updateSubscriptions = false;
				List<Group> list = Pool.GetList<Group>();
				List<Group> list2 = Pool.GetList<Group>();
				List<Group> list3 = Pool.GetList<Group>();
				this.sv.visibility.GetVisibleFromFar(this.group, list3);
				this.AddVisibleFromNear(this.secondaryGroup, list3);
				this.subscriber.subscribed.Compare(list3, list, list2, null);
				for (int i = 0; i < list2.Count; i++)
				{
					Group group = list2[i];
					if (removeLimit > 0)
					{
						this.subscriber.Unsubscribe(group);
						if (this.handler != null)
						{
							this.handler.OnNetworkGroupLeave(group);
						}
						removeLimit -= group.networkables.Count;
					}
					else
					{
						this.updateSubscriptions = true;
					}
				}
				for (int j = 0; j < list.Count; j++)
				{
					Group group2 = list[j];
					if (addLimit > 0)
					{
						this.subscriber.Subscribe(group2);
						if (this.handler != null)
						{
							this.handler.OnNetworkGroupEnter(group2);
						}
						addLimit -= group2.networkables.Count;
					}
					else
					{
						this.updateSubscriptions = true;
					}
				}
				Pool.FreeList<Group>(ref list);
				Pool.FreeList<Group>(ref list2);
				Pool.FreeList<Group>(ref list3);
			}
			return true;
		}

		// Token: 0x060000ED RID: 237 RVA: 0x00004324 File Offset: 0x00002524
		public bool UpdateHighPrioritySubscriptions()
		{
			if (this.subscriber == null)
			{
				return false;
			}
			using (TimeWarning.New("UpdateHighPrioritySubscriptions", 0))
			{
				List<Group> list = Pool.GetList<Group>();
				List<Group> list2 = Pool.GetList<Group>();
				this.sv.visibility.GetVisibleFromNear(this.group, list2);
				this.AddVisibleFromNear(this.secondaryGroup, list2);
				this.subscriber.subscribed.Compare(list2, list, null, null);
				for (int i = 0; i < list.Count; i++)
				{
					Group group = list[i];
					this.subscriber.Subscribe(group);
					if (this.handler != null)
					{
						this.handler.OnNetworkGroupEnter(group);
					}
				}
				Pool.FreeList<Group>(ref list);
				Pool.FreeList<Group>(ref list2);
			}
			return true;
		}

		// Token: 0x04000066 RID: 102
		public uint ID;

		// Token: 0x04000068 RID: 104
		public Group group;

		// Token: 0x04000069 RID: 105
		public Group secondaryGroup;

		// Token: 0x0400006A RID: 106
		public Subscriber subscriber;

		// Token: 0x0400006B RID: 107
		public NetworkHandler handler;

		// Token: 0x0400006C RID: 108
		private bool updateSubscriptions;

		// Token: 0x0400006D RID: 109
		internal Server sv;

		// Token: 0x0400006E RID: 110
		internal Client cl;
	}
}
