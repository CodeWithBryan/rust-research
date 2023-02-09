using System;
using Rust;
using UnityEngine;

// Token: 0x020006F2 RID: 1778
public class SubsurfaceProfile : ScriptableObject
{
	// Token: 0x170003DA RID: 986
	// (get) Token: 0x0600317E RID: 12670 RVA: 0x001304CA File Offset: 0x0012E6CA
	public static Texture2D Texture
	{
		get
		{
			if (SubsurfaceProfile.profileTexture == null)
			{
				return null;
			}
			return SubsurfaceProfile.profileTexture.Texture;
		}
	}

	// Token: 0x170003DB RID: 987
	// (get) Token: 0x0600317F RID: 12671 RVA: 0x001304DF File Offset: 0x0012E6DF
	public int Id
	{
		get
		{
			return this.id;
		}
	}

	// Token: 0x06003180 RID: 12672 RVA: 0x001304E7 File Offset: 0x0012E6E7
	private void OnEnable()
	{
		this.id = SubsurfaceProfile.profileTexture.AddProfile(this.Data, this);
	}

	// Token: 0x06003181 RID: 12673 RVA: 0x00130500 File Offset: 0x0012E700
	private void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		SubsurfaceProfile.profileTexture.RemoveProfile(this.id);
	}

	// Token: 0x06003182 RID: 12674 RVA: 0x0013051A File Offset: 0x0012E71A
	public void Update()
	{
		SubsurfaceProfile.profileTexture.UpdateProfile(this.id, this.Data);
	}

	// Token: 0x0400282A RID: 10282
	private static SubsurfaceProfileTexture profileTexture = new SubsurfaceProfileTexture();

	// Token: 0x0400282B RID: 10283
	public SubsurfaceProfileData Data = SubsurfaceProfileData.Default;

	// Token: 0x0400282C RID: 10284
	private int id = -1;
}
