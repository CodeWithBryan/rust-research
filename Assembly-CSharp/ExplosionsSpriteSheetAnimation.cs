using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000967 RID: 2407
internal class ExplosionsSpriteSheetAnimation : MonoBehaviour
{
	// Token: 0x060038BB RID: 14523 RVA: 0x0014E8B8 File Offset: 0x0014CAB8
	private void Start()
	{
		this.currentRenderer = base.GetComponent<Renderer>();
		this.InitDefaultVariables();
		this.isInizialised = true;
		this.isVisible = true;
		this.Play();
	}

	// Token: 0x060038BC RID: 14524 RVA: 0x0014E8E0 File Offset: 0x0014CAE0
	private void InitDefaultVariables()
	{
		this.currentRenderer = base.GetComponent<Renderer>();
		if (this.currentRenderer == null)
		{
			throw new Exception("UvTextureAnimator can't get renderer");
		}
		if (!this.currentRenderer.enabled)
		{
			this.currentRenderer.enabled = true;
		}
		this.allCount = 0;
		this.animationStoped = false;
		this.animationLifeTime = (float)(this.TilesX * this.TilesY) / this.AnimationFPS;
		this.count = this.TilesY * this.TilesX;
		this.index = this.TilesX - 1;
		Vector3 zero = Vector3.zero;
		this.StartFrameOffset -= this.StartFrameOffset / this.count * this.count;
		Vector2 value = new Vector2(1f / (float)this.TilesX, 1f / (float)this.TilesY);
		if (this.currentRenderer != null)
		{
			this.instanceMaterial = this.currentRenderer.material;
			this.instanceMaterial.SetTextureScale("_MainTex", value);
			this.instanceMaterial.SetTextureOffset("_MainTex", zero);
		}
	}

	// Token: 0x060038BD RID: 14525 RVA: 0x0014EA03 File Offset: 0x0014CC03
	private void Play()
	{
		if (this.isCorutineStarted)
		{
			return;
		}
		if (this.StartDelay > 0.0001f)
		{
			base.Invoke("PlayDelay", this.StartDelay);
		}
		else
		{
			base.StartCoroutine(this.UpdateCorutine());
		}
		this.isCorutineStarted = true;
	}

	// Token: 0x060038BE RID: 14526 RVA: 0x0014EA42 File Offset: 0x0014CC42
	private void PlayDelay()
	{
		base.StartCoroutine(this.UpdateCorutine());
	}

	// Token: 0x060038BF RID: 14527 RVA: 0x0014EA51 File Offset: 0x0014CC51
	private void OnEnable()
	{
		if (!this.isInizialised)
		{
			return;
		}
		this.InitDefaultVariables();
		this.isVisible = true;
		this.Play();
	}

	// Token: 0x060038C0 RID: 14528 RVA: 0x0014EA6F File Offset: 0x0014CC6F
	private void OnDisable()
	{
		this.isCorutineStarted = false;
		this.isVisible = false;
		base.StopAllCoroutines();
		base.CancelInvoke("PlayDelay");
	}

	// Token: 0x060038C1 RID: 14529 RVA: 0x0014EA90 File Offset: 0x0014CC90
	private IEnumerator UpdateCorutine()
	{
		this.animationStartTime = Time.time;
		while (this.isVisible && (this.IsLoop || !this.animationStoped))
		{
			this.UpdateFrame();
			if (!this.IsLoop && this.animationStoped)
			{
				break;
			}
			float value = (Time.time - this.animationStartTime) / this.animationLifeTime;
			float num = this.FrameOverTime.Evaluate(Mathf.Clamp01(value));
			yield return new WaitForSeconds(1f / (this.AnimationFPS * num));
		}
		this.isCorutineStarted = false;
		this.currentRenderer.enabled = false;
		yield break;
	}

	// Token: 0x060038C2 RID: 14530 RVA: 0x0014EAA0 File Offset: 0x0014CCA0
	private void UpdateFrame()
	{
		this.allCount++;
		this.index++;
		if (this.index >= this.count)
		{
			this.index = 0;
		}
		if (this.count == this.allCount)
		{
			this.animationStartTime = Time.time;
			this.allCount = 0;
			this.animationStoped = true;
		}
		Vector2 value = new Vector2((float)this.index / (float)this.TilesX - (float)(this.index / this.TilesX), 1f - (float)(this.index / this.TilesX) / (float)this.TilesY);
		if (this.currentRenderer != null)
		{
			this.instanceMaterial.SetTextureOffset("_MainTex", value);
		}
		if (this.IsInterpolateFrames)
		{
			this.currentInterpolatedTime = 0f;
		}
	}

	// Token: 0x060038C3 RID: 14531 RVA: 0x0014EB78 File Offset: 0x0014CD78
	private void Update()
	{
		if (!this.IsInterpolateFrames)
		{
			return;
		}
		this.currentInterpolatedTime += Time.deltaTime;
		int num = this.index + 1;
		if (this.allCount == 0)
		{
			num = this.index;
		}
		Vector4 value = new Vector4(1f / (float)this.TilesX, 1f / (float)this.TilesY, (float)num / (float)this.TilesX - (float)(num / this.TilesX), 1f - (float)(num / this.TilesX) / (float)this.TilesY);
		if (this.currentRenderer != null)
		{
			this.instanceMaterial.SetVector("_MainTex_NextFrame", value);
			float value2 = (Time.time - this.animationStartTime) / this.animationLifeTime;
			float num2 = this.FrameOverTime.Evaluate(Mathf.Clamp01(value2));
			this.instanceMaterial.SetFloat("InterpolationValue", Mathf.Clamp01(this.currentInterpolatedTime * this.AnimationFPS * num2));
		}
	}

	// Token: 0x060038C4 RID: 14532 RVA: 0x0014EC6D File Offset: 0x0014CE6D
	private void OnDestroy()
	{
		if (this.instanceMaterial != null)
		{
			UnityEngine.Object.Destroy(this.instanceMaterial);
			this.instanceMaterial = null;
		}
	}

	// Token: 0x04003317 RID: 13079
	public int TilesX = 4;

	// Token: 0x04003318 RID: 13080
	public int TilesY = 4;

	// Token: 0x04003319 RID: 13081
	public float AnimationFPS = 30f;

	// Token: 0x0400331A RID: 13082
	public bool IsInterpolateFrames;

	// Token: 0x0400331B RID: 13083
	public int StartFrameOffset;

	// Token: 0x0400331C RID: 13084
	public bool IsLoop = true;

	// Token: 0x0400331D RID: 13085
	public float StartDelay;

	// Token: 0x0400331E RID: 13086
	public AnimationCurve FrameOverTime = AnimationCurve.Linear(0f, 1f, 1f, 1f);

	// Token: 0x0400331F RID: 13087
	private bool isInizialised;

	// Token: 0x04003320 RID: 13088
	private int index;

	// Token: 0x04003321 RID: 13089
	private int count;

	// Token: 0x04003322 RID: 13090
	private int allCount;

	// Token: 0x04003323 RID: 13091
	private float animationLifeTime;

	// Token: 0x04003324 RID: 13092
	private bool isVisible;

	// Token: 0x04003325 RID: 13093
	private bool isCorutineStarted;

	// Token: 0x04003326 RID: 13094
	private Renderer currentRenderer;

	// Token: 0x04003327 RID: 13095
	private Material instanceMaterial;

	// Token: 0x04003328 RID: 13096
	private float currentInterpolatedTime;

	// Token: 0x04003329 RID: 13097
	private float animationStartTime;

	// Token: 0x0400332A RID: 13098
	private bool animationStoped;
}
