using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000955 RID: 2389
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(Camera))]
public class CoverageQueries : MonoBehaviour
{
	// Token: 0x040032BD RID: 12989
	public bool debug;

	// Token: 0x040032BE RID: 12990
	public float depthBias = -0.1f;

	// Token: 0x02000E70 RID: 3696
	public class BufferSet
	{
		// Token: 0x0600508E RID: 20622 RVA: 0x001A1D60 File Offset: 0x0019FF60
		public void Attach(Material coverageMat)
		{
			this.coverageMat = coverageMat;
		}

		// Token: 0x0600508F RID: 20623 RVA: 0x001A1D6C File Offset: 0x0019FF6C
		public void Dispose(bool data = true)
		{
			if (this.inputTexture != null)
			{
				UnityEngine.Object.DestroyImmediate(this.inputTexture);
				this.inputTexture = null;
			}
			if (this.resultTexture != null)
			{
				RenderTexture.active = null;
				this.resultTexture.Release();
				UnityEngine.Object.DestroyImmediate(this.resultTexture);
				this.resultTexture = null;
			}
			if (data)
			{
				this.inputData = new Color[0];
				this.resultData = new Color32[0];
			}
		}

		// Token: 0x06005090 RID: 20624 RVA: 0x001A1DE8 File Offset: 0x0019FFE8
		public bool CheckResize(int count)
		{
			if (count > this.inputData.Length || (this.resultTexture != null && !this.resultTexture.IsCreated()))
			{
				this.Dispose(false);
				this.width = Mathf.CeilToInt(Mathf.Sqrt((float)count));
				this.height = Mathf.CeilToInt((float)count / (float)this.width);
				this.inputTexture = new Texture2D(this.width, this.height, TextureFormat.RGBAFloat, false, true);
				this.inputTexture.name = "_Input";
				this.inputTexture.filterMode = FilterMode.Point;
				this.inputTexture.wrapMode = TextureWrapMode.Clamp;
				this.resultTexture = new RenderTexture(this.width, this.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
				this.resultTexture.name = "_Result";
				this.resultTexture.filterMode = FilterMode.Point;
				this.resultTexture.wrapMode = TextureWrapMode.Clamp;
				this.resultTexture.useMipMap = false;
				this.resultTexture.Create();
				int num = this.resultData.Length;
				int num2 = this.width * this.height;
				Array.Resize<Color>(ref this.inputData, num2);
				Array.Resize<Color32>(ref this.resultData, num2);
				Color32 color = new Color32(byte.MaxValue, 0, 0, 0);
				for (int i = num; i < num2; i++)
				{
					this.resultData[i] = color;
				}
				return true;
			}
			return false;
		}

		// Token: 0x06005091 RID: 20625 RVA: 0x001A1F44 File Offset: 0x001A0144
		public void UploadData()
		{
			if (this.inputData.Length != 0)
			{
				this.inputTexture.SetPixels(this.inputData);
				this.inputTexture.Apply();
			}
		}

		// Token: 0x06005092 RID: 20626 RVA: 0x001A1F6C File Offset: 0x001A016C
		public void Dispatch(int count)
		{
			if (this.inputData.Length != 0)
			{
				RenderBuffer activeColorBuffer = Graphics.activeColorBuffer;
				RenderBuffer activeDepthBuffer = Graphics.activeDepthBuffer;
				this.coverageMat.SetTexture("_Input", this.inputTexture);
				Graphics.Blit(this.inputTexture, this.resultTexture, this.coverageMat, 0);
				Graphics.SetRenderTarget(activeColorBuffer, activeDepthBuffer);
			}
		}

		// Token: 0x06005093 RID: 20627 RVA: 0x001A1FC1 File Offset: 0x001A01C1
		public void IssueRead()
		{
			if (this.asyncRequests.Count < 10)
			{
				this.asyncRequests.Enqueue(AsyncGPUReadback.Request(this.resultTexture, 0, null));
			}
		}

		// Token: 0x06005094 RID: 20628 RVA: 0x001A1FEC File Offset: 0x001A01EC
		public void GetResults()
		{
			if (this.resultData.Length != 0)
			{
				while (this.asyncRequests.Count > 0)
				{
					AsyncGPUReadbackRequest asyncGPUReadbackRequest = this.asyncRequests.Peek();
					if (asyncGPUReadbackRequest.hasError)
					{
						this.asyncRequests.Dequeue();
					}
					else
					{
						if (!asyncGPUReadbackRequest.done)
						{
							break;
						}
						NativeArray<Color32> data = asyncGPUReadbackRequest.GetData<Color32>(0);
						for (int i = 0; i < data.Length; i++)
						{
							this.resultData[i] = data[i];
						}
						this.asyncRequests.Dequeue();
					}
				}
			}
		}

		// Token: 0x04004A73 RID: 19059
		public int width;

		// Token: 0x04004A74 RID: 19060
		public int height;

		// Token: 0x04004A75 RID: 19061
		public Texture2D inputTexture;

		// Token: 0x04004A76 RID: 19062
		public RenderTexture resultTexture;

		// Token: 0x04004A77 RID: 19063
		public Color[] inputData = new Color[0];

		// Token: 0x04004A78 RID: 19064
		public Color32[] resultData = new Color32[0];

		// Token: 0x04004A79 RID: 19065
		private Material coverageMat;

		// Token: 0x04004A7A RID: 19066
		private const int MaxAsyncGPUReadbackRequests = 10;

		// Token: 0x04004A7B RID: 19067
		private Queue<AsyncGPUReadbackRequest> asyncRequests = new Queue<AsyncGPUReadbackRequest>();
	}

	// Token: 0x02000E71 RID: 3697
	public enum RadiusSpace
	{
		// Token: 0x04004A7D RID: 19069
		ScreenNormalized,
		// Token: 0x04004A7E RID: 19070
		World
	}

	// Token: 0x02000E72 RID: 3698
	public class Query
	{
		// Token: 0x1700068B RID: 1675
		// (get) Token: 0x06005096 RID: 20630 RVA: 0x001A20A4 File Offset: 0x001A02A4
		public bool IsRegistered
		{
			get
			{
				return this.intern.id >= 0;
			}
		}

		// Token: 0x04004A7F RID: 19071
		public CoverageQueries.Query.Input input;

		// Token: 0x04004A80 RID: 19072
		public CoverageQueries.Query.Internal intern;

		// Token: 0x04004A81 RID: 19073
		public CoverageQueries.Query.Result result;

		// Token: 0x02000F70 RID: 3952
		public struct Input
		{
			// Token: 0x04004E49 RID: 20041
			public Vector3 position;

			// Token: 0x04004E4A RID: 20042
			public CoverageQueries.RadiusSpace radiusSpace;

			// Token: 0x04004E4B RID: 20043
			public float radius;

			// Token: 0x04004E4C RID: 20044
			public int sampleCount;

			// Token: 0x04004E4D RID: 20045
			public float smoothingSpeed;
		}

		// Token: 0x02000F71 RID: 3953
		public struct Internal
		{
			// Token: 0x0600526F RID: 21103 RVA: 0x001A7241 File Offset: 0x001A5441
			public void Reset()
			{
				this.id = -1;
			}

			// Token: 0x04004E4E RID: 20046
			public int id;
		}

		// Token: 0x02000F72 RID: 3954
		public struct Result
		{
			// Token: 0x06005270 RID: 21104 RVA: 0x001A724C File Offset: 0x001A544C
			public void Reset()
			{
				this.passed = 0;
				this.coverage = 0f;
				this.smoothCoverage = 0f;
				this.weightedCoverage = 0f;
				this.weightedSmoothCoverage = 0f;
				this.originOccluded = true;
				this.frame = -1;
				this.originVisibility = 0f;
				this.originSmoothVisibility = 0f;
			}

			// Token: 0x04004E4F RID: 20047
			public int passed;

			// Token: 0x04004E50 RID: 20048
			public float coverage;

			// Token: 0x04004E51 RID: 20049
			public float smoothCoverage;

			// Token: 0x04004E52 RID: 20050
			public float weightedCoverage;

			// Token: 0x04004E53 RID: 20051
			public float weightedSmoothCoverage;

			// Token: 0x04004E54 RID: 20052
			public bool originOccluded;

			// Token: 0x04004E55 RID: 20053
			public int frame;

			// Token: 0x04004E56 RID: 20054
			public float originVisibility;

			// Token: 0x04004E57 RID: 20055
			public float originSmoothVisibility;
		}
	}
}
