using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using UnityEngine;

// Token: 0x020002E0 RID: 736
public class HitboxSystem : MonoBehaviour, IPrefabPreProcess
{
	// Token: 0x06001D1A RID: 7450 RVA: 0x000C69D4 File Offset: 0x000C4BD4
	public void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		List<HitboxDefinition> list = Pool.GetList<HitboxDefinition>();
		base.GetComponentsInChildren<HitboxDefinition>(list);
		if (serverside)
		{
			foreach (HitboxDefinition component in list)
			{
				if (preProcess != null)
				{
					preProcess.RemoveComponent(component);
				}
			}
			if (preProcess != null)
			{
				preProcess.RemoveComponent(this);
			}
		}
		if (clientside)
		{
			this.hitboxes.Clear();
			foreach (HitboxDefinition hitboxDefinition in from x in list
			orderby x.priority
			select x)
			{
				HitboxSystem.HitboxShape item = new HitboxSystem.HitboxShape
				{
					bone = hitboxDefinition.transform,
					localTransform = hitboxDefinition.LocalMatrix,
					colliderMaterial = hitboxDefinition.physicMaterial,
					type = hitboxDefinition.type
				};
				this.hitboxes.Add(item);
				if (preProcess != null)
				{
					preProcess.RemoveComponent(hitboxDefinition);
				}
			}
		}
		Pool.FreeList<HitboxDefinition>(ref list);
	}

	// Token: 0x040016A1 RID: 5793
	public List<HitboxSystem.HitboxShape> hitboxes = new List<HitboxSystem.HitboxShape>();

	// Token: 0x02000C4E RID: 3150
	[Serializable]
	public class HitboxShape
	{
		// Token: 0x1700061B RID: 1563
		// (get) Token: 0x06004C71 RID: 19569 RVA: 0x001959F7 File Offset: 0x00193BF7
		public Matrix4x4 Transform
		{
			get
			{
				return this.transform;
			}
		}

		// Token: 0x1700061C RID: 1564
		// (get) Token: 0x06004C72 RID: 19570 RVA: 0x001959FF File Offset: 0x00193BFF
		public Vector3 Position
		{
			get
			{
				return this.transform.MultiplyPoint(Vector3.zero);
			}
		}

		// Token: 0x1700061D RID: 1565
		// (get) Token: 0x06004C73 RID: 19571 RVA: 0x00195A11 File Offset: 0x00193C11
		public Quaternion Rotation
		{
			get
			{
				return this.transform.rotation;
			}
		}

		// Token: 0x1700061E RID: 1566
		// (get) Token: 0x06004C74 RID: 19572 RVA: 0x00195A1E File Offset: 0x00193C1E
		// (set) Token: 0x06004C75 RID: 19573 RVA: 0x00195A26 File Offset: 0x00193C26
		public Vector3 Size { get; private set; }

		// Token: 0x06004C76 RID: 19574 RVA: 0x00195A30 File Offset: 0x00193C30
		public void UpdateTransform()
		{
			using (TimeWarning.New("HitboxSystem.UpdateTransform", 0))
			{
				this.transform = this.bone.localToWorldMatrix * this.localTransform;
				this.Size = this.transform.lossyScale;
				this.transform = Matrix4x4.TRS(this.Position, this.Rotation, Vector3.one);
				this.inverseTransform = this.transform.inverse;
			}
		}

		// Token: 0x06004C77 RID: 19575 RVA: 0x00195AC0 File Offset: 0x00193CC0
		public Vector3 TransformPoint(Vector3 pt)
		{
			return this.transform.MultiplyPoint(pt);
		}

		// Token: 0x06004C78 RID: 19576 RVA: 0x00195ACE File Offset: 0x00193CCE
		public Vector3 InverseTransformPoint(Vector3 pt)
		{
			return this.inverseTransform.MultiplyPoint(pt);
		}

		// Token: 0x06004C79 RID: 19577 RVA: 0x00195ADC File Offset: 0x00193CDC
		public Vector3 TransformDirection(Vector3 pt)
		{
			return this.transform.MultiplyVector(pt);
		}

		// Token: 0x06004C7A RID: 19578 RVA: 0x00195AEA File Offset: 0x00193CEA
		public Vector3 InverseTransformDirection(Vector3 pt)
		{
			return this.inverseTransform.MultiplyVector(pt);
		}

		// Token: 0x06004C7B RID: 19579 RVA: 0x00195AF8 File Offset: 0x00193CF8
		public bool Trace(Ray ray, out RaycastHit hit, float forgivness = 0f, float maxDistance = float.PositiveInfinity)
		{
			bool result;
			using (TimeWarning.New("Hitbox.Trace", 0))
			{
				ray.origin = this.InverseTransformPoint(ray.origin);
				ray.direction = this.InverseTransformDirection(ray.direction);
				if (this.type == HitboxDefinition.Type.BOX)
				{
					AABB aabb = new AABB(Vector3.zero, this.Size);
					if (!aabb.Trace(ray, out hit, forgivness, maxDistance))
					{
						return false;
					}
				}
				else
				{
					Capsule capsule = new Capsule(Vector3.zero, this.Size.x, this.Size.y * 0.5f);
					if (!capsule.Trace(ray, out hit, forgivness, maxDistance))
					{
						return false;
					}
				}
				hit.point = this.TransformPoint(hit.point);
				hit.normal = this.TransformDirection(hit.normal);
				result = true;
			}
			return result;
		}

		// Token: 0x06004C7C RID: 19580 RVA: 0x00195BE4 File Offset: 0x00193DE4
		public Bounds GetBounds()
		{
			Matrix4x4 matrix4x = this.Transform;
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					matrix4x[i, j] = Mathf.Abs(matrix4x[i, j]);
				}
			}
			return new Bounds
			{
				center = this.Transform.MultiplyPoint(Vector3.zero),
				extents = matrix4x.MultiplyVector(this.Size)
			};
		}

		// Token: 0x040041B5 RID: 16821
		public Transform bone;

		// Token: 0x040041B6 RID: 16822
		public HitboxDefinition.Type type;

		// Token: 0x040041B7 RID: 16823
		public Matrix4x4 localTransform;

		// Token: 0x040041B8 RID: 16824
		public PhysicMaterial colliderMaterial;

		// Token: 0x040041B9 RID: 16825
		private Matrix4x4 transform;

		// Token: 0x040041BA RID: 16826
		private Matrix4x4 inverseTransform;
	}
}
