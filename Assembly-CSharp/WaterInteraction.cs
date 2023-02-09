using System;
using UnityEngine;

// Token: 0x020006D5 RID: 1749
[ExecuteInEditMode]
public class WaterInteraction : MonoBehaviour
{
	// Token: 0x170003BC RID: 956
	// (get) Token: 0x060030F8 RID: 12536 RVA: 0x0012D925 File Offset: 0x0012BB25
	// (set) Token: 0x060030F9 RID: 12537 RVA: 0x0012D92D File Offset: 0x0012BB2D
	public Texture2D Texture
	{
		get
		{
			return this.texture;
		}
		set
		{
			this.texture = value;
			this.CheckRegister();
		}
	}

	// Token: 0x170003BD RID: 957
	// (get) Token: 0x060030FA RID: 12538 RVA: 0x0012D93C File Offset: 0x0012BB3C
	// (set) Token: 0x060030FB RID: 12539 RVA: 0x0012D944 File Offset: 0x0012BB44
	public WaterDynamics.Image Image { get; private set; }

	// Token: 0x170003BE RID: 958
	// (get) Token: 0x060030FC RID: 12540 RVA: 0x0012D94D File Offset: 0x0012BB4D
	// (set) Token: 0x060030FD RID: 12541 RVA: 0x0012D955 File Offset: 0x0012BB55
	public Vector2 Position { get; private set; } = Vector2.zero;

	// Token: 0x170003BF RID: 959
	// (get) Token: 0x060030FE RID: 12542 RVA: 0x0012D95E File Offset: 0x0012BB5E
	// (set) Token: 0x060030FF RID: 12543 RVA: 0x0012D966 File Offset: 0x0012BB66
	public Vector2 Scale { get; private set; } = Vector2.one;

	// Token: 0x170003C0 RID: 960
	// (get) Token: 0x06003100 RID: 12544 RVA: 0x0012D96F File Offset: 0x0012BB6F
	// (set) Token: 0x06003101 RID: 12545 RVA: 0x0012D977 File Offset: 0x0012BB77
	public float Rotation { get; private set; }

	// Token: 0x06003102 RID: 12546 RVA: 0x0012D980 File Offset: 0x0012BB80
	protected void OnEnable()
	{
		this.CheckRegister();
		this.UpdateTransform();
	}

	// Token: 0x06003103 RID: 12547 RVA: 0x0012D98E File Offset: 0x0012BB8E
	protected void OnDisable()
	{
		this.Unregister();
	}

	// Token: 0x06003104 RID: 12548 RVA: 0x0012D998 File Offset: 0x0012BB98
	public void CheckRegister()
	{
		if (!base.enabled || this.texture == null)
		{
			this.Unregister();
			return;
		}
		if (this.Image == null || this.Image.texture != this.texture)
		{
			this.Register();
		}
	}

	// Token: 0x06003105 RID: 12549 RVA: 0x0012D9E8 File Offset: 0x0012BBE8
	private void UpdateImage()
	{
		this.Image = new WaterDynamics.Image(this.texture);
	}

	// Token: 0x06003106 RID: 12550 RVA: 0x0012D9FB File Offset: 0x0012BBFB
	private void Register()
	{
		this.UpdateImage();
		WaterDynamics.RegisterInteraction(this);
	}

	// Token: 0x06003107 RID: 12551 RVA: 0x0012DA09 File Offset: 0x0012BC09
	private void Unregister()
	{
		if (this.Image != null)
		{
			WaterDynamics.UnregisterInteraction(this);
			this.Image = null;
		}
	}

	// Token: 0x06003108 RID: 12552 RVA: 0x0012DA20 File Offset: 0x0012BC20
	public void UpdateTransform()
	{
		this.cachedTransform = ((this.cachedTransform != null) ? this.cachedTransform : base.transform);
		if (this.cachedTransform.hasChanged)
		{
			Vector3 position = this.cachedTransform.position;
			Vector3 lossyScale = this.cachedTransform.lossyScale;
			this.Position = new Vector2(position.x, position.z);
			this.Scale = new Vector2(lossyScale.x, lossyScale.z);
			this.Rotation = this.cachedTransform.rotation.eulerAngles.y;
			this.cachedTransform.hasChanged = false;
		}
	}

	// Token: 0x040027B2 RID: 10162
	[SerializeField]
	private Texture2D texture;

	// Token: 0x040027B3 RID: 10163
	[Range(0f, 1f)]
	public float Displacement = 1f;

	// Token: 0x040027B4 RID: 10164
	[Range(0f, 1f)]
	public float Disturbance = 0.5f;

	// Token: 0x040027B9 RID: 10169
	private Transform cachedTransform;
}
