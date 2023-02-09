using System;

// Token: 0x0200028A RID: 650
public abstract class ComponentInfo<T> : ComponentInfo
{
	// Token: 0x06001BFC RID: 7164 RVA: 0x000C1C99 File Offset: 0x000BFE99
	public void Initialize(T source)
	{
		this.component = source;
		this.Setup();
	}

	// Token: 0x04001542 RID: 5442
	public T component;
}
