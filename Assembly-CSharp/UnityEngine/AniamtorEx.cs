using System;

namespace UnityEngine
{
	// Token: 0x020009DB RID: 2523
	public static class AniamtorEx
	{
		// Token: 0x06003B80 RID: 15232 RVA: 0x0015BC00 File Offset: 0x00159E00
		public static void SetFloatFixed(this Animator animator, int id, float value, float dampTime, float deltaTime)
		{
			if (value == 0f)
			{
				float @float = animator.GetFloat(id);
				if (@float == 0f)
				{
					return;
				}
				if (@float < 1E-45f)
				{
					animator.SetFloat(id, 0f);
					return;
				}
			}
			animator.SetFloat(id, value, dampTime, deltaTime);
		}

		// Token: 0x06003B81 RID: 15233 RVA: 0x0015BC46 File Offset: 0x00159E46
		public static void SetBoolChecked(this Animator animator, int id, bool value)
		{
			if (animator.GetBool(id) != value)
			{
				animator.SetBool(id, value);
			}
		}
	}
}
