using System;
using UnityEngine;

// Token: 0x020004B8 RID: 1208
public interface IRFObject
{
	// Token: 0x060026E5 RID: 9957
	Vector3 GetPosition();

	// Token: 0x060026E6 RID: 9958
	float GetMaxRange();

	// Token: 0x060026E7 RID: 9959
	void RFSignalUpdate(bool on);

	// Token: 0x060026E8 RID: 9960
	int GetFrequency();
}
