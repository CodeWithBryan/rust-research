using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000972 RID: 2418
public class VertexColorAnimator : MonoBehaviour
{
	// Token: 0x0600392E RID: 14638 RVA: 0x00152088 File Offset: 0x00150288
	public void initLists()
	{
		this.animationMeshes = new List<MeshHolder>();
		this.animationKeyframes = new List<float>();
	}

	// Token: 0x0600392F RID: 14639 RVA: 0x001520A0 File Offset: 0x001502A0
	public void addMesh(Mesh mesh, float atPosition)
	{
		MeshHolder meshHolder = new MeshHolder();
		meshHolder.setAnimationData(mesh);
		this.animationMeshes.Add(meshHolder);
		this.animationKeyframes.Add(atPosition);
	}

	// Token: 0x06003930 RID: 14640 RVA: 0x001520D2 File Offset: 0x001502D2
	private void Start()
	{
		this.elapsedTime = 0f;
	}

	// Token: 0x06003931 RID: 14641 RVA: 0x001520DF File Offset: 0x001502DF
	public void replaceKeyframe(int frameIndex, Mesh mesh)
	{
		this.animationMeshes[frameIndex].setAnimationData(mesh);
	}

	// Token: 0x06003932 RID: 14642 RVA: 0x001520F3 File Offset: 0x001502F3
	public void deleteKeyframe(int frameIndex)
	{
		this.animationMeshes.RemoveAt(frameIndex);
		this.animationKeyframes.RemoveAt(frameIndex);
	}

	// Token: 0x06003933 RID: 14643 RVA: 0x00152110 File Offset: 0x00150310
	public void scrobble(float scrobblePos)
	{
		if (this.animationMeshes.Count == 0)
		{
			return;
		}
		Color[] array = new Color[base.GetComponent<MeshFilter>().sharedMesh.colors.Length];
		int num = 0;
		for (int i = 0; i < this.animationKeyframes.Count; i++)
		{
			if (scrobblePos >= this.animationKeyframes[i])
			{
				num = i;
			}
		}
		if (num >= this.animationKeyframes.Count - 1)
		{
			base.GetComponent<VertexColorStream>().setColors(this.animationMeshes[num]._colors);
			return;
		}
		float num2 = this.animationKeyframes[num + 1] - this.animationKeyframes[num];
		float num3 = this.animationKeyframes[num];
		float t = (scrobblePos - num3) / num2;
		for (int j = 0; j < array.Length; j++)
		{
			array[j] = Color.Lerp(this.animationMeshes[num]._colors[j], this.animationMeshes[num + 1]._colors[j], t);
		}
		base.GetComponent<VertexColorStream>().setColors(array);
	}

	// Token: 0x06003934 RID: 14644 RVA: 0x00152230 File Offset: 0x00150430
	private void Update()
	{
		if (this.mode == 0)
		{
			this.elapsedTime += Time.fixedDeltaTime / this.timeScale;
		}
		else if (this.mode == 1)
		{
			this.elapsedTime += Time.fixedDeltaTime / this.timeScale;
			if (this.elapsedTime > 1f)
			{
				this.elapsedTime = 0f;
			}
		}
		else if (this.mode == 2)
		{
			if (Mathf.FloorToInt(Time.fixedTime / this.timeScale) % 2 == 0)
			{
				this.elapsedTime += Time.fixedDeltaTime / this.timeScale;
			}
			else
			{
				this.elapsedTime -= Time.fixedDeltaTime / this.timeScale;
			}
		}
		Color[] array = new Color[base.GetComponent<MeshFilter>().sharedMesh.colors.Length];
		int num = 0;
		for (int i = 0; i < this.animationKeyframes.Count; i++)
		{
			if (this.elapsedTime >= this.animationKeyframes[i])
			{
				num = i;
			}
		}
		if (num < this.animationKeyframes.Count - 1)
		{
			float num2 = this.animationKeyframes[num + 1] - this.animationKeyframes[num];
			float num3 = this.animationKeyframes[num];
			float t = (this.elapsedTime - num3) / num2;
			for (int j = 0; j < array.Length; j++)
			{
				array[j] = Color.Lerp(this.animationMeshes[num]._colors[j], this.animationMeshes[num + 1]._colors[j], t);
			}
		}
		else
		{
			array = this.animationMeshes[num]._colors;
		}
		base.GetComponent<VertexColorStream>().setColors(array);
	}

	// Token: 0x040033A2 RID: 13218
	public List<MeshHolder> animationMeshes;

	// Token: 0x040033A3 RID: 13219
	public List<float> animationKeyframes;

	// Token: 0x040033A4 RID: 13220
	public float timeScale = 2f;

	// Token: 0x040033A5 RID: 13221
	public int mode;

	// Token: 0x040033A6 RID: 13222
	private float elapsedTime;
}
