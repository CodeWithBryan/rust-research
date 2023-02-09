using System;
using UnityEngine;

// Token: 0x020005F2 RID: 1522
public class MoveOverTime : MonoBehaviour
{
	// Token: 0x06002C90 RID: 11408 RVA: 0x0010AD7C File Offset: 0x00108F7C
	private void Update()
	{
		base.transform.rotation = Quaternion.Euler(base.transform.rotation.eulerAngles + this.rotation * this.speed * Time.deltaTime);
		base.transform.localScale += this.scale * this.speed * Time.deltaTime;
		base.transform.localPosition += this.position * this.speed * Time.deltaTime;
	}

	// Token: 0x0400245A RID: 9306
	[Range(-10f, 10f)]
	public float speed = 1f;

	// Token: 0x0400245B RID: 9307
	public Vector3 position;

	// Token: 0x0400245C RID: 9308
	public Vector3 rotation;

	// Token: 0x0400245D RID: 9309
	public Vector3 scale;
}
