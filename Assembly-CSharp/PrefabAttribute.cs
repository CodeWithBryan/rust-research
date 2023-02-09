using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020008E1 RID: 2273
public abstract class PrefabAttribute : MonoBehaviour, IPrefabPreProcess
{
	// Token: 0x1700041A RID: 1050
	// (get) Token: 0x0600366E RID: 13934 RVA: 0x0014422E File Offset: 0x0014242E
	public bool isClient
	{
		get
		{
			return !this.isServer;
		}
	}

	// Token: 0x0600366F RID: 13935 RVA: 0x0014423C File Offset: 0x0014243C
	public virtual void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (bundling)
		{
			return;
		}
		this.fullName = name;
		this.hierachyName = base.transform.GetRecursiveName("");
		this.prefabID = StringPool.Get(name);
		this.instanceID = base.GetInstanceID();
		this.worldPosition = base.transform.position;
		this.worldRotation = base.transform.rotation;
		this.worldForward = base.transform.forward;
		this.localPosition = base.transform.localPosition;
		this.localScale = base.transform.localScale;
		this.localRotation = base.transform.localRotation;
		if (serverside)
		{
			this.prefabAttribute = PrefabAttribute.server;
			this.gameManager = GameManager.server;
			this.isServer = true;
		}
		this.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		if (serverside)
		{
			PrefabAttribute.server.Add(this.prefabID, this);
		}
		preProcess.RemoveComponent(this);
		preProcess.NominateForDeletion(base.gameObject);
	}

	// Token: 0x06003670 RID: 13936 RVA: 0x000059DD File Offset: 0x00003BDD
	protected virtual void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
	}

	// Token: 0x06003671 RID: 13937
	protected abstract Type GetIndexedType();

	// Token: 0x06003672 RID: 13938 RVA: 0x00144340 File Offset: 0x00142540
	public static bool operator ==(PrefabAttribute x, PrefabAttribute y)
	{
		return PrefabAttribute.ComparePrefabAttribute(x, y);
	}

	// Token: 0x06003673 RID: 13939 RVA: 0x00144349 File Offset: 0x00142549
	public static bool operator !=(PrefabAttribute x, PrefabAttribute y)
	{
		return !PrefabAttribute.ComparePrefabAttribute(x, y);
	}

	// Token: 0x06003674 RID: 13940 RVA: 0x00144358 File Offset: 0x00142558
	public override bool Equals(object o)
	{
		PrefabAttribute y;
		return (y = (o as PrefabAttribute)) != null && PrefabAttribute.ComparePrefabAttribute(this, y);
	}

	// Token: 0x06003675 RID: 13941 RVA: 0x00144378 File Offset: 0x00142578
	public override int GetHashCode()
	{
		if (this.hierachyName == null)
		{
			return base.GetHashCode();
		}
		return this.hierachyName.GetHashCode();
	}

	// Token: 0x06003676 RID: 13942 RVA: 0x00143CE8 File Offset: 0x00141EE8
	public static implicit operator bool(PrefabAttribute exists)
	{
		return exists != null;
	}

	// Token: 0x06003677 RID: 13943 RVA: 0x00144394 File Offset: 0x00142594
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool ComparePrefabAttribute(PrefabAttribute x, PrefabAttribute y)
	{
		bool flag = x == null;
		bool flag2 = y == null;
		return (flag && flag2) || (!flag && !flag2 && x.instanceID == y.instanceID);
	}

	// Token: 0x06003678 RID: 13944 RVA: 0x001443CA File Offset: 0x001425CA
	public override string ToString()
	{
		if (this == null)
		{
			return "null";
		}
		return this.hierachyName;
	}

	// Token: 0x04003177 RID: 12663
	[NonSerialized]
	public Vector3 worldPosition;

	// Token: 0x04003178 RID: 12664
	[NonSerialized]
	public Quaternion worldRotation;

	// Token: 0x04003179 RID: 12665
	[NonSerialized]
	public Vector3 worldForward;

	// Token: 0x0400317A RID: 12666
	[NonSerialized]
	public Vector3 localPosition;

	// Token: 0x0400317B RID: 12667
	[NonSerialized]
	public Vector3 localScale;

	// Token: 0x0400317C RID: 12668
	[NonSerialized]
	public Quaternion localRotation;

	// Token: 0x0400317D RID: 12669
	[NonSerialized]
	public string fullName;

	// Token: 0x0400317E RID: 12670
	[NonSerialized]
	public string hierachyName;

	// Token: 0x0400317F RID: 12671
	[NonSerialized]
	public uint prefabID;

	// Token: 0x04003180 RID: 12672
	[NonSerialized]
	public int instanceID;

	// Token: 0x04003181 RID: 12673
	[NonSerialized]
	public PrefabAttribute.Library prefabAttribute;

	// Token: 0x04003182 RID: 12674
	[NonSerialized]
	public GameManager gameManager;

	// Token: 0x04003183 RID: 12675
	[NonSerialized]
	public bool isServer;

	// Token: 0x04003184 RID: 12676
	public static PrefabAttribute.Library server = new PrefabAttribute.Library(false, true);

	// Token: 0x02000E5B RID: 3675
	public class AttributeCollection
	{
		// Token: 0x0600505A RID: 20570 RVA: 0x001A16CC File Offset: 0x0019F8CC
		internal List<PrefabAttribute> Find(Type t)
		{
			List<PrefabAttribute> list;
			if (this.attributes.TryGetValue(t, out list))
			{
				return list;
			}
			list = new List<PrefabAttribute>();
			this.attributes.Add(t, list);
			return list;
		}

		// Token: 0x0600505B RID: 20571 RVA: 0x001A1700 File Offset: 0x0019F900
		public T[] Find<T>()
		{
			if (this.cache == null)
			{
				this.cache = new Dictionary<Type, object>();
			}
			object obj;
			if (this.cache.TryGetValue(typeof(T), out obj))
			{
				return (T[])obj;
			}
			obj = this.Find(typeof(T)).Cast<T>().ToArray<T>();
			this.cache.Add(typeof(T), obj);
			return (T[])obj;
		}

		// Token: 0x0600505C RID: 20572 RVA: 0x001A1777 File Offset: 0x0019F977
		public void Add(PrefabAttribute attribute)
		{
			List<PrefabAttribute> list = this.Find(attribute.GetIndexedType());
			Assert.IsTrue(!list.Contains(attribute), "AttributeCollection.Add: Adding twice to list");
			list.Add(attribute);
			this.cache = null;
		}

		// Token: 0x04004A28 RID: 18984
		private Dictionary<Type, List<PrefabAttribute>> attributes = new Dictionary<Type, List<PrefabAttribute>>();

		// Token: 0x04004A29 RID: 18985
		private Dictionary<Type, object> cache = new Dictionary<Type, object>();
	}

	// Token: 0x02000E5C RID: 3676
	public class Library
	{
		// Token: 0x0600505E RID: 20574 RVA: 0x001A17C4 File Offset: 0x0019F9C4
		public Library(bool clientside, bool serverside)
		{
			this.clientside = clientside;
			this.serverside = serverside;
		}

		// Token: 0x0600505F RID: 20575 RVA: 0x001A17E8 File Offset: 0x0019F9E8
		public PrefabAttribute.AttributeCollection Find(uint prefabID, bool warmup = true)
		{
			PrefabAttribute.AttributeCollection attributeCollection;
			if (this.prefabs.TryGetValue(prefabID, out attributeCollection))
			{
				return attributeCollection;
			}
			attributeCollection = new PrefabAttribute.AttributeCollection();
			this.prefabs.Add(prefabID, attributeCollection);
			if (warmup && (!this.clientside || this.serverside))
			{
				if (!this.clientside && this.serverside)
				{
					GameManager.server.FindPrefab(prefabID);
				}
				else if (this.clientside)
				{
					bool flag = this.serverside;
				}
			}
			return attributeCollection;
		}

		// Token: 0x06005060 RID: 20576 RVA: 0x001A185C File Offset: 0x0019FA5C
		public T Find<T>(uint prefabID) where T : PrefabAttribute
		{
			T[] array = this.Find(prefabID, true).Find<T>();
			if (array.Length == 0)
			{
				return default(T);
			}
			return array[0];
		}

		// Token: 0x06005061 RID: 20577 RVA: 0x001A188C File Offset: 0x0019FA8C
		public T[] FindAll<T>(uint prefabID) where T : PrefabAttribute
		{
			return this.Find(prefabID, true).Find<T>();
		}

		// Token: 0x06005062 RID: 20578 RVA: 0x001A189B File Offset: 0x0019FA9B
		public void Add(uint prefabID, PrefabAttribute attribute)
		{
			this.Find(prefabID, false).Add(attribute);
		}

		// Token: 0x06005063 RID: 20579 RVA: 0x001A18AB File Offset: 0x0019FAAB
		public void Invalidate(uint prefabID)
		{
			this.prefabs.Remove(prefabID);
		}

		// Token: 0x04004A2A RID: 18986
		public bool clientside;

		// Token: 0x04004A2B RID: 18987
		public bool serverside;

		// Token: 0x04004A2C RID: 18988
		private Dictionary<uint, PrefabAttribute.AttributeCollection> prefabs = new Dictionary<uint, PrefabAttribute.AttributeCollection>();
	}
}
