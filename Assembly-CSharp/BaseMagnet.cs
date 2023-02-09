using System;
using UnityEngine;

// Token: 0x02000468 RID: 1128
public class BaseMagnet : MonoBehaviour
{
	// Token: 0x060024D7 RID: 9431 RVA: 0x000E81F0 File Offset: 0x000E63F0
	public bool HasConnectedObject()
	{
		return this.fixedJoint.connectedBody != null && this.isMagnetOn;
	}

	// Token: 0x060024D8 RID: 9432 RVA: 0x000E8210 File Offset: 0x000E6410
	public OBB GetConnectedOBB(float scale = 1f)
	{
		if (this.fixedJoint.connectedBody == null)
		{
			Debug.LogError("BaseMagnet returning fake OBB because no connected body!");
			return new OBB(Vector3.zero, Vector3.one, Quaternion.identity);
		}
		BaseEntity component = this.fixedJoint.connectedBody.gameObject.GetComponent<BaseEntity>();
		Bounds bounds = component.bounds;
		bounds.extents *= scale;
		return new OBB(component.transform.position, component.transform.rotation, bounds);
	}

	// Token: 0x060024D9 RID: 9433 RVA: 0x000E829C File Offset: 0x000E649C
	public void SetCollisionsEnabled(GameObject other, bool wants)
	{
		Collider[] componentsInChildren = other.GetComponentsInChildren<Collider>();
		Collider[] componentsInChildren2 = this.colliderSource.GetComponentsInChildren<Collider>();
		foreach (Collider collider in componentsInChildren)
		{
			foreach (Collider collider2 in componentsInChildren2)
			{
				Physics.IgnoreCollision(collider, collider2, !wants);
			}
		}
	}

	// Token: 0x060024DA RID: 9434 RVA: 0x000E82F8 File Offset: 0x000E64F8
	public virtual void SetMagnetEnabled(bool wantsOn, BasePlayer forPlayer)
	{
		if (this.isMagnetOn == wantsOn)
		{
			return;
		}
		this.associatedPlayer = forPlayer;
		this.isMagnetOn = wantsOn;
		if (this.isMagnetOn)
		{
			this.OnMagnetEnabled();
		}
		else
		{
			this.OnMagnetDisabled();
		}
		if (this.entityOwner != null)
		{
			this.entityOwner.SetFlag(this.magnetFlag, this.isMagnetOn, false, true);
		}
	}

	// Token: 0x060024DB RID: 9435 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnMagnetEnabled()
	{
	}

	// Token: 0x060024DC RID: 9436 RVA: 0x000E835C File Offset: 0x000E655C
	public virtual void OnMagnetDisabled()
	{
		if (this.fixedJoint.connectedBody)
		{
			this.SetCollisionsEnabled(this.fixedJoint.connectedBody.gameObject, true);
			Rigidbody connectedBody = this.fixedJoint.connectedBody;
			this.fixedJoint.connectedBody = null;
			connectedBody.WakeUp();
		}
	}

	// Token: 0x060024DD RID: 9437 RVA: 0x000E83AE File Offset: 0x000E65AE
	public bool IsMagnetOn()
	{
		return this.isMagnetOn;
	}

	// Token: 0x060024DE RID: 9438 RVA: 0x000E83B8 File Offset: 0x000E65B8
	public void MagnetThink(float delta)
	{
		if (!this.isMagnetOn)
		{
			return;
		}
		Vector3 position = this.magnetTrigger.transform.position;
		if (this.magnetTrigger.entityContents != null)
		{
			foreach (BaseEntity baseEntity in this.magnetTrigger.entityContents)
			{
				if (baseEntity.syncPosition)
				{
					Rigidbody component = baseEntity.GetComponent<Rigidbody>();
					if (!(component == null) && !component.isKinematic && !baseEntity.isClient)
					{
						OBB obb = new OBB(baseEntity.transform.position, baseEntity.transform.rotation, baseEntity.bounds);
						if (obb.Contains(this.attachDepthPoint.position))
						{
							baseEntity.GetComponent<MagnetLiftable>().SetMagnetized(true, this, this.associatedPlayer);
							if (this.fixedJoint.connectedBody == null)
							{
								Effect.server.Run(this.attachEffect.resourcePath, this.attachDepthPoint.position, -this.attachDepthPoint.up, null, false);
								this.fixedJoint.connectedBody = component;
								this.SetCollisionsEnabled(component.gameObject, false);
								continue;
							}
						}
						if (this.fixedJoint.connectedBody == null)
						{
							Vector3 position2 = baseEntity.transform.position;
							float b = Vector3.Distance(position2, position);
							Vector3 a = Vector3Ex.Direction(position, position2);
							float d = 1f / Mathf.Max(1f, b);
							component.AddForce(a * this.magnetForce * d, ForceMode.Acceleration);
						}
					}
				}
			}
		}
	}

	// Token: 0x04001D7D RID: 7549
	public BaseEntity entityOwner;

	// Token: 0x04001D7E RID: 7550
	public BaseEntity.Flags magnetFlag = BaseEntity.Flags.Reserved6;

	// Token: 0x04001D7F RID: 7551
	public TriggerMagnet magnetTrigger;

	// Token: 0x04001D80 RID: 7552
	public FixedJoint fixedJoint;

	// Token: 0x04001D81 RID: 7553
	public Rigidbody kinematicAttachmentBody;

	// Token: 0x04001D82 RID: 7554
	public float magnetForce;

	// Token: 0x04001D83 RID: 7555
	public Transform attachDepthPoint;

	// Token: 0x04001D84 RID: 7556
	public GameObjectRef attachEffect;

	// Token: 0x04001D85 RID: 7557
	public bool isMagnetOn;

	// Token: 0x04001D86 RID: 7558
	public GameObject colliderSource;

	// Token: 0x04001D87 RID: 7559
	private BasePlayer associatedPlayer;
}
