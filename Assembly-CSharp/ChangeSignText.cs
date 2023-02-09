using System;
using UnityEngine;

// Token: 0x02000798 RID: 1944
public class ChangeSignText : UIDialog
{
	// Token: 0x04002B01 RID: 11009
	public Action<int, Texture2D> onUpdateTexture;

	// Token: 0x04002B02 RID: 11010
	public GameObject objectContainer;

	// Token: 0x04002B03 RID: 11011
	public GameObject currentFrameSection;

	// Token: 0x04002B04 RID: 11012
	public GameObject[] frameOptions;

	// Token: 0x04002B05 RID: 11013
	public Camera cameraPreview;

	// Token: 0x04002B06 RID: 11014
	public Camera camera3D;
}
