using System;
using UnityEngine;

// Token: 0x020001BA RID: 442
public class v_chainsaw : MonoBehaviour
{
	// Token: 0x060017EA RID: 6122 RVA: 0x000B12FA File Offset: 0x000AF4FA
	public void OnEnable()
	{
		if (this.block == null)
		{
			this.block = new MaterialPropertyBlock();
		}
		this.saveST = this.chainRenderer.sharedMaterial.GetVector("_MainTex_ST");
	}

	// Token: 0x060017EB RID: 6123 RVA: 0x000B132F File Offset: 0x000AF52F
	private void Awake()
	{
		this.chainlink = this.chainRenderer.sharedMaterial;
	}

	// Token: 0x060017EC RID: 6124 RVA: 0x000059DD File Offset: 0x00003BDD
	private void Start()
	{
	}

	// Token: 0x060017ED RID: 6125 RVA: 0x000B1344 File Offset: 0x000AF544
	private void ScrollChainTexture()
	{
		float z = this.chainAmount = (this.chainAmount + Time.deltaTime * this.chainSpeed) % 1f;
		this.block.Clear();
		this.block.SetVector("_MainTex_ST", new Vector4(this.saveST.x, this.saveST.y, z, 0f));
		this.chainRenderer.SetPropertyBlock(this.block);
	}

	// Token: 0x060017EE RID: 6126 RVA: 0x000B13C4 File Offset: 0x000AF5C4
	private void Update()
	{
		this.chainsawAnimator.SetBool("attacking", this.bAttacking);
		this.smokeEffect.enableEmission = this.bEngineOn;
		ParticleSystem[] array;
		if (this.bHitMetal)
		{
			this.chainsawAnimator.SetBool("attackHit", true);
			array = this.hitMetalFX;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enableEmission = true;
			}
			array = this.hitWoodFX;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enableEmission = false;
			}
			array = this.hitFleshFX;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enableEmission = false;
			}
			this.DoHitSound(this.hitMetalSoundDef);
			return;
		}
		if (this.bHitWood)
		{
			this.chainsawAnimator.SetBool("attackHit", true);
			array = this.hitMetalFX;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enableEmission = false;
			}
			array = this.hitWoodFX;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enableEmission = true;
			}
			array = this.hitFleshFX;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enableEmission = false;
			}
			this.DoHitSound(this.hitWoodSoundDef);
			return;
		}
		if (this.bHitFlesh)
		{
			this.chainsawAnimator.SetBool("attackHit", true);
			array = this.hitMetalFX;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enableEmission = false;
			}
			array = this.hitWoodFX;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enableEmission = false;
			}
			array = this.hitFleshFX;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enableEmission = true;
			}
			this.DoHitSound(this.hitFleshSoundDef);
			return;
		}
		this.chainsawAnimator.SetBool("attackHit", false);
		array = this.hitMetalFX;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enableEmission = false;
		}
		array = this.hitWoodFX;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enableEmission = false;
		}
		array = this.hitFleshFX;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enableEmission = false;
		}
	}

	// Token: 0x060017EF RID: 6127 RVA: 0x000059DD File Offset: 0x00003BDD
	private void DoHitSound(SoundDefinition soundDef)
	{
	}

	// Token: 0x0400111B RID: 4379
	public bool bAttacking;

	// Token: 0x0400111C RID: 4380
	public bool bHitMetal;

	// Token: 0x0400111D RID: 4381
	public bool bHitWood;

	// Token: 0x0400111E RID: 4382
	public bool bHitFlesh;

	// Token: 0x0400111F RID: 4383
	public bool bEngineOn;

	// Token: 0x04001120 RID: 4384
	public ParticleSystem[] hitMetalFX;

	// Token: 0x04001121 RID: 4385
	public ParticleSystem[] hitWoodFX;

	// Token: 0x04001122 RID: 4386
	public ParticleSystem[] hitFleshFX;

	// Token: 0x04001123 RID: 4387
	public SoundDefinition hitMetalSoundDef;

	// Token: 0x04001124 RID: 4388
	public SoundDefinition hitWoodSoundDef;

	// Token: 0x04001125 RID: 4389
	public SoundDefinition hitFleshSoundDef;

	// Token: 0x04001126 RID: 4390
	public Sound hitSound;

	// Token: 0x04001127 RID: 4391
	public GameObject hitSoundTarget;

	// Token: 0x04001128 RID: 4392
	public float hitSoundFadeTime = 0.1f;

	// Token: 0x04001129 RID: 4393
	public ParticleSystem smokeEffect;

	// Token: 0x0400112A RID: 4394
	public Animator chainsawAnimator;

	// Token: 0x0400112B RID: 4395
	public Renderer chainRenderer;

	// Token: 0x0400112C RID: 4396
	public Material chainlink;

	// Token: 0x0400112D RID: 4397
	private MaterialPropertyBlock block;

	// Token: 0x0400112E RID: 4398
	private Vector2 saveST;

	// Token: 0x0400112F RID: 4399
	private float chainSpeed;

	// Token: 0x04001130 RID: 4400
	private float chainAmount;

	// Token: 0x04001131 RID: 4401
	public float temp1;

	// Token: 0x04001132 RID: 4402
	public float temp2;
}
