using System;
using Facepunch;
using ProtoBuf;

// Token: 0x0200040D RID: 1037
public class Modifier
{
	// Token: 0x170002A2 RID: 674
	// (get) Token: 0x060022CB RID: 8907 RVA: 0x000DE3C9 File Offset: 0x000DC5C9
	// (set) Token: 0x060022CC RID: 8908 RVA: 0x000DE3D1 File Offset: 0x000DC5D1
	public global::Modifier.ModifierType Type { get; private set; }

	// Token: 0x170002A3 RID: 675
	// (get) Token: 0x060022CD RID: 8909 RVA: 0x000DE3DA File Offset: 0x000DC5DA
	// (set) Token: 0x060022CE RID: 8910 RVA: 0x000DE3E2 File Offset: 0x000DC5E2
	public global::Modifier.ModifierSource Source { get; private set; }

	// Token: 0x170002A4 RID: 676
	// (get) Token: 0x060022CF RID: 8911 RVA: 0x000DE3EB File Offset: 0x000DC5EB
	// (set) Token: 0x060022D0 RID: 8912 RVA: 0x000DE3F3 File Offset: 0x000DC5F3
	public float Value { get; private set; } = 1f;

	// Token: 0x170002A5 RID: 677
	// (get) Token: 0x060022D1 RID: 8913 RVA: 0x000DE3FC File Offset: 0x000DC5FC
	// (set) Token: 0x060022D2 RID: 8914 RVA: 0x000DE404 File Offset: 0x000DC604
	public float Duration { get; private set; } = 10f;

	// Token: 0x170002A6 RID: 678
	// (get) Token: 0x060022D3 RID: 8915 RVA: 0x000DE40D File Offset: 0x000DC60D
	// (set) Token: 0x060022D4 RID: 8916 RVA: 0x000DE415 File Offset: 0x000DC615
	public float TimeRemaining { get; private set; }

	// Token: 0x170002A7 RID: 679
	// (get) Token: 0x060022D5 RID: 8917 RVA: 0x000DE41E File Offset: 0x000DC61E
	// (set) Token: 0x060022D6 RID: 8918 RVA: 0x000DE426 File Offset: 0x000DC626
	public bool Expired { get; private set; }

	// Token: 0x060022D7 RID: 8919 RVA: 0x000DE42F File Offset: 0x000DC62F
	public void Init(global::Modifier.ModifierType type, global::Modifier.ModifierSource source, float value, float duration, float remaining)
	{
		this.Type = type;
		this.Source = source;
		this.Value = value;
		this.Duration = duration;
		this.Expired = false;
		this.TimeRemaining = remaining;
	}

	// Token: 0x060022D8 RID: 8920 RVA: 0x000DE45D File Offset: 0x000DC65D
	public void Tick(BaseCombatEntity ownerEntity, float delta)
	{
		this.TimeRemaining -= delta;
		this.Expired = (this.TimeRemaining <= 0f);
	}

	// Token: 0x060022D9 RID: 8921 RVA: 0x000DE483 File Offset: 0x000DC683
	public ProtoBuf.Modifier Save()
	{
		ProtoBuf.Modifier modifier = Pool.Get<ProtoBuf.Modifier>();
		modifier.type = (int)this.Type;
		modifier.source = (int)this.Source;
		modifier.value = this.Value;
		modifier.timeRemaing = this.TimeRemaining;
		return modifier;
	}

	// Token: 0x060022DA RID: 8922 RVA: 0x000DE4BA File Offset: 0x000DC6BA
	public void Load(ProtoBuf.Modifier m)
	{
		this.Type = (global::Modifier.ModifierType)m.type;
		this.Source = (global::Modifier.ModifierSource)m.source;
		this.Value = m.value;
		this.TimeRemaining = m.timeRemaing;
	}

	// Token: 0x02000C8C RID: 3212
	public enum ModifierType
	{
		// Token: 0x040042D7 RID: 17111
		Wood_Yield,
		// Token: 0x040042D8 RID: 17112
		Ore_Yield,
		// Token: 0x040042D9 RID: 17113
		Radiation_Resistance,
		// Token: 0x040042DA RID: 17114
		Radiation_Exposure_Resistance,
		// Token: 0x040042DB RID: 17115
		Max_Health,
		// Token: 0x040042DC RID: 17116
		Scrap_Yield
	}

	// Token: 0x02000C8D RID: 3213
	public enum ModifierSource
	{
		// Token: 0x040042DE RID: 17118
		Tea
	}
}
