using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Rust;
using UnityEngine;

// Token: 0x02000555 RID: 1365
public class TriggerBase : BaseMonoBehaviour
{
	// Token: 0x1700033D RID: 829
	// (get) Token: 0x0600298E RID: 10638 RVA: 0x000FC501 File Offset: 0x000FA701
	public bool HasAnyContents
	{
		get
		{
			return !this.contents.IsNullOrEmpty<GameObject>();
		}
	}

	// Token: 0x1700033E RID: 830
	// (get) Token: 0x0600298F RID: 10639 RVA: 0x000FC511 File Offset: 0x000FA711
	public bool HasAnyEntityContents
	{
		get
		{
			return !this.entityContents.IsNullOrEmpty<BaseEntity>();
		}
	}

	// Token: 0x06002990 RID: 10640 RVA: 0x000FC524 File Offset: 0x000FA724
	internal virtual GameObject InterestedInObject(GameObject obj)
	{
		int num = 1 << obj.layer;
		if ((this.interestLayers.value & num) != num)
		{
			return null;
		}
		return obj;
	}

	// Token: 0x06002991 RID: 10641 RVA: 0x000FC550 File Offset: 0x000FA750
	protected virtual void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		if (this.contents == null)
		{
			return;
		}
		foreach (GameObject targetObj in this.contents.ToArray<GameObject>())
		{
			this.OnTriggerExit(targetObj);
		}
		this.contents = null;
	}

	// Token: 0x06002992 RID: 10642 RVA: 0x000FC59A File Offset: 0x000FA79A
	internal virtual void OnEntityEnter(BaseEntity ent)
	{
		if (ent == null)
		{
			return;
		}
		if (this.entityContents == null)
		{
			this.entityContents = new HashSet<BaseEntity>();
		}
		this.entityContents.Add(ent);
	}

	// Token: 0x06002993 RID: 10643 RVA: 0x000FC5C6 File Offset: 0x000FA7C6
	internal virtual void OnEntityLeave(BaseEntity ent)
	{
		if (this.entityContents == null)
		{
			return;
		}
		this.entityContents.Remove(ent);
	}

	// Token: 0x06002994 RID: 10644 RVA: 0x000FC5E0 File Offset: 0x000FA7E0
	internal virtual void OnObjectAdded(GameObject obj, Collider col)
	{
		if (obj == null)
		{
			return;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity)
		{
			baseEntity.EnterTrigger(this);
			this.OnEntityEnter(baseEntity);
		}
	}

	// Token: 0x06002995 RID: 10645 RVA: 0x000FC618 File Offset: 0x000FA818
	internal virtual void OnObjectRemoved(GameObject obj)
	{
		if (obj == null)
		{
			return;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity)
		{
			bool flag = false;
			foreach (GameObject gameObject in this.contents)
			{
				if (gameObject == null)
				{
					Debug.LogWarning("Trigger " + this.ToString() + " contains null object.");
				}
				else if (gameObject.ToBaseEntity() == baseEntity)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				baseEntity.LeaveTrigger(this);
				this.OnEntityLeave(baseEntity);
			}
		}
	}

	// Token: 0x06002996 RID: 10646 RVA: 0x000FC6C8 File Offset: 0x000FA8C8
	internal void RemoveInvalidEntities()
	{
		if (this.entityContents.IsNullOrEmpty<BaseEntity>())
		{
			return;
		}
		Collider component = base.GetComponent<Collider>();
		if (component == null)
		{
			return;
		}
		Bounds bounds = component.bounds;
		bounds.Expand(1f);
		List<BaseEntity> list = null;
		foreach (BaseEntity baseEntity in this.entityContents)
		{
			if (baseEntity == null)
			{
				if (Debugging.checktriggers)
				{
					Debug.LogWarning("Trigger " + this.ToString() + " contains destroyed entity.");
				}
				if (list == null)
				{
					list = Facepunch.Pool.GetList<BaseEntity>();
				}
				list.Add(baseEntity);
			}
			else if (!bounds.Contains(baseEntity.ClosestPoint(base.transform.position)))
			{
				if (Debugging.checktriggers)
				{
					Debug.LogWarning("Trigger " + this.ToString() + " contains entity that is too far away: " + baseEntity.ToString());
				}
				if (list == null)
				{
					list = Facepunch.Pool.GetList<BaseEntity>();
				}
				list.Add(baseEntity);
			}
		}
		if (list != null)
		{
			foreach (BaseEntity ent in list)
			{
				this.RemoveEntity(ent);
			}
			Facepunch.Pool.FreeList<BaseEntity>(ref list);
		}
	}

	// Token: 0x06002997 RID: 10647 RVA: 0x000FC82C File Offset: 0x000FAA2C
	internal bool CheckEntity(BaseEntity ent)
	{
		if (ent == null)
		{
			return true;
		}
		Collider component = base.GetComponent<Collider>();
		if (component == null)
		{
			return true;
		}
		Bounds bounds = component.bounds;
		bounds.Expand(1f);
		return bounds.Contains(ent.ClosestPoint(base.transform.position));
	}

	// Token: 0x06002998 RID: 10648 RVA: 0x000059DD File Offset: 0x00003BDD
	internal virtual void OnObjects()
	{
	}

	// Token: 0x06002999 RID: 10649 RVA: 0x000FC881 File Offset: 0x000FAA81
	internal virtual void OnEmpty()
	{
		this.contents = null;
		this.entityContents = null;
	}

	// Token: 0x0600299A RID: 10650 RVA: 0x000FC894 File Offset: 0x000FAA94
	public void RemoveObject(GameObject obj)
	{
		if (obj == null)
		{
			return;
		}
		Collider component = obj.GetComponent<Collider>();
		if (component == null)
		{
			return;
		}
		this.OnTriggerExit(component);
	}

	// Token: 0x0600299B RID: 10651 RVA: 0x000FC8C4 File Offset: 0x000FAAC4
	public void RemoveEntity(BaseEntity ent)
	{
		if (this == null || this.contents == null || ent == null)
		{
			return;
		}
		List<GameObject> list = Facepunch.Pool.GetList<GameObject>();
		foreach (GameObject gameObject in this.contents)
		{
			if (gameObject != null && gameObject.GetComponentInParent<BaseEntity>() == ent)
			{
				list.Add(gameObject);
			}
		}
		foreach (GameObject targetObj in list)
		{
			this.OnTriggerExit(targetObj);
		}
		Facepunch.Pool.FreeList<GameObject>(ref list);
	}

	// Token: 0x0600299C RID: 10652 RVA: 0x000FC998 File Offset: 0x000FAB98
	public void OnTriggerEnter(Collider collider)
	{
		if (this == null)
		{
			return;
		}
		if (!base.enabled)
		{
			return;
		}
		using (TimeWarning.New("TriggerBase.OnTriggerEnter", 0))
		{
			GameObject gameObject = this.InterestedInObject(collider.gameObject);
			if (gameObject == null)
			{
				return;
			}
			if (this.contents == null)
			{
				this.contents = new HashSet<GameObject>();
			}
			if (this.contents.Contains(gameObject))
			{
				return;
			}
			bool count = this.contents.Count != 0;
			this.contents.Add(gameObject);
			this.OnObjectAdded(gameObject, collider);
			if (!count && this.contents.Count == 1)
			{
				this.OnObjects();
			}
		}
		if (Debugging.checktriggers)
		{
			this.RemoveInvalidEntities();
		}
	}

	// Token: 0x0600299D RID: 10653 RVA: 0x00007074 File Offset: 0x00005274
	internal virtual bool SkipOnTriggerExit(Collider collider)
	{
		return false;
	}

	// Token: 0x0600299E RID: 10654 RVA: 0x000FCA60 File Offset: 0x000FAC60
	public void OnTriggerExit(Collider collider)
	{
		if (this == null)
		{
			return;
		}
		if (collider == null)
		{
			return;
		}
		if (this.SkipOnTriggerExit(collider))
		{
			return;
		}
		GameObject gameObject = this.InterestedInObject(collider.gameObject);
		if (gameObject == null)
		{
			return;
		}
		this.OnTriggerExit(gameObject);
		if (Debugging.checktriggers)
		{
			this.RemoveInvalidEntities();
		}
	}

	// Token: 0x0600299F RID: 10655 RVA: 0x000FCAB8 File Offset: 0x000FACB8
	private void OnTriggerExit(GameObject targetObj)
	{
		if (this.contents == null)
		{
			return;
		}
		if (!this.contents.Contains(targetObj))
		{
			return;
		}
		this.contents.Remove(targetObj);
		this.OnObjectRemoved(targetObj);
		if (this.contents == null || this.contents.Count == 0)
		{
			this.OnEmpty();
		}
	}

	// Token: 0x040021AB RID: 8619
	public LayerMask interestLayers;

	// Token: 0x040021AC RID: 8620
	[NonSerialized]
	public HashSet<GameObject> contents;

	// Token: 0x040021AD RID: 8621
	[NonSerialized]
	public HashSet<BaseEntity> entityContents;
}
