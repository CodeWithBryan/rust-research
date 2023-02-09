using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000953 RID: 2387
internal class UVTextureAnimator : MonoBehaviour
{
	// Token: 0x06003854 RID: 14420 RVA: 0x0014CB7C File Offset: 0x0014AD7C
	private void Start()
	{
		this.currentRenderer = base.GetComponent<Renderer>();
		this.InitDefaultVariables();
		this.isInizialised = true;
		this.isVisible = true;
		this.Play();
	}

	// Token: 0x06003855 RID: 14421 RVA: 0x0014CBA4 File Offset: 0x0014ADA4
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
		this.deltaFps = 1f / this.Fps;
		this.count = this.Rows * this.Columns;
		this.index = this.Columns - 1;
		Vector3 zero = Vector3.zero;
		this.OffsetMat -= this.OffsetMat / this.count * this.count;
		Vector2 value = new Vector2(1f / (float)this.Columns, 1f / (float)this.Rows);
		if (this.currentRenderer != null)
		{
			this.instanceMaterial = this.currentRenderer.material;
			this.instanceMaterial.SetTextureScale("_MainTex", value);
			this.instanceMaterial.SetTextureOffset("_MainTex", zero);
		}
	}

	// Token: 0x06003856 RID: 14422 RVA: 0x0014CCB7 File Offset: 0x0014AEB7
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

	// Token: 0x06003857 RID: 14423 RVA: 0x0014CCF6 File Offset: 0x0014AEF6
	private void PlayDelay()
	{
		base.StartCoroutine(this.UpdateCorutine());
	}

	// Token: 0x06003858 RID: 14424 RVA: 0x0014CD05 File Offset: 0x0014AF05
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

	// Token: 0x06003859 RID: 14425 RVA: 0x0014CD23 File Offset: 0x0014AF23
	private void OnDisable()
	{
		this.isCorutineStarted = false;
		this.isVisible = false;
		base.StopAllCoroutines();
		base.CancelInvoke("PlayDelay");
	}

	// Token: 0x0600385A RID: 14426 RVA: 0x0014CD44 File Offset: 0x0014AF44
	private IEnumerator UpdateCorutine()
	{
		while (this.isVisible && (this.IsLoop || this.allCount != this.count))
		{
			this.UpdateCorutineFrame();
			if (!this.IsLoop && this.allCount == this.count)
			{
				break;
			}
			yield return new WaitForSeconds(this.deltaFps);
		}
		this.isCorutineStarted = false;
		this.currentRenderer.enabled = false;
		yield break;
	}

	// Token: 0x0600385B RID: 14427 RVA: 0x0014CD54 File Offset: 0x0014AF54
	private void UpdateCorutineFrame()
	{
		this.allCount++;
		this.index++;
		if (this.index >= this.count)
		{
			this.index = 0;
		}
		Vector2 value = new Vector2((float)this.index / (float)this.Columns - (float)(this.index / this.Columns), 1f - (float)(this.index / this.Columns) / (float)this.Rows);
		if (this.currentRenderer != null)
		{
			this.instanceMaterial.SetTextureOffset("_MainTex", value);
		}
	}

	// Token: 0x0600385C RID: 14428 RVA: 0x0014CDF2 File Offset: 0x0014AFF2
	private void OnDestroy()
	{
		if (this.instanceMaterial != null)
		{
			UnityEngine.Object.Destroy(this.instanceMaterial);
			this.instanceMaterial = null;
		}
	}

	// Token: 0x040032AE RID: 12974
	public int Rows = 4;

	// Token: 0x040032AF RID: 12975
	public int Columns = 4;

	// Token: 0x040032B0 RID: 12976
	public float Fps = 20f;

	// Token: 0x040032B1 RID: 12977
	public int OffsetMat;

	// Token: 0x040032B2 RID: 12978
	public bool IsLoop = true;

	// Token: 0x040032B3 RID: 12979
	public float StartDelay;

	// Token: 0x040032B4 RID: 12980
	private bool isInizialised;

	// Token: 0x040032B5 RID: 12981
	private int index;

	// Token: 0x040032B6 RID: 12982
	private int count;

	// Token: 0x040032B7 RID: 12983
	private int allCount;

	// Token: 0x040032B8 RID: 12984
	private float deltaFps;

	// Token: 0x040032B9 RID: 12985
	private bool isVisible;

	// Token: 0x040032BA RID: 12986
	private bool isCorutineStarted;

	// Token: 0x040032BB RID: 12987
	private Renderer currentRenderer;

	// Token: 0x040032BC RID: 12988
	private Material instanceMaterial;
}
