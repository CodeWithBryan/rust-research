using System;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x0200096A RID: 2410
public class OccludeeState : OcclusionCulling.SmartListValue
{
	// Token: 0x17000444 RID: 1092
	// (get) Token: 0x060038CD RID: 14541 RVA: 0x0014EE32 File Offset: 0x0014D032
	public bool isVisible
	{
		get
		{
			return this.states[this.slot].isVisible > 0;
		}
	}

	// Token: 0x060038CE RID: 14542 RVA: 0x0014EE50 File Offset: 0x0014D050
	public OccludeeState Initialize(OcclusionCulling.SimpleList<OccludeeState.State> states, OcclusionCulling.BufferSet set, int slot, Vector4 sphereBounds, bool isVisible, float minTimeVisible, bool isStatic, int layer, OcclusionCulling.OnVisibilityChanged onVisibilityChanged)
	{
		states[slot] = new OccludeeState.State
		{
			sphereBounds = sphereBounds,
			minTimeVisible = minTimeVisible,
			waitTime = (isVisible ? (Time.time + minTimeVisible) : 0f),
			waitFrame = (uint)(Time.frameCount + 1),
			isVisible = (isVisible ? 1 : 0),
			active = 1,
			callback = ((onVisibilityChanged != null) ? 1 : 0)
		};
		this.slot = slot;
		this.isStatic = isStatic;
		this.layer = layer;
		this.onVisibilityChanged = onVisibilityChanged;
		this.cell = null;
		this.states = states;
		return this;
	}

	// Token: 0x060038CF RID: 14543 RVA: 0x0014EEFB File Offset: 0x0014D0FB
	public void Invalidate()
	{
		this.states[this.slot] = OccludeeState.State.Unused;
		this.slot = -1;
		this.onVisibilityChanged = null;
		this.cell = null;
	}

	// Token: 0x060038D0 RID: 14544 RVA: 0x0014EF28 File Offset: 0x0014D128
	public void MakeVisible()
	{
		this.states.array[this.slot].waitTime = Time.time + this.states[this.slot].minTimeVisible;
		this.states.array[this.slot].isVisible = 1;
		if (this.onVisibilityChanged != null)
		{
			this.onVisibilityChanged(true);
		}
	}

	// Token: 0x04003333 RID: 13107
	public int slot;

	// Token: 0x04003334 RID: 13108
	public bool isStatic;

	// Token: 0x04003335 RID: 13109
	public int layer;

	// Token: 0x04003336 RID: 13110
	public OcclusionCulling.OnVisibilityChanged onVisibilityChanged;

	// Token: 0x04003337 RID: 13111
	public OcclusionCulling.Cell cell;

	// Token: 0x04003338 RID: 13112
	public OcclusionCulling.SimpleList<OccludeeState.State> states;

	// Token: 0x02000E76 RID: 3702
	[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 32)]
	public struct State
	{
		// Token: 0x04004A90 RID: 19088
		[FieldOffset(0)]
		public Vector4 sphereBounds;

		// Token: 0x04004A91 RID: 19089
		[FieldOffset(16)]
		public float minTimeVisible;

		// Token: 0x04004A92 RID: 19090
		[FieldOffset(20)]
		public float waitTime;

		// Token: 0x04004A93 RID: 19091
		[FieldOffset(24)]
		public uint waitFrame;

		// Token: 0x04004A94 RID: 19092
		[FieldOffset(28)]
		public byte isVisible;

		// Token: 0x04004A95 RID: 19093
		[FieldOffset(29)]
		public byte active;

		// Token: 0x04004A96 RID: 19094
		[FieldOffset(30)]
		public byte callback;

		// Token: 0x04004A97 RID: 19095
		[FieldOffset(31)]
		public byte pad1;

		// Token: 0x04004A98 RID: 19096
		public static OccludeeState.State Unused = new OccludeeState.State
		{
			active = 0
		};
	}
}
