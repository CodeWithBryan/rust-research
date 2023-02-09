using System;
using UnityEngine;

// Token: 0x02000264 RID: 612
public class Socket_Base : PrefabAttribute
{
	// Token: 0x06001BA2 RID: 7074 RVA: 0x000C05DC File Offset: 0x000BE7DC
	public Socket_Base()
	{
		this.cachedType = base.GetType();
	}

	// Token: 0x06001BA3 RID: 7075 RVA: 0x000BCF3B File Offset: 0x000BB13B
	public Vector3 GetSelectPivot(Vector3 position, Quaternion rotation)
	{
		return position + rotation * this.worldPosition;
	}

	// Token: 0x06001BA4 RID: 7076 RVA: 0x000C0636 File Offset: 0x000BE836
	public OBB GetSelectBounds(Vector3 position, Quaternion rotation)
	{
		return new OBB(position + rotation * this.worldPosition, Vector3.one, rotation * this.worldRotation, new Bounds(this.selectCenter, this.selectSize));
	}

	// Token: 0x06001BA5 RID: 7077 RVA: 0x000C0671 File Offset: 0x000BE871
	protected override Type GetIndexedType()
	{
		return typeof(Socket_Base);
	}

	// Token: 0x06001BA6 RID: 7078 RVA: 0x000C0680 File Offset: 0x000BE880
	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		this.position = base.transform.position;
		this.rotation = base.transform.rotation;
		this.socketMods = base.GetComponentsInChildren<SocketMod>(true);
		SocketMod[] array = this.socketMods;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].baseSocket = this;
		}
	}

	// Token: 0x06001BA7 RID: 7079 RVA: 0x000C06E7 File Offset: 0x000BE8E7
	public virtual bool TestTarget(Construction.Target target)
	{
		return target.socket != null;
	}

	// Token: 0x06001BA8 RID: 7080 RVA: 0x000C06F8 File Offset: 0x000BE8F8
	public virtual bool IsCompatible(Socket_Base socket)
	{
		return !(socket == null) && (socket.male || this.male) && (socket.female || this.female) && socket.cachedType == this.cachedType;
	}

	// Token: 0x06001BA9 RID: 7081 RVA: 0x000C0745 File Offset: 0x000BE945
	public virtual bool CanConnect(Vector3 position, Quaternion rotation, Socket_Base socket, Vector3 socketPosition, Quaternion socketRotation)
	{
		return this.IsCompatible(socket);
	}

	// Token: 0x06001BAA RID: 7082 RVA: 0x000C0750 File Offset: 0x000BE950
	public virtual Construction.Placement DoPlacement(Construction.Target target)
	{
		Quaternion quaternion = Quaternion.LookRotation(target.normal, Vector3.up) * Quaternion.Euler(target.rotation);
		Vector3 a = target.position;
		a -= quaternion * this.position;
		return new Construction.Placement
		{
			rotation = quaternion,
			position = a
		};
	}

	// Token: 0x06001BAB RID: 7083 RVA: 0x000C07AC File Offset: 0x000BE9AC
	public virtual bool CheckSocketMods(Construction.Placement placement)
	{
		SocketMod[] array = this.socketMods;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ModifyPlacement(placement);
		}
		foreach (SocketMod socketMod in this.socketMods)
		{
			if (!socketMod.DoCheck(placement))
			{
				if (socketMod.FailedPhrase.IsValid())
				{
					Construction.lastPlacementError = "Failed Check: (" + socketMod.FailedPhrase.translated + ")";
				}
				return false;
			}
		}
		return true;
	}

	// Token: 0x040014AD RID: 5293
	public bool male = true;

	// Token: 0x040014AE RID: 5294
	public bool maleDummy;

	// Token: 0x040014AF RID: 5295
	public bool female;

	// Token: 0x040014B0 RID: 5296
	public bool femaleDummy;

	// Token: 0x040014B1 RID: 5297
	public bool femaleNoStability;

	// Token: 0x040014B2 RID: 5298
	public bool monogamous;

	// Token: 0x040014B3 RID: 5299
	[NonSerialized]
	public Vector3 position;

	// Token: 0x040014B4 RID: 5300
	[NonSerialized]
	public Quaternion rotation;

	// Token: 0x040014B5 RID: 5301
	private Type cachedType;

	// Token: 0x040014B6 RID: 5302
	public Vector3 selectSize = new Vector3(2f, 0.1f, 2f);

	// Token: 0x040014B7 RID: 5303
	public Vector3 selectCenter = new Vector3(0f, 0f, 1f);

	// Token: 0x040014B8 RID: 5304
	[ReadOnly]
	public string socketName;

	// Token: 0x040014B9 RID: 5305
	[NonSerialized]
	public SocketMod[] socketMods;

	// Token: 0x040014BA RID: 5306
	public Socket_Base.OccupiedSocketCheck[] checkOccupiedSockets;

	// Token: 0x02000C3A RID: 3130
	[Serializable]
	public class OccupiedSocketCheck
	{
		// Token: 0x0400416A RID: 16746
		public Socket_Base Socket;

		// Token: 0x0400416B RID: 16747
		public bool FemaleDummy;
	}
}
